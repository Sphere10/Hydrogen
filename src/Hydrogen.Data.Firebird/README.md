<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 💾 Hydrogen.Data.Firebird

**Firebird database implementation** for Hydrogen.Data abstraction layer, providing open-source relational database access with ACID compliance and advanced SQL features.

Hydrogen.Data.Firebird brings **powerful open-source database capabilities** to the Hydrogen framework while maintaining database-agnostic abstraction. Supports both **embedded (single-file) and server architectures**, making it ideal for desktop applications, small-to-medium enterprises, and resource-constrained environments.

## ⚡ 10-Second Example

```csharp
using Hydrogen.Data;

// Connect to Firebird database
var dac = Tools.Firebird.Open(
    "ServerType=1;DataSource=localhost;Database=myapp.fdb;User=sysdba;Password=masterkey");

// Create table with Firebird dialect
dac.ExecuteNonQuery(@"CREATE TABLE Users (
    ID INTEGER PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(100)
)");

// Insert record
dac.Insert("Users", new[] {
    new ColumnValue("ID", 1),
    new ColumnValue("Name", "Alice"),
    new ColumnValue("Email", "alice@example.com")
});

// Query with parameters
var users = dac.ExecuteQuery(
    "SELECT * FROM Users WHERE Name LIKE @search",
    new ColumnValue("@search", "%Alice%"));
```

## 🏗️ Core Concepts

**Embedded & Server Modes**: Single-file database for embedded scenarios or server deployment for multi-user access.

**ACID Compliance**: Full transaction support with isolation levels for data consistency and reliability.

**Stored Procedures & Triggers**: PL/SQL stored procedures and trigger support for complex business logic.

**Multi-Generational Architecture**: Advanced transaction architecture enabling long-running queries without blocking writers.

**SQL Dialect**: Firebird-specific SQL syntax with advanced features like EXECUTE BLOCK and stored procedure definitions.

## 🔧 Core Examples

### Connection Strings & Database Setup

```csharp
using Hydrogen.Data;

// Local server connection (server-based Firebird)
var serverDac = Tools.Firebird.Open(
    "ServerType=1;DataSource=localhost;Database=myapp.fdb;User=sysdba;Password=masterkey");

// Embedded database (single-file, no server required)
var embeddedDac = Tools.Firebird.Open(
    "DataSource=C:\\data\\myapp.fdb;User=sysdba;Password=masterkey;ServerType=0");

// Remote server
var remoteDac = Tools.Firebird.Open(
    "ServerType=1;DataSource=db.company.com;Database=/opt/firebird/myapp.fdb;User=admin;Password=P@ssw0rd");

// Connection pooling
var pooledDac = Tools.Firebird.Open(
    "ServerType=1;DataSource=localhost;Database=myapp.fdb;User=sysdba;Password=masterkey;Pooling=true;MinPoolSize=5;MaxPoolSize=20");
```

### CRUD Operations with Generator Keys

```csharp
using Hydrogen.Data;

var dac = Tools.Firebird.Open(
    "DataSource=shop.fdb;User=sysdba;Password=masterkey");

// Create table with generator (sequence)
dac.ExecuteNonQuery(@"
    CREATE TABLE Products (
        ID INTEGER PRIMARY KEY,
        Name VARCHAR(100) NOT NULL,
        Price DECIMAL(10,2),
        StockQuantity INTEGER
    )");

// Create generator (sequence) for auto-incrementing IDs
dac.ExecuteNonQuery("CREATE SEQUENCE GEN_PRODUCTS_ID");

// Insert records with generator
dac.ExecuteNonQuery(@"
    INSERT INTO Products (ID, Name, Price, StockQuantity)
    VALUES (NEXT VALUE FOR GEN_PRODUCTS_ID, 'Laptop', 999.99, 50)");

dac.ExecuteNonQuery(@"
    INSERT INTO Products (ID, Name, Price, StockQuantity)
    VALUES (NEXT VALUE FOR GEN_PRODUCTS_ID, 'Mouse', 29.99, 200)");

// Query all records
var products = dac.ExecuteQuery("SELECT * FROM Products");

// Parameterized query
var expensiveProducts = dac.ExecuteQuery(
    "SELECT * FROM Products WHERE Price > ? ORDER BY Price DESC",
    new ColumnValue(null, 100.0));

// Update product
dac.Update("Products",
    new[] {
        new ColumnValue("StockQuantity", 45),
        new ColumnValue("Price", 1099.99)
    },
    "WHERE Name = ?",
    new ColumnValue(null, "Laptop"));

// Delete low-stock items
dac.ExecuteNonQuery(
    "DELETE FROM Products WHERE StockQuantity < ?",
    new ColumnValue(null, 10));
```

### Transactions & ACID Properties

