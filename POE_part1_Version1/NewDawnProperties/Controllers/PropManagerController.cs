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
            // Logged-in Property Owner ID
            var ownerId = HttpContext.Session.GetInt32("UserID") ?? 0;
            if (ownerId == 0)
                return RedirectToAction("Login", "Home");

            // Step 1: Find all properties owned by this user
            var ownerPropertyIds = await _context.Property
                .Where(p => p.UserID == ownerId)
                .Select(p => p.PropID)
                .ToListAsync();

            // Step 2: Find all rooms under those properties (TenantAssignment)
            var ownerRoomIds = await _context.TenantAssignment
                .Where(t => t.PropID.HasValue && ownerPropertyIds.Contains(t.PropID.Value))
                .Select(t => t.RoomID)
                .ToListAsync();

            // Step 3: Filter escalations by those room IDs
            var escalationsQuery = _context.Escalations
                .Where(e => ownerRoomIds.Contains(e.RoomId));

            // Step 4: Apply optional date filtering
            if (startDate.HasValue)
                escalationsQuery = escalationsQuery.Where(e => e.EscalationDate >= startDate.Value);

            if (endDate.HasValue)
                escalationsQuery = escalationsQuery.Where(e => e.EscalationDate <= endDate.Value);

            // Execute query
            var escalationList = await escalationsQuery
                .OrderByDescending(e => e.EscalationDate)
                .ToListAsync();

            // Step 5: Summary counts
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


        [HttpPost]
        public async Task<IActionResult> UpdateProperty(PropertyModel updatedProp)
        {
            var prop = await _context.Property.FindAsync(updatedProp.PropID);
            if (prop == null)
                return NotFound();

            prop.PropName = updatedProp.PropName;
            prop.ListPrice = updatedProp.ListPrice;
            prop.Address = updatedProp.Address;
            prop.City = updatedProp.City;
            prop.RoomsCount = updatedProp.RoomsCount;

            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> AddNewListing(PropertyModel model, IFormFile ImageFile, IFormFile VideoFile)
        {
            var userId = HttpContext.Session.GetInt32("UserID") ?? 0;

            model.UserID = userId;

            if (ImageFile != null)
            {
                using (var ms = new MemoryStream())
                {
                    await ImageFile.CopyToAsync(ms);
                    model.ListImage = ms.ToArray();
                }
            }

            if (VideoFile != null)
            {
                using (var ms = new MemoryStream())
                {
                    await VideoFile.CopyToAsync(ms);
                    model.ListVideo = ms.ToArray();
                }
            }

            _context.Property.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManagerLease");
        }


        // Display all properties for the logged-in user
        public IActionResult ManagerListing()
        {
            var userId = HttpContext.Session.GetInt32("UserID") ?? 0;

            var properties = _context.Property
                .Where(p => p.UserID == userId)
                .ToList();

            return View(properties);
        }

        // GET: Add/Edit property
        public IActionResult EditProperty(int? id)
        {
            if (id == null) return NotFound();

            var property = _context.Property.Find(id);
            if (property == null) return NotFound();

            return View(property);
        }

        // POST: Update property
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProperty(PropertyModel property)
        {
            if (ModelState.IsValid)
            {
                _context.Update(property);
                _context.SaveChanges();
                return RedirectToAction("ManagerListing");
            }
            return View(property);
        }

        // POST: Delete property
        [HttpPost]
        public IActionResult DeleteProperty(int id)
        {
            var property = _context.Property.Find(id);
            if (property != null)
            {
                _context.Property.Remove(property);
                _context.SaveChanges();
            }
            return RedirectToAction("ManagerListing");
        }

        // POST: Add new property
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProperty(PropertyModel property)
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            property.UserID = userId;

            if (ModelState.IsValid)
            {
                _context.Property.Add(property);
                _context.SaveChanges();
                return RedirectToAction("ManagerListing");
            }

            // If validation fails, reload the page with existing data
            var properties = _context.Property
                .Where(p => p.UserID == userId)
                .ToList();
            return View("ManagerListing", properties);
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
