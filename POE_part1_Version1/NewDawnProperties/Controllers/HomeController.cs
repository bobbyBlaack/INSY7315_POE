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

        [HttpPost]
        public IActionResult RegisterAccount(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                // If model validation fails, return the form with errors
                return View(model);
            }

            // Check if email already exists
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Email already in use. Please choose another email.";
                return View(model);
            }

            
            _context.Users.Add(model);
            _context.SaveChanges();

            // Optional: Automatically log in the user after registration
            HttpContext.Session.SetString("UserEmail", model.Email);
            HttpContext.Session.SetString("UserRole", model.Role);

            // Redirect based on role
            if (model.Role == "Tenant")
                return RedirectToAction("TenantDashboard", "Tenant");
            if (model.Role == "CareTaker")
                return RedirectToAction("CaretakerDashboard", "Caretaker");
             if (model.Role == "PropManager")
                return RedirectToAction("PropManDashboard", "PropManager");
            else
                return RedirectToAction("Index", "Home");
        }

        public IActionResult CreateAccount() { 
        
            return View();
        
        }

        public IActionResult Profile() { return View(); }
        
        public IActionResult SignIn()
        {

            return View();
        }

        [HttpPost]
        public IActionResult VerifyUser(UserModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

            if (user != null)
            {
                // storing session data
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetInt32("UserID", user.UserID);

                var role = user.Role?.Trim().ToLower();

                if (role == "admin")
                    return RedirectToAction("AdminDashboard", "Admin");
                else if (role == "tenant")
                    return RedirectToAction("TenantDashboard", "Tenant");
                else if (role == "caretaker")
                    return RedirectToAction("CaretakerDashboard", "Caretaker");
                else if (role == "propmanager")
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
