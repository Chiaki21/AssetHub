# AssetHub 🚀

AssetHub is a professional-grade Windows desktop application for managing IT inventory and employee asset assignments. Built with **C#**, **WPF**, and **.NET 8**, it utilizes **Docker** for database containerization to ensure a seamless "zero-configuration" setup.

## 🌟 Key Features
* **Inventory Management:** Track hardware, serial numbers, and purchase dates.
* **Employee Tracking:** Manage staff members and see their assigned equipment at a glance.
* **Real-time Notifications:** Modern Material Design "Snackbars" for instant user feedback.
* **PDF Reporting:** Generate professional inventory reports using QuestPDF.
* **Auto-Healing Database:** Uses Entity Framework Core Migrations to automatically build the database schema on first launch.

---

## 🛠 Tech Stack
* **Frontend:** WPF with Material Design In XAML
* **Backend:** .NET 8 (C#)
* **Database:** Microsoft SQL Server (running in **Docker**)
* **ORM:** Entity Framework Core (Code-First)
* **Containerization:** Docker & Docker Compose

---

## 🚀 Quick Start (For Non-Technical Users)

I've designed AssetHub to be as easy to start as possible. No SQL Server installation is required on your machine.

### Prerequisites
1.  **Docker Desktop:** [Download and install here](https://www.docker.com/products/docker-desktop/). (Ensure it is running before launching the app).
2.  **.NET 8 Runtime:** Usually included with Windows, but can be [found here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).




## Setup Instructions (No Database Errors)

1. Install the .NET SDK (8.0 or higher) and Docker Desktop.
2. Clone this repository.
3. Open a terminal in the project directory.
4. Run the setup script:
   
   ```powershell
   ./SETUP.ps1
   ```
   
   This will:
   - Start the SQL Server Docker container
   - Drop any existing AssetHubDB database
   - Apply the latest EF Core migrations

5. Run the application:
   
   ```powershell
   dotnet run --project AssetHub.csproj
   ```

**Note:** If you change the database schema, always create a new migration and commit it.

ill take you to dashboard, or you can use the test credentials Email: testuser@assethub.com Password: testuser@123

> **Note:** On the first run, the app may take a few seconds to start while Docker pulls the SQL Server image and builds the database tables.

---

## 👨‍💻 Developer Setup
If you want to contribute or modify the code:
1. Clone the repo: `git clone https://github.com/Chiaki21/AssetHub`
2. Open `AssetHub.sln` in **Visual Studio 2022**.
3. Ensure Docker Desktop is running.
4. Press **F5** to build and run.

---

## 📸 Screenshots
<img width="888" height="667" alt="image" src="https://github.com/user-attachments/assets/670949f0-13fc-4c63-9a09-a9920a33cc0f" />
<img width="1576" height="781" alt="image" src="https://github.com/user-attachments/assets/3e0d30d6-db35-462b-ac8c-28935aab86dd" />
<img width="1583" height="780" alt="image" src="https://github.com/user-attachments/assets/639f3cba-6316-4252-b05f-335307feb84f" />
<img width="1572" height="768" alt="image" src="https://github.com/user-attachments/assets/058d6d5e-a9e8-4694-b50f-38d88c177fdc" />

---

## 📄 License
This project is for portfolio and educational purposes.
