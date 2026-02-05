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
        IPurchaseService _purchaseService;
        UserManager<AppUser> _userManager;
        



        public PurchaseController(IPurchaseService purchaseService, UserManager<AppUser> userManager)
        {
            _purchaseService = purchaseService;
            _userManager = userManager;
            
        }

        public async Task<IActionResult> Index()
        {
            AppUser user = await _userManager.GetUserAsync(User);

          Result<IEnumerable<PurchaseDTO>> result = await _purchaseService.GetAllAsync(x => x.AppUserId == user.Id && x.IsActive, includes: ["Item"]);

            return View(result.Data);
        }
    }
}
