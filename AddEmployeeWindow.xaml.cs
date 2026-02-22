using System;
using System.Windows;
using System.Windows.Controls;
using AssetHub.Models;

namespace AssetHub
{
    public partial class AddEmployeeWindow : Window
    {
        // Add this property to store the name for the toast
        public string EmployeeName { get; private set; }

        public AddEmployeeWindow() => InitializeComponent();

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. Capture the name from the TextBox
            EmployeeName = TxtName.Text.Trim();

            if (string.IsNullOrEmpty(EmployeeName))
            {
                MessageBox.Show("Please enter a name.");
                return;
            }

            using (var db = new AssetHubDbContext())
            {
                var newEmp = new Employee
                {
                    FullName = EmployeeName,
                    Email = TxtEmail.Text,
                    JobTitle = TxtPosition.Text,
                    Department = (CmbDept.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    DateAdded = DateTime.Now,
                    IsActive = true
                };

                db.Employees.Add(newEmp);
                db.SaveChanges();

                this.DialogResult = true; // This closes the window
            }
        }
    }
}