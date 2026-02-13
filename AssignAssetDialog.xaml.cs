using AssetHub.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AssetHub
{
    /// <summary>
    /// Interaction logic for AssignAssetDialog.xaml
    /// </summary>
    public partial class AssignAssetDialog : UserControl
    {
        public AssignAssetDialog()
        {
            InitializeComponent();
            LoadDropdowns();
        }

        private void LoadDropdowns()
        {
            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // Use .AsNoTracking() here for better performance in dropdowns
                    cmbEmployees.ItemsSource = db.Employees.AsNoTracking().ToList();

                    // Only show assets that aren't already taken
                    cmbAssets.ItemsSource = db.Assets
                                              .AsNoTracking()
                                              .Where(a => a.Status == "Available")
                                              .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private void Assign_Click(object sender, RoutedEventArgs e)
        {
            // 1. Grab the actual objects from the comboboxes
            var selectedEmp = cmbEmployees.SelectedItem as Employee;
            var selectedAsset = cmbAssets.SelectedItem as Asset;

            if (selectedEmp == null || selectedAsset == null)
            {
                MessageBox.Show("Please select both an employee and an asset.", "Selection Required");
                return;
            }

            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // 2. Find the asset in the database using the ID from our selection
                    var asset = db.Assets.FirstOrDefault(a => a.AssetId == selectedAsset.AssetId);

                    if (asset != null)
                    {
                        // 3. Update the Foreign Key and Status
                        asset.AssignedEmployeeId = selectedEmp.EmployeeId;
                        asset.Status = "Assigned";

                        db.SaveChanges();

                        // 4. Close the DialogHost using the correct Identifier
                        MaterialDesignThemes.Wpf.DialogHost.Close("RootDialog");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error assigning asset: {ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) =>
            MaterialDesignThemes.Wpf.DialogHost.Close("RootDialog");
    }
}