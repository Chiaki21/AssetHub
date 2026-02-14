using System;
using System.Collections.Generic;

namespace AssetHub.Models
{
    public class Asset
    {
        public int AssetId { get; set; }
        public string AssetName { get; set; }
        public string AssetType { get; set; }
        public string SerialNumber { get; set; }
        public string Status { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? Price { get; set; }
        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }


        // This is the Foreign Key (the ID number)
        public int? AssignedEmployeeId { get; set; }

        public virtual Employee AssignedEmployee { get; set; }

        

        public string AssetIcon => AssetType switch
        {
            "Laptop" => "Laptop",
            "Monitor" => "Monitor",
            "Server" => "Server",
            "Networking Gear" => "Router",
            "Furniture" => "ChairRolling",
            _ => "Devices"
        };

        // Background color for the status pill
        public string StatusBgColor => Status switch
        {
            "Available" => "#DCFCE7", // Light Green
            "Assigned" => "#DBEAFE",  // Light Blue
            "Maintenance" => "#FEE2E2", // Light Red
            _ => "#F1F5F9"
        };

        // Text color for the status pill
        public string StatusFgColor => Status switch
        {
            "Available" => "#16A34A", // Dark Green
            "Assigned" => "#2563EB",  // Dark Blue
            "Maintenance" => "#EF4444", // Dark Red
            _ => "#64748B"
        };

    }
}