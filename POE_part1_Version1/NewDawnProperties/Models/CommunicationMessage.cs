namespace NewDawnProperties.Models
{
    public class CommunicationMessage
    {
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string Role { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string? RecipientId { get; set; }
        public string? RecipientName { get; set; }
    }
}