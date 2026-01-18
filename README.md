# PharmacyApp

PharmacyApp is a **desktop pharmacy management application** developed using **C# and WPF**, following the **MVVM (Model–View–ViewModel)** architectural pattern.
This repository contains the **full source code** and an accompanying **SQL script** for database initialization.

The purpose of this README is to provide a **complete, step‑by‑step tutorial** so that the application can be built and run successfully on a new machine.

---

## Table of Contents

1. Prerequisites
2. Development Environment
3. Clone the Repository
4. Database Setup
5. Open the Project in Visual Studio
6. Restore NuGet Packages
7. Configure the Application
8. Build and Run
9. Common Issues

---

## 1. Prerequisites

Ensure the following software is installed:

* **Windows OS** (required for WPF)
* **Visual Studio 2019 / 2022 or later**

  * Workload: **.NET desktop development**
  * Include support for **WPF**
* **.NET Framework** matching the project target version
* **SQL Server** (Express or higher)
* **SQL Server Management Studio (SSMS)**

---

## 2. Development Environment

Recommended environment:

* IDE: Visual Studio 2022
* Language: C#
* UI Framework: WPF
* Architecture: MVVM
* Database: SQL Server

---

## 3. Clone the Repository

Clone the project using Git:

```bash
git clone https://github.com/khoamai291299/PharmacyApp.git
```

Or download the ZIP file from GitHub and extract it locally.

---

## 4. Database Setup

The repository includes an **SQL script** for initializing the database.

### Steps

1. Open **SQL Server Management Studio (SSMS)**.
2. Connect to your local SQL Server instance.
3. Create a new query.
4. Open the provided SQL file in the repository (for example: `Database.sql`).
5. Execute the script to:

   * Create the database
   * Create required tables
   * Initialize relationships and constraints

Ensure the database is created successfully before running the application.

---

## 5. Open the Project in Visual Studio

1. Launch **Visual Studio**.
2. Select **File → Open → Project/Solution**.
3. Navigate to the cloned project folder.
4. Open the solution file:

```text
PharmacyApp.slnx
```

---

## 6. Restore NuGet Packages

NuGet packages are restored automatically when the solution is opened.

If not, restore manually:

### Using Visual Studio UI

* Right‑click the solution → **Restore NuGet Packages**

### Using Package Manager Console

```powershell
Update-Package -reinstall
```

---

## 7. Configure the Application

Update the **database connection string** if required.

1. Open the configuration file used by the project (e.g. `App.config`).
2. Modify the connection string to match your SQL Server instance:

```xml
Data Source=.;Initial Catalog=PharmacyDB;Integrated Security=True
```

Ensure the database name matches the one created by the SQL script.

---

## 8. Build and Run

1. Select **Build → Build Solution** (`Ctrl + Shift + B`).
2. Confirm there are no build errors.
3. Press **F5** or select **Start Debugging**.
4. The **PharmacyApp main window** will be displayed.

---

## 9. Common Issues

### Missing .NET Framework

Install the required **.NET Framework Developer Pack** via Visual Studio Installer.

### Database Connection Errors

* Verify SQL Server is running
* Check database name and connection string
* Ensure correct authentication mode

### NuGet Restore Failure

* Check internet connection
* Clear NuGet cache if necessary

---

## Notes

* This project follows MVVM strictly: Views contain no business logic.
* Database schema changes should be managed through SQL scripts.
* The project is suitable for academic projects, learning, and demonstration purposes.

---

## Author

Developed as a WPF MVVM desktop application for learning and academic use.
