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
            // 1. Get the selected Employee object directly
            var selectedEmp = CbEmployees.SelectedItem as Employee;

            if (selectedEmp == null)
            {
                MessageBox.Show("Please select an employee before confirming.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // 2. Find the asset in the database
                    var asset = db.Assets.FirstOrDefault(a => a.AssetId == _assetId);

                    if (asset != null)
                    {
                        // 3. Update properties
                        asset.AssignedEmployeeId = selectedEmp.EmployeeId;
                        asset.Status = "Assigned";

                        // If you kept the LastModified column, uncomment this:
                        // asset.LastModified = DateTime.Now;

                        // 4. Save changes
                        db.SaveChanges();

                        MessageBox.Show($"Asset assigned successfully to {selectedEmp.FullName}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("The asset could not be found in the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}