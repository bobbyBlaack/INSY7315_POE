using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewDawnProperties.Models
{
    public class MaintenanceModel
    {

        [Key]
        public int MaintenanceId { get; set; }

        public int UserId { get; set; }

        public int UserRole {  get; set; }
        
        public string Description { get; set; }

        public string Type {  get; set; }

        public DateOnly MaintenanceDate { get; set; }

        public int RoomID { get; set; }

        public int PropID { get; set; }

        public int Status { get; set; }

    }
}
