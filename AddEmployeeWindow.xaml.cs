using System;
using System.Windows;
using System.Windows.Controls;
using AssetHub.Models;

namespace AssetHub
{
    public partial class AddEmployeeWindow : Window
    {
        public AddEmployeeWindow() => InitializeComponent();

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AssetHubDbContext())
            {
                var newEmp = new Employee
                {
                    FullName = TxtName.Text,
                    Email = TxtEmail.Text,
                    JobTitle = TxtPosition.Text, // Now saving the position
                    Department = (CmbDept.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    DateAdded = DateTime.Now,    // Real date
                    IsActive = true
                };

                db.Employees.Add(newEmp);
                db.SaveChanges();
                this.DialogResult = true;
            }
        }
    }
}