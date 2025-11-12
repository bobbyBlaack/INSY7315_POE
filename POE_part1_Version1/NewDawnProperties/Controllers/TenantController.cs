using Microsoft.AspNetCore.Mvc;

namespace NewDawnProperties.Controllers
{
    public class TenantController : Controller
    {
        public IActionResult TenantDashboard()
        {
            return View();
        }
    }
}
