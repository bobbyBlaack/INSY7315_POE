namespace NewDawnProperties.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }   // maps from 'name'
        public string Email { get; set; }
        public string Phone { get; set; }      // maps from 'phoneNumber'
        public string Role { get; set; }       // maps from 'userType'
        public string Block { get; set; }      // maps from 'located'
        public string Unit { get; set; }       // maps from 'unit'
    }
}