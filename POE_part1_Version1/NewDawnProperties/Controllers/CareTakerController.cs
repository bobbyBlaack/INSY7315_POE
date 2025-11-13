using Microsoft.AspNetCore.Mvc;
using NewDawnProperties.Models;
using NewDawnProperties.Services;

namespace NewDawnProperties.Controllers
{
    public class CareTakerController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<CareTakerController> _logger;

        public CareTakerController(ApiService apiService, ILogger<CareTakerController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var tasks = await _apiService.GetCaretakerTasksAsync();

                // Firebase → MVC mapping
                var pendingTasks = tasks
                    .Where(t => t.Status.Equals("open", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var inProgressTasks = tasks
                    .Where(t => t.Status.Equals("inprogress", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var completedTasks = tasks
                    .Where(t => t.Status.Equals("closed", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                ViewBag.PendingTasks = pendingTasks;
                ViewBag.InProgressTasks = inProgressTasks;
                ViewBag.CompletedTasks = completedTasks;
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