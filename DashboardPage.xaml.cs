using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AssetHub.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetHub
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            LoadDashboardStats();
        }

        private void LoadDashboardStats()
        {
            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // 1. Count Total Assets
                    int total = db.Assets.Count();

                    // 2. Count Assigned Assets (where status is 'Assigned')
                    int assigned = db.Assets.Count(a => a.Status == "Assigned");

                    // 3. Count Available Assets
                    int available = db.Assets.Count(a => a.Status == "Available");

                    // 4. Update the UI
                    TxtTotalAssets.Text = total.ToString();
                    TxtAssignedAssets.Text = assigned.ToString();
                    TxtAvailableAssets.Text = available.ToString();
                }
            }
            catch (Exception ex)
            {
                // In a dashboard, we might not want to pop up an error box immediately, 
                // but for debugging, it helps.
                MessageBox.Show($"Could not load stats: {ex.Message}");
            }
        }
    }
}