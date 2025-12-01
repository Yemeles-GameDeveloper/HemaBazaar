using Microsoft.AspNetCore.Mvc;

namespace HemaBazaar.MVC.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }



        //18 Kasım 2:36:00 Index cshtml e login ve register kısmı ekleyeceksin unutma.
    }
}
