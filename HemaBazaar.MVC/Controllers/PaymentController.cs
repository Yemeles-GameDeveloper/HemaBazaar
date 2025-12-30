using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Application.ViewModels;
using Domain.Entities;
using Domain.Enums;
using HemaBazaar.MVC.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Threading.Tasks;
using Options = Iyzipay.Options;

namespace HemaBazaar.MVC.Controllers
{
    public class PaymentController : Controller
    {
        IPaymentService paymentService;
        IPurchaseService purchaseService;
        IOptions<IyzicoOptions> _iyzicoOptions;
        ICartService cartService;
        UserManager<AppUser> _userManager;
        CartDTO cartDTO;

        public PaymentController(IOptions<IyzicoOptions> iyzicoOptions, UserManager<AppUser> userManager, ICartService cartService, IPurchaseService purchaseService, IPaymentService paymentService)
        {
            _iyzicoOptions = iyzicoOptions;

            _userManager = userManager;
            this.cartService = cartService;
            this.purchaseService = purchaseService;
            this.paymentService = paymentService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Pay()
        {
           AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
          Result<IEnumerable<CartDTO>> carts = await cartService.FindAsync(x=>x.AppUserId == user.Id && x.IsActive, includes: ["Item", "Item.Category"] );

            CheckoutViewModel model = new CheckoutViewModel();
            model.PaidPrice = carts.Data.Sum(x => x.TotalPrice);
            model.Price = carts.Data.Sum(x => x.TotalPrice);

            model.CartItems = carts.Data;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Pay(CheckoutViewModel model)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            var conversationId = Guid.NewGuid().ToString();

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
                ConversationId = Guid.NewGuid().ToString(),
                Price = model.Price.ToString("0.00", CultureInfo.InvariantCulture),
                PaidPrice = model.PaidPrice.ToString("0.00", CultureInfo.InvariantCulture),
                Currency = Currency.TRY.ToString(),
                BasketId = Guid.NewGuid().ToString(),
                CallbackUrl = Url.Action("Callback", "Payment", new { id = user.Id.ToString(), transactionId = conversationId }, Request.Scheme),
                PaymentGroup = PaymentGroup.PRODUCT.ToString()

            };



            PaymentDTO payment = new PaymentDTO
            {
                AppUserId = user.Id,
                Amount = model.Price,
                PaymentDate = DateTime.Now,
                Status = PaymentStatus.Pending,
                TransactionId = conversationId,
            };

            await paymentService.AddAsync(payment);

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
        
 

             var checkoutInitialize = await CheckoutFormInitialize.Create(request,options);

            if (string.Equals(checkoutInitialize.Status, "success", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.CheckoutFormContent = checkoutInitialize.CheckoutFormContent;

                return View(model);
            }
            ViewBag.Error = checkoutInitialize.ErrorMessage;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Callback(string id, string transactionId)
        {
            var token = Request.Form["token"].ToString();

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

            IEnumerable<CartDTO> carts = (await cartService.GetAllAsync(x => x.AppUserId == int.Parse(id) && x.IsActive)).Data;

            PaymentDTO payment = (await paymentService.FindAsync(x => x.TransactionId == transactionId && x.IsActive)).Data.FirstOrDefault();


            var checkoutFrom = await CheckoutForm.Retrieve(request, options);

            if (string.Equals(checkoutFrom.Status, "success", StringComparison.OrdinalIgnoreCase))
            {
                payment.Status = PaymentStatus.Completed;

                foreach (var cart in carts)
                {
                    cart.IsActive = false;
                    cartService.Update(cart);
                    purchaseService.AddAsync(new PurchaseDTO { AppUserId = int.Parse(id), ItemId = cart.ItemId, PurchaseDate = DateTime.Now, PaymentId = payment.Id });
                }
                return RedirectToAction("SuccessPayment");
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
                await paymentService.Update(payment);
            }
            return RedirectToAction("FailPayment");

        }
        public IActionResult SuccessPayment()
        {
            return View();
        }

        public IActionResult FailPayment()
        {
            return View();
        }

        // 26 Kasım 1:04:00
        
    }

}
