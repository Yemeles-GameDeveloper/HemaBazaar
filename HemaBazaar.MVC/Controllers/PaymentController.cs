using Application.ViewModels;
using HemaBazaar.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Options = Iyzipay.Options;
using Application.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Domain.Enums;
using Application.Interfaces;
using Application.Common;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using HemaBazaar.MVC.Services;
using Microsoft.Extensions.Logging;

namespace HemaBazaar.MVC.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        IOptions<IyzicoOptions> _iyzicoOptions;
        ICartService cartService;
        IPaymentService paymentService;
        IPurchaseService purchaseService;
        UserManager<AppUser> _userManager;
        CartDTO cartDTO;

        RabbitMqProducerService producerService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IOptions<IyzicoOptions> iyzicoOptions, UserManager<AppUser> userManager, ICartService cartService, IPaymentService paymentService, IPurchaseService purchaseService, RabbitMqProducerService producerService, ILogger<PaymentController> logger)
        {
            _iyzicoOptions = iyzicoOptions;
            _userManager = userManager;
            this.cartService = cartService;
            this.paymentService = paymentService;
            this.purchaseService = purchaseService;
            this.producerService = producerService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Pay()
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Challenge();

            string? userName = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userName))
                return Challenge();

            AppUser? user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                TempData["PaymentError"] = "Authenticated user could not be found at SuccessPayment.";
                return RedirectToAction("FailPayment");
            }

            Result<IEnumerable<CartDTO>> carts = await cartService.FindAsync(x=>x.AppUserId == user.Id && x.IsActive,tracking:false, includes: ["Item", "Item.Category"] );

            CheckoutViewModel model = new CheckoutViewModel();
            model.PaidPrice = carts.Data.Sum(x => x.TotalPrice);
            model.Price = carts.Data.Sum(x => x.TotalPrice);

            model.CartItems = carts.Data;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Pay(CheckoutViewModel model)
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Challenge();

            string? userName = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userName))
                return Challenge();

            AppUser? user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return Unauthorized("User account could not be found.");

            Result<IEnumerable<CartDTO>> carts = await cartService.FindAsync(x => x.AppUserId == user.Id && x.IsActive, includes: ["Item", "Item.Category"]);
            model.PaidPrice = carts.Data.Sum(x => x.TotalPrice);
            model.Price = carts.Data.Sum(x => x.TotalPrice);
            model.CartItems = carts.Data;

            var options = new Options()
            {
                ApiKey = _iyzicoOptions.Value.ApiKey,
                BaseUrl = _iyzicoOptions.Value.BaseUrl,
                SecretKey = _iyzicoOptions.Value.SecretKey
            };

            var request = new CreateCheckoutFormInitializeRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = user.Id.ToString(),
                Price = model.Price.ToString("0.00", CultureInfo.InvariantCulture),
                PaidPrice = model.PaidPrice.ToString("0.00", CultureInfo.InvariantCulture),
                Currency = Currency.TRY.ToString(),
                BasketId = Guid.NewGuid().ToString(),
                CallbackUrl = _iyzicoOptions.Value.CallbackUrl,
                PaymentGroup = PaymentGroup.PRODUCT.ToString()

            };

            request.Buyer = new Buyer
            {
                Id = User.Identity.Name,
                Name = model.Firstname,
                Surname = model.Lastname,
                GsmNumber = model.PhoneNumber,
                Email = model.Email,
                IdentityNumber = "23532326344",
                RegistrationAddress = model.Address,
                Ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "7109.0.0.1",
                City = model.City,
                Country = model.Country
            };

            HttpContext.Session.SetString("BillingEmail", model.Email ?? string.Empty);
            TempData["BillingEmail"] = model.Email ?? string.Empty;

            request.ShippingAddress = new Address
            {
                ContactName = string.Concat(model.Firstname," ",model.Lastname),
                City = model.City,
                Country = model.Country,
                Description = "Delivery Address"
            };

            request.BillingAddress = new Address
            {
                ContactName = string.Concat(model.Firstname, " ", model.Lastname),
                City = model.City,
                Country = model.Country,
                Description = "Delivery Address"
            };

            request.BasketItems = carts.Data.Select(cart => new BasketItem
            {
                Id = cart.ItemId.ToString(),
                Name = cart.Title?? string.Empty,
                Category1 = cart.CategoryName ?? string.Empty,
                Category2 = cart.CategoryName ?? string.Empty,
                ItemType = BasketItemType.PHYSICAL.ToString(),
                Price = cart.TotalPrice.ToString("0.00", CultureInfo.InvariantCulture),
            }).ToList();
        
 

            var checkoutInitialize = await CheckoutFormInitialize.Create(request, options);

            string status = (checkoutInitialize?.Status ?? string.Empty).Trim();
            bool isSuccess = string.Equals(status, "success", StringComparison.OrdinalIgnoreCase)
                             || checkoutInitialize?.StatusCode == 200;

            if (isSuccess && !string.IsNullOrWhiteSpace(checkoutInitialize?.CheckoutFormContent))
            {
                ViewBag.CheckoutFormContent = checkoutInitialize.CheckoutFormContent;
                return View(model);
            }

            ViewBag.Error = checkoutInitialize is null
                ? "Checkout initialization returned null response."
                : $"{checkoutInitialize.ErrorMessage ?? "Checkout initialization failed."} (Status: {checkoutInitialize.Status}, StatusCode: {checkoutInitialize.StatusCode})";

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Callback()
        {
            var token = Request.Form["token"].ToString();

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["PaymentError"] = "Payment callback token was empty.";
                _logger.LogWarning("Payment callback token is empty.");
                return RedirectToAction("FailPayment");
            }

            var options = new Options()
            {
                ApiKey = _iyzicoOptions.Value.ApiKey,
                BaseUrl = _iyzicoOptions.Value.BaseUrl,
                SecretKey = _iyzicoOptions.Value.SecretKey
            };

            var request = new RetrieveCheckoutFormRequest
            {
                Token = token,
            };

           

            var checkoutForm = await CheckoutForm.Retrieve(request, options);

            // Iyzipay CheckoutForm in this package version may not expose Buyer directly.
            // Keep callback-safe fallback logic without using checkoutForm.Buyer.
            string? callbackBillingEmail = null;

            if (checkoutForm?.PaymentItems != null)
            {
                foreach (var paymentItem in checkoutForm.PaymentItems)
                {
                    // If a future Iyzipay package/version adds accessible email-like fields,
                    // this block can be extended. For now, keep compile-safe behavior.
                }
            }

            if (!string.IsNullOrWhiteSpace(callbackBillingEmail))
            {
                HttpContext.Session.SetString("BillingEmail", callbackBillingEmail);
                TempData["BillingEmail"] = callbackBillingEmail;
                _logger.LogInformation("Billing email captured from callback provider response: {BillingEmail}", callbackBillingEmail);
            }

            string status = (checkoutForm?.Status ?? string.Empty).Trim();
            string paymentStatus = (checkoutForm?.PaymentStatus ?? string.Empty).Trim();
            string errorCode = (checkoutForm?.ErrorCode ?? string.Empty).Trim();
            string errorMessage = (checkoutForm?.ErrorMessage ?? string.Empty).Trim();
            string conversationId = (checkoutForm?.ConversationId ?? string.Empty).Trim();

            TempData["PaymentDebug"] =
                $"Status={status} | PaymentStatus={paymentStatus} | ErrorCode={errorCode} | ErrorMessage={errorMessage} | ConversationId={conversationId} | Token={token}";

            _logger.LogInformation(
                "Iyzipay callback result. Status: {Status}, PaymentStatus: {PaymentStatus}, ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}, ConversationId: {ConversationId}, Token: {Token}",
                status,
                paymentStatus,
                errorCode,
                errorMessage,
                conversationId,
                token
            );

            bool isStatusSuccess = string.Equals(status, "success", StringComparison.OrdinalIgnoreCase);
            bool isPaymentSuccess = string.Equals(paymentStatus, "SUCCESS", StringComparison.OrdinalIgnoreCase)
                                    || string.Equals(paymentStatus, "success", StringComparison.OrdinalIgnoreCase);

            if (isStatusSuccess && isPaymentSuccess)

            {
                TempData["PaymentSuccess"] = true;
                TempData["PaymentToken"] = token;
                return RedirectToAction("SuccessPayment");
            }

            TempData["PaymentError"] = string.IsNullOrWhiteSpace(errorMessage)
                ? $"Payment provider returned unsuccessful status. Status={status}, PaymentStatus={paymentStatus}, ErrorCode={errorCode}"
                : errorMessage;

            return RedirectToAction("FailPayment");

        }
        public async Task<IActionResult> SuccessPayment()
        {
            if (!(TempData["PaymentSuccess"] as bool? ?? false))
            {
                TempData["PaymentError"] = "Payment success callback marker was not found.";
                return RedirectToAction("FailPayment");
            }

            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Challenge();

            string? userName = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userName))
                return Challenge();

            AppUser? user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return Unauthorized("User account could not be found.");

            Result<IEnumerable<CartDTO>> carts = await cartService.FindAsync(x => x.AppUserId == user.Id && x.IsActive, tracking: false, includes: ["Item", "Item.Category"]);

            // Create Payment record
            PaymentDTO payment = new PaymentDTO
            {
                AppUserId = user.Id,
                Amount = carts.Data.Sum(x => x.TotalPrice),
                Status = PaymentStatus.Success,
                TransactionId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                PaymentDay = DateTime.Now
            };
            var paymentResult = await paymentService.AddAsync(payment);

            if (paymentResult.Success)
            {
                // Create Purchase records
                IEnumerable<PurchaseDTO> purchases = carts.Data.Select(cart => new PurchaseDTO
                {
                    AppUserId = user.Id,
                    UserName = user.UserName,
                    ItemTitle = cart.Title ?? string.Empty,
                    ItemId = cart.ItemId,
                    PaymentId = paymentResult.Data.Id,
                    PurchaseDate = DateTime.Now,
                    CartId = cart.Id,
                    IsActive = true
                });
                var purchaseResult = await purchaseService.AddRangeAsync(purchases);
                if (purchaseResult.Success)
                {
                    var billingEmail = HttpContext.Session.GetString("BillingEmail");
                    var emailSource = "session";

                    if (string.IsNullOrWhiteSpace(billingEmail))
                    {
                        billingEmail = TempData.Peek("BillingEmail")?.ToString();
                        emailSource = "tempdata";
                    }
                    if (string.IsNullOrWhiteSpace(billingEmail))
                    {
                        billingEmail = user.Email;
                        emailSource = "account-fallback";
                    }

                    _logger.LogInformation("Invoice recipient email selected: {BillingEmail} (Source: {Source}, UserEmail: {UserEmail})", billingEmail, emailSource, user.Email);

                    InvoiceViewModel invoiceModel = new InvoiceViewModel
                    {
                        CustomerAddress = user.Address ?? "Ankara/Yenimahalle",
                        CustomerMail = billingEmail,
                        CustomerName = user.FullName ?? "Ad Soyad",
                        InvoiceDate = DateTime.Now,
                        InvoiceNumber = $"INV-{DateTime.Now:yyyyMMddHHmmss}",
                    };

                    foreach (var cart in carts.Data)
                    {
                        invoiceModel.Items.Add(new InvoiceItem
                        {
                            Title = cart.Title ?? string.Empty,
                            Description = cart.Description ?? string.Empty,
                            Quantity = cart.Quantity,
                            UnitPrice = cart.Price
                        });

                        cart.IsActive = false;
                        await cartService.Update(cart);
                    }

                    try
                    {
                        await producerService.SendMessageAsync(invoiceModel);
                        HttpContext.Session.Remove("BillingEmail");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to publish invoice to RabbitMQ for user {UserId}, payment {PaymentId}", user.Id, paymentResult.Data.Id);
                    }

                    return View();
                }
            }

            TempData["PaymentError"] = "Payment was captured but post-payment operations failed.";
            return RedirectToAction("FailPayment");

             
        }

        public IActionResult FailPayment()
        {
            return View();
        }


        
    }

}
