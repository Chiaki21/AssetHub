using System.Windows;
using System.Windows.Input;

namespace AssetHub
{
    public partial class ConfirmDeleteWindow : Window
    {
        public ConfirmDeleteWindow(string employeeName)
        {
            InitializeComponent();
            TxtMessage.Text = $"Are you sure you want to delete {employeeName}? This action cannot be undone and will unassign their hardware.";
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true; 
        }
    }
}