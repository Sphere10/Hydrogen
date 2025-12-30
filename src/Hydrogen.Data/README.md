# 💾 Hydrogen.Data

**Comprehensive data access and database abstraction layer** providing ADO.NET enhancements, schema support, fluent SQL query building, and CSV support.

## 📋 Overview

`Hydrogen.Data` provides a complete data access framework for Hydrogen applications, with abstractions over ADO.NET, schema management, and support for multiple databases. It enables efficient data persistence without vendor lock-in.

## 🚀 Key Features

- **ADO.NET Enhancements**: Simplified data access with extensions and helpers
- **Schema Support**: Database schema introspection and manipulation
- **SQL Query Builder**: Fluent API for constructing SQL queries  
- **CSV Support**: Import/export data in CSV format
- **Database Abstraction**: Support for multiple database engines
- **Connection Management**: Safe connection and transaction handling
- **Type Mapping**: Automatic type conversion between CLR and database types
- **Async Operations**: Full async/await support

## 🏗️ Architecture

The library is organized into layers:

- **Core ADO.NET**: Enhanced connection, command, and reader extensions
- **Schema**: Database introspection and schema operations
- **Query Building**: Fluent SQL query construction
- **CSV Processing**: CSV import and export
- **Type Mapping**: CLR to database type conversion
- **Connection Pooling**: Efficient connection management
- **Data Access Context (DAC)**: Transaction scope and query execution

## 🔧 Usage

### Basic CRUD Operations

Insert, read, update, and delete records using the Data Access Context (DAC):

```csharp
using Hydrogen.Data;

// Create a database scope
using (var dac = CreateDatabaseScope(DBMSType.SQLite)) {
	// Insert a record
	dac.Insert("Users", new[] {
		new ColumnValue("ID", 1),
		new ColumnValue("Name", "Alice"),
		new ColumnValue("Email", "alice@example.com")
	});
    
	// Read records
	var users = dac.Select("Users");
    
	// Update a record
	dac.Update("Users", new[] {
		new ColumnValue("Name", "Alice Smith")
	}, "WHERE ID = 1");
    
	// Delete a record
	dac.Delete("Users", "WHERE ID = 1");
}
```

### Connection Pooling and Reuse

Efficiently reuse connections within a scope:

```csharp
using (var dac = CreateDatabaseScope(DBMSType.SQLite)) {
	// First query uses/creates connection
	var count1 = dac.ExecuteScalar("SELECT COUNT(*) FROM Users");
    
	// Second query reuses the same connection (pooled)
	var count2 = dac.ExecuteScalar("SELECT COUNT(*) FROM Orders");
    
	// Connection is returned to pool when scope exits
}
```

### Transactions

Execute multiple queries within an atomic transaction:

```csharp
using (var dac = CreateDatabaseScope(DBMSType.SQLite)) {
	try {
		// Start transaction
		dac.BeginTransaction();
        
		// Execute multiple operations
		dac.Insert("Users", new[] {
			new ColumnValue("ID", 1),
			new ColumnValue("Name", "Bob")
		});
        
		dac.Update("Accounts", new[] {
			new ColumnValue("Balance", 100)
		}, "WHERE UserID = 1");
        
		// Commit all changes atomically
		dac.Commit();
	} catch {
		// Rollback all changes on error
		dac.Rollback();
		throw;
	}
}
```

### SQL Query Builder

Construct SQL queries fluently:

```csharp
using Hydrogen.Data;

var query = new SqlQueryBuilder()
	.Select("ID", "Name", "Email")
	.From("Users")
	.Where("Status = @status")
	.OrderBy("Name");

// Use with DAC
using (var dac = CreateDatabaseScope(DBMSType.SQLite)) {
	var results = dac.ExecuteQuery(query.ToString(), 
		new[] { new ParameterValue("@status", "Active") });
}
```

### Database-Specific Operations

Work with multiple DBMS providers:

```csharp
// SQLite (embedded)
using (var dac = CreateDatabaseScope(DBMSType.SQLite, "Data Source=mydb.db")) {
	var count = dac.ExecuteScalar("SELECT COUNT(*) FROM Users");
}

// SQL Server
using (var dac = CreateDatabaseScope(DBMSType.MSSQL, 
	"Server=localhost;Database=mydb;Trusted_Connection=true")) {
	var count = dac.ExecuteScalar("SELECT COUNT(*) FROM Users");
}

// Firebird
using (var dac = CreateDatabaseScope(DBMSType.Firebird,
	"ServerType=1;DataSource=localhost;Database=mydb.fdb;User=sysdba;Password=masterkey")) {
	var count = dac.ExecuteScalar("SELECT COUNT(*) FROM Users");
}
```

## 📦 Dependencies

- **Hydrogen**: Core framework
- **Newtonsoft.Json**: JSON support
- **System.Data.SqlClient** or **Microsoft.Data.SqlClient**: SQL Server support (optional per database)

## 📄 Database-Specific Projects

Each database engine has its own implementation project:

- [Hydrogen.Data.Sqlite](../Hydrogen.Data.Sqlite) - SQLite implementation
- [Hydrogen.Data.Firebird](../Hydrogen.Data.Firebird) - Firebird database support
- [Hydrogen.Data.MSSQL](../Hydrogen.Data.MSSQL) - SQL Server support
- [Hydrogen.Data.NHibernate](../Hydrogen.Data.NHibernate) - NHibernate integration

## 💡 Key Concepts

- **Data Access Context (DAC)**: Wrapper around database connection handling transactions and queries
- **Connection Pooling**: Connections are reused within a scope for efficiency
- **Scope Isolation**: Each `using` block gets its own transaction scope
- **Column Values**: Named parameter approach for safe SQL operations (prevents SQL injection)
- **Query Results**: Results are returned as DataSets, DataTables, or strongly-typed collections

## ⚠️ Best Practices

- Always use `using` statements to ensure connections are properly returned to the pool
- Use parameterized queries with `ColumnValue` to prevent SQL injection
- Keep transaction scopes small and focused
- Use `Commit()` to persist changes, `Rollback()` to discard them
- Catch `DataException` for database-specific errors

## 📄 Related Projects

- [Hydrogen](../Hydrogen) - Core framework
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core) - Blockchain persistence using this layer
