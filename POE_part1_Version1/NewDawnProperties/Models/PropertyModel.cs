using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewDawnProperties.Models
{
    public class PropertyModel
    {

        [Key]
        public int PropID { get; set; }

        
        public string PropName { get; set; }

        [Required]
        public string ListPrice { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        public int UserID { get; set; }

        public int RoomsCount { get; set; }

        public byte[] ListImage { get; set; }

        public byte[] ListVideo { get; set; }
    }
}
