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
            this.Loaded += (s, e) => {
                MainContentFrame.Navigate(new DashboardPage());
            };
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
            // Create new Login window
            LoginWindow login = new LoginWindow();
            login.Show();

            // Close current Dashboard window
            this.Close();
        }
    }
}