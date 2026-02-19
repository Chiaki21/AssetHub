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
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=AssetHubDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<ActivityLog>()
        .HasKey(l => l.LogId);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(e => e.AssetId);

            entity.HasIndex(e => e.SerialNumber).IsUnique();

            entity.Property(e => e.AssetName).HasMaxLength(100);
            entity.Property(e => e.AssetType).HasMaxLength(50);
            entity.Property(e => e.SerialNumber).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Available");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            // --- CLEAN RELATIONSHIP (No hardcoded constraint names) ---
            entity.HasOne(d => d.AssignedEmployee)
                .WithMany(p => p.Assets)
                .HasForeignKey(d => d.AssignedEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F117357430D");

            entity.Property(e => e.Department).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.JobTitle).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C48118427");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E417093AF6").IsUnique();

            entity.Property(e => e.PasswordHash).HasMaxLength(256);

        
            entity.Property(e => e.Email).HasMaxLength(100);

            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("Admin");

            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}