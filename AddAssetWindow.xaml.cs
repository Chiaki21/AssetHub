using AssetHub.Models;
using System.Windows;
using System.Windows.Controls;

namespace AssetHub
{
    public partial class AddAssetWindow : Window
    {
        public AddAssetWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtAssetName.Text))
            {
                MessageBox.Show("Please enter an Asset Name.");
                return;
            }

            try
            {
                using (var db = new AssetHubDbContext())
                {
                    var newAsset = new Asset
                    {
                        AssetName = TxtAssetName.Text,
                        AssetType = (CbAssetType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "General",
                        Status = "Available",

                        // FIX: Auto-generate a unique Serial Number to satisfy the database rule
                        SerialNumber = "SN-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
                    };

                    db.Assets.Add(newAsset);
                    db.SaveChanges();
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                // THIS IS THE KEY FIX: Look at the InnerException
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show($"Database Error: {innerMessage}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General Error: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}