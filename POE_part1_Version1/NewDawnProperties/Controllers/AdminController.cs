using Microsoft.AspNetCore.Mvc;
using NewDawnProperties.Data;

namespace NewDawnProperties.Controllers
{
    public class AdminController : Controller
    {

        

        public IActionResult AdminDashboard()
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
