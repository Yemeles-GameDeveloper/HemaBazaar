﻿using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using HemaBazaar.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace HemaBazaar.MVC.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;
        private readonly RedisCacheService<IEnumerable<CategoryDTO>> _categoryCache;

        public CategoryViewComponent(
            ICategoryService categoryService,
            RedisCacheService<IEnumerable<CategoryDTO>> categoryCache)
        {
            _categoryService = categoryService;
            _categoryCache = categoryCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cacheKey = "HeaderCategories";

            var categories = await _categoryCache.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    var result = await _categoryService.GetAllAsync();
                    return result.Data ?? Enumerable.Empty<CategoryDTO>();
                },
                TimeSpan.FromMinutes(10));

            return View(categories ?? Enumerable.Empty<CategoryDTO>());
        }
    }
}
