using System;
using System.Windows;
using System.Windows.Controls;
using AssetHub.Models;

namespace AssetHub
{
    public partial class EditAssetWindow : Window
    {
        private Asset _assetToEdit;

        public EditAssetWindow(Asset asset)
        {
            InitializeComponent();
            _assetToEdit = asset;
            PrepopulateFields();
        }

        private void PrepopulateFields()
        {
            // Load current data into the boxes
            TxtAssetName.Text = _assetToEdit.AssetName;
            TxtSerialNumber.Text = _assetToEdit.SerialNumber;

            // Select the correct item in ComboBox
            foreach (ComboBoxItem item in CbAssetType.Items)
            {
                if (item.Content.ToString() == _assetToEdit.AssetType)
                {
                    CbAssetType.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // 1. Fetch the asset from the DB again (safest way)
                    var assetInDb = db.Assets.Find(_assetToEdit.AssetId);

                    if (assetInDb != null)
                    {
                        // 2. Update the fields
                        assetInDb.AssetName = TxtAssetName.Text;
                        assetInDb.AssetType = (CbAssetType.SelectedItem as ComboBoxItem)?.Content.ToString();

                        // 3. Save
                        db.SaveChanges();

                        this.DialogResult = true;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}