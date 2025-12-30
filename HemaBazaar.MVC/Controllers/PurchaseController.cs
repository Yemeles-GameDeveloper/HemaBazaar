using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HemaBazaar.MVC.Controllers
{
    public class PurchaseController : Controller
    {
        IPurchaseService purchaseService;
        UserManager<AppUser> userManager;
        public PurchaseController(IPurchaseService purchaseService, UserManager<AppUser> userManager)
        {
            this.purchaseService = purchaseService;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            AppUser user = await userManager.GetUserAsync(User);
          Result<IEnumerable<PurchaseDTO>> items = await purchaseService.GetAllAsync(x => x.AppUserId == user.Id && x.IsActive, includes: ["Item", "Item.Category"]);
            return View(items.Data);
        }
    }

    //2:08 den devam et. Siparişleri göstermiyor.
}

