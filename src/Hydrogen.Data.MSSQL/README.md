<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 💾 Hydrogen.Data.MSSQL

**Microsoft SQL Server implementation** for Hydrogen.Data abstraction layer, enabling enterprise-grade database access for large-scale applications with advanced features like linked servers, full-text search, and SQL Agent integration.

Hydrogen.Data.MSSQL brings **enterprise SQL Server capabilities** to the Hydrogen framework while maintaining database-agnostic abstraction. Fully compatible with **Azure SQL Database**, on-premises SQL Server, and SQL Express instances.

## ⚡ 10-Second Example

```csharp
using Hydrogen.Data;

// Connect to SQL Server (local or remote)
var dac = Tools.MSSQL.Open(
    "Server=.;Database=myapp;Integrated Security=true;");

// Create and query table
dac.ExecuteNonQuery(@"CREATE TABLE Users (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100)
)");

// Insert record
dac.Insert("Users", new[] {
    new ColumnValue("Name", "Alice"),
    new ColumnValue("Email", "alice@example.com")
});

// Parameterized query
var users = dac.ExecuteQuery(
    "SELECT * FROM Users WHERE Name LIKE @search",
    new ColumnValue("@search", "%Alice%"));
```

## 🏗️ Core Concepts

**Enterprise Features**: Full support for SQL Server features including stored procedures, functions, triggers, and service broker.

**Connection Pooling**: Advanced connection pool configuration optimized for multi-threaded server applications.

**Transaction Isolation Levels**: Support for READ UNCOMMITTED, READ COMMITTED, REPEATABLE READ, and SERIALIZABLE isolation.

**Bulk Operations**: Efficient bulk insert/update for large datasets using BULK INSERT and table-valued parameters.

**Async Support**: Native async operations leveraging .NET async/await patterns for scalability.

## 🔧 Core Examples

### Connection Strings & Server Access

```csharp
using Hydrogen.Data;

// Local server with integrated security (Windows Authentication)
var localDac = Tools.MSSQL.Open(
    "Server=.;Database=myapp;Integrated Security=true;");

// Named instance
var instanceDac = Tools.MSSQL.Open(
    "Server=.\\SQLEXPRESS;Database=myapp;Integrated Security=true;");

// Remote server with SQL login
var remoteDac = Tools.MSSQL.Open(
    "Server=db.company.com;Database=prod;User Id=sa;Password=P@ssw0rd;");

// Azure SQL Database
var azureDac = Tools.MSSQL.Open(
    "Server=myserver.database.windows.net;Database=mydb;User Id=admin@myserver;Password=P@ssw0rd;");

// Connection pool settings
var pooledDac = Tools.MSSQL.Open(
    "Server=.;Database=myapp;Integrated Security=true;Min Pool Size=10;Max Pool Size=100;");
```

### CRUD Operations with IDENTITY Keys

```csharp
using Hydrogen.Data;

var dac = Tools.MSSQL.Open(
    "Server=.;Database=shopdb;Integrated Security=true;");

// Create table with IDENTITY primary key
dac.ExecuteNonQuery(@"
    CREATE TABLE Products (
        ProductID INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Price DECIMAL(10,2),
        StockQuantity INT,
        CategoryID INT
    )");

// Insert records (IDENTITY automatically assigned)
dac.Insert("Products", new[] {
    new ColumnValue("Name", "Laptop"),
    new ColumnValue("Price", 999.99),
    new ColumnValue("StockQuantity", 50),
    new ColumnValue("CategoryID", 1)
});

dac.Insert("Products", new[] {
    new ColumnValue("Name", "Mouse"),
    new ColumnValue("Price", 29.99),
    new ColumnValue("StockQuantity", 200),
    new ColumnValue("CategoryID", 2)
});

// Query with parameters
var expensiveProducts = dac.ExecuteQuery(
    "SELECT * FROM Products WHERE Price > @minPrice ORDER BY Price DESC",
    new ColumnValue("@minPrice", 100.0));

// Update product
dac.Update("Products",
    new[] {
        new ColumnValue("StockQuantity", 45),
        new ColumnValue("Price", 1099.99)
    },
    "WHERE Name = @name",
    new ColumnValue("@name", "Laptop"));

// Delete low-stock items
dac.ExecuteNonQuery(
    "DELETE FROM Products WHERE StockQuantity < @minStock",
    new ColumnValue("@minStock", 10));
```

### Transactions & ACID Compliance

