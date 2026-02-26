using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HemaBazaar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ItemController : ControllerBase
    {
        IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetItems()
        {
            return Ok((await _itemService.GetAllAsync(includes: "Category")).Data);
        }
    }
}
