using Microsoft.AspNetCore.Mvc;
using NewDawnProperties.Models;
using NewDawnProperties.Services;
using System.Reflection;

namespace NewDawnProperties.Controllers
{
    public class CommunicationController : Controller
    {
        private readonly ApiService _api;

        public CommunicationController(ApiService api)
        {
            _api = api;
        }

        // GET: Communication?chatWithId
        public async Task<IActionResult> Index(string? chatWithId)
        {
            string uid = HttpContext.Session.GetString("UserId") ?? "";
            string userName = HttpContext.Session.GetString("UserName") ?? "You";
            string role = HttpContext.Session.GetString("Role") ?? "resident";

            var allMessages = await _api.GetMessagesAsync(uid);

            // Build contacts list (unique people you’ve chatted with)
            var contacts = allMessages
                .Select(m =>
                {
                    // If I'm the sender, the contact is the recipient; otherwise the sender
                    var isMe = m.SenderId == uid;
                    var otherId = isMe ? m.RecipientId : m.SenderId;
                    var otherName = isMe ? m.RecipientName : m.SenderName;
                    var otherRole = isMe ? m.Role : m.Role;

                    return new { otherId, otherName, otherRole };
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.otherId))
                .GroupBy(x => x.otherId)
                .Select(g => new ChatContact
                {
                    UserId = g.Key!,
                    Name = g.First().otherName ?? "Unknown",
                    Role = g.First().otherRole ?? "resident"
                })
                .OrderBy(c => c.Name)
                .ToList();

            // Filter messages for selected contact
            IEnumerable<CommunicationMessage> filtered = allMessages;

            if (!string.IsNullOrEmpty(chatWithId))
            {
                filtered = allMessages.Where(m =>
                    (m.SenderId == uid && m.RecipientId == chatWithId) ||
                    (m.SenderId == chatWithId && m.RecipientId == uid));
            }

            var vm = new CommunicationViewModel
            {
                Messages = filtered.OrderBy(m => m.Timestamp).ToList(),
                Contacts = contacts,
                SelectedContactId = chatWithId,
                SelectedContactName = contacts.FirstOrDefault(c => c.UserId == chatWithId)?.Name,
                CurrentUserId = uid,
                CurrentUserName = userName,
                CurrentUserRole = role
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Send(string messageText, string? chatWithId)
        {
            if (string.IsNullOrWhiteSpace(messageText))
                return RedirectToAction("Index", new { chatWithId });

            string uid = HttpContext.Session.GetString("UserId") ?? "";
            string userName = HttpContext.Session.GetString("UserName") ?? "Unknown User";
            string role = HttpContext.Session.GetString("Role") ?? "resident";

            // Fetch contact name so the conversation can build properly
            string recipientName = "";
            if (!string.IsNullOrEmpty(chatWithId))
            {
                // Get the messages to detect contact list properly
                var allMessages = await _api.GetMessagesAsync(uid);

                var contact = allMessages
                    .Where(m => m.SenderId == chatWithId || m.RecipientId == chatWithId)
                    .Select(m => m.SenderId == chatWithId ? m.SenderName : m.RecipientName)
                    .FirstOrDefault();

                recipientName = contact ?? "";
            }

            var msg = new CommunicationMessage
            {
                SenderId = uid,
                SenderName = userName,
                Role = role,
                Message = messageText,
                Timestamp = DateTime.Now,
                RecipientId = chatWithId,
                RecipientName = recipientName
            };

            await _api.SendMessageAsync(msg);

            return RedirectToAction("Index", new { chatWithId });
        }
    }
}