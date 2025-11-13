using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Google.Cloud.Firestore;


namespace NewDawnProperties.Models
{
    [FirestoreData]
    public class EscalationModel
    {

        [Key]
        public int EscalationId { get; set; }

        public DateTime EscalationDate { get; set; }

        public int RoomId { get; set; }

        public int UserId {  get; set; }

        public int Category { get; set; }

        public string Summary { get; set; }

        public string Actions { get; set; }

        public bool IsSynced { get; set; } = false;

    }
}
