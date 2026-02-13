using System.Windows;
using System.Windows.Input;

namespace AssetHub
{
    public partial class ConfirmDeleteAssetWindow : Window
    {
        public ConfirmDeleteAssetWindow(string assetName, string serialNumber)
        {
            InitializeComponent();
            // Show specific details so the user knows exactly what they are deleting
            TxtMessage.Text = $"Are you sure you want to delete {assetName} (SN: {serialNumber})? This action cannot be undone.";
        }

        // Allows moving the window by clicking and dragging anywhere on the border
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true; // Signals back to AssetPage that we should proceed
        }
    }
}