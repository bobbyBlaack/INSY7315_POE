using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Google.Cloud.Firestore;


namespace NewDawnProperties.Models
{
    [FirestoreData]
    public class CaretakerAssignmentModel
    {
        [Key]
        public int CaretakerAssignmentID { get; set; }

        public int?  UserID { get; set; }

        public int? PropID { get; set; }

    }
}
