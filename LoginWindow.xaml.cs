using AssetHub.Models;
using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace AssetHub
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            // Auto-login removed from here to prevent the crash you saw
            this.MouseDown += (s, e) => {
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                    this.DragMove();
            };
        }

        /// <summary>
        /// This method is called by App.xaml.cs to check if we can skip the login screen
        /// </summary>
        public bool TryAutoLogin()
        {
            if (!AssetHub.Properties.Settings.Default.IsRemembered) return false;

            string savedEmail = AssetHub.Properties.Settings.Default.SavedEmail;
            string savedPass = AssetHub.Properties.Settings.Default.SavedPassword;

            if (string.IsNullOrEmpty(savedEmail) || string.IsNullOrEmpty(savedPass)) return false;

            try
            {
                using (var db = new AssetHubDbContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Email == savedEmail);

                    if (user != null && BC.Verify(savedPass, user.PasswordHash))
                    {
                        // Set the session data
                        SessionManager.UserId = user.UserId;
                        SessionManager.Username = user.Username;
                        SessionManager.FullName = user.FullName;
                        SessionManager.Role = user.Role;
                        return true;
                    }
                }
            }
            catch
            {
                // If database fails, return false to show the manual login screen
            }
            return false;
        }

        #region Login Logic
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string inputEmail = txtEmail.Text.Trim();
            string inputPass = txtPassword.Password;

            if (string.IsNullOrEmpty(inputEmail) || string.IsNullOrEmpty(inputPass))
            {
                lblError.Text = "Please enter both fields";
                lblError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                using (var db = new AssetHubDbContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Email == inputEmail);

                    if (user != null && BC.Verify(inputPass, user.PasswordHash))
                    {
                        if (chkRememberMe.IsChecked == true)
                        {
                            AssetHub.Properties.Settings.Default.SavedEmail = inputEmail;
                            AssetHub.Properties.Settings.Default.SavedPassword = inputPass;
                            AssetHub.Properties.Settings.Default.IsRemembered = true;
                        }
                        else
                        {
                            AssetHub.Properties.Settings.Default.SavedEmail = string.Empty;
                            AssetHub.Properties.Settings.Default.SavedPassword = string.Empty;
                            AssetHub.Properties.Settings.Default.IsRemembered = false;
                        }
                        AssetHub.Properties.Settings.Default.Save();

                        SessionManager.UserId = user.UserId;
                        SessionManager.Username = user.Username;
                        SessionManager.FullName = user.FullName;
                        SessionManager.Role = user.Role;

                        MainWindow main = new MainWindow();
                        main.Show();
                        this.Close();
                    }
                    else
                    {
                        lblError.Text = "Invalid Email or Password";
                        lblError.Visibility = Visibility.Visible;
                        txtPassword.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Exception realError = ex;
                while (realError.InnerException != null)
                    realError = realError.InnerException;

                MessageBox.Show($"ACTUAL SQL ERROR: {realError.Message}", "Database Fault", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Registration Logic
        private void PasswordToggle_Checked(object sender, RoutedEventArgs e)
        {
            txtRegPasswordVisible.Text = txtRegPassword.Password;
            txtRegPassword.Visibility = Visibility.Collapsed;
            txtRegPasswordVisible.Visibility = Visibility.Visible;
        }

        private void PasswordToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            txtRegPassword.Password = txtRegPasswordVisible.Text;
            txtRegPasswordVisible.Visibility = Visibility.Collapsed;
            txtRegPassword.Visibility = Visibility.Visible;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtRegFullName.Text.Trim();
            string email = txtRegEmail.Text.Trim();
            string password = txtRegPassword.Visibility == Visibility.Visible ? txtRegPassword.Password : txtRegPasswordVisible.Text;
            string confirmPassword = txtRegConfirmPassword.Password;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                MainSnackbar.MessageQueue?.Enqueue("All fields are required!");
                return;
            }

            if (password != confirmPassword)
            {
                MainSnackbar.MessageQueue?.Enqueue("Passwords do not match!");
                return;
            }

            try
            {
                using (var db = new AssetHubDbContext())
                {
                    if (db.Users.Any(u => u.Email == email))
                    {
                        MainSnackbar.MessageQueue?.Enqueue("Email is already registered.");
                        return;
                    }

                    string hashedPassword = BC.HashPassword(password);
                    var newUser = new User
                    {
                        FullName = fullName,
                        Username = fullName,
                        Email = email,
                        PasswordHash = hashedPassword,
                        Role = "Admin",
                        CreatedAt = DateTime.Now
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();

                    MainSnackbar.MessageQueue?.Enqueue($"Welcome {fullName}! Account Created.");
                    ToggleViews_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Exception realError = ex;
                while (realError.InnerException != null)
                    realError = realError.InnerException;
                MessageBox.Show($"Registration Error: {realError.Message}");
            }
        }
        #endregion

        #region Navigation & Window Controls
        private void ToggleViews_Click(object sender, RoutedEventArgs e)
        {
            if (BorderLogin.Visibility == Visibility.Visible)
            {
                BorderLogin.Visibility = Visibility.Collapsed;
                BorderRegister.Visibility = Visibility.Visible;
            }
            else
            {
                BorderLogin.Visibility = Visibility.Visible;
                BorderRegister.Visibility = Visibility.Collapsed;

                txtRegFullName.Clear();
                txtRegEmail.Clear();
                txtRegPassword.Clear();
                txtRegPasswordVisible.Clear();
                txtRegConfirmPassword.Clear();
                if (BtnShowPass != null) BtnShowPass.IsChecked = false;
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion
    }
}