using AssetHub.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32; // For SaveFileDialog
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AssetHub
{
    public partial class AssetPage : Page
    {
        public AssetPage()
        {
            InitializeComponent();
            LoadAssets();
        }

        // 1. Search Logic (Safe Version)
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string query = txtSearch.Text?.ToLower() ?? ""; // Handle null search text

                using (var db = new AssetHubDbContext())
                {
                    // FIX: Load everything first to avoid SQL translation errors
                    var allAssets = db.Assets.ToList();

                    var filteredList = db.Assets
                        .Where(a => (a.AssetName != null && a.AssetName.ToLower().Contains(query)) ||
                                    (a.AssetType != null && a.AssetType.ToLower().Contains(query)) ||
                                    (a.SerialNumber != null && a.SerialNumber.ToLower().Contains(query))) // Added Serial Number Search
                        .ToList();



                    // Display safe values
                    foreach (var asset in filteredList)
                    {
                        if (asset.AssetName == null) asset.AssetName = "Unknown Asset";
                        if (asset.AssetType == null) asset.AssetType = "Unspecified";
                        if (asset.SerialNumber == null) asset.SerialNumber = "N/A"; // Handle null serial display
                        if (asset.Status == null) asset.Status = "Available";
                    }



                    AssetItemsControl.ItemsSource = filteredList;
                }
            }
            catch (Exception ex)
            {
                // If it fails, just show the empty list, don't crash
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        // 2. Load Assets (CRASH FIX IS HERE)

        private List<Asset> _allAssets = new List<Asset>();
        private void LoadAssets()
        {
            try
            {
                using (var db = new AssetHubDbContext())
                {
                    // 1. Fetch data with Eager Loading
                    // We remove AsNoTracking to ensure the objects are fully "attached"
                    _allAssets = db.Assets
                                   .Include(a => a.AssignedEmployee)
                                   .ToList();

                    // 2. Sanitize and Verify
                    foreach (var asset in _allAssets)
                    {
                        if (string.IsNullOrEmpty(asset.Status)) asset.Status = "Available";

                        // This will print to your "Output" window in Visual Studio
                        if (asset.AssignedEmployee != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"SUCCESS: {asset.AssetName} linked to {asset.AssignedEmployee.FullName}");
                        }
                    }

                    // 3. Assign to UI
                    AssetItemsControl.ItemsSource = null;
                    AssetItemsControl.ItemsSource = _allAssets;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"UI Error: {ex.Message}");
            }
        }

        private void BtnStatus_Click(object sender, RoutedEventArgs e)
        {
            var asset = (sender as Button).DataContext as Asset;

            if (asset?.Status == "Assigned" && asset.AssignedEmployee != null)
            {
                EmployeeDetailsWindow detailsWindow = new EmployeeDetailsWindow(asset.AssignedEmployee);
                detailsWindow.Owner = Window.GetWindow(this); // Centers it relative to the app
                detailsWindow.ShowDialog();
            }
        }
        // 3. Add Asset Button
        private void BtnAddAsset_Click(object sender, RoutedEventArgs e)
        {
            AddAssetWindow addWindow = new AddAssetWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadAssets();
            }
        }

        // 4. Delete Button (With Database Attach Fix)
        private void BtnUnassign_Click(object sender, RoutedEventArgs e)
        {
            var asset = (sender as Button)?.DataContext as Asset;
            if (asset == null) return;

            // Only allow unassigning if the asset is actually assigned
            if (asset.Status != "Assigned")
            {
                MessageBox.Show("This asset is already unassigned.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to unassign {asset.AssetName}?",
                                       "Confirm Unassign", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new AssetHubDbContext())
                    {
                        var dbAsset = db.Assets.FirstOrDefault(a => a.AssetId == asset.AssetId);
                        if (dbAsset != null)
                        {
                            dbAsset.AssignedEmployeeId = null;
                            dbAsset.Status = "Available";
                            dbAsset.UpdatedAt = DateTime.Now;

                            db.ActivityLogs.Add(new ActivityLog
                            {
                                Details = $"{dbAsset.AssetName} was unassigned and returned to inventory",
                                SerialNumber = dbAsset.SerialNumber,
                                ActionDate = DateTime.Now
                            });

                            db.SaveChanges();

                            NotificationService.Show(
    "Asset Returned",
    $"{dbAsset.AssetName} was unassigned and returned to inventory.",
    NotificationToast.NotificationType.Info
);
                        }
                    }
                    LoadAssets(); // Refresh the UI
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not unassign: {ex.Message}");
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var asset = (sender as Button)?.DataContext as Asset;
                if (asset == null) return;

                // Use your custom delete window
                var dialog = new ConfirmDeleteAssetWindow(asset.AssetName, asset.SerialNumber);
                dialog.Owner = Window.GetWindow(this);

                if (dialog.ShowDialog() == true)
                {
                    // Store the name in a variable before deleting
                    string deletedName = asset.AssetName;

                    using (var db = new AssetHubDbContext())
                    {
                        var dbAsset = db.Assets.FirstOrDefault(a => a.AssetId == asset.AssetId);
                        if (dbAsset != null)
                        {
                            db.Assets.Remove(dbAsset);
                            db.SaveChanges();
                        }
                    }

                    LoadAssets();

                    // Use the name in the notification
                    NotificationService.Show(
                        "Asset Deleted",
                        $"{deletedName} has been permanently removed.",
                        NotificationToast.NotificationType.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not delete: {ex.Message}");
            }
        }

        // 5. Assign Button (Placeholder)
        private void BtnAssign_Click(object sender, RoutedEventArgs e)
        {
            // Get the asset from the row where the button was clicked
            var button = sender as Button;
            var asset = button.DataContext as Asset;

            if (asset == null) return;

            // Open the Assign Window and pass the asset to it
            AssignAssetWindow assignWindow = new AssignAssetWindow(asset);

            // If the window returns "true" (Success), reload the list to show changes
            if (assignWindow.ShowDialog() == true)
            {
                LoadAssets();
            }
        }
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var asset = button.DataContext as Asset;

            if (asset == null) return;

            EditAssetWindow editWindow = new EditAssetWindow(asset);

            // If the user clicked "Save", reload the list to show the new name
            if (editWindow.ShowDialog() == true)
            {
                LoadAssets();
            }
        }
        private void BtnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            List<Asset> assets;
            using (var db = new AssetHubDbContext())
            {
                // 1. Use .Include to bring in the Employee data for each asset
                assets = db.Assets
                           .Include(a => a.AssignedEmployee)
                           .ToList();
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = $"Asset_Inventory_Report_{DateTime.Now:yyyyMMdd}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                QuestPDF.Settings.License = LicenseType.Community;

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(50);

                        // Header
                        page.Header().Row(row =>
                        {
                            row.RelativeItem().Text("AssetHub Inventory Report").FontSize(24).SemiBold().FontColor(Colors.Blue.Medium);
                            row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("f")).FontSize(10).FontColor(Colors.Grey.Medium);
                        });

                        page.Content().PaddingVertical(10).Table(table =>
                        {
                            // Define 5 columns now to include the "Assigned To" column
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);  // #
                                columns.RelativeColumn(2);   // Asset Name
                                columns.RelativeColumn(2);   // Serial Number
                                columns.RelativeColumn(2);   // Assigned To (New!)
                                columns.ConstantColumn(80);  // Status
                            });

                            // Header Row
                            table.Header(header =>
                            {
                                var headerStyle = TextStyle.Default.SemiBold();

                                header.Cell().Text("#").Style(headerStyle);
                                header.Cell().Text("Asset Name").Style(headerStyle);
                                header.Cell().Text("Serial Number").Style(headerStyle);
                                header.Cell().Text("Assigned To").Style(headerStyle);
                                header.Cell().Text("Status").Style(headerStyle);

                                header.Cell().ColumnSpan(5).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            });

                            // Data Rows
                            foreach (var asset in assets)
                            {
                                table.Cell().Text(asset.AssetId.ToString());
                                table.Cell().Text(asset.AssetName);
                                table.Cell().Text(asset.SerialNumber);

                                // Check if an employee is assigned; if not, show "N/A"
                                string assignedPerson = asset.AssignedEmployee != null
                        ? asset.AssignedEmployee.FullName
                        : "Unassigned";

                                table.Cell().Text(assignedPerson);
                                table.Cell().Text(asset.Status);
                            }
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                    });
                }).GeneratePdf(saveFileDialog.FileName);

                MessageBox.Show("Report with assignment details generated!", "Success");
            }
        }
    }
}