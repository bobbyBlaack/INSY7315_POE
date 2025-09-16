using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewDawnProperties.Models
{
    public class UserModel
    {

        [Key]
        public int UserID { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string? PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string FName { get; set; }

        [Required]
        public string SName { get; set; }

    }
}
