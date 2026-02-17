using System;
using System.Windows;
using System.Windows.Threading; // Required for DispatcherPriority

namespace AssetHub
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Safety check: use Username if FullName isn't set
            string displayName = !string.IsNullOrEmpty(SessionManager.FullName)
                                 ? SessionManager.FullName
                                 : SessionManager.Username;

            txtUserDisplay.Text = displayName;
            txtRoleDisplay.Text = SessionManager.Role;

            // Set Initials (e.g., "Justine" -> "JU" or "Brian Jariel" -> "BJ")
            if (!string.IsNullOrEmpty(displayName))
            {
                var parts = displayName.Split(' ');
                if (parts.Length > 1)
                    txtUserInitials.Text = (parts[0][0].ToString() + parts[1][0].ToString()).ToUpper();
                else
                    txtUserInitials.Text = displayName.Substring(0, Math.Min(2, displayName.Length)).ToUpper();
            }

            MainContentFrame.Navigate(new DashboardPage());
        }

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
        private void BtnSignOut_Click(object sender, RoutedEventArgs e)
        {
            // Clear session
            SessionManager.UserId = 0;
            SessionManager.Username = null;

            // Open Login
            LoginWindow login = new LoginWindow();
            login.Show();

            // Close the current MainWindow
            this.Close();
        }
    }
}