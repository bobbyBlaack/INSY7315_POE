using Microsoft.AspNetCore.Mvc;

namespace NewDawnProperties.Controllers
{
    public class AdminController : Controller
    {
        
        private static List<AppUser> Users = new()
        {
            new AppUser{ Id=1, Name="John Doe", Email="john@example.com", Role="Tenant", IsActive=true },
            new AppUser{ Id=2, Name="Sarah Smith", Email="sarah@example.com", Role="Manager", IsActive=true }
        };

        private static List<EscalationItem> EscalationList = new()
        {
            new EscalationItem{ Id=1, Date=DateTime.Today, Name="John Doe", Issue="Leaking pipe in Unit 4" }
        };

       
        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult Broadcast() => View();

        [HttpPost]
        public IActionResult SendBroadcast(string urgency, string property, bool staffOnly, string message)
        {
            TempData["Message"] = "Broadcast has been sent.";
            return RedirectToAction("Broadcast");
        }

        public IActionResult Management(string search)
        {
            var results = string.IsNullOrWhiteSpace(search)
                ? Users
                : Users.Where(u => u.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            return View(results);
        }

        public IActionResult EditUser(int id)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(AppUser updated)
        {
            var existing = Users.FirstOrDefault(x => x.Id == updated.Id);
            if (existing != null)
            {
                existing.Name = updated.Name;
                existing.Email = updated.Email;
                existing.Role = updated.Role;
                existing.IsActive = updated.IsActive;
            }

            TempData["Message"] = "User updated.";
            return RedirectToAction("Management");
        }

        public IActionResult DeleteUser(int id)
        {
            Users.RemoveAll(x => x.Id == id);
            TempData["Message"] = "User deleted.";
            return RedirectToAction("Management");
        }

        public IActionResult Escalations() => View(EscalationList);

        public IActionResult EscalationDetails(int id)
        {
            var esc = EscalationList.FirstOrDefault(x => x.Id == id);
            if (esc == null) return NotFound();
            return View(esc);
        }

        public IActionResult Reports() => View();

        [HttpPost]
        public IActionResult GenerateReport(string property, string tenant, DateTime startDate, DateTime endDate)
        {
            var summary = $"Report for {property} ({tenant}) from {startDate:d} to {endDate:d}.";
            return View("ReportResult", summary);
        }
    }


    public class AppUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }

    public class EscalationItem
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Issue { get; set; }
    }
}
