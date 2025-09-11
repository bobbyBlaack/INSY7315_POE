using Microsoft.AspNetCore.Mvc;

namespace POE_part1_Version1.Controllers
{
    public class TenantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
