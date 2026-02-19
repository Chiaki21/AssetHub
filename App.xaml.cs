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

            // 2. Create the LoginWindow instance (but don't Show() it yet)
            LoginWindow login = new LoginWindow();

            // 3. Try to auto-login. If successful, jump to MainWindow.
            if (login.TryAutoLogin())
            {
                MainWindow main = new MainWindow();
                main.Show();
                login.Close(); // We close the invisible login window to clean up
            }
            else
            {
                // 4. If auto-login fails, show the Login screen as usual
                login.Show();
            }
        }
    }
}