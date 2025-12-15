using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HemaBazaar.MVC.Controllers
{
    public class CartController : Controller
    {
        ICartService _cartService;
        UserManager<AppUser> _userManager;
        public CartController(ICartService cartService, UserManager<AppUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCart(int itemId)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            CartDTO cartDTO = new CartDTO();
            cartDTO.AppUserId = user.Id;
            cartDTO.ItemId = itemId;
            cartDTO.Quantity = 1;
           Result<CartDTO> result = await _cartService.AddAsync(cartDTO);
            if (result.Success)
                return Ok("Item added to cart successfully.");
            else
                return BadRequest("Item could not added to cart");
            
        }
    }
}
