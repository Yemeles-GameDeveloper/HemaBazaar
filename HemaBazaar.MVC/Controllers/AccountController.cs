
using Application.ViewModels;
using AutoMapper;
using Domain.Entities;
using HemaBazaar.MVC.Models;
using HemaBazaar.MVC.Services;
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
        TokenServices _tokenServices;
        IHttpClientFactory _httpClientFactory;

        public AccountController(
            UserManager<AppUser> userManager,
            IMapper mapper,
            SignInManager<AppUser> signInManager,
            IConfiguration config,
            TokenServices tokenServices,
            IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
            _config = config;
            _tokenServices = tokenServices;
            _httpClientFactory = httpClientFactory;
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
            {
                // Fetch JWT from API and store in session so ApiClient can use it.
                await FetchAndStoreApiTokenAsync(model.UserName, model.Password);
                await HttpContext.Session.CommitAsync();
                return RedirectToAction("Index", "Home");
            }

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
               IdentityResult roleResult = await _userManager.AddToRoleAsync(user, "UserApp");
               

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
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            _tokenServices.ClearToken();
            Response.Cookies.Delete("access_token");
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Calls the API login endpoint to obtain a JWT and stores it in the session.
        /// Failures are silently swallowed so the MVC Identity login still succeeds.
        /// </summary>
        private async Task FetchAndStoreApiTokenAsync(string username, string password)
        {
            try
            {
                var apiBaseUrl = _config["ApiBaseUrl"] ?? "https://localhost:7293";
                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsJsonAsync(
                    $"{apiBaseUrl}/api/Auth/login",
                    new { Username = username, Password = password });

                if (response.IsSuccessStatusCode)
                {
                    // Primary source: token returned in JSON body.
                    var tokenResponse = await response.Content.ReadFromJsonAsync<JwtTokenResponseModel>(
                        new System.Text.Json.JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    if (!string.IsNullOrWhiteSpace(tokenResponse?.Token))
                    {
                        _tokenServices.StoreToken(tokenResponse.Token);
                        Response.Cookies.Append("access_token", tokenResponse.Token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = Request.IsHttps,
                            SameSite = SameSiteMode.Lax,
                            Expires = tokenResponse.ExpireDate
                        });
                        return;
                    }

                    // Fallback source: access_token cookie set by API login response.
                    if (response.Headers.TryGetValues("Set-Cookie", out var setCookieValues))
                    {
                        foreach (var setCookie in setCookieValues)
                        {
                            const string cookiePrefix = "access_token=";
                            var startIndex = setCookie.IndexOf(cookiePrefix, StringComparison.OrdinalIgnoreCase);
                            if (startIndex < 0)
                                continue;

                            startIndex += cookiePrefix.Length;
                            var endIndex = setCookie.IndexOf(';', startIndex);
                            var cookieToken = endIndex > startIndex
                                ? setCookie.Substring(startIndex, endIndex - startIndex)
                                : setCookie.Substring(startIndex);

                            if (!string.IsNullOrWhiteSpace(cookieToken))
                            {
                                _tokenServices.StoreToken(cookieToken);
                                Response.Cookies.Append("access_token", cookieToken, new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = Request.IsHttps,
                                    SameSite = SameSiteMode.Lax,
                                    Expires = DateTimeOffset.UtcNow.AddMinutes(60)
                                });
                                return;
                            }
                        }
                    }
                }
            }
            catch
            {
                // API may not be running; MVC Identity auth still works independently.
            }
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

          

            if(!ModelState.IsValid)
                return View(model);

            AppUser user = await _userManager.GetUserAsync(User);
            if(user.UserName  != model.Email)
            {
                IdentityResult result = await _userManager.SetUserNameAsync(user, model.UserName);
            }

            if(user.Email != model.Email)
            {
                await _userManager.SetEmailAsync(user, model.Email);
                user.EmailConfirmed = true;
            }

            user.FullName = model.FullName;
            user.Address = model.Address;

            await _userManager.UpdateAsync(user);

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
               IdentityResult passresult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (!passresult.Succeeded)
                    return View(model);

                await _signInManager.RefreshSignInAsync(user);
            }

            
            return View(model);
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

