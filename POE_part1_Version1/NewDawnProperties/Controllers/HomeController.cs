using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Listings()
        {
            var listings = _context.Property.ToList(); // fetch all properties
            return View(listings);
        }

        // ===========================
        // Register account
        // ===========================
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

            // Store basic session values
            var roleLower = model.Role?.Trim().ToLower() ?? "";

            HttpContext.Session.SetInt32("UserID", model.UserID);
            HttpContext.Session.SetString("UserId", model.UserID.ToString());
            HttpContext.Session.SetString("UserEmail", model.Email);
            HttpContext.Session.SetString("UserRole", model.Role);
            HttpContext.Session.SetString("UserName",
                string.IsNullOrWhiteSpace(model.UserName)
                    ? $"{model.FName} {model.SName}"
                    : model.UserName);
            HttpContext.Session.SetString("Role", roleLower);

            // Redirect based on role (using same route names as VerifyUser)
            if (roleLower == "tenant")
                return RedirectToAction("TenantDashboard", "Tenant");
            if (roleLower == "caretaker")
                return RedirectToAction("Index", "Caretaker");
            if (roleLower == "propmanager")
                return RedirectToAction("PropManDashboard", "PropManager");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult CreateAccount()
        {
            return View();
        }

        // ===========================
        // Profile
        // ===========================
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserID") ?? 0;
            if (userId == 0)
                return RedirectToAction("SignIn", "Home");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserModel updatedUser)
        {
            var user = await _context.Users.FindAsync(updatedUser.UserID);
            if (user == null)
                return NotFound();

            user.FName = updatedUser.FName;
            user.SName = updatedUser.SName;
            user.Email = updatedUser.Email;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.Password = updatedUser.Password;
            user.UserName = updatedUser.UserName;

            await _context.SaveChangesAsync();

            return Ok();
        }

        // ===========================
        // Sign in
        // ===========================
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyUser(UserModel model)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Email == model.Email && u.Password == model.Password);

            if (user != null)
            {
                // normalised role
                var roleLower = user.Role?.Trim().ToLower() ?? "";

                // store ALL session keys used across app
                HttpContext.Session.SetInt32("UserID", user.UserID);
                HttpContext.Session.SetString("UserId", user.UserID.ToString()); // for API / comms
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName",
                    string.IsNullOrWhiteSpace(user.UserName)
                        ? $"{user.FName} {user.SName}"
                        : user.UserName);
                HttpContext.Session.SetString("Role", roleLower);

                // redirect based on role
                if (roleLower == "admin")
                    return RedirectToAction("AdminDashboard", "Admin");
                else if (roleLower == "tenant")
                    return RedirectToAction("TenantDashboard", "Tenant");
                else if (roleLower == "caretaker")
                    // IMPORTANT: CaretakerController action is Index
                    return RedirectToAction("Index", "Caretaker");
                else if (roleLower == "propmanager")
                    return RedirectToAction("PropManDashboard", "PropManager");

                // fallback
                return RedirectToAction("Index", "Home");
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