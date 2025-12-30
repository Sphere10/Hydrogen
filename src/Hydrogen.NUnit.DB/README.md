<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 🗄️ Hydrogen.NUnit.DB

**Database-specific NUnit test utilities** providing fixtures, transaction management, data seeding, and assertion helpers for comprehensive database layer testing with Hydrogen.Data.

Hydrogen.NUnit.DB extends **Hydrogen.NUnit with database-focused testing infrastructure** including automatic transaction rollback, test data generators, schema validation assertions, and multi-database fixture support for testing across SQLite, SQL Server, Firebird, and other databases.

## ⚡ 10-Second Example

```csharp
using Hydrogen.NUnit.DB;
using NUnit.Framework;

[TestFixture]
public class UserRepositoryTests : DatabaseTestFixture {
    private UserRepository _repository;

    [SetUp]
    public void Setup() {
        // Automatic in-memory SQLite database with rollback after test
        _repository = new UserRepository(Dac);
    }

    [Test]
    public void InsertUser_WithValidData_CreatesRecord() {
        // Arrange
        var user = new User { Name = "Alice", Email = "alice@example.com" };

        // Act
        var userId = _repository.Insert(user);

        // Assert
        That.IsGreater(userId, 0);
        AssertUserExists(userId, "Alice");
    }

    [Test]
    public void GetUser_WithValidId_ReturnsUser() {
        // Arrange
        var userId = InsertTestUser("Bob");

        // Act
        var user = _repository.GetById(userId);

        // Assert
        That.IsNotNull(user);
        That.AreEqual("Bob", user.Name);
    }
}
```

## 🏗️ Core Concepts

**DatabaseTestFixture**: Base class providing database context with automatic transaction management.

**Automatic Rollback**: Transactions automatically roll back after each test for isolation.

**Test Data Builders**: Utilities for inserting and managing test data across multiple databases.

**Schema Assertions**: Validate table existence, column definitions, constraints, and indexes.

**Multi-Database Support**: Test against different database engines (SQLite, SQL Server, Firebird, etc.).

## 🔧 Core Examples

### Basic Database Testing with Auto-Rollback

```csharp
using Hydrogen.NUnit.DB;
using Hydrogen.Data;
using NUnit.Framework;

[TestFixture]
public class UserRepositoryTests : DatabaseTestFixture {
    private UserRepository _repository;

    protected override IDataAccessContext CreateDatabase() {
        // Create in-memory SQLite for fast testing
        return Tools.Sqlite.Open(":memory:");
    }

    protected override void SetupSchema() {
        // Create test schema
        Dac.ExecuteNonQuery(@"
            CREATE TABLE Users (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT UNIQUE,
                IsActive INTEGER DEFAULT 1
            )");
    }

    [SetUp]
    public void TestSetup() {
        _repository = new UserRepository(Dac);
    }

    [Test]
    public void Insert_WithValidUser_CreatesRecord() {
        // Arrange
        var user = new User { Name = "Alice", Email = "alice@example.com" };

        // Act
        var id = _repository.Insert(user);

        // Assert
        That.IsGreater(id, 0);
        
        // Query database to verify
        var count = Dac.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM Users WHERE Email = @email",
            new ColumnValue("@email", "alice@example.com"));
        
        That.AreEqual(1, count);
    }

    [Test]
    public void Update_WithValidData_ModifiesRecord() {
        // Arrange
        var id = InsertTestUser("Alice", "alice@example.com");

        // Act
        _repository.Update(id, new User { 
            Name = "Alicia", 
            Email = "alicia@example.com" 
        });

        // Assert
        var user = _repository.GetById(id);
        That.AreEqual("Alicia", user.Name);
        That.AreEqual("alicia@example.com", user.Email);
    }

    [Test]
    public void Delete_WithValidId_RemovesRecord() {
        // Arrange
        var id = InsertTestUser("Bob", "bob@example.com");

        // Act
        _repository.Delete(id);

        // Assert
        var user = _repository.GetById(id);
        That.IsNull(user);
    }

    [Test]
    public void GetActiveUsers_WithMixedData_ReturnsOnlyActive() {
        // Arrange
        InsertTestUser("Alice", "alice@example.com", isActive: true);
        InsertTestUser("Bob", "bob@example.com", isActive: false);
        InsertTestUser("Charlie", "charlie@example.com", isActive: true);

        // Act
        var activeUsers = _repository.GetActiveUsers();

        // Assert
        That.AreEqual(2, activeUsers.Count);
        That.Contains("Alice", activeUsers.Select(u => u.Name).ToList());
        That.Contains("Charlie", activeUsers.Select(u => u.Name).ToList());
    }

    // Helper methods
    private int InsertTestUser(string name, string email, bool isActive = true) {
        return Dac.ExecuteScalar<int>(@"
            INSERT INTO Users (Name, Email, IsActive) 
            VALUES (@name, @email, @isActive);
            SELECT last_insert_rowid()",
            new ColumnValue("@name", name),
            new ColumnValue("@email", email),
            new ColumnValue("@isActive", isActive ? 1 : 0));
    }

    private void AssertUserExists(int id, string expectedName) {
        var name = Dac.ExecuteScalar<string>(
            "SELECT Name FROM Users WHERE ID = @id",
            new ColumnValue("@id", id));
        
        That.AreEqual(expectedName, name);
    }
}

public class User {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
}

public class UserRepository {
    private readonly IDataAccessContext _dac;

    public UserRepository(IDataAccessContext dac) {
        _dac = dac;
    }

    public int Insert(User user) {
        return _dac.ExecuteScalar<int>(@"
            INSERT INTO Users (Name, Email, IsActive)
            VALUES (@name, @email, @isActive);
            SELECT last_insert_rowid()",
            new ColumnValue("@name", user.Name),
            new ColumnValue("@email", user.Email),
            new ColumnValue("@isActive", user.IsActive ? 1 : 0));
    }

    public void Update(int id, User user) {
        _dac.Update("Users",
            new[] {
                new ColumnValue("Name", user.Name),
                new ColumnValue("Email", user.Email)
            },
            "WHERE ID = @id",
            new ColumnValue("@id", id));
    }

    public void Delete(int id) {
        _dac.ExecuteNonQuery("DELETE FROM Users WHERE ID = @id",
            new ColumnValue("@id", id));
    }

    public User GetById(int id) {
        var result = _dac.ExecuteQuery(@"
            SELECT ID, Name, Email, IsActive FROM Users WHERE ID = @id",
            new ColumnValue("@id", id));
        
        if (!result.Read()) return null;
        
        return new User {
            Id = result.GetInt32(0),
            Name = result.GetString(1),
            Email = result.GetString(2),
            IsActive = result.GetInt32(3) == 1
        };
    }

    public List<User> GetActiveUsers() {
        var users = new List<User>();
        var result = _dac.ExecuteQuery("SELECT ID, Name, Email FROM Users WHERE IsActive = 1");
        
        while (result.Read()) {
            users.Add(new User {
                Id = result.GetInt32(0),
                Name = result.GetString(1),
                Email = result.GetString(2),
                IsActive = true
            });
        }
        
        return users;
    }
}
```

### Multi-Database Testing

```csharp
using Hydrogen.NUnit.DB;
using Hydrogen.Data;
using NUnit.Framework;

[TestFixture(DatabaseType.Sqlite)]
[TestFixture(DatabaseType.SqlServer)]
[TestFixture(DatabaseType.Firebird)]
public class RepositoryMultiDbTests : DatabaseTestFixture {
    private readonly DatabaseType _databaseType;
    private ProductRepository _repository;

    public RepositoryMultiDbTests(DatabaseType databaseType) {
        _databaseType = databaseType;
    }

    protected override IDataAccessContext CreateDatabase() {
        return _databaseType switch {
            DatabaseType.Sqlite => Tools.Sqlite.Open(":memory:"),
            DatabaseType.SqlServer => Tools.MSSQL.Open(
                "Server=.;Database=TestDb;Integrated Security=true;"),
            DatabaseType.Firebird => Tools.Firebird.Open(
                "DataSource=test.fdb;User=sysdba;Password=masterkey"),
            _ => throw new NotSupportedException()
        };
    }

    protected override void SetupSchema() {
        // Schema is database-agnostic but may vary slightly
        Dac.ExecuteNonQuery(@"
            CREATE TABLE Products (
                ProductID INTEGER PRIMARY KEY,
                Name VARCHAR(100) NOT NULL,
                Price DECIMAL(10, 2),
                Stock INTEGER
            )");
    }

    [SetUp]
    public void TestSetup() {
        _repository = new ProductRepository(Dac);
    }

    [Test]
    public void FindByPrice_WithPriceRange_ReturnsMatchingProducts() {
        // Insert test data
        InsertProduct(1, "Laptop", 999.99m, 10);
        InsertProduct(2, "Mouse", 29.99m, 50);
        InsertProduct(3, "Monitor", 299.99m, 5);

        // Act
        var products = _repository.FindByPriceRange(100, 500);

        // Assert
        That.AreEqual(2, products.Count);  // Laptop and Monitor
    }

    [Test]
    public void BulkInsert_WithMultipleProducts_CreatesAllRecords() {
        // Arrange
        var products = new[] {
            new Product { Id = 1, Name = "Item 1", Price = 10.0m, Stock = 100 },
            new Product { Id = 2, Name = "Item 2", Price = 20.0m, Stock = 50 },
            new Product { Id = 3, Name = "Item 3", Price = 30.0m, Stock = 25 }
        };

        // Act
        _repository.InsertBatch(products);

        // Assert
        var count = Dac.ExecuteScalar<int>("SELECT COUNT(*) FROM Products");
        That.AreEqual(3, count);
    }

    private void InsertProduct(int id, string name, decimal price, int stock) {
        Dac.Insert("Products", new[] {
            new ColumnValue("ProductID", id),
            new ColumnValue("Name", name),
            new ColumnValue("Price", price),
            new ColumnValue("Stock", stock)
        });
    }
}

public enum DatabaseType {
    Sqlite,
    SqlServer,
    Firebird
}
```

### Schema & Constraint Validation

```csharp
using Hydrogen.NUnit.DB;
using NUnit.Framework;

[TestFixture]
public class SchemaTests : DatabaseTestFixture {
    
    protected override void SetupSchema() {
        Dac.ExecuteNonQuery(@"
            CREATE TABLE Users (
                ID INTEGER PRIMARY KEY,
                Name VARCHAR(100) NOT NULL,
                Email VARCHAR(100) UNIQUE NOT NULL,
                CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                IsActive INTEGER DEFAULT 1
            )");

        Dac.ExecuteNonQuery(@"
            CREATE INDEX idx_Users_Email ON Users(Email)");
    }

    [Test]
    public void Table_Users_Exists() {
        That.TableExists(Dac, "Users");
    }

    [Test]
    public void Table_Users_HasRequiredColumns() {
        That.ColumnExists(Dac, "Users", "ID");
        That.ColumnExists(Dac, "Users", "Name");
        That.ColumnExists(Dac, "Users", "Email");
        That.ColumnExists(Dac, "Users", "CreatedDate");
        That.ColumnExists(Dac, "Users", "IsActive");
    }

    [Test]
    public void Column_Name_IsNotNullable() {
        That.ColumnIsNotNullable(Dac, "Users", "Name");
    }

    [Test]
    public void Column_Email_HasUniqueConstraint() {
        // Try to insert duplicate emails
        Dac.Insert("Users", new[] {
            new ColumnValue("ID", 1),
            new ColumnValue("Name", "Alice"),
            new ColumnValue("Email", "alice@example.com")
        });

        // This should fail with constraint violation
        That.Throws<Exception>(() => {
            Dac.Insert("Users", new[] {
                new ColumnValue("ID", 2),
                new ColumnValue("Name", "Bob"),
                new ColumnValue("Email", "alice@example.com")  // Duplicate
            });
        });
    }

    [Test]
    public void Index_Users_Email_Exists() {
        That.IndexExists(Dac, "Users", "idx_Users_Email");
    }

    [Test]
    public void PrimaryKey_Cannot_BeNull() {
        That.Throws<Exception>(() => {
            Dac.ExecuteNonQuery(@"
                INSERT INTO Users (ID, Name, Email)
                VALUES (NULL, 'Test', 'test@example.com')");
        });
    }
}
```

### Transaction & Data Integrity Testing

```csharp
using Hydrogen.NUnit.DB;
using NUnit.Framework;

[TestFixture]
public class TransactionTests : DatabaseTestFixture {
    
    protected override void SetupSchema() {
        Dac.ExecuteNonQuery(@"
            CREATE TABLE Accounts (
                AccountID INTEGER PRIMARY KEY,
                Balance DECIMAL(18, 2)
            )");
    }

    [Test]
    public void Transaction_WithCommit_PersistsData() {
        // Arrange
        using (var scope = Dac.BeginTransactionScope()) {
            Dac.Insert("Accounts", new[] {
                new ColumnValue("AccountID", 1),
                new ColumnValue("Balance", 1000.0m)
            });
            scope.Commit();
        }

        // Assert - data persists beyond transaction
        var balance = Dac.ExecuteScalar<decimal>(
            "SELECT Balance FROM Accounts WHERE AccountID = 1");
        
        That.AreEqual(1000.0m, balance);
    }

    [Test]
    public void Transaction_WithRollback_Discards Changes() {
        // Arrange - insert initial data
        Dac.Insert("Accounts", new[] {
            new ColumnValue("AccountID", 1),
            new ColumnValue("Balance", 1000.0m)
        });

        // Act - attempt transfer in transaction
        try {
            using (var scope = Dac.BeginTransactionScope()) {
                Dac.Update("Accounts",
                    new[] { new ColumnValue("Balance", 500.0m) },
                    "WHERE AccountID = 1");
                
                throw new Exception("Simulated error");  // Force rollback
                // scope.Commit();  // Never reached
            }
        } catch { }

        // Assert - original balance unchanged
        var balance = Dac.ExecuteScalar<decimal>(
            "SELECT Balance FROM Accounts WHERE AccountID = 1");
        
        That.AreEqual(1000.0m, balance);  // Not 500!
    }

    [Test]
    public void Transaction_Isolation_PreventsConflicts() {
        // Arrange
        Dac.Insert("Accounts", new[] {
            new ColumnValue("AccountID", 1),
            new ColumnValue("Balance", 1000.0m)
        });

        // Act - multiple transactions
        using (var scope1 = Dac.BeginTransactionScope()) {
            using (var scope2 = Dac.BeginTransactionScope()) {
                Dac.Update("Accounts",
                    new[] { new ColumnValue("Balance", 500.0m) },
                    "WHERE AccountID = 1");
                
                scope2.Commit();
            }
            
            scope1.Commit();
        }

        // Assert - final state is consistent
        var balance = Dac.ExecuteScalar<decimal>(
            "SELECT Balance FROM Accounts WHERE AccountID = 1");
        
        That.AreEqual(500.0m, balance);
    }
}
```

## 🏗️ Architecture

**DatabaseTestFixture**: Base class providing database context with transaction management.

**Automatic Rollback**: Each test runs in a transaction that rolls back automatically.

**Test Data Helpers**: Builder and insertion utilities for test data creation.

**Schema Assertions**: Validation methods for tables, columns, indexes, and constraints.

**Multi-Database Support**: TestFixture attributes for parameterized testing across databases.

## 📋 Best Practices

- **Transaction isolation**: Tests are isolated by automatic rollback
- **Fresh schema**: Setup creates schema before each test
- **Test data builders**: Use helpers to create consistent test data
- **Assertion clarity**: Use schema assertion helpers for validation
- **Multi-database**: Use TestFixture parameters for cross-database testing
- **Transaction testing**: Test both commit and rollback scenarios
- **Constraint validation**: Test database constraints explicitly
- **Performance isolation**: Each test is independent with fresh data

## 📊 Status & Compatibility

- **Version**: 2.0+
- **Framework**: NUnit 3.0+, Hydrogen.NUnit
- **.NET**: .NET 5.0+, .NET Framework 4.7+
- **Databases**: SQLite, SQL Server, Firebird, others via Hydrogen.Data

## 📦 Dependencies

- **Hydrogen.NUnit**: Base test framework
- **Hydrogen.Data**: Data abstraction layer
- **Database providers**: SQLite, SQL Server, Firebird, etc.

## 📚 Related Projects

- [Hydrogen.NUnit](../Hydrogen.NUnit) - Base NUnit testing framework
- [Hydrogen.Data](../Hydrogen.Data) - Data abstraction layer
- [Hydrogen.Data.Tests](../../tests/Hydrogen.Data.Tests) - Data test examples
- Database projects - Test targets using Hydrogen.NUnit.DB

## 📄 License & Author

**License**: [Refer to repository LICENSE](../../LICENSE)  
**Author**: Herman Schoenfeld, Sphere 10 Software (sphere10.com)  
**Copyright**: © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.
