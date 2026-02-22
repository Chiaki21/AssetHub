using System;
using System.Windows;
using materialDesign = MaterialDesignThemes.Wpf; // Useful for explicit typing

namespace AssetHub
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Initialize the Snackbar Message Queue
            MainSnackbar.MessageQueue = new materialDesign.SnackbarMessageQueue(TimeSpan.FromSeconds(3));

            // Set User Display Information
            string displayName = !string.IsNullOrEmpty(SessionManager.FullName)
                                 ? SessionManager.FullName
                                 : SessionManager.Username;

            txtUserDisplay.Text = displayName;
            txtRoleDisplay.Text = SessionManager.Role;

            // Generate Initials
            if (!string.IsNullOrEmpty(displayName))
            {
                var parts = displayName.Split(' ');
                if (parts.Length > 1)
                    txtUserInitials.Text = (parts[0][0].ToString() + parts[1][0].ToString()).ToUpper();
                else
                    txtUserInitials.Text = displayName.Substring(0, Math.Min(2, displayName.Length)).ToUpper();
            }

            // Load initial page
            MainContentFrame.Navigate(new DashboardPage());
        }

        // PUBLIC HELPER METHOD: Call this from any Page to show a toast
        public void ShowNotification(string message)
        {
            MainSnackbar.MessageQueue?.Enqueue(message);
        }

        #region Navigation
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = "Dashboard";
            MainContentFrame.Navigate(new DashboardPage());
        }

        private void BtnEmployees_Click(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = "Employees";
            MainContentFrame.Navigate(new EmployeePage());
        }

        private void BtnAssets_Click(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = "Assets";
            MainContentFrame.Navigate(new AssetPage());
        }
        #endregion

        private void BtnSignOut_Click(object sender, RoutedEventArgs e)
        {
            // Clear Auto-Login settings
            AssetHub.Properties.Settings.Default.IsRemembered = false;
            AssetHub.Properties.Settings.Default.Save();

            // Clear session data
            SessionManager.UserId = 0;
            SessionManager.Username = null;

            // Redirect to Login
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}