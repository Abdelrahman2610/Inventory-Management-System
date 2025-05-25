# Inventory Management System (IMS)

## Project Scope

The **Inventory Management System (IMS)** is a robust web-based application tailored for businesses, particularly in the **automotive repair industry**, to manage their inventory operations across multiple branches. Its core functionality includes monitoring stock levels, managing product data, recording sales, and generating reports.

The IMS addresses the common challenges faced by inventory managers and administrators by leveraging a relational database and an intuitive web interface. It streamlines inventory-related workflows, reduces manual effort, and enhances operational efficiency.

With flexible configuration options, the IMS supports customized inventory policies and reporting needs, enabling **data-driven decision-making** and **supply chain optimization**.

IMS ensures streamlined inventory operations with features such as:

* Real-time inventory tracking across multiple branches for accurate and reliable stock records
* Role-based access control to ensure secure and appropriate system access
* Sales logging and inter-location order management to streamline transactions and stock movement
* Client and employee data management for centralized and organized information
* Identity integration via ASP.NET Core Identity for secure authentication and authorization
* Real-time stock visibility across all locations to enhance decision-making
* Automated alerts to prevent overstocking and understocking issues
* Enhanced customer service through optimized stock levels and faster response times

---

## Architecture

### Application Layers – 3-Tier Architecture

1. **Presentation Layer (Frontend)**  
   * Built with **ASP.NET Core MVC** (Views and Controllers).  
   * Uses **HTML** for structured content, **CSS** for styling, and **Bootstrap** for responsive, mobile-first design.  
   * Implements a clear separation of concerns following the MVC pattern for a maintainable UI.

2. **Business Logic Layer**  
   * Developed using **C#** within the .NET ecosystem.  
   * Handles all core logic, such as inventory checks, sales operations, and role-based access control.  
   * Ensures scalability and clean application behavior through service classes and controller logic.

3. **Data Access Layer**  
   * Utilizes **Entity Framework Core** as the ORM.  
   * Connects to a **SQL Server** database for efficient, type-safe data operations.  
   * Supports complex transactions and queries for reliable inventory and user data management.

---

## 🔧 Core Technologies

| Component         | Technology / Description                                                                 |
| ----------------- | ----------------------------------------------------------------------------------------- |
| **Language**       | C# – Type-safe and robust, ideal for scalable application logic                          |
| **Framework**      | .NET / ASP.NET Core MVC – High-performance, cross-platform web framework                 |
| **ORM**            | Entity Framework Core – Simplifies database interaction and supports LINQ queries        |
| **Database**       | SQL Server – Secure and efficient relational database system                             |
| **Authentication** | ASP.NET Core Identity – Role-based access control and user authentication                |
| **UI Framework**   | Bootstrap + HTML/CSS – Responsive design and consistent, modern UI                       |
| **Web Server**     | IIS (Internet Information Services) – Reliable hosting for .NET Core applications        |

---

## Setup Instructions

Follow these steps to configure and run the IMS locally:

### 1. Clone the Repository

```bash
git clone https://github.com/Abdelrahman2610/Inventory-Management-System.git
cd Inventory-Management-System
```

### 2. Configure the Database

Update `appsettings.json` with your SQL Server connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=IMS_DB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### 3. Apply Migrations and Seed Data

Use the CLI to apply migrations and seed initial data:

```bash
dotnet ef database update
```

(Optional) You can include a data seeding class in `Startup.cs` or `Program.cs`.

### 4. Run the Application

```bash
dotnet run
```

Visit `https://localhost:5001` in your browser to access the application.

---

## Database Design

### Core Business Tables

#### Products

Stores product details.

| Column    | Type   | Description               |
| --------- | ------ | ------------------------- |
| ProductID | PK     | Unique product identifier |
| Name      | string | Product name              |
| Type      | string | Product type              |
| Color     | string | Product color             |

#### Inventory

Tracks product stock per location.

| Column      | Type | Description                |
| ----------- | ---- | -------------------------- |
| InventoryID | PK   | Unique inventory record ID |
| ProductID   | FK   | Links to `Products`        |
| LocationID  | FK   | Links to `Locations`       |
| Quantity    | int  | Current stock level        |

#### Locations

Manages warehouse or branch data.

| Column       | Type   | Description          |
| ------------ | ------ | -------------------- |
| LocationID   | PK     | Unique location ID   |
| LocationName | string | Name of the location |
| Address      | string | Physical address     |

#### Orders

Logs inter-location transfers.

| Column                | Type   | Description                    |
| --------------------- | ------ | ------------------------------ |
| OrderID               | PK     | Unique order ID                |
| SourceLocationID      | FK     | Sending location               |
| DestinationLocationID | FK     | Receiving location             |
| OrderDate             | date   | Date of transfer initiation    |
| Status                | string | Transfer status (Pending/Done) |

#### OrderDetails

Line items for each order.

| Column        | Type | Description      |
| ------------- | ---- | ---------------- |
| OrderDetailID | PK   | Unique ID        |
| OrderID       | FK   | Parent order     |
| ProductID     | FK   | Product included |
| Quantity      | int  | Number of units  |

#### Sales

Logs customer sales transactions.

| Column         | Type    | Description                    |
| -------------- | ------- | ------------------------------ |
| SaleID         | PK      | Unique sale ID                 |
| EmployeeID     | FK      | Associated employee            |
| ClientID       | FK      | Associated client              |
| SaleDate       | date    | Date of sale                   |
| WindshieldCode | string  | Optional, specific to industry |
| AdhesiveAmount | decimal | Optional, specific measurement |

#### SaleDetails

Individual products in a sale.

| Column       | Type    | Description         |
| ------------ | ------- | ------------------- |
| SaleDetailID | PK      | Unique ID           |
| SaleID       | FK      | Linked sale         |
| ProductID    | FK      | Product sold        |
| Quantity     | int     | Quantity sold       |
| Price        | decimal | Sale price per unit |

#### Clients

Stores client data.

| Column        | Type   | Description      |
| ------------- | ------ | ---------------- |
| ClientID      | PK     | Unique client ID |
| FullName      | string | Client name      |
| ContactNumber | string | Phone number     |
| Email         | string | Email address    |
| Address       | string | Mailing address  |

#### Employees

Staff details and access role.

| Column             | Type   | Description       |
| ------------------ | ------ | ----------------- |
| EmployeeID         | PK     | Unique ID         |
| FullName           | string | Employee name     |
| Username           | string | Login name        |
| PasswordHash       | string | Hashed password   |
| Role               | string | System role       |
| AssignedLocationID | FK     | Associated branch |

---

### ASP.NET Identity Tables

#### AspNetUsers

Manages authentication and user profile data.

| Column                 | Description             |
| ---------------------- | ----------------------- |
| Id (PK)                | Unique user ID          |
| UserName               | User's login name       |
| Email                  | User's email address    |
| PasswordHash           | Hashed user password    |
| EmailConfirmed         | Email confirmation flag |
| LockoutEnabled         | Enables account lockout |
| AccessFailedCount      | Failed login attempts   |
| PhoneNumber, 2FA, etc. | Security-related fields |

#### AspNetRoles

Defines available roles (Admin, Manager, etc.).

| Column         | Description                |
| -------------- | -------------------------- |
| Id (PK)        | Unique role ID             |
| Name           | Role name                  |
| NormalizedName | Searchable version of name |

#### AspNetUserRoles

Links users to roles.

| Column | Description       |
| ------ | ----------------- |
| UserId | FK to AspNetUsers |
| RoleId | FK to AspNetRoles |

#### AspNetUserClaims

Stores user-specific claims.

| Column     | Description       |
| ---------- | ----------------- |
| Id (PK)    | Claim record ID   |
| UserId     | FK to AspNetUsers |
| ClaimType  | Type of the claim |
| ClaimValue | Value assigned    |

#### AspNetRoleClaims

Claims assigned to a role instead of individual users.

| Column     | Description       |
| ---------- | ----------------- |
| Id (PK)    | Claim record ID   |
| RoleId     | FK to AspNetRoles |
| ClaimType  | Type of the claim |
| ClaimValue | Value assigned    |

#### AspNetUserLogins

Supports third-party login providers.

| Column             | Description                  |
| ------------------ | ---------------------------- |
| LoginProvider (PK) | Provider name (e.g., Google) |
| ProviderKey (PK)   | Unique key from the provider |
| UserId             | FK to AspNetUsers            |

#### AspNetUserTokens

Used for storing tokens (e.g., reset password, 2FA).

| Column        | Description         |
| ------------- | ------------------- |
| UserId        | FK to AspNetUsers   |
| LoginProvider | Token provider name |
| Name          | Token name          |
| Value         | Token value         |


