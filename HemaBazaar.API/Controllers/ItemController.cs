using Application.Interfaces;
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
        public async Task <IActionResult> GetItems()
        {
            return Ok(( await _itemService.GetAllAsync(includes:"Category")).Data);
        }
    }
}
