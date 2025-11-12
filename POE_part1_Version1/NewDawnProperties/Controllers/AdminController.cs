using Microsoft.AspNetCore.Mvc;

namespace NewDawnProperties.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdminDashboard()
        {
            return View();
        }
    }
}
