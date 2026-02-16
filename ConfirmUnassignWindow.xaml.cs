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
    public partial class ConfirmUnassignWindow : Window
    {
        // Change the parameter from 'string' to 'Asset'
        public ConfirmUnassignWindow(AssetHub.Models.Asset asset)
        {
            InitializeComponent();

            // Now you can show both Name and Serial Number for better clarity
            TxtAssetName.Text = $"{asset.AssetName} (SN: {asset.SerialNumber})";
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
