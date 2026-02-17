using AssetHub.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

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
                    // 1. Stats Cards
                    TxtTotalAssets.Text = db.Assets.Count().ToString();
                    TxtAssignedAssets.Text = db.Assets.Count(a => a.Status == "Assigned").ToString();
                    TxtAvailableAssets.Text = db.Assets.Count(a => a.Status == "Available").ToString();

                    // Calculate Total System Value (Sum of all assets in DB)
                    _totalInventoryValue = db.Assets.Sum(a => a.Price ?? 0);
                    UpdateValueDisplay();

                    // 2. Department Breakdown
                    // Get all assets that are assigned to someone
                    var assignedAssets = db.Assets
                        .Include(a => a.AssignedEmployee)
                        .Where(a => a.AssignedEmployeeId != null)
                        .ToList();

                    // Group by Department and calculate
                    var deptData = assignedAssets
                        .GroupBy(a => a.AssignedEmployee?.Department ?? "Unassigned")
                        .Select(g => {
                            decimal deptTotal = g.Sum(a => a.Price ?? 0);

                            // Logic: If the total system value is 0, we can't show a percentage.
                            // If the department has assets but no price, show a 1% sliver so it's visible.
                            double calculatedPercent = 0;
                            if (_totalInventoryValue > 0)
                            {
                                calculatedPercent = (double)(deptTotal / _totalInventoryValue * 100);
                            }
                            else if (g.Any())
                            {
                                calculatedPercent = 1; // Visual fallback
                            }

                            return new
                            {
                                DepartmentName = g.Key,
                                TotalValue = deptTotal,
                                Percentage = calculatedPercent
                            };
                        })
                        .OrderByDescending(x => x.TotalValue)
                        .ToList();

                    DeptBreakdownControl.ItemsSource = deptData;

                    // 3. Top 5 Expensive Assets
                    TopAssetsControl.ItemsSource = db.Assets
                        .OrderByDescending(a => a.Price)
                        .Take(5)
                        .ToList();

                    // 4. Load Recent Employees
                    var recentEmployees = db.Employees
                        .OrderByDescending(e => e.EmployeeId)
                        .Take(10)
                        .ToList();

                    RecentEmployeesControl.ItemsSource = recentEmployees.Select(e => new {
                        e.FullName,
                        e.JobTitle,
                        // Using a placeholder or actual DateAdded if it exists
                        DateAdded = DateTime.Now,
                        Initials = GetInitials(e.FullName)
                    }).ToList();

                    // 5. Activity Feed
                    ActivityFeedControl.ItemsSource = db.ActivityLogs
                        .AsNoTracking()
                        .OrderByDescending(l => l.LogId)
                        .Take(10)
                        .ToList()
                        .Select(l => new {
                            Description = $"{l.Details} | SN: {l.SerialNumber ?? "N/A"}",
                            TimeAgo = l.ActionDate.ToString("MMM dd, hh:mm tt")
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dashboard Load Error: {ex.Message}");
            }
        }

        // Helper Method for proper initials (e.g. Brian Jariel -> BJ)
        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "E";
            var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                return $"{parts[0][0]}{parts[parts.Length - 1][0]}".ToUpper();
            }
            return parts[0][0].ToString().ToUpper();
        }

        private void BtnToggleValue_Click(object sender, RoutedEventArgs e)
        {
            _isValueHidden = !_isValueHidden;
            UpdateValueDisplay();
        }

        private void UpdateValueDisplay()
        {
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