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
            // Store user details in TempData
            TempData["UserName"] = fullname;
            TempData["Email"] = email;
            TempData["Block"] = block;
            TempData["Unit"] = unit;

            // Redirect to Profile page
            return RedirectToAction("Profile", "Home");
        }

        public IActionResult Login()
        {
            return View("Index");
        }
    }
}