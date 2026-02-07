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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AssetHub
{
    /// <summary>
    /// Interaction logic for AssignAssetDialog.xaml
    /// </summary>
    public partial class AssignAssetDialog : UserControl
    {
        public AssignAssetDialog()
        {
            InitializeComponent();
            LoadDropdowns();
        }

        private void LoadDropdowns()
        {
            using (var db = new AssetHubDbContext())
            {
                cmbEmployees.ItemsSource = db.Employees.ToList();
                // Only show assets that aren't already taken
                cmbAssets.ItemsSource = db.Assets.Where(a => a.Status == "Available").ToList();
            }
        }

        private void Assign_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmp = (Employee)cmbEmployees.SelectedItem;
            var selectedAsset = (Asset)cmbAssets.SelectedItem;

            if (selectedEmp == null || selectedAsset == null) return;

            using (var db = new AssetHubDbContext())
            {
                var asset = db.Assets.Find(selectedAsset.AssetId);
                asset.AssignedEmployeeId = selectedEmp.EmployeeId;
                asset.Status = "Assigned";
                db.SaveChanges();
            }
            MaterialDesignThemes.Wpf.DialogHost.Close("RootDialog");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) =>
            MaterialDesignThemes.Wpf.DialogHost.Close("RootDialog");
    }
}
