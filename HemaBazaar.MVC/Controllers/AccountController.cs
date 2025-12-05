using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using HemaBazaar.MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HemaBazaar.MVC.Controllers
{
    public class AccountController : Controller
    {
        UserManager<AppUser> _userManager;
        SignInManager<AppUser> _signInManager;
        IMapper _mapper;
        IConfiguration _config;


        public AccountController(UserManager<AppUser> userManager, IMapper mapper, SignInManager<AppUser> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
            _config = config;
        }

        [HttpGet]
        public IActionResult Login()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Username or password is wrong");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1) Kullanıcı adı zaten var mı?
            var existingUser = await _userManager.FindByNameAsync(model.UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError(nameof(model.UserName), "Bu kullanıcı adı zaten alınmış.");
                return View(model);
            }

            // 2) Email zaten var mı? (İstersen)
            var existingEmailUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmailUser != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Bu e-posta ile kayıtlı bir hesap zaten var.");
                return View(model);
            }

            // 3) Map + create
            var user = _mapper.Map<AppUser>(model);

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {

               var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var verificationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                await new EmailProcess(_config).SendEmail("Email Verification", $"To verify your account <a href='{verificationLink}'> click here.</a> ", emailAddresses:user.Email);
                // ileride email onay vs yaparsın
                return RedirectToAction("EmailVerification",new {enail = user.Email});
            }

            // 4) Identity’den gelen hataları göster
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult EmailVerification(string email)
        {
            return View(model: email);
        }



        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
           AppUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return BadRequest();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
                return View("EmailConfirmed");

            return BadRequest();
            
        }

        public  async Task<IActionResult> Logout()
        {
           await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }


    //19 Kasım 02:58:00 Register iki kez girildiğinde hata ekranına yolluyor bunu düzelt ve Login sayfasından sonra emailConfiguration sayfası oluşturup oraya yönlendirsin ve login olduğu zaman üstteki şeyler değişsin.

}

