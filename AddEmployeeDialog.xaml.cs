using System.Windows;
using System.Windows.Controls;
using AssetHub.Models;
using MaterialDesignThemes.Wpf;

namespace AssetHub
{
    public partial class AddEmployeeDialog : UserControl
    {
        public AddEmployeeDialog()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Simple validation
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a name.");
                return;
            }

            using (var context = new AssetHubDbContext())
            {
                var newEmp = new Employee
                {
                    FullName = txtName.Text,
                    Email = txtEmail.Text,
                    JobTitle = txtJob.Text,
                    Department = cmbDept.Text ?? "Unassigned"
                };

                context.Employees.Add(newEmp);
                context.SaveChanges();
            }

            // This closes the dialog and returns control to EmployeePage
            DialogHost.Close("RootDialog");
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Just close without saving
            DialogHost.Close("RootDialog");
        }
    }
}