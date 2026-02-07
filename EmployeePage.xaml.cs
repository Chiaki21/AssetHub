using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AssetHub.Models;

namespace AssetHub
{
    public partial class EmployeePage : Page
    {
        public EmployeePage()
        {
            InitializeComponent();
            LoadEmployees();
        }

        // Fetch all employees from SQL Express
        private void LoadEmployees()
        {
            try
            {
                using (var db = new AssetHubDbContext())
                {
                    var list = db.Employees.ToList();
                    EmployeeGrid.ItemsSource = list;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load employees: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Search Logic: Filters by Name, Email, or Department
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = TxtSearch.Text.ToLower().Trim();

            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // If search is empty, show everyone
                    if (string.IsNullOrWhiteSpace(query))
                    {
                        LoadEmployees();
                        return;
                    }

                    // Perform search
                    var filtered = db.Employees
                        .Where(emp => emp.FullName.ToLower().Contains(query) ||
                                      emp.Email.ToLower().Contains(query) ||
                                      emp.Department.ToLower().Contains(query))
                        .ToList();

                    EmployeeGrid.ItemsSource = filtered;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Search failed: {ex.Message}");
            }
        }

        private void BtnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            AddEmployeeWindow addWin = new AddEmployeeWindow();
            if (addWin.ShowDialog() == true)
            {
                LoadEmployees(); // Refresh list after adding new person
            }
        }
    }
}