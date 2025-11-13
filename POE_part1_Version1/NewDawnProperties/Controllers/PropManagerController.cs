using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewDawnProperties.Data;
using NewDawnProperties.Models;
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

        [HttpPost]
        public IActionResult UpdateAction(int id, string newAction)
        {
            var escalation = _context.Escalations.FirstOrDefault(e => e.EscalationId == id);
            if (escalation == null)
                return NotFound();

            escalation.Actions = newAction;
            _context.SaveChanges();

            return RedirectToAction("ManagerEscalation");
        }

        public async Task<IActionResult> ManagerEscalation(DateTime? startDate, DateTime? endDate)
        {
            // Logged-in user
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            // Get the Room IDs assigned to this user (TenantAssignment)
            var userRoomIds = await _context.TenantAssignment
                .Where(t => t.UserID == userId)
                .Select(t => t.RoomID)
                .ToListAsync();

            // Get escalations for those rooms only
            var escalationsQuery = _context.Escalations
                .Where(e => userRoomIds.Contains(e.RoomId));

            // Optional date filter
            if (startDate.HasValue)
                escalationsQuery = escalationsQuery.Where(e => e.EscalationDate >= startDate.Value);
            if (endDate.HasValue)
                escalationsQuery = escalationsQuery.Where(e => e.EscalationDate <= endDate.Value);

            var escalationList = await escalationsQuery
                .OrderByDescending(e => e.EscalationDate)
                .ToListAsync();

            // Summary boxes
            ViewBag.Total = escalationList.Count;
            ViewBag.High = escalationList.Count(e => e.Actions == "High");
            ViewBag.Medium = escalationList.Count(e => e.Actions == "Medium");
            ViewBag.Low = escalationList.Count(e => e.Actions == "Low");

            return View(escalationList);
        }



        public IActionResult ManagerLease()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Get all properties managed by this user
            var managedProps = _context.Property
                .Where(p => p.UserID == userId)
                .Select(p => p.PropID)
                .ToList();

            // Get all rooms that belong to those properties
            var rooms = _context.Rooms
                .Where(r => managedProps.Contains(r.PropID ?? 0))
                .Select(r => r.RoomID)
                .ToList();

            // Get leases that belong to those rooms
            var leases = _context.Leases
                .Where(l => rooms.Contains(l.RoomId))
                .ToList();

            return View(leases);
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
