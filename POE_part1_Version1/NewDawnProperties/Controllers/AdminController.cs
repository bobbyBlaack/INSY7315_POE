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

        


    }
}
