using System;
using System.Linq;
using System.Windows;
using AssetHub.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetHub
{
    public partial class AssignAssetWindow : Window
    {
        private int _assetId;

        public AssignAssetWindow(Asset assetToAssign)
        {
            InitializeComponent();
            _assetId = assetToAssign.AssetId;

            // Ensure TxtAssetName exists in your XAML
            if (TxtAssetName != null)
                TxtAssetName.Text = $"Assigning: {assetToAssign.AssetName}";

            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // Fetching active employees
                    var employees = db.Employees
                                      .AsNoTracking() // Better performance for read-only lists
                                      .Where(e => e.IsActive == true)
                                      .OrderBy(e => e.FullName)
                                      .ToList();

                    CbEmployees.ItemsSource = employees;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmp = CbEmployees.SelectedItem as Employee;
            if (selectedEmp == null)
            {
                MessageBox.Show("Please select an employee.");
                return;
            }

            try
            {
                using (var db = new AssetHubDbContext())
                {
                    var asset = db.Assets.FirstOrDefault(a => a.AssetId == _assetId);

                    if (asset != null)
                    {
                        // --- STEP 1: Update the actual Asset in the database ---
                        asset.AssignedEmployeeId = selectedEmp.EmployeeId;
                        asset.Status = "Assigned";
                        asset.UpdatedAt = DateTime.Now;

                        // --- STEP 2: Create the permanent Activity Log ---
                        db.ActivityLogs.Add(new ActivityLog
                        {
                            Details = $"{asset.AssetName} assigned to {selectedEmp.FullName}",
                            SerialNumber = asset.SerialNumber,
                            ActionDate = DateTime.Now
                        });

                        // --- STEP 3: Save BOTH changes to the database ---
                        db.SaveChanges();

                        NotificationService.Show("Assigned", $"Asset linked to {selectedEmp.FullName}", NotificationToast.NotificationType.Info);

                        this.DialogResult = true;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}