using Microsoft.AspNetCore.Mvc;

namespace HemaBazaar.MVC.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