```csharp
using Hydrogen.Data;

var dac = Tools.MSSQL.Open(
    "Server=.;Database=bankdb;Integrated Security=true;");

// Create account table
dac.ExecuteNonQuery(@"
    CREATE TABLE Accounts (
        AccountID INT PRIMARY KEY IDENTITY(1,1),
        AccountNumber NVARCHAR(20) UNIQUE NOT NULL,
        Balance DECIMAL(18,2) NOT NULL,
        LastModified DATETIME2 DEFAULT GETUTCDATE()
    )");

// Initialize accounts
dac.Insert("Accounts", new[] {
    new ColumnValue("AccountNumber", "ACC001"),
    new ColumnValue("Balance", 10000.00)
});

dac.Insert("Accounts", new[] {
    new ColumnValue("AccountNumber", "ACC002"),
    new ColumnValue("Balance", 5000.00)
});

// Transfer with transaction isolation
using (var scope = dac.BeginTransactionScope()) {
    try {
        // Withdraw from source account
        dac.ExecuteNonQuery(
            "UPDATE Accounts SET Balance = Balance - @amount WHERE AccountID = 1",
            new ColumnValue("@amount", 1000.00));

        // Verify sufficient funds after withdrawal
        var balance = dac.ExecuteScalar<decimal>(
            "SELECT Balance FROM Accounts WHERE AccountID = 1");
        
        if (balance < 0) {
            throw new Exception("Insufficient funds");
        }

        // Deposit to destination account
        dac.ExecuteNonQuery(
            "UPDATE Accounts SET Balance = Balance + @amount WHERE AccountID = 2",
            new ColumnValue("@amount", 1000.00));

        // Both succeed or both rollback
        scope.Commit();
    } catch (Exception ex) {
        Console.WriteLine($"Transaction failed: {ex.Message}");
        // Automatic rollback
    }
}

// Verify final balances
var acc1 = dac.ExecuteScalar<decimal>(
    "SELECT Balance FROM Accounts WHERE AccountID = 1");
var acc2 = dac.ExecuteScalar<decimal>(
    "SELECT Balance FROM Accounts WHERE AccountID = 2");

Console.WriteLine($"Account 1: {acc1}");  // 9000.00
Console.WriteLine($"Account 2: {acc2}");  // 6000.00
```

### Stored Procedure Integration

```csharp
using Hydrogen.Data;

var dac = Tools.MSSQL.Open(
    "Server=.;Database=appdb;Integrated Security=true;");

// Create stored procedure
dac.ExecuteNonQuery(@"
    CREATE PROCEDURE spGetUsersSince
        @createdAfter DATETIME2
    AS
    BEGIN
        SELECT * FROM Users 
        WHERE CreatedDate >= @createdAfter
        ORDER BY CreatedDate DESC
    END");

// Execute stored procedure with parameters
var recentUsers = dac.ExecuteQuery(
    "EXECUTE spGetUsersSince @createdAfter",
    new ColumnValue("@createdAfter", 
        DateTime.UtcNow.AddDays(-30)));

// Stored procedure for complex operations
dac.ExecuteNonQuery(@"
    CREATE PROCEDURE spProcessMonthlyBilling
    AS
    BEGIN
        DECLARE @processDate DATETIME2 = GETUTCDATE()
        
        UPDATE Invoices 
        SET Status = 'BILLED', BilledDate = @processDate
        WHERE Status = 'PENDING' AND DueDate <= @processDate
        
        INSERT INTO AuditLog (Action, Timestamp)
        VALUES ('Monthly billing processed', @processDate)
    END");

dac.ExecuteNonQuery("EXECUTE spProcessMonthlyBilling");
```

## 🏗️ Architecture

**MSSQLDatabaseManager**: Core implementation of IDataAccessContext for SQL Server operations.

**Connection Pool Management**: Leverages SQL Server's built-in connection pooling with configurable pool sizes.

**SQL Server-Specific Features**: Support for hierarchical queries, full-text search, JSON operations, and temporal tables.

**Transaction Isolation**: Configurable transaction isolation levels for different consistency requirements.

## 📋 Best Practices

- Use **integrated security** (Windows Authentication) for domain-joined applications
- Configure **connection pool size** based on expected concurrent connections
- Leverage **stored procedures** for complex business logic and performance optimization
- Use **parameterized queries** consistently to prevent SQL injection
- Implement **transaction scopes** for multi-step operations requiring atomicity
- Monitor **connection pool** usage to identify bottlenecks
- Use **IDENTITY(1,1)** for auto-incrementing primary keys instead of manual ID management
- Consider **read replicas** or **read-only secondaries** for reporting workloads

## 📊 Status & Compatibility

- **Version**: 2.0+
- **Framework**: .NET 5.0+, .NET Framework 4.7+
- **SQL Server Versions**: 2016+, Azure SQL Database, SQL Express
- **Performance**: Enterprise-grade, supports thousands of concurrent connections
- **Scalability**: Horizontal scaling through multiple connection pools and read replicas

## 📦 Dependencies

- **Hydrogen.Data**: Data abstraction layer
- **Microsoft.Data.SqlClient**: Modern SQL Server provider (recommended)
- **System.Data.SqlClient**: Legacy provider (.NET Framework)
- **.NET Standard 2.1+**: Cross-platform compatibility

## 📚 Related Projects

- [Hydrogen.Data](../Hydrogen.Data) - Core data abstraction layer
- [Hydrogen.Data.Sqlite](../Hydrogen.Data.Sqlite) - SQLite embedded implementation
- [Hydrogen.Data.Firebird](../Hydrogen.Data.Firebird) - Firebird implementation
- [Hydrogen.Data.NHibernate](../Hydrogen.Data.NHibernate) - NHibernate ORM integration
- [Hydrogen.Windows.Forms.MSSQL](../Hydrogen.Windows.Forms.MSSQL) - WinForms data binding for SQL Server
- [Hydrogen.Tests](../../tests/Hydrogen.Tests) - Test patterns and examples

## 📄 License & Author

**License**: [Refer to repository LICENSE](../../LICENSE)  
**Author**: Herman Schoenfeld, Sphere 10 Software (sphere10.com)  
**Copyright**: © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.
