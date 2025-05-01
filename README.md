# Inventory Management System (IMS)

## Project Scope

The **Inventory Management System (IMS)** is a robust web-based application tailored for businesses particularly in the **automotive repair industry** to manage their inventory operations across multiple branches. Its core functionality includes monitoring stock levels, managing product data, recording sales, and generating reports.

The IMS addresses the common challenges faced by inventory managers and administrators by leveraging a relational database and an intuitive web interface. It streamlines inventory-related workflows, reduces manual effort, and enhances operational efficiency.

Key benefits include:
- Improved accuracy and reliability of inventory records
- Real-time stock tracking across multiple locations
- Automated alerts to prevent overstocking and understocking
- Enhanced customer service through optimized stock levels

With flexible configuration options, the IMS supports customized inventory policies and reporting needs, enabling **data-driven decision-making** and **supply chain optimization**.


## Planned Tools

- **Programming Language**: C# within the .NET ecosystem, providing a robust and type-safe environment for building scalable and maintainable application logic.
- **Framework**: .NET enabling high performance, cross-platform compatibility, and support for modern web applications.
- **Database**: SQL Server, used for efficient data storage, retrieval, and management, ensuring reliable and secure handling of inventory data with support for complex queries and transactions.
- **ORM (Optional)**: Entity Framework Core, integrated with .NET C# to simplify database interactions, improve development speed, and ensure type-safe data operations.
- **Frontend Web Framework**: ASP.NET Core MVC, used for the frontend to implement a Model-View-Controller architecture, enabling the development of dynamic, scalable, and maintainable web interfaces with clear separation of concerns.
- **Frontend Markup and Styling**:
  - **HTML**: Utilized as the standard markup language for structuring the frontend web interfaces, ensuring semantic and accessible content delivery within ASP.NET Core MVC views.
  - **CSS**: Employed for custom styling and layout adjustments, complementing Bootstrap to fine-tune the visual design and ensure a cohesive user experience.
  - **Bootstrap**: Used as the primary frontend CSS framework to provide responsive, mobile-first styling, ensuring user-friendly, visually consistent, and cross-device-compatible web interfaces with minimal custom CSS.
- **Web Server**: Microsoft Internet Information Services (IIS), selected to host the ASP.NET Core MVC application, offering robust performance, security features, and seamless integration with the .NET ecosystem for reliable deployment.
- **Authentication/Authorization**: ASP.NET Core Identity, paired with SQL Server, to implement secure user management and role-based access control.

## Database Design

**Decision**: Use SQL Server with a normalized schema comprising the following tables:

### Products Table
Stores product details:
- `ProductID` [PK]
- `Name`
- `Type`
- `Color`

### Inventory Table
Tracks stock levels:
- `InventoryID` [PK]
- `ProductID` [FK]
- `LocationID` [FK]
- `Quantity`

### Locations Table
Manages warehouse/store locations:
- `LocationID` [PK]
- `LocationName`
- `Address`

### Orders Table
Records inter-location transfers:
- `OrderID` [PK]
- `SourceLocationID` [FK]
- `DestinationLocationID` [FK]
- `OrderDate`
- `Status`

### OrderDetails Table
Details products in orders:
- `OrderDetailID` [PK]
- `OrderID` [FK]
- `ProductID` [FK]
- `Quantity`

### Sales Table
Logs sales transactions:
- `SaleID` [PK]
- `EmployeeID` [FK]
- `ClientID` [FK]
- `SaleDate`
- `WindshieldCode` [nullable]
- `AdhesiveAmount` [nullable]

### SaleDetails Table
Specifies products sold:
- `SaleDetailID` [PK]
- `SaleID` [FK]
- `ProductID` [FK]
- `Quantity`
- `Price`

### Clients Table
Stores client information:
- `ClientID` [PK]
- `FullName`
- `ContactNumber`
- `Email`
- `Address`

### Employees Table
Manages employee data:
- `EmployeeID` [PK]
- `FullName`
- `Username`
- `PasswordHash`
- `Role`
- `AssignedLocationID` [FK]

## Table Structure

- **Products**: Includes `Type` and `Color` to support diverse inventory categorization, enabling flexible filtering and reporting.
- **Inventory**: Links `ProductID` and `LocationID` to track stock per product and location, supporting multi-warehouse management.
- **Locations**: Stores `LocationName` and `Address` for clear identification of stock locations, critical for order fulfillment.
- **Orders and OrderDetails**: Separates order metadata (Orders) from product specifics (OrderDetails) to handle multiple products per order efficiently, supporting stock transfers between locations.
- **Sales and SaleDetails**: Similar to Orders, separates sale metadata (including niche fields like `WindshieldCode` and `AdhesiveAmount` for specific industries) from product details, enabling detailed sales tracking and reporting.
- **Clients**: Centralizes client data for sales and customer relationship management, with fields like `Email` and `ContactNumber` for communication.
- **Employees**: Includes `Role` and `AssignedLocationID` to support role-based access control and location-specific responsibilities, with `PasswordHash` for secure authentication.

- **Foreign Keys**: Enforce relationships (e.g., `ProductID` in `OrderDetails` and `SaleDetails`) to maintain consistency and support cascading updates/deletes where applicable.
- **Nullable Fields**: Fields like `WindshieldCode` and `AdhesiveAmount` in `Sales` are nullable to accommodate industry-specific data without enforcing unnecessary constraints.
