using Microsoft.AspNetCore.Mvc;

namespace NewDawnProperties.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
