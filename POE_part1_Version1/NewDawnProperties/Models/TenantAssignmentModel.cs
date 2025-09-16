using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewDawnProperties.Models
{
    public class TenantAssignmentModel
    {
        [Key]
        public int TenantAssignment {  get; set; }

        public int? UserID { get; set; }

        public int? PropID { get; set; }

        public int? RoomID { get; set; }

    }
}
