using System;
using System.ComponentModel.DataAnnotations; // Add this using directive

namespace AssetHub.Models
{
    public class ActivityLog
    {
        [Key] // Explicitly tell EF this is the Primary Key
        public int LogId { get; set; }
        public string Details { get; set; }
        public DateTime ActionDate { get; set; }
        public string SerialNumber { get; set; }
    }
}