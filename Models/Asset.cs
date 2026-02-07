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


        // This is the Foreign Key (the ID number)
        public int? AssignedEmployeeId { get; set; }

        public virtual Employee AssignedEmployee { get; set; }

    }
}