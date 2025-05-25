# Inventory Management System (IMS)

## Overview

The **Inventory Management System (IMS)** is a web-based application built with ASP.NET Core MVC and Entity Framework, designed to facilitate efficient inventory tracking, sales recording, and stock movement across multiple business locations—particularly in the **automotive repair industry**.

IMS ensures streamlined inventory operations with features such as:

* Real-time inventory tracking across branches
* Role-based access control
* Sales logging and inter-location order management
* Client and employee data management
* Identity integration via ASP.NET Core Identity

---

## Architecture

### Application Layers

The solution follows a **3-tier architecture**:

1. **Presentation Layer (Frontend)**

   * ASP.NET Core MVC (Views and Controllers)
   * Bootstrap + HTML/CSS for responsive UI

2. **Business Logic Layer**

   * C# service classes and controller logic
   * Enforces domain rules (e.g., inventory checks)

3. **Data Access Layer**

   * Entity Framework Core ORM
   * DBMS relational database

### Core Technologies

| Component      | Technology            |
| -------------- | --------------------- |
| Language       | C#                    |
| Framework      | ASP.NET Core MVC      |
| ORM            | Entity Framework Core |
| Database       | DBMS            |
| Authentication | ASP.NET Core Identity |
| UI Framework   | Bootstrap + HTML/CSS  |
| Web Server     | IIS                   |

---

## Setup Instructions

Follow these steps to configure and run the IMS locally:

### 1. Clone the Repository

```bash
git clone https://github.com/Abdelrahman2610/inventory-management-system.git
cd inventory-management-system
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


