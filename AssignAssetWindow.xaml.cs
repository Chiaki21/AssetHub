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
            // 1. Get the text currently typed/selected in the ComboBox
            string inputName = CbEmployees.Text.Trim();

            if (string.IsNullOrWhiteSpace(inputName))
            {
                NotificationService.Show("Validation", "Please select an employee.", NotificationToast.NotificationType.Warning);
                return;
            }

            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // 2. VALIDATION: Check if this name exists in our database
                    // We do a case-insensitive check to be user-friendly
                    var selectedEmp = db.Employees
                        .FirstOrDefault(emp => emp.FullName.ToLower() == inputName.ToLower() && emp.IsActive == true);

                    if (selectedEmp == null)
                    {
                        // If no match is found, show a warning and stop the process
                        NotificationService.Show("Invalid Employee",
                            $"'{inputName}' is not a registered employee. Please select from the list.",
                            NotificationToast.NotificationType.Warning);
                        return;
                    }

                    // 3. Proceed with Assignment since the employee is valid
                    var asset = db.Assets.FirstOrDefault(a => a.AssetId == _assetId);

                    if (asset != null)
                    {
                        asset.AssignedEmployeeId = selectedEmp.EmployeeId;
                        asset.Status = "Assigned";
                        asset.UpdatedAt = DateTime.Now;

                        db.ActivityLogs.Add(new ActivityLog
                        {
                            Details = $"{asset.AssetName} assigned to {selectedEmp.FullName}",
                            SerialNumber = asset.SerialNumber,
                            ActionDate = DateTime.Now
                        });

                        db.SaveChanges();

                        NotificationService.Show("Assignment Success",
                            $"{asset.AssetName} assigned to {selectedEmp.FullName}",
                            NotificationToast.NotificationType.Success);

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