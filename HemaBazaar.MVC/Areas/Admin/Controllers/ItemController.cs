using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HemaBazaar.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ItemController : Controller
    {
        IItemService _itemService;
        ICategoryService _categoryService;

        public ItemController(IItemService itemService, ICategoryService categoryService)
        {
            _itemService = itemService;
            _categoryService = categoryService;
        }

        public async Task <IActionResult> Index()
        {
           Result<IEnumerable<CategoryDTO>> result = await _categoryService.GetAllAsync();
            ViewBag.Categories = result.Data;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            Result<IEnumerable<ItemDTO>> result = await _itemService.GetAllAsync(
                filter: c => c.IsActive == true,
                tracking: false,
                includes: "Category"
            );
            return Json(new { data = result.Data });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Result<ItemDTO> item = await _itemService.GetByIdAsync(id);
            if (item.Data == null || !item.Success)
                return Json(new { success = false, message = "Item cannot be found." });

            Result<ItemDTO> result = await _itemService.Remove(item.Data);

            if (result.Success)
                return Json(new { success = true, message = "Item is deleted." });
            else
                return Json(new { success = false, message = "Item cannot be deleted." });
        }

        [HttpPost]
        public async Task<IActionResult> Get(int id)
        {
            Result<ItemDTO> item = await _itemService.GetByIdAsync(id);
            if (item.Data == null || !item.Success)
                return Json(new { success = false, message = "Item cannot be found." });
            else
                return Json(new { success = true, data = item.Data });
        }

        [HttpPost]
        public async Task<IActionResult> Create(string title, string content, string description, decimal price, int categoryId)
        {
            if (string.IsNullOrEmpty(title))
                return Json(new { success = false, message = "Title cannot be empty." });

            ItemDTO item = new ItemDTO
            {
                Title = title,
                Content = content, 
                Description = description, 
                Price = price,
                CategoryId = categoryId

            };
            Result<ItemDTO> result = await _itemService.AddAsync(item);

            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, string title, string content, string description, decimal price, int categoryId)
        {

            Result<ItemDTO> item = await _itemService.GetByIdAsync(id, tracking: false);

            if (item.Data == null || !item.Success)
                return Json(new { success = false, message = "Item cannot be found." });
            item.Data.Title = title;
            item.Data.Content = content;
            item.Data.Description = description;
            item.Data.Price = price;
            item.Data.CategoryId = categoryId;
            Result<ItemDTO> result = await _itemService.Update(item.Data);

            return Json(new { success = result.Success, message = result.Message });
        }

        //27 Kasım 2:41:00 dan devam et.
    }
}
