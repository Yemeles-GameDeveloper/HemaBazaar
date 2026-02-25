using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HemaBazaar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly UserManager<AppUser> _userManager;

        public CartController(ICartService cartService, UserManager<AppUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }

        [HttpPost("add")]
        public async Task<IActionResult> CartAdd(int itemId, int quantity, int userId)
        {
            if (quantity < 1)
                return BadRequest("Quantity must be at least 1.");

            AppUser? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return BadRequest("User account could not be found.");

            // Use AppUserId (FK) to avoid navigation property null ref
            Result<IEnumerable<CartDTO>> result = await _cartService.FindAsync(
                x => x.AppUserId == userId && x.ItemId == itemId && x.IsActive,
                tracking: false);

            Result<CartDTO> cartResult;

            if (result.Success && result.Data != null && result.Data.Any())
            {
                CartDTO cart = result.Data.First();
                cart.Quantity += quantity;
                cartResult = await _cartService.Update(cart);
            }
            else
            {
                CartDTO cartDTO = new CartDTO
                {
                    AppUserId = userId,
                    ItemId = itemId,
                    Quantity = quantity,
                    IsActive = true
                };
                cartResult = await _cartService.AddAsync(cartDTO);
            }

            return cartResult.Success ? Ok(cartResult) : BadRequest(cartResult);
        }
    }
}
