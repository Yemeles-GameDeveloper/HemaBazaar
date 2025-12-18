using Application.ViewModels;
using HemaBazaar.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Options = Iyzipay.Options;
using Microsoft.EntityFrameworkCore.Metadata;
using Application.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Application.Interfaces;
using Application.Common;

namespace HemaBazaar.MVC.Controllers
{
    public class PaymentController : Controller
    {
        IOptions<IyzicoOptions> _iyzicoOptions;
        ICartService cartService;
        UserManager<AppUser> _userManager;
        CartDTO cartDTO;
        public PaymentController(IOptions<IyzicoOptions> iyzicoOptions, UserManager<AppUser> userManager, ICartService cartService, CartDTO cartDTO)
        {
            _iyzicoOptions = iyzicoOptions;

            _userManager = userManager;
            this.cartService = cartService;
            this.cartDTO = cartDTO;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Pay()
        {
           AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
          Result<IEnumerable<CartDTO>> carts = await cartService.FindAsync(x=>x.AppUserId == user.Id && x.IsActive, includes: ["Item"] );

            CheckoutViewModel model = new CheckoutViewModel();
            model.PaidPrice = carts.Data.Sum(x => x.TotalPrice);
            model.Price = carts.Data.Sum(x => x.TotalPrice);
           

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Pay(CheckoutViewModel model)
        {
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
                Price = model.Price.ToString("0.00"),
                PaidPrice = model.PaidPrice.ToString("0.00"),
                Currency = Currency.USD.ToString(),
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

            request.BasketItems = new List<BasketItem>
            {
                new BasketItem
                {
                    Id = cartDTO.Id.ToString(),
                    Name = cartDTO.Title,
                    Category1 = cartDTO.CategoryName,
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = model.Price.ToString("0.00"),
                }
            };

            var checkoutInitialize = await CheckoutFormInitialize.Create(request,options);

            if(checkoutInitialize.Status == "Success")
            {
                ViewBag.CheckoutFormContent = checkoutInitialize.CheckoutFormContent;

                return View();
            }
            ViewBag.Error = checkoutInitialize.ErrorMessage;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Callback()
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

            var checkoutFrom = await CheckoutForm.Retrieve(request, options);

            if (checkoutFrom.Status == "success" && checkoutFrom.Status == "SUCCESS")
            {
                return RedirectToAction("SuccessPayment");
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
    }

}
