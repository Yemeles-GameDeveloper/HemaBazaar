using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using HemaBazaar.MVC.Models;
using HemaBazaar.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HemaBazaar.MVC.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;

        IItemService itemService;

        IMemoryCache _memoryCache;
        IDistributedCache _distributedCache;
        RedisCacheService<IEnumerable<ItemDTO>> _itemCache;

        ApiClient _apiClient;

        public HomeController(ILogger<HomeController> logger, IItemService itemService, IMemoryCache memoryCache, IDistributedCache distributedCache, RedisCacheService<IEnumerable<ItemDTO>> itemCache, ApiClient apiClient)
        {
            _logger = logger;
            this.itemService = itemService;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            this._itemCache = itemCache;
            _apiClient = apiClient;
        }
        //[ResponseCache(Duration = 50, Location = ResponseCacheLocation.Client)]
        [OutputCache(Duration = 60)]
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





            //Result<IEnumerable<ItemDTO>> items;

            //if (!_memoryCache.TryGetValue("items", out items))
            //{
            //    items = await itemService.GetAllAsync(includes: "Category");

            //    var cacheOptions = new MemoryCacheEntryOptions()
            //        .SetAbsoluteExpiration(TimeSpan.FromSeconds(50));
            //    _memoryCache.Set("items", items, cacheOptions);
            //}


            //return View(items.Data);

            //list

            string cacheKey = "AllItems";
            IEnumerable<ItemDTO> data;
          data = await _itemCache.GetOrSetAsync(cacheKey,
                async () =>
                {
                    var items = await _apiClient.GetAsync<IEnumerable<ItemDTO>>("/Item");
                    return items;
                },
                TimeSpan.FromMinutes(5)
                );

            
            return View(data);


   

        }
    }
}
