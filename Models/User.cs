using System;
using System.ComponentModel.DataAnnotations;

namespace AssetHub.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } 
        public string FullName { get; set; }
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}