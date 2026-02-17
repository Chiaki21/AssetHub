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

            // Allows the user to drag the borderless window
            this.MouseDown += (s, e) => {
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                    this.DragMove();
            };
        }

        #region Login Logic
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string inputUser = txtUsername.Text.Trim();
            string inputPass = txtPassword.Password;

            if (string.IsNullOrEmpty(inputUser) || string.IsNullOrEmpty(inputPass))
            {
                lblError.Text = "Please enter both fields";
                lblError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                using (var db = new AssetHubDbContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Username == inputUser);

                    if (user != null && BC.Verify(inputPass, user.PasswordHash))
                    {
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
                        lblError.Text = "Invalid Username or Password";
                        lblError.Visibility = Visibility.Visible;
                        txtPassword.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Connection Error: {ex.Message}", "Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Registration Logic

        // Password Visibility Toggle Logic
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

            string password = txtRegPassword.Visibility == Visibility.Visible
                              ? txtRegPassword.Password
                              : txtRegPasswordVisible.Text;

            string confirmPassword = txtRegConfirmPassword.Password;

            // 1. Validation (Using FullName as the login name)
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match!", "Mismatch", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // Use FullName as the unique login key now
                    if (db.Users.Any(u => u.Username == fullName))
                    {
                        MessageBox.Show("This name is already registered.");
                        return;
                    }

                    string hashedPassword = BC.HashPassword(password);

                    var newUser = new User
                    {
                        FullName = fullName,
                        Username = fullName, // Setting Username equal to FullName for consistency
                        PasswordHash = hashedPassword,
                        Role = "Admin",
                        CreatedAt = DateTime.Now
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();

                    MessageBox.Show("Registration Successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ToggleViews_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
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

                // Clear registration fields
                txtRegFullName.Clear();
                txtRegEmail.Clear(); // Added this
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