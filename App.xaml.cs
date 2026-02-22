using System;
using System.Windows;
using Microsoft.EntityFrameworkCore; // Required for .Migrate()
using AssetHub.Models;               // Required for your DbContext
using System.Threading;              // Required for Sleep

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

            // --- AUTO-MIGRATION LOGIC FOR YOUR FRIEND ---
            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // If the Docker SQL is still booting up, we wait a few seconds
                    // This prevents the app from crashing on the very first run
                    int retries = 5;
                    while (retries > 0)
                    {
                        try
                        {
                            // This builds the tables, columns, and seed data automatically
                            db.Database.Migrate();
                            break; // Success! Exit the retry loop
                        }
                        catch
                        {
                            retries--;
                            if (retries == 0) throw; // If it still fails after 5 tries, show error
                            Thread.Sleep(3000); // Wait 3 seconds before trying again
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Connection Error: {ex.Message}\n\nPlease ensure Docker Desktop is running.",
                                "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }
            // --- END OF MIGRATION LOGIC ---

            // 1. Clear any old session data just in case
            SessionManager.UserId = 0;
            SessionManager.Username = null;

            // 2. Create the LoginWindow instance
            LoginWindow login = new LoginWindow();

            // 3. Try to auto-login. If successful, jump to MainWindow.
            if (login.TryAutoLogin())
            {
                MainWindow main = new MainWindow();
                main.Show();
                login.Close();
            }
            else
            {
                // 4. If auto-login fails, show the Login screen as usual
                login.Show();
            }
        }
    }
}