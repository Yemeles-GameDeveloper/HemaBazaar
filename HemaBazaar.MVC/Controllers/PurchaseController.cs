using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using HemaBazaar.MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HemaBazaar.MVC.Controllers
{
    public class PurchaseController : Controller
    {
        readonly IPurchaseService _purchaseService;
        readonly UserManager<AppUser> _userManager;

        public PurchaseController(IPurchaseService purchaseService, UserManager<AppUser> userManager)
        {
            _purchaseService = purchaseService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
            {
                return RedirectToAction("Login", "Account");
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Result<IEnumerable<PurchaseDTO>> result = await _purchaseService.FindAsync(
                x => x.AppUserId == user.Id,
                OrderType.DESC,
                "Item",
                "AppUser");

            var model = new PurchaseViewModel
            {
                Purchases = result.Data?.ToList() ?? new List<PurchaseDTO>()
            };

            return View(model);
        }
    }
}
