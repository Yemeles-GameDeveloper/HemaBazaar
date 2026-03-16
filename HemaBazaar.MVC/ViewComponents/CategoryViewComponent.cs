using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HemaBazaar.MVC.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        ICategoryService _categoryService;

        public CategoryViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
           Result<IEnumerable<CategoryDTO>> categories =  await _categoryService.GetAllAsync();
            return View(categories.Data);


        }
    }
}
