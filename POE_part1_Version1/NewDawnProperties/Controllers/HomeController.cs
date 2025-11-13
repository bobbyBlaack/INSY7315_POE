using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NewDawnProperties.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NewDawnProperties.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
        }

        // Default landing page (Login Page)
        public IActionResult Index()
        {
            return View();
        }

        // Property listings page
        [HttpGet]
        public async Task<IActionResult> Listings()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Properties");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var properties = JsonConvert.DeserializeObject<List<PropertyModel>>(json);

                return View(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching listings");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        // Handles language selection
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        // Sign up page
        [HttpPost]
        public IActionResult SignUp()
        {
            return View();
        }

        // Profile page
        public IActionResult Profile()
        {
            ViewBag.UserName = TempData["UserName"] ?? "Guest User";
            ViewBag.Email = TempData["Email"] ?? "Not Provided";
            ViewBag.Block = TempData["Block"]?.ToString();
            ViewBag.Unit = TempData["Unit"]?.ToString();

            return View();
        }

        // Handles login form submission
        [HttpPost]
        public IActionResult SignIn(string email, string password)
        {
            if (email.EndsWith("@caretaker.com", System.StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "CareTaker");
            }
            else
            {
                TempData["Email"] = email;
                TempData["UserName"] = "Resident User"; 
                return RedirectToAction("Profile", "Home");
            }
        }

        // Handles logout
        [HttpPost]
        public IActionResult SignOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        // Error handler
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}