using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace AssetHub
{
    public static class NotificationService
    {
       
        public static void Show(string title, string message, NotificationToast.NotificationType type = NotificationToast.NotificationType.Success)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            var area = mainWindow?.FindName("NotificationArea") as StackPanel;
            if (area == null) return;

            var toast = new NotificationToast(title, message, type);

            area.Children.Add(toast);

            // Animation
            toast.Opacity = 0;
            var fadeIn = new DoubleAnimation(1, TimeSpan.FromMilliseconds(400));
            toast.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            timer.Tick += (s, e) =>
            {
                var fadeOut = new DoubleAnimation(0, TimeSpan.FromMilliseconds(400));
                fadeOut.Completed += (s2, e2) => area.Children.Remove(toast);
                toast.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                timer.Stop();
            };
            timer.Start();
        }
    }
}