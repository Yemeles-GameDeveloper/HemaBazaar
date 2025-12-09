using HemaBazaar.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HemaBazaar.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            if (User.Identity.IsAuthenticated)
            {
                Response.Cookies.Append("userName", User.Identity.Name, cookieOptions);
            }

            return View();
        }

      
    }
}
