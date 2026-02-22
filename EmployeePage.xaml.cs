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
            addWin.Owner = Window.GetWindow(this);

            if (addWin.ShowDialog() == true)
            {
                // 1. Refresh the UI list
                LoadEmployees();

                // 2. Get the name from the window we just closed
                string newName = addWin.EmployeeName;

                // 3. Show the personalized toast
                if (Window.GetWindow(this) is MainWindow mainWindow)
                {
                    mainWindow.ShowNotification($"Welcome to the team, {newName}!");
                }
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
                    string deletedName = employeeFromUi.FullName; // Store for the toast

                    using (var db = new AssetHubDbContext())
                    {
                        var employeeToDelete = db.Employees
                            .Include(emp => emp.Assets)
                            .FirstOrDefault(emp => emp.EmployeeId == employeeFromUi.EmployeeId);

                        if (employeeToDelete != null)
                        {
                            foreach (var asset in employeeToDelete.Assets)
                            {
                                asset.AssignedEmployeeId = null;
                                asset.Status = "Available";
                            }

                            db.Employees.Remove(employeeToDelete);
                            db.SaveChanges();

                            // Show the toast
                            if (Window.GetWindow(this) is MainWindow mainWindow)
                            {
                                mainWindow.ShowNotification($"{deletedName} has been removed and linked assets updated.");
                            }
                        }
                        LoadEmployees();
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