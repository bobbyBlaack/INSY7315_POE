using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewDawnProperties.Models
{
    [FirestoreData]
    public class RoomModel
    {

        [Key]
        public int RoomID { get; set; }

        public string Block {  get; set; }

        public int? PropID { get; set; }

        public bool IsSynced { get; set; } = false;


    }
}
