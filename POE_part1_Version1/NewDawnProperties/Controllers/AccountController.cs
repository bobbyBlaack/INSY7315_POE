using Microsoft.AspNetCore.Mvc;

namespace NewDawnProperties.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(string email, string password, string fullname, string phone, string location, string block, string unit, string security)
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View("Index"); 
        }
    }
}