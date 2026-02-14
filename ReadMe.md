🚀 AssetHub - IT Asset Management System

AssetHub is a modern WPF-based desktop application designed for IT departments to track hardware inventory, manage employee assignments, and maintain a permanent audit log of all equipment movement.



✨ Features

Inventory Management: Full CRUD operations for hardware with manual Serial Number entry and duplicate checks.



Dynamic Dashboard: Real-time stats on asset distribution and total inventory value.



Activity Logs: A permanent history of every assignment and return, ensuring you never lose track of asset movement.



Professional Notifications: Sleek, color-coded "Toast" notifications for a responsive user experience.



PDF Reporting: Generate professional inventory reports using QuestPDF.



🛠 Tech Stack

Language: C# / .NET 8.0+



Framework: Windows Presentation Foundation (WPF)



Database: SQL Server



ORM: Entity Framework Core (Database First)



UI Library: Material Design in XAML



🚀 Getting Started

1\. Database Setup

To run this project, you need to have SQL Server installed. Run the following scripts to prepare your database:



SQL



-- Create the Assets Table

CREATE TABLE Assets (

&nbsp;   AssetId INT PRIMARY KEY IDENTITY(1,1),

&nbsp;   AssetName NVARCHAR(200),

&nbsp;   AssetType NVARCHAR(100),

&nbsp;   SerialNumber NVARCHAR(100) UNIQUE,

&nbsp;   Status NVARCHAR(50),

&nbsp;   Price DECIMAL(18,2),

&nbsp;   CreatedAt DATETIME DEFAULT GETDATE(),

&nbsp;   UpdatedAt DATETIME DEFAULT GETDATE(),

&nbsp;   AssignedEmployeeId INT

);



-- Create the Activity Logs Table

CREATE TABLE ActivityLogs (

&nbsp;   LogId INT PRIMARY KEY IDENTITY(1,1),

&nbsp;   Details NVARCHAR(MAX),

&nbsp;   ActionDate DATETIME DEFAULT GETDATE(),

&nbsp;   SerialNumber NVARCHAR(100)

);

2\. Configuration

Update your AssetHubDbContext.cs connection string to point to your local SQL Server instance:



C#



optionsBuilder.UseSqlServer("Server=YOUR\_SERVER\_NAME;Database=AssetHubDb;Trusted\_Connection=True;");

📸 Preview

Dashboard: View your IT environment at a glance.



Inventory: Manage and filter your hardware assets.



Audit Log: Track who had what and when.

