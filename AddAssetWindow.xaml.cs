using AssetHub.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AssetHub
{
    public partial class AddAssetWindow : Window
    {
        public AddAssetWindow()
        {
            InitializeComponent();
        }

        #region Input Validation & Formatting

        // Blocks non-numeric keys from being typed (Allows 0-9 and one decimal point)
        private void TxtPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            // Prevent multiple decimal points
            if (e.Text == "." && textBox.Text.Contains("."))
            {
                e.Handled = true;
                return;
            }

            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // Blocks non-numeric text from being pasted
        private void TxtPrice_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);
                if (new Regex("[^0-9.]+").IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
        }

        // When user clicks in, strip formatting (e.g., ₱ or commas) for easy editing
        private void TxtPrice_GotFocus(object sender, RoutedEventArgs e)
        {
            string cleanText = Regex.Replace(TxtPrice.Text, @"[^0-9.]", "");
            TxtPrice.Text = cleanText;
            TxtPrice.SelectAll();
        }

        // When user clicks out, format as currency (e.g., 1200 becomes ₱1,200.00)
        private void TxtPrice_LostFocus(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(TxtPrice.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal price))
            {
                TxtPrice.Text = price.ToString("C2");
            }
        }

        #endregion

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Basic Validation
            if (string.IsNullOrWhiteSpace(TxtAssetName.Text))
            {
                MessageBox.Show("Please enter an Asset Name.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Parse price allowing for currency symbols
            if (!decimal.TryParse(TxtPrice.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal price))
            {
                MessageBox.Show("Please enter a valid price.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        Price = price,
                        // Unique Serial Number Generation
                        SerialNumber = "SN-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
                    };

                    db.Assets.Add(newAsset);
                    db.SaveChanges();
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show($"Error saving asset: {message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}