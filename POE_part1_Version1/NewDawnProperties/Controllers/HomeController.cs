using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using NewDawnProperties.Models;
using NewDawnProperties.Services;
using System.Diagnostics;

namespace NewDawnProperties.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApiService _api;

        public HomeController(ILogger<HomeController> logger, ApiService api)
        {
            _logger = logger;
            _api = api;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Login using Firebase API
        [HttpPost]
        public async Task<IActionResult> SignIn(string email, string password)
        {
            var user = await _api.LoginAsync(email, password);

            if (user == null)
            {
                TempData["Error"] = "Invalid email or password.";
                return RedirectToAction("Index");
            }

            // Normalize role safely
            string rawRole = user.Role?.Trim().ToLower() ?? "resident";

            string role =
                rawRole.Contains("caretaker") ||
                rawRole.Contains("care taker") ||
                rawRole.Contains("care_taker") ||
                rawRole.Contains("maintenance") ||
                rawRole.Contains("manager") ||
                rawRole == "ct"
                ? "caretaker"
                : "resident";

            // Save session
            HttpContext.Session.SetString("Role", role);
            HttpContext.Session.SetString("UserId", user.Id ?? "");
            HttpContext.Session.SetString("UserName", user.FullName ?? "");
            HttpContext.Session.SetString("Email", user.Email ?? "");

            if (!string.IsNullOrWhiteSpace(user.Block))
                HttpContext.Session.SetString("Block", user.Block);

            if (!string.IsNullOrWhiteSpace(user.Unit))
                HttpContext.Session.SetString("Unit", user.Unit);

            // Redirect correctly
            if (role == "caretaker")
                return RedirectToAction("/Caretaker/Index");

            return RedirectToAction("Profile");
        }

        public IActionResult Profile()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Block = HttpContext.Session.GetString("Block");
            ViewBag.Unit = HttpContext.Session.GetString("Unit");

            return View();
        }

        [HttpPost]
        public IActionResult SignOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy() => View();

        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}