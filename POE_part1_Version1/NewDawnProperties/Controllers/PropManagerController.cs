using Microsoft.AspNetCore.Mvc;
using NewDawnProperties.Data;
using System.Linq;

namespace NewDawnProperties.Controllers
{
    public class PropManagerController : Controller
    {
        private readonly AppDbContext _context;

        public PropManagerController(AppDbContext context)
        {
            _context = context;
        }



        public IActionResult ManagerNotif() { 
        
            return View();
        
        }

        public IActionResult ManagerEscalation() { 
        
            return View();
        
        }

        public IActionResult ManagerLease() { 
        
        
            return View();
        
        }

        public IActionResult ManagerListing() { 
        
        
            return View();
        }

        public IActionResult PropManDashboard()
        {
            // Retrieve UserID from session
            var userId = HttpContext.Session.GetInt32("UserID");

            // Redirect to sign-in if not logged in
            if (userId == null)
            {
                return RedirectToAction("SignIn", "User");
            }

            // Fetch only the properties owned by the logged-in Property Manager
            var properties = _context.Property
                                     .Where(p => p.UserID == userId)
                                     .ToList();

            return View(properties);
        }
    }
}
