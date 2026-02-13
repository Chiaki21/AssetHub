using AssetHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AssetHub
{
    /// <summary>
    /// Interaction logic for EmployeeDetailsWindow.xaml
    /// </summary>
    public partial class EmployeeDetailsWindow : Window
    {
        public EmployeeDetailsWindow(Employee emp)
        {
            InitializeComponent();
            TxtName.Text = emp.FullName;
            TxtJobTitle.Text = emp.JobTitle;
            TxtEmail.Text = emp.Email ?? "No email provided";
            TxtDept.Text = emp.Department ?? "General";
            TxtInitials.Text = emp.Initials;
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
