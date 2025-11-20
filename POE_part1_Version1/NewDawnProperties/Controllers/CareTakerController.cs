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
            string role = HttpContext.Session.GetString("Role")?.ToLower() ?? "";

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