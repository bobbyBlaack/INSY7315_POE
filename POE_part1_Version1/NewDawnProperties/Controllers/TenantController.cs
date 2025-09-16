using Microsoft.AspNetCore.Mvc;

namespace NewDawnProperties.Controllers
{
    public class TenantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
