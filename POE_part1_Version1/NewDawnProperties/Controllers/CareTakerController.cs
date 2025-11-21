using Microsoft.AspNetCore.Mvc;
using NewDawnProperties.Models;
using NewDawnProperties.Services;

namespace NewDawnProperties.Controllers
{
    public class CaretakerController : Controller
    {
        private readonly ApiService _api;
        private readonly ILogger<CaretakerController> _logger;

        public CaretakerController(ApiService api, ILogger<CaretakerController> logger)
        {
            _api = api;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Normalise role, checking both keys just in case
            var role = HttpContext.Session.GetString("Role")
                       ?? HttpContext.Session.GetString("UserRole")
                       ?? "";
            role = role.Trim().ToLower();

            // If you want to lock it down to caretakers only, keep this:
            if (role != "caretaker")
            {
                // not caretaker – send back to home/profile
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var tasks = await _api.GetCaretakerTasksAsync();

                ViewBag.PendingTasks = tasks.Where(t => t.Status == "open").ToList();
                ViewBag.InProgressTasks = tasks.Where(t => t.Status == "inprogress").ToList();
                ViewBag.CompletedTasks = tasks.Where(t => t.Status == "closed").ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching caretaker tasks");
                ViewBag.PendingTasks = new List<MaintenanceTaskModel>();
                ViewBag.InProgressTasks = new List<MaintenanceTaskModel>();
                ViewBag.CompletedTasks = new List<MaintenanceTaskModel>();
            }

            return View();
        }
    }
}