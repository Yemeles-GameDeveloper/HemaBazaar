using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using HemaBazaar.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HemaBazaar.MVC.Controllers
{
    public class HomeController : Controller
    {
       
        private readonly ILogger<HomeController> _logger;

        IItemService itemService;

        public HomeController(ILogger<HomeController> logger, IItemService itemService)
        {
            _logger = logger;
            this.itemService = itemService;
        }

        public async Task<IActionResult> Index()
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

            var result = await itemService.GetAllAsync(null, OrderType.ASC, "Category");

            // BURAYA BREAKPOINT KOY
            var list = (result.Data ?? Enumerable.Empty<ItemDTO>()).ToList();
            var count = list.Count; // Debug penceresinde count’a bak

            return View(list);
        }




    }
}
