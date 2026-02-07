using AssetHub.Models; 
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace AssetHub
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string inputUser = txtUsername.Text;
            string inputPass = txtPassword.Password;

            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // DEBUG: Basic check without complex extensions
                    var userCount = db.Users.Count();
                    var firstUser = db.Users.FirstOrDefault();

                    // Get the connection string directly from the context configuration
                    // Note: We are just grabbing the raw string this time
                    string connString = db.Database.GetConnectionString() ?? "Unknown Connection";

                    if (userCount == 0)
                    {
                        MessageBox.Show($"DATABASE IS EMPTY!\n\n" +
                                        $"Connected to: {connString}\n\n" +
                                        "The app is looking at the wrong database.",
                                        "Debug Info");
                        return;
                    }

                    var user = db.Users.FirstOrDefault(u => u.Username == inputUser && u.Password == inputPass);

                    if (user != null)
                    {
                        MainWindow main = new MainWindow();
                        main.Show();
                        this.Close();
                    }
                    else
                    {
                        // Show mismatch details
                        MessageBox.Show($"Login Failed.\n\n" +
                                        $"You typed: '{inputUser}' / '{inputPass}'\n" +
                                        $"DB has: '{firstUser.Username}' / '{firstUser.Password}'",
                                        "Credentials Mismatch");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"CRITICAL ERROR: {ex.Message}");
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}