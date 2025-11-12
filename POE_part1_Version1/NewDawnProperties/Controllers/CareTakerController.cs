using Microsoft.AspNetCore.Mvc;

namespace NewDawnProperties.Controllers
{
    public class CareTakerController : Controller
    {
        public IActionResult CaretakerDashboard()
        {
            return View();
        }
    }
}
