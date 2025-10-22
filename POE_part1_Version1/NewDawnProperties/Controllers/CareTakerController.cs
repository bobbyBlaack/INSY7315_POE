using Microsoft.AspNetCore.Mvc;

namespace NewDawnProperties.Controllers
{
    public class CareTakerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
