using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using HemaBazaar.MVC.Models;
using HemaBazaar.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HemaBazaar.MVC.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        ICartService _cartService;
        UserManager<AppUser> _userManager;
        ApiClient _apiClient;
        TokenServices _tokenServices;
        MvcJwtTokenService _mvcJwtTokenService;

        public CartController(
            ICartService cartService,
            UserManager<AppUser> userManager,
            ApiClient apiClient,
            TokenServices tokenServices,
            MvcJwtTokenService mvcJwtTokenService)
        {
            _cartService = cartService;
            _userManager = userManager;
            _apiClient = apiClient;
            _tokenServices = tokenServices;
            _mvcJwtTokenService = mvcJwtTokenService;
        }
        public async Task<IActionResult> Index()
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Challenge();

            string? userName = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userName))
                return Challenge();

            AppUser? user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return Unauthorized("User account could not be found.");

            Result<IEnumerable<CartDTO>> carts = await _cartService.FindAsync(
                x => x.AppUserId == user.Id && x.IsActive,
                tracking: false,
                includes: ["Item", "Item.Category"]);

            return View(carts.Data);
        }
        [HttpPost]
        public async Task<IActionResult> AddCart(int itemId, int quantity)
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Unauthorized("Please log in to add items to your cart.");

            string? userName = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(userName))
                return Unauthorized("Please log in to add items to your cart.");

            AppUser? user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return Unauthorized("User account could not be found.");

            // Hard fallback: if no valid API token is available, mint one from current identity
            // and persist it so ApiClient can always attach Bearer token.
            string? currentToken = await _tokenServices.GetValidTokenAsync(User);
            if (string.IsNullOrWhiteSpace(currentToken))
            {
                var mintedToken = _mvcJwtTokenService.CreateToken(user);
                _tokenServices.StoreToken(mintedToken);

                Response.Cookies.Append("access_token", mintedToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });
            }

            HemaBazaar.MVC.Models.HttpResponse result = await _apiClient.PostStatusAsync(
                $"Cart/add?itemId={itemId}&quantity={quantity}&userId={user.Id}",
                new CartRequestModel { ItemId = itemId, UserId = user.Id, Quantity = quantity });

            if (result.IsSuccessStatusCode)
                return Ok("Item added to cart successfully.");

            var apiError = string.IsNullOrWhiteSpace(result.Content)
                ? $"Item could not be added to cart. API Status: {result.StatusCode}"
                : $"Item could not be added to cart. API Status: {result.StatusCode}. Details: {result.Content}";

            return BadRequest(apiError);
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
            
            // 3 Kasım 0:53:00

        }

        
    }
}
