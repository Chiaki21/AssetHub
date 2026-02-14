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
            this.Loaded += (s, e) => LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // 1. Stats Cards
                    // These stay the same as they reflect the CURRENT state of your inventory
                    TxtTotalAssets.Text = db.Assets.Count().ToString();
                    TxtAssignedAssets.Text = db.Assets.Count(a => a.Status == "Assigned").ToString();
                    TxtAvailableAssets.Text = db.Assets.Count(a => a.Status == "Available").ToString();
                    TxtTotalValue.Text = db.Assets.Sum(a => a.Price ?? 0).ToString("C2");

                    // 2. Load Category Distribution
                    CategorySummaryControl.ItemsSource = db.Assets
                        .GroupBy(a => a.AssetType)
                        .Select(g => new {
                            Category = g.Key ?? "Uncategorized",
                            Count = g.Count()
                        })
                        .OrderByDescending(x => x.Count).ToList();

                    // 3. Load Recent Employees
                    RecentEmployeesControl.ItemsSource = db.Employees
                        .OrderByDescending(e => e.DateAdded).Take(5).ToList();

                    // 4. Smart Activity Feed (NOW USING ACTIVITYLOGS TABLE)
                    // This prevents "overwriting" history because every action is a new row
                    var recentActivities = db.ActivityLogs
                        .AsNoTracking()
                        .OrderByDescending(l => l.LogId) // Pull the most recent logs based on ID
                        .Take(10) // Show a longer history of 10 items
                        .ToList()
                        .Select(l => new
                        {
                            // Formats the output to include the details (with employee name) and Serial Number
                            Description = $"{l.Details} | SN: {l.SerialNumber ?? "N/A"}",
                            TimeAgo = l.ActionDate.ToString("MMM dd, hh:mm tt")
                        })
                        .ToList();

                    ActivityFeedControl.ItemsSource = recentActivities;
                }
            }
            catch (Exception ex)
            {
                // Helpful for debugging if the ActivityLogs table is missing or LogId is null
                System.Diagnostics.Debug.WriteLine($"Dashboard Load Error: {ex.Message}");
            }
        }
    }
}