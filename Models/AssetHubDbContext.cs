using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AssetHub.Models;

public partial class AssetHubDbContext : DbContext
{
    public AssetHubDbContext()
    {
    }

    public AssetHubDbContext(DbContextOptions<AssetHubDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Asset> Assets { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<ActivityLog> ActivityLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // localhost,1433 points to your Docker container
        // User Id 'sa' and the password match your docker-compose.yml file
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=AssetHubDB;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Seed a Default Admin User (Password is 'testuser@123' hashed)
        modelBuilder.Entity<User>().HasData(new User
        {
            UserId = 1,
            Username = "admin",
            FullName = "System Administrator",
            Email = "testuser@assethub.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("testuser@123"),
            Role = "Admin",
            CreatedAt = DateTime.Now
        });

        // 2. Seed some Employees
        modelBuilder.Entity<Employee>().HasData(
            new Employee { EmployeeId = 1, FullName = "Brian Jariel", JobTitle = "IT Developer", Department = "IT", IsActive = true },
            new Employee { EmployeeId = 2, FullName = "Alice Chen", JobTitle = "Manager", Department = "HR", IsActive = true }
        );

        // 3. Seed some Assets
        modelBuilder.Entity<Asset>().HasData(
            new Asset { AssetId = 1, AssetName = "MacBook Pro M3", AssetType = "Laptop", SerialNumber = "SN12345", Status = "Available", Price = 2500 },
            new Asset { AssetId = 2, AssetName = "Dell UltraSharp 27", AssetType = "Monitor", SerialNumber = "SN67890", Status = "Assigned", AssignedEmployeeId = 1, Price = 500 }
        );
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}