using Google.Cloud.Firestore;




namespace NewDawnProperties.Models
{
    [FirestoreData]
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
