using AssetHub.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf; // This is the key using statement

namespace AssetHub
{
    public partial class DashboardPage : Page
    {
        private decimal _totalInventoryValue = 0;
        private bool _isValueHidden = true;

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
                    TxtTotalAssets.Text = db.Assets.Count().ToString();
                    TxtAssignedAssets.Text = db.Assets.Count(a => a.Status == "Assigned").ToString();
                    TxtAvailableAssets.Text = db.Assets.Count(a => a.Status == "Available").ToString();

                    // 1. Stats Cards - Store value
                    _totalInventoryValue = db.Assets.Sum(a => a.Price ?? 0);
                    UpdateValueDisplay();

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
                        .OrderByDescending(e => e.DateAdded)
                        .Take(10).ToList();

                    // 4. Activity Feed
                    var recentActivities = db.ActivityLogs
                        .AsNoTracking()
                        .OrderByDescending(l => l.LogId)
                        .Take(10).ToList()
                        .Select(l => new {
                            Description = $"{l.Details} | SN: {l.SerialNumber ?? "N/A"}",
                            TimeAgo = l.ActionDate.ToString("MMM dd, hh:mm tt")
                        }).ToList();

                    ActivityFeedControl.ItemsSource = recentActivities;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dashboard Load Error: {ex.Message}");
            }
        }

        private void BtnToggleValue_Click(object sender, RoutedEventArgs e)
        {
            _isValueHidden = !_isValueHidden;
            UpdateValueDisplay();
        }

        private void UpdateValueDisplay()
        {
            // FIX: Removed 'materialDesign.' prefix
            if (_isValueHidden)
            {
                TxtTotalValue.Text = "₱ ••••••";
                IconPrivacy.Kind = PackIconKind.EyeOffOutline;
            }
            else
            {
                TxtTotalValue.Text = _totalInventoryValue.ToString("C2");
                IconPrivacy.Kind = PackIconKind.EyeOutline;
            }
        }
    }
}