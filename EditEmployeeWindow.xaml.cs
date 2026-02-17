using AssetHub.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AssetHub
{
    public partial class EditEmployeeWindow : Window
    {
        private int _employeeId;

        public EditEmployeeWindow(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            using (var db = new AssetHubDbContext())
            {
                var employee = db.Employees.Find(_employeeId);
                if (employee != null)
                {
                    TxtFullName.Text = employee.FullName;
                    TxtEmail.Text = employee.Email;
                    TxtJobTitle.Text = employee.JobTitle;

                    // Logic to select the correct item in the ComboBox
                    foreach (ComboBoxItem item in CmbDepartment.Items)
                    {
                        if (item.Content.ToString() == employee.Department)
                        {
                            CmbDepartment.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AssetHubDbContext())
            {
                var employee = db.Employees.Find(_employeeId);
                if (employee != null)
                {
                    employee.FullName = TxtFullName.Text.Trim();
                    employee.Email = TxtEmail.Text.Trim();
                    employee.JobTitle = TxtJobTitle.Text.Trim();

                    // Get the text from the selected ComboBoxItem
                    if (CmbDepartment.SelectedItem is ComboBoxItem selectedItem)
                    {
                        employee.Department = selectedItem.Content.ToString();
                    }

                    db.SaveChanges();
                    this.DialogResult = true;
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}