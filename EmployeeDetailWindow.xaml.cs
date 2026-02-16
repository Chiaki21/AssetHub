using AssetHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System;
using System.Windows.Controls;

namespace AssetHub
{
    public partial class EmployeeDetailWindow : Window
    {
        private int _employeeId;

        public EmployeeDetailWindow(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
            LoadEmployeeDetails();
        }

        private void LoadEmployeeDetails()
        {
            using (var db = new AssetHubDbContext())
            {
                var employee = db.Employees
                    .Include(e => e.Assets)
                    .FirstOrDefault(e => e.EmployeeId == _employeeId);

                if (employee != null)
                {
                    TxtFullName.Text = employee.FullName;
                    TxtJobTitle.Text = employee.JobTitle;

                    // Handle the UI States
                    if (employee.Assets != null && employee.Assets.Any())
                    {
                        var assetsList = employee.Assets.ToList();
                        AssignedAssetsControl.ItemsSource = assetsList;

                        // 1. Calculate Total Value
                        decimal totalValue = assetsList.Sum(a => a.Price ?? 0);
                        TxtTotalEmployeeValue.Text = totalValue.ToString("C2"); // Formats as ₱0.00

                        // 2. Set Visibility
                        AssetsScrollViewer.Visibility = Visibility.Visible;
                        TotalValueBorder.Visibility = Visibility.Visible;
                        EmptyStatePanel.Visibility = Visibility.Collapsed;
                        BtnReturnAll.IsEnabled = true;
                    }
                    else
                    {
                        AssetsScrollViewer.Visibility = Visibility.Collapsed;
                        TotalValueBorder.Visibility = Visibility.Collapsed;
                        EmptyStatePanel.Visibility = Visibility.Visible;
                        BtnReturnAll.IsEnabled = false;
                    }
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnReturnAll_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to return all assets to inventory?",
                                        "Confirm Bulk Return", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new AssetHubDbContext())
                    {
                        // Fetch the employee and their current assets
                        var employee = db.Employees
                            .Include(e => e.Assets)
                            .FirstOrDefault(e => e.EmployeeId == _employeeId);

                        if (employee != null && employee.Assets.Any())
                        {
                            int count = employee.Assets.Count;

                            foreach (var asset in employee.Assets.ToList())
                            {
                                // 1. Create the Audit Log
                                db.ActivityLogs.Add(new ActivityLog
                                {
                                    Details = $"{asset.AssetName} returned via Bulk Return (Employee: {employee.FullName})",
                                    SerialNumber = asset.SerialNumber,
                                    ActionDate = DateTime.Now
                                });

                                // 2. Reset Asset Status
                                asset.AssignedEmployeeId = null;
                                asset.Status = "Available";
                                asset.UpdatedAt = DateTime.Now;
                            }

                            db.SaveChanges();

                            // Show success notification
                            NotificationService.Show("Bulk Return", $"{count} assets returned to inventory.", NotificationToast.NotificationType.Success);

                            // Refresh the local UI
                            LoadEmployeeDetails();
                        }
                        else
                        {
                            NotificationService.Show("Info", "This employee has no assigned assets.", NotificationToast.NotificationType.Info);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during bulk return: {ex.Message}");
                }
            }
        }

        private void BtnUnassignIndividual_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Asset asset)
            {
                // Open your custom confirm window
                var confirmWin = new ConfirmUnassignWindow(asset);
                confirmWin.Owner = this;

                if (confirmWin.ShowDialog() == true)
                {
                    try
                    {
                        using (var db = new AssetHubDbContext())
                        {
                            var dbAsset = db.Assets.FirstOrDefault(a => a.AssetId == asset.AssetId);
                            if (dbAsset != null)
                            {
                                dbAsset.AssignedEmployeeId = null;
                                dbAsset.Status = "Available";
                                dbAsset.UpdatedAt = DateTime.Now;

                                db.ActivityLogs.Add(new ActivityLog
                                {
                                    Details = $"{dbAsset.AssetName} was unassigned via Employee Details.",
                                    SerialNumber = dbAsset.SerialNumber,
                                    ActionDate = DateTime.Now
                                });

                                db.SaveChanges();

                                // --- NEW: Add the Toast Notification here ---
                                NotificationService.Show(
                                    "Asset Unassigned",
                                    $"{dbAsset.AssetName} has been returned to inventory.",
                                    NotificationToast.NotificationType.Success);

                                // Refresh the list in this window
                                LoadEmployeeDetails();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }
    }
}