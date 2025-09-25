using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace NewDawnProperties.Models
{
    public class LeaseModel
    {

        [Key]
        public int LeaseID { get; set; }

        public int LeaseStatus { get; set; }

        public DateOnly LeaseStart{ get; set; }

        public DateOnly LeaseEnd { get; set; }

        public int RentAmount { get; set; }


        //Foreign Key for the user to ensure the correct type
        public int UserId { get; set; }

        public int Role { get; set; }

        public string LeaseAction { get; set; }

        //foreign key for the room/ building ID
        public int RoomId { get; set; }

        
    }
}
