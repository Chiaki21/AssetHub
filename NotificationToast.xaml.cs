using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace AssetHub
{
    public partial class NotificationToast : UserControl
    {
        public enum NotificationType { Success, Info, Warning }

        public NotificationToast(string title, string message, NotificationType type)
        {
            InitializeComponent();
            TxtTitle.Text = title;
            TxtMessage.Text = message;

            switch (type)
            {
                case NotificationType.Success:
                    TypeIcon.Kind = PackIconKind.CheckCircle;
                    TypeIcon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
                    break;
                case NotificationType.Warning:
                    TypeIcon.Kind = PackIconKind.AlertCircle;
                    TypeIcon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444"));
                    break;
                case NotificationType.Info:
                    TypeIcon.Kind = PackIconKind.Information;
                    TypeIcon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
                    break;
            }
        }
    }
}