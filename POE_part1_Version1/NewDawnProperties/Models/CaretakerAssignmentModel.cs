using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewDawnProperties.Models
{
    public class CaretakerAssignmentModel
    {
        [Key]
        public int CaretakerAssignmentID { get; set; }

        public int?  UserID { get; set; }

        public int? PropID { get; set; }

    }
}
