using Microsoft.AspNetCore.Mvc;

namespace NewDawnProperties.Models
{
    public class PropertyModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}