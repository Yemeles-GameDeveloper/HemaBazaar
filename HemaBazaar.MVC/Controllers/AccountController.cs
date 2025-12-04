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

        public AccountController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Login()
        {
            return View();
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
            {
                return View(model);
            }

            var user = new AppUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.UserName
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            // HATA VARSA BURADA GÖRELİM
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }




    }
}
