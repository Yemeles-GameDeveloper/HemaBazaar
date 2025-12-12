using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;

using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Microsoft.Identity.Client;

namespace HemaBazaar.MVC.Controllers
{
    public class ItemController : Controller
    {
        
         private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            
            _itemService = itemService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _itemService.GetAllAsync(null, OrderType.ASC, "Category");
            var items = result.Data ?? Enumerable.Empty<ItemDTO>();

            return View(items);
        }

        public async Task<IActionResult> Details(int itemId)
        {
          Result<IEnumerable<ItemDTO>> result = await _itemService.GetAllAsync(x => x.Id == itemId && x.IsActive, includes: "Category");
           ItemDTO itemDTO = result.Data.FirstOrDefault();
            return View(itemDTO);
        }
    }
}
