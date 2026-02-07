using System;
using System.Linq;
using System.Windows;
using AssetHub.Models;
using Microsoft.EntityFrameworkCore; // Important for DB operations

namespace AssetHub
{
    public partial class AssignAssetWindow : Window
    {
        private int _assetId; // Store the ID of the asset we are assigning

        // Constructor receives the Asset object passed from the main page
        public AssignAssetWindow(Asset assetToAssign)
        {
            InitializeComponent();
            _assetId = assetToAssign.AssetId;
            TxtAssetName.Text = $"Assigning: {assetToAssign.AssetName}";
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // Get all active employees to fill the dropdown
                    var employees = db.Employees
                                      .Where(e => e.IsActive == true)
                                      .ToList();

                    CbEmployees.ItemsSource = employees;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}");
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (CbEmployees.SelectedValue == null)
            {
                MessageBox.Show("Please select an employee.");
                return;
            }

            int selectedEmployeeId = (int)CbEmployees.SelectedValue;

            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // 1. Find the asset in the database
                    var asset = db.Assets.Find(_assetId);

                    if (asset != null)
                    {
                        // 2. Update its properties
                        asset.AssignedEmployeeId = selectedEmployeeId;
                        asset.Status = "Assigned"; // Auto-update status!

                        // 3. Save changes
                        db.SaveChanges();

                        MessageBox.Show("Asset assigned successfully!");
                        this.DialogResult = true; // Tells the parent window we succeeded
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}