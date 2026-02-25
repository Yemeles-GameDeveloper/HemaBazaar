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
        public async Task<IActionResult> Index()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
          Result<IEnumerable<CartDTO>> carts = await _cartService.FindAsync(x=>x.AppUserId == user.Id && x.IsActive,tracking:false, includes:["Item","Item.Category"]);
            return View(carts.Data);
            
        }
        [HttpPost]
        public async Task<IActionResult> AddCart(int itemId, int quantity)
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Unauthorized("Please log in to add items to your cart.");

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return Unauthorized("User account could not be found.");

            using HttpClient httpClient = new HttpClient();
            HttpResponseMessage result = await httpClient.PostAsync(
                $"https://localhost:7293/api/Cart/add?itemId={itemId}&quantity={quantity}&userId={user.Id}",
                null);

            if (result.IsSuccessStatusCode)
                return Ok("Item added to cart successfully.");
            else
                return BadRequest("Item could not be added to cart.");
        }
       [HttpPost]
       public async Task<IActionResult> RemoveCart (int cartid)
       {
            
            
                Result<CartDTO> cart = await _cartService.GetByIdAsync(cartid,false);
                if (!cart.Success || cart.Data == null)
                {
                    return NotFound("Cart item could not be found.");
                }


                Result<CartDTO> removeResult = await _cartService.Remove(cart.Data);
                if (removeResult.Success)
                {
                    return Ok("Item removed from the cart successfully.");
                }
                else
                {
                    return BadRequest("Item could not be removed from the cart");
                }
            
            

        }

        
    }
}
