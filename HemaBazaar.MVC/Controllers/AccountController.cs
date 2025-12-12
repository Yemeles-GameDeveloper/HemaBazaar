
using Application.ViewModels;
using AutoMapper;
using Domain.Entities;
using HemaBazaar.MVC.Models;
using Microsoft.AspNetCore.Authorization;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Hesap çok fazla hatalı denemeden dolayı kilitlendi.");
                return View(model);
            }

            if (result.IsNotAllowed)
            {
                // Kullanıcıyı bulalım ki email’i view’e gönderebilelim
                var user = await _userManager.FindByNameAsync(model.UserName);

                // Email doğrulaması tamamlanmamış → yeni view’e yönlendir
                return RedirectToAction("EmailVerifyRequired", new { email = user?.Email });
            }

            if (result.RequiresTwoFactor)
            {
                ModelState.AddModelError("", "Bu hesap için iki faktörlü doğrulama gerekiyor.");
                return View(model);
            }

            ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
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

        [HttpGet]
        public IActionResult EmailVerifyRequired(string email)
        {
            return View(model: email);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailConfirmation(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Message"] = "Email address is required to resend verification.";
                return RedirectToAction("Login");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                TempData["Message"] = "No account found for that email.";
                return RedirectToAction("Login");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = token }, Request.Scheme);

            await new EmailProcess(_config).SendEmail(
                "Email Verification",
                $"Please verify your email <a href='{link}'>by clicking here</a>.",
                emailAddresses: user.Email
            );

            TempData["Message"] = "Verification email resent.";
            return RedirectToAction("EmailVerifyRequired", new { email });
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user == null)
                return View(model);

           string token = await _userManager.GeneratePasswordResetTokenAsync(user);

           string resetLink =  Url.Action("ResetPassword","Account", new {token = token, email=model.Email}, Request.Scheme);

           await new EmailProcess(_config).SendEmail("Password Reset Link", $"<a href='{resetLink}'>Click to reset the password.</a>", emailAddresses: user.Email);

            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel { Email = email, Token = token };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);

            AppUser user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return View(model);

           var result = await _userManager.ResetPasswordAsync(user,model.Token,model.NewPassword);
            if (result.Succeeded)
                return View("ResetPasswordConfirmation");

            return View();
        }
        [Authorize]
        [HttpGet]
       
        public async Task<IActionResult> Profile()
        {
            AppUser user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login");
            ProfileUpdateViewModel model = _mapper.Map<ProfileUpdateViewModel>(user);
            return View(model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileUpdateViewModel model)
        {

            AppUser user = await _userManager.GetUserAsync(User);
            

            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
                return View(model);

            user.FullName = model.FullName;
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;

            IdentityResult setUserNameResult = await _userManager.SetUserNameAsync(user, model.UserName);
            AddErrors(setUserNameResult);

            IdentityResult setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
            AddErrors(setEmailResult);

            if (!setUserNameResult.Succeeded || !setEmailResult.Succeeded)
            {
                ProfileUpdateViewModel erroredModel = _mapper.Map<ProfileUpdateViewModel>(user);
                return View(erroredModel);
            }

            IdentityResult updateResult = await _userManager.UpdateAsync(user);
            AddErrors(updateResult);

            if (!updateResult.Succeeded)
            {
                ProfileUpdateViewModel erroredModel = _mapper.Map<ProfileUpdateViewModel>(user);
                return View(erroredModel);
            }

            bool wantsToChangePassword =
                !string.IsNullOrWhiteSpace(model.CurrentPassword) ||
                !string.IsNullOrWhiteSpace(model.NewPassword) ||
                !string.IsNullOrWhiteSpace(model.ConfirmPassword);

            if (wantsToChangePassword)
            {
                IdentityResult passwordResult = await _userManager.ChangePasswordAsync(
                    user,
                    model.CurrentPassword,
                    model.NewPassword);

                AddErrors(passwordResult);

                if (!passwordResult.Succeeded)
                {
                    ProfileUpdateViewModel erroredModel = _mapper.Map<ProfileUpdateViewModel>(user);
                    return View(erroredModel);
                }
            }

            ViewData["Message"] = "Profile updated successfully.";
            ModelState.Clear();

            ProfileUpdateViewModel updatedModel = _mapper.Map<ProfileUpdateViewModel>(user);
            return View(updatedModel);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }

    

  

}

