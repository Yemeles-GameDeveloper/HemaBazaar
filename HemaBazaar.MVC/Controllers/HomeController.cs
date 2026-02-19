using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using HemaBazaar.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HemaBazaar.MVC.Controllers
{
    public class HomeController : Controller
    {
       
        private readonly ILogger<HomeController> _logger;

        IItemService itemService;

        IMemoryCache _memoryCache;

        public HomeController(ILogger<HomeController> logger, IItemService itemService, IMemoryCache memoryCache)
        {
            _logger = logger;
            this.itemService = itemService;
            _memoryCache = memoryCache;
        }
        //[ResponseCache(Duration = 50, Location = ResponseCacheLocation.Client)]
        [OutputCache(Duration = 50)]
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

            //var result = await itemService.GetAllAsync(null, OrderType.ASC,true, "Category");

            
            //var list = (result.Data ?? Enumerable.Empty<ItemDTO>()).ToList();
            //var count = list.Count;



            Result<IEnumerable<ItemDTO>> items;

            if(!_memoryCache.TryGetValue("items",out  items))
            {
                items = await itemService.GetAllAsync(includes: "Category");

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(50));
                _memoryCache.Set("items", items,cacheOptions);
            }


            return View(items.Data);

            //list
        }


        

    }
}
