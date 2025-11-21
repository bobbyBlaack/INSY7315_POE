using System;

namespace NewDawnProperties.Models
{
    public class MaintenanceTaskModel
    {
        public string Id { get; set; }
        public string AssignedTo { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public string PropertyID { get; set; }
        public string UnitID { get; set; }
        public string TenantID { get; set; }
        public string Status { get; set; }     
        public string Urgency { get; set; }
    }

    public class TaskLog
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Status { get; set; } = "";
        public string? Note { get; set; }
        public string? FileUrl { get; set; }
    }
}