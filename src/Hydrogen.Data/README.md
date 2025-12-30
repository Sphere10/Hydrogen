# 💾 Hydrogen.Data

<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

**Universal data access abstraction layer** providing ADO.NET enhancements, schema management, transaction scopes, and database-agnostic persistence.

Hydrogen.Data enables **vendor-independent database access** through a unified API while supporting **SQLite, SQL Server, Firebird, and NHibernate**. Core abstractions handle connection pooling, transactions, type mapping, and bulk operations.

## ⚡ 10-Second Example

```csharp
using Hydrogen.Data;

// SQLite in-memory database
var dac = Tools.Sqlite.Open(":memory:");

// Insert records
dac.Insert("Users", new[] {
    new ColumnValue("ID", 1),
    new ColumnValue("Name", "Alice")
});

// Query with parameters (safe from injection)
dac.ExecuteScalar<int>("SELECT COUNT(*) FROM Users WHERE Name = @name",
    new ColumnValue("@name", "Alice"));  // Returns 1

// Transactional scope
using (var scope = dac.BeginTransactionScope()) {
    dac.Update("Users", new[] { new ColumnValue("Name", "Alice Smith") }, "WHERE ID = 1");
    scope.Commit();  // Atomic persist
}
```

## 🏗️ Core Concepts

**Data Access Context (DAC)**: Wrapper around `IDbConnection` providing transaction scoping, parameterized queries, and connection reuse.

**Connection Pooling**: Connections are automatically managed and reused within a scope for efficiency.

**Column Values**: Named parameter approach using `ColumnValue` objects prevents SQL injection.

**Transaction Scopes**: Atomic boundaries for ACID operations; auto-rollback on exception.

## 🔧 Core Examples

### Connection & Basic Operations

```csharp
using Hydrogen.Data;

// Open SQLite database
var dac = Tools.Sqlite.Open("mydata.db");

// Create and open connection
using (var conn = dac.CreateOpenConnection()) {
    var state = conn.State;  // ConnectionState.Open
}

// Create unopened connection (for lazy initialization)
using (var conn = dac.CreateConnection()) {
    var state = conn.State;  // ConnectionState.Closed
}
```

### Insert & Retrieve

```csharp
using Hydrogen.Data;

var dac = Tools.Sqlite.Open(":memory:");

// Insert with named columns
dac.Insert("BasicTable", new[] {
    new ColumnValue("ID", 1),
    new ColumnValue("Text", "Hello World")
});

// Read all records
var results = dac.ExecuteQuery("SELECT * FROM BasicTable");

// Count records with parameterized safety
int count = dac.ExecuteScalar<int>(
    "SELECT COUNT(*) FROM BasicTable WHERE Text = @text",
    new ColumnValue("@text", "Hello World")
);  // Returns 1
```

### Transactions with Scopes

```csharp
var dac = Tools.Sqlite.Open(":memory:");

// Create table
dac.ExecuteNonQuery(@"CREATE TABLE Accounts (
    ID INTEGER PRIMARY KEY,
    Name TEXT,
    Balance REAL
)");

// Transactional scope ensures atomicity
using (var scope = dac.BeginTransactionScope()) {
    dac.Insert("Accounts", new[] {
        new ColumnValue("ID", 1),
        new ColumnValue("Name", "Alice"),
        new ColumnValue("Balance", 1000.0)
    });
    
    dac.Insert("Accounts", new[] {
        new ColumnValue("ID", 2),
        new ColumnValue("Name", "Bob"),
        new ColumnValue("Balance", 500.0)
    });
    
    // Transfer: withdraw from Alice
    dac.Update("Accounts",
        new[] { new ColumnValue("Balance", 900.0) },
        "WHERE ID = 1");
    
    // Transfer: deposit to Bob
    dac.Update("Accounts",
        new[] { new ColumnValue("Balance", 600.0) },
        "WHERE ID = 2");
    
    // Commit atomically (rollback auto-occurs on exception)
    scope.Commit();
}

// Verify final state
var aliceBalance = dac.ExecuteScalar<double>(
    "SELECT Balance FROM Accounts WHERE ID = 1");
Console.WriteLine($"Alice: {aliceBalance}");  // 900.0
```

### Multi-Database Support

```csharp
// SQLite (embedded, in-memory)
var sqliteDac = Tools.Sqlite.Open(":memory:");

// SQL Server
var sqlServerDac = Tools.MSSQL.Open(
    "Server=localhost;Database=mydb;Trusted_Connection=true");

// Firebird
var firebirdDac = Tools.Firebird.Open(
    "ServerType=1;DataSource=localhost;Database=mydb.fdb;User=sysdba;Password=masterkey");

// All use same DAC interface
sqliteDac.ExecuteScalar("SELECT COUNT(*) FROM Users");
sqlServerDac.ExecuteScalar("SELECT COUNT(*) FROM Users");
firebirdDac.ExecuteScalar("SELECT COUNT(*) FROM Users");
```

### Type Mapping & GUIDs

```csharp
var dac = Tools.Sqlite.Open(":memory:");

// Define table with GUID column
dac.ExecuteNonQuery(@"CREATE TABLE [Items] (
    ID INTEGER PRIMARY KEY,
    UniqueKey UNIQUEIDENTIFIER NOT NULL
)");

// Write GUID
var guid = Guid.NewGuid();
dac.Insert("Items", new[] {
    new ColumnValue("ID", 1),
    new ColumnValue("UniqueKey", guid)
});

// Read GUID back (automatic type conversion)
var retrieved = dac.ExecuteScalar<Guid>(
    "SELECT UniqueKey FROM Items WHERE ID = 1");

Console.WriteLine($"Written: {guid}");
Console.WriteLine($"Retrieved: {retrieved}");
Console.WriteLine($"Match: {guid == retrieved}");  // true
```

### Parameterized Queries (SQL Injection Safe)

```csharp
var dac = Tools.Sqlite.Open(":memory:");

dac.ExecuteNonQuery(@"CREATE TABLE [Users] (
    ID INTEGER PRIMARY KEY,
    Name TEXT,
    Email TEXT
)");

dac.Insert("Users", new[] {
    new ColumnValue("ID", 1),
    new ColumnValue("Name", "Alice"),
    new ColumnValue("Email", "alice@example.com")
});

// Safe parameterized query - prevents SQL injection
string userInput = "Alice"; // Could be malicious
var results = dac.ExecuteQuery(
    "SELECT * FROM Users WHERE Name = @name",
    new ColumnValue("@name", userInput));

// Multiple parameters
results = dac.ExecuteQuery(
    "SELECT * FROM Users WHERE Name = @name AND Email = @email",
    new ColumnValue("@name", "Alice"),
    new ColumnValue("@email", "alice@example.com"));
```

## 📊 Database-Specific Implementations

Hydrogen.Data provides database-specific projects with optimized implementations:

- [Hydrogen.Data.Sqlite](../Hydrogen.Data.Sqlite) - SQLite support (embedded, in-memory)
- [Hydrogen.Data.MSSQL](../Hydrogen.Data.MSSQL) - SQL Server support
- [Hydrogen.Data.Firebird](../Hydrogen.Data.Firebird) - Firebird database support
- [Hydrogen.Data.NHibernate](../Hydrogen.Data.NHibernate) - NHibernate ORM integration

Corresponding WinForms projects available for GUI applications:
- [Hydrogen.Windows.Forms.Sqlite](../Hydrogen.Windows.Forms.Sqlite)
- [Hydrogen.Windows.Forms.MSSQL](../Hydrogen.Windows.Forms.MSSQL)
- [Hydrogen.Windows.Forms.Firebird](../Hydrogen.Windows.Forms.Firebird)

## 🔧 Usage

## 📦 Dependencies

- **Hydrogen**: Core framework
- **System.Data**: ADO.NET abstraction (.NET built-in)
- **Database-specific drivers**: Installed by platform-specific packages (SQLite, SQL Server, Firebird, NHibernate)

## ⚠️ Best Practices

- **Always use `using` statements** for scopes to ensure proper connection cleanup and transaction handling
- **Use `ColumnValue` for all parameters** to prevent SQL injection
- **Keep transaction scopes small** - only operations that must be atomic
- **Parameterized queries only** - never concatenate user input into SQL strings
- **Handle exceptions properly** - transaction scopes auto-rollback on exception; explicit `Commit()` required for success
- **Test with multiple databases** - Hydrogen.Data is database-agnostic, but specific DBMS edge cases may exist

## 🧬 Architecture Layers

- **ADO.NET Extensions**: `IDbConnection`, `IDbCommand`, `IDbReader` helpers
- **DAC (Data Access Context)**: Transaction and connection management (`IDataAccessContext`)
- **Schema Support**: Database introspection and DDL operations
- **Type Mapping**: CLR ↔ Database type conversion
- **Bulk Operations**: Efficient batch insert/update patterns
- **CSV/XML Support**: Data import/export utilities

## 🔌 Extensibility

Implement `IDataAccessContext` to create custom DAC implementations for unsupported databases or specialized persistence needs. All database-specific projects (SQLite, MSSQL, Firebird) extend the abstract `DataAccessContext` base class.

## ⚡ Performance Considerations

- **Connection Pooling**: Reuse connections within transaction scopes for efficiency
- **Batch Operations**: Use bulk insert patterns for large datasets
- **Parameterized Queries**: Cached query plans improve execution speed
- **Type Mapping**: Constant-size types (int, long, GUID) faster than variable-size (string, byte[])
- **Database Selection**: SQLite optimal for single-process/embedded; MSSQL/Firebird for multi-user/distributed

## ✅ Status & Compatibility

- **Maturity**: Production-tested, core abstraction stable
- **.NET Target**: .NET 8.0+ (primary), .NET Standard 2.0 compatibility for some components
- **Thread Safety**: Each DAC instance should be used single-threaded; create separate instances for concurrent access
- **Backward Compatibility**: Database schemas may require migration between Hydrogen versions

## 📖 Related Projects

- [Hydrogen](../Hydrogen) - Core framework
- [Hydrogen.Windows.Forms](../Hydrogen.Windows.Forms) - WinForms integration with DAC support
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core) - Blockchain persistence using this layer
- [Hydrogen.Application](../Hydrogen.Application) - Application framework with data access patterns

## ⚖️ License

Distributed under the **MIT NON-AI License**.

See the LICENSE file for full details. More information: [Sphere10 NON-AI-MIT License](https://sphere10.com/legal/NON-AI-MIT)

## 👤 Author

**Herman Schoenfeld** - Software Engineer

---

**Version**: 2.0+
