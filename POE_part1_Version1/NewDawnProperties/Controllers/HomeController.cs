using Microsoft.AspNetCore.Mvc;
using NewDawnProperties.Data;
using NewDawnProperties.Models;
using System.Diagnostics;

namespace NewDawnProperties.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult myAiAssistant()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Listings()
        {

            return View();
        }

        
        public IActionResult CreateAccount() { 
        
            return View();
        
        }

        [HttpPost]
        public IActionResult SignIn()
        {

            return View();
        }

        [HttpPost]
        public IActionResult VerifyUser(UserModel model)
        { 
        
            var user= _context.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

            if (user != null) {

                //storing session data for the user signing in
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);

                if (user.Role == "Admin")
                    return RedirectToAction("AdminDashboard", "Admin");
                else if (user.Role == "User")
                    return RedirectToAction("TenantDashboard", "Tenant");
                else if (user.Role == "CareTaker")
                    return RedirectToAction("CaretakerDashboard", "Caretaker");
                else if (user.Role == "PropManager")
                    return RedirectToAction("PropManDashboard", "PropManager");

            }

            ViewBag.ErrorMessage = "Invalid email or password.";
            return View("SignIn");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
