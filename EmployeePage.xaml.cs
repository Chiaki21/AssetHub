using AssetHub.Models;
using Microsoft.EntityFrameworkCore; // Required for .Include
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AssetHub
{
    public partial class EmployeePage : Page
    {
        public EmployeePage()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // Use .Include(e => e.Assets) so we can count them in the UI
                    var list = db.Employees
                                 .Include(e => e.Assets)
                                 .ToList();

                    // Changed from EmployeeGrid to EmployeeItemsControl
                    EmployeeItemsControl.ItemsSource = list;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load employees: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Ensure this matches the x:Name in your XAML exactly
            if (txtSearch == null) return;

            string query = txtSearch.Text.ToLower().Trim();

            try
            {
                using (var db = new AssetHubDbContext())
                {
                    if (string.IsNullOrWhiteSpace(query))
                    {
                        LoadEmployees();
                        return;
                    }

                    var filtered = db.Employees
                        .Include(emp => emp.Assets)
                        .Where(emp => emp.FullName.ToLower().Contains(query) ||
                                      emp.Email.ToLower().Contains(query) ||
                                      emp.Department.ToLower().Contains(query))
                        .ToList();

                    EmployeeItemsControl.ItemsSource = filtered;
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
                LoadEmployees();
            }
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // 1. Get the employee object from the button's data context
            var employeeFromUi = (sender as Button)?.DataContext as Employee;
            if (employeeFromUi == null) return;

            var dialog = new ConfirmDeleteWindow(employeeFromUi.FullName) { Owner = Window.GetWindow(this) };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    using (var db = new AssetHubDbContext())
                    {
                        // 2. Fetch the "Fresh" employee from the DB including their assets
                        // This prevents the "Already being tracked" error
                        var employeeToDelete = db.Employees
                            .Include(emp => emp.Assets)
                            .FirstOrDefault(emp => emp.EmployeeId == employeeFromUi.EmployeeId);

                        if (employeeToDelete != null)
                        {
                            // 3. Unassign all linked assets
                            foreach (var asset in employeeToDelete.Assets)
                            {
                                asset.AssignedEmployeeId = null;
                                asset.Status = "Available";
                            }

                            // 4. Delete the employee
                            db.Employees.Remove(employeeToDelete);

                            // 5. Save everything in one go
                            db.SaveChanges();
                        }

                        LoadEmployees(); // Refresh your beautiful UI list
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void EmployeeRow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var employee = (sender as Border)?.DataContext as Employee;
            if (employee == null) return;

            var detailWindow = new EmployeeDetailWindow(employee.EmployeeId);
            detailWindow.Owner = Window.GetWindow(this);

            // This code waits here until the window is closed
            detailWindow.ShowDialog();

            // REFRESH the main list as soon as they close the popup!
            LoadEmployees();
        }
    }
}