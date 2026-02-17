using System;
using System.Collections.Generic;
using System.Linq; // Added this to ensure Select() and Concat() work

namespace AssetHub.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Email { get; set; }

    public string JobTitle { get; set; }

    public DateTime DateAdded { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public string? Department { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();

    // Improved Initials logic to handle single names and multiple spaces
    public string Initials
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FullName)) return "??";

            var parts = FullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
            }

            return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
        }
    }
}