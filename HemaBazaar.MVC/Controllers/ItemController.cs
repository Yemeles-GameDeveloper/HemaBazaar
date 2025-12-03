using Microsoft.AspNetCore.Mvc;

namespace HemaBazaar.MVC.Controllers
{
    public class ItemController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }
    }
}
