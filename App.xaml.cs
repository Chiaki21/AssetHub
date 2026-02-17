using System;
using System.Windows;
using BC = BCrypt.Net.BCrypt;

namespace AssetHub
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Force regional formatting
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    System.Windows.Markup.XmlLanguage.GetLanguage(
                        System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)));

            // 1. Clear any old session data just in case
            SessionManager.UserId = 0;
            SessionManager.Username = null;

            // 2. Start ONLY the Login Window
            LoginWindow login = new LoginWindow();
            login.Show();
        }
    }
}