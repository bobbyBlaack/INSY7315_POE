using Microsoft.AspNetCore.Mvc;

namespace NewDawnProperties.Controllers
{
    public class PropManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
