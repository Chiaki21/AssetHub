using AssetHub.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AssetHub
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // 1. Load Stats Cards
                    int total = db.Assets.Count();
                    int assigned = db.Assets.Count(a => a.Status == "Assigned");
                    int available = db.Assets.Count(a => a.Status == "Available");
                    decimal totalValue = db.Assets.Sum(a => a.Price ?? 0);

                    TxtTotalAssets.Text = total.ToString();
                    TxtAssignedAssets.Text = assigned.ToString();
                    TxtAvailableAssets.Text = available.ToString();
                    TxtTotalValue.Text = totalValue.ToString("C2"); // Formats to local currency (₱ or $)

                    // 2. Load Category Distribution (LINQ GroupBy)
                    var categorySummary = db.Assets
                        .GroupBy(a => a.AssetType)
                        .Select(group => new
                        {
                            Category = group.Key ?? "Uncategorized",
                            Count = group.Count()
                        })
                        .OrderByDescending(x => x.Count)
                        .ToList();

                    CategorySummaryControl.ItemsSource = categorySummary;

                    // 3. Load Recent Employees
                    var recentEmployees = db.Employees
                        .OrderByDescending(e => e.DateAdded)
                        .Take(5)
                        .ToList();

                    RecentEmployeesControl.ItemsSource = recentEmployees;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dashboard Load Error: {ex.Message}");
            }
        }
    }
}