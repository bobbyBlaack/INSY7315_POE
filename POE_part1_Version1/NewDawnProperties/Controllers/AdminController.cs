using Microsoft.AspNetCore.Mvc;

namespace NewDawnProperties.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // returns the broadcast page
        public IActionResult Broadcast()
        {
            return View();
        }

        // returns the Escalations page
        public IActionResult Escalations()
        {
            return View();
        }

        // returns the Reports page
        public IActionResult Reports()
        {
            return View();
        }

        // returns the Management page
        public IActionResult Management()
        {
            return View();
        }

    }
}
