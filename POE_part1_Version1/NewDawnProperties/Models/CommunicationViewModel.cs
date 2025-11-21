namespace NewDawnProperties.Models
{
    public class ChatContact
    {
        public string UserId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Role { get; set; } = "";
    }

    public class CommunicationViewModel
    {
        public List<CommunicationMessage> Messages { get; set; } = new();
        public List<ChatContact> Contacts { get; set; } = new();

        public string? SelectedContactId { get; set; }
        public string? SelectedContactName { get; set; }

        public string CurrentUserId { get; set; } = "";
        public string CurrentUserName { get; set; } = "";
        public string CurrentUserRole { get; set; } = "";
    }
}