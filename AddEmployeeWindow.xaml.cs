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
            if (string.IsNullOrWhiteSpace(TxtName.Text)) return;

            using (var db = new AssetHubDbContext())
            {
                var emp = new Employee
                {
                    FullName = TxtName.Text,
                    Email = TxtEmail.Text,
                    Department = (CbDept.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    JobTitle = TxtTitle.Text,
                    IsActive = true
                };
                db.Employees.Add(emp);
                db.SaveChanges();
            }
            this.DialogResult = true;
        }
    }
}