using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HemaBazaar.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task <IActionResult> GetAll()
        {
            Result <IEnumerable<CategoryDTO>> result = await _categoryService.GetAllAsync(
                filter: c => c.IsActive == true, 
                tracking: false
            );
            return Json(new {data = result.Data});
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Result <CategoryDTO> category = await _categoryService.GetByIdAsync(id);
            if (category.Data == null || !category.Success)
                return Json(new {success = false, message="Category cannot be found."});
            
            Result<CategoryDTO> result = await _categoryService.Remove(category.Data);
            
            if(result.Success)
                return Json(new {success = true, message="Category is deleted."});
            else
                return Json(new { success = false, message = "Category cannot be deleted." });
        }

        [HttpPost]
        public async Task<IActionResult> Get(int id)
        {
            Result<CategoryDTO> category = await _categoryService.GetByIdAsync(id);
            if (category.Data == null || !category.Success)
                return Json(new { success = false, message = "Category cannot be found." });
            else
                return Json(new { success = false,data=category.Data});
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (string.IsNullOrEmpty(name))
                return Json(new { success = false, message = "Category Name cannot be empty." });
            CategoryDTO category = new CategoryDTO
            {
                CategoryName = name
            };
           Result<CategoryDTO> result = await _categoryService.AddAsync(category);

            return Json(new { success = result.Success, message=result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, string name)
        {

           Result<CategoryDTO> category = await _categoryService.GetByIdAsync(id, tracking:false);

            if (category.Data == null || !category.Success)
                return Json(new { success = false, message = "Category Name cannot be found." });
            category.Data.CategoryName = name;
            Result<CategoryDTO> result = await _categoryService.Update(category.Data);

            return Json(new { success = result.Success, message = result.Message });
        }
    }
}