```csharp
using Hydrogen.Data;

var dac = Tools.Firebird.Open(
    "DataSource=bank.fdb;User=sysdba;Password=masterkey");

// Create account table
dac.ExecuteNonQuery(@"
    CREATE TABLE Accounts (
        ID INTEGER PRIMARY KEY,
        AccountNumber VARCHAR(20) UNIQUE NOT NULL,
        Balance DECIMAL(18,2) NOT NULL,
        LastModified TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    )");

// Initialize accounts
dac.ExecuteNonQuery(@"
    CREATE SEQUENCE GEN_ACCOUNTS_ID");

dac.Insert("Accounts", new[] {
    new ColumnValue("ID", 1),
    new ColumnValue("AccountNumber", "ACC001"),
    new ColumnValue("Balance", 10000.00)
});

dac.Insert("Accounts", new[] {
    new ColumnValue("ID", 2),
    new ColumnValue("AccountNumber", "ACC002"),
    new ColumnValue("Balance", 5000.00)
});

// Transactional money transfer with Firebird's multi-generational architecture
using (var scope = dac.BeginTransactionScope()) {
    try {
        // Withdraw from source account
        dac.ExecuteNonQuery(
            "UPDATE Accounts SET Balance = Balance - ? WHERE ID = 1",
            new ColumnValue(null, 1000.00));

        // Verify sufficient funds
        var balance = dac.ExecuteScalar<decimal>(
            "SELECT Balance FROM Accounts WHERE ID = 1");
        
        if (balance < 0) {
            throw new Exception("Insufficient funds");
        }

        // Deposit to destination account
        dac.ExecuteNonQuery(
            "UPDATE Accounts SET Balance = Balance + ? WHERE ID = 2",
            new ColumnValue(null, 1000.00));

        scope.Commit();
    } catch (Exception ex) {
        Console.WriteLine($"Transfer failed: {ex.Message}");
        // Auto-rollback
    }
}

// Verify final balances
var acc1Balance = dac.ExecuteScalar<decimal>(
    "SELECT Balance FROM Accounts WHERE ID = 1");
var acc2Balance = dac.ExecuteScalar<decimal>(
    "SELECT Balance FROM Accounts WHERE ID = 2");

Console.WriteLine($"Account 1: {acc1Balance}");  // 9000.00
Console.WriteLine($"Account 2: {acc2Balance}");  // 6000.00
```

### Stored Procedures & Triggers

```csharp
using Hydrogen.Data;

var dac = Tools.Firebird.Open(
    "DataSource=app.fdb;User=sysdba;Password=masterkey");

// Create stored procedure using EXECUTE BLOCK
dac.ExecuteNonQuery(@"
    CREATE PROCEDURE GetProductsByPrice(MaxPrice DECIMAL(10,2))
    RETURNS (
        ProductID INTEGER,
        ProductName VARCHAR(100),
        ProductPrice DECIMAL(10,2)
    )
    AS
    BEGIN
        FOR SELECT ID, Name, Price FROM Products WHERE Price <= :MaxPrice
        INTO :ProductID, :ProductName, :ProductPrice
        DO SUSPEND;
    END");

// Execute stored procedure
var cheapProducts = dac.ExecuteQuery(
    "EXECUTE PROCEDURE GetProductsByPrice ?",
    new ColumnValue(null, 50.00));

// Create audit trigger
dac.ExecuteNonQuery(@"
    CREATE TABLE AuditLog (
        AuditID INTEGER PRIMARY KEY,
        TableName VARCHAR(50),
        Action VARCHAR(10),
        AuditTime TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    )");

dac.ExecuteNonQuery(@"
    CREATE TRIGGER PRODUCTS_AUDIT AFTER UPDATE ON Products
    AS
    BEGIN
        INSERT INTO AuditLog (TableName, Action)
        VALUES ('Products', 'UPDATE');
    END");

// Update triggers audit log automatically
dac.Update("Products",
    new[] { new ColumnValue("Price", 99.99) },
    "WHERE ID = 1");
```

## 🏗️ Architecture

**FirebirdDatabaseManager**: Core implementation of IDataAccessContext for Firebird operations.

**Embedded vs Server**: Supports both embedded single-file databases and server-based deployments.

**Generator Support**: Firebird generators (sequences) for auto-incrementing values using `NEXT VALUE FOR`.

**PL/SQL Integration**: Full stored procedure and trigger support using Firebird PL/SQL dialect.

**Multi-Generational Transactions**: Concurrent readers and writers with minimal blocking.

## 📋 Best Practices

- Use **embedded mode** for desktop applications with single concurrent user
- Deploy **server mode** for multi-user environments requiring higher throughput
- Create **generators (sequences)** for auto-incrementing primary keys
- Leverage **stored procedures** for complex business logic
- Use **transaction isolation** levels appropriate for your consistency requirements
- Monitor **database statistics** and maintain indexes for performance
- Use **TIMESTAMP DEFAULT CURRENT_TIMESTAMP** for audit trails
- Enable **archiving** for large transaction logs

## 📊 Status & Compatibility

- **Version**: 2.0+
- **Framework**: .NET 5.0+, .NET Framework 4.7+
- **Firebird Versions**: 2.5+, 3.0+, 4.0+
- **Performance**: Good for small-to-medium databases, embedded and server deployments
- **Concurrency**: Multi-generational architecture supports concurrent readers and writers

## 📦 Dependencies

- **Hydrogen.Data**: Data abstraction layer
- **FirebirdSql.Data.FirebirdClient**: Firebird .NET provider
- **.NET Standard 2.1+**: Cross-platform compatibility

## 📚 Related Projects

- [Hydrogen.Data](../Hydrogen.Data) - Core data abstraction layer
- [Hydrogen.Data.Sqlite](../Hydrogen.Data.Sqlite) - SQLite embedded implementation
- [Hydrogen.Data.MSSQL](../Hydrogen.Data.MSSQL) - SQL Server implementation
- [Hydrogen.Data.NHibernate](../Hydrogen.Data.NHibernate) - NHibernate ORM integration
- [Hydrogen.Windows.Forms.Firebird](../Hydrogen.Windows.Forms.Firebird) - WinForms data binding for Firebird
- [Hydrogen.Tests](../../tests/Hydrogen.Tests) - Test patterns and examples

## 📄 License & Author

**License**: [Refer to repository LICENSE](../../LICENSE)  
**Author**: Herman Schoenfeld, Sphere 10 Software (sphere10.com)  
**Copyright**: © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.
