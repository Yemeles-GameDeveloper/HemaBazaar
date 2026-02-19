using Microsoft.AspNetCore.Mvc;

namespace HemaBazaar.MVC.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult NotFound()
        {
            return View();
        }

        public IActionResult Unauthorized()
        {
            return View();
        }

        public IActionResult Forbidden()
        {
            return View();
        }

        // 1 Aralık 1:42:00dan devam et.
    }
}
