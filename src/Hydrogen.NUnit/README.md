<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# ✅ Hydrogen.NUnit

**NUnit test framework extensions** providing utilities, fixtures, assertions, and helpers for writing comprehensive unit tests for Hydrogen framework applications.

Hydrogen.NUnit simplifies **test fixture setup, assertion patterns, and data-driven testing** with reusable base classes, mock factories, performance assertions, and collection utilities that reduce boilerplate while improving test readability.

## ⚡ 10-Second Example

```csharp
using Hydrogen.NUnit;
using NUnit.Framework;

[TestFixture]
public class CalculatorTests : HydrogenTestBase {
    private Calculator _calculator;

    [SetUp]
    public void Setup() {
        _calculator = new Calculator();
    }

    [Test]
    public void Add_WithValidNumbers_ReturnsSum() {
        // Arrange
        int a = 5, b = 3;

        // Act
        int result = _calculator.Add(a, b);

        // Assert
        That.AreEqual(8, result);
    }

    [TestCase(2, 3, 5)]
    [TestCase(10, 5, 15)]
    [TestCase(-1, 1, 0)]
    public void Add_WithMultipleInputs_ReturnsCorrectSum(int a, int b, int expected) {
        int result = _calculator.Add(a, b);
        That.AreEqual(expected, result);
    }
}
```

## 🏗️ Core Concepts

**HydrogenTestBase**: Base class providing common test infrastructure and lifecycle management.

**Enhanced Assertions**: Type-safe assertion methods with better error messages than standard NUnit assertions.

**Test Fixtures**: Reusable test data generators and object builders for complex objects.

**Mock Factories**: Simplified mock creation for dependencies and external services.

**Performance Testing**: Built-in performance measurement and assertion utilities.

## 🔧 Core Examples

### Basic Test Structure & Assertions

```csharp
using Hydrogen.NUnit;
using NUnit.Framework;

[TestFixture]
public class StringUtilsTests : HydrogenTestBase {
    
    [Test]
    public void IsNullOrEmpty_WithNull_ReturnsTrue() {
        // Use HydrogenTestBase's That assertion helper
        That.IsTrue(StringUtils.IsNullOrEmpty(null));
    }

    [Test]
    public void IsNullOrEmpty_WithEmptyString_ReturnsTrue() {
        That.IsTrue(StringUtils.IsNullOrEmpty(""));
    }

    [Test]
    public void IsNullOrEmpty_WithValidString_ReturnsFalse() {
        That.IsFalse(StringUtils.IsNullOrEmpty("hello"));
    }

    [Test]
    public void Concatenate_WithMultipleStrings_ConcatenatesCorrectly() {
        // Arrange
        string[] strings = { "Hello", " ", "World" };

        // Act
        string result = StringUtils.Concatenate(strings);

        // Assert
        That.AreEqual("Hello World", result);
        That.IsNotNull(result);
    }

    [Test]
    public void TrimWhitespace_WithWhitespace_RemovesIt() {
        That.AreEqual("Hello", StringUtils.TrimWhitespace("  Hello  "));
        That.AreEqual("World", StringUtils.TrimWhitespace("\nWorld\n"));
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ToUpperInvariant_WithNull_ThrowsException() {
        StringUtils.ToUpperInvariant(null);
    }
}
```

### Data-Driven Tests with TestCase

```csharp
using Hydrogen.NUnit;
using NUnit.Framework;

[TestFixture]
public class MathTests : HydrogenTestBase {
    
    [TestCase(2, 3, 5)]
    [TestCase(0, 0, 0)]
    [TestCase(-5, 5, 0)]
    [TestCase(100, 200, 300)]
    public void Add_WithVariousInputs_ReturnsCorrectSum(int a, int b, int expected) {
        int result = a + b;
        That.AreEqual(expected, result);
    }

    [TestCase(10, 2, 5)]
    [TestCase(100, 10, 10)]
    [TestCase(7, 2, 3)]
    public void Divide_WithValidInputs_ReturnsCorrectResult(int numerator, int denominator, int expected) {
        int result = numerator / denominator;
        That.AreEqual(expected, result);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-100)]
    [ExpectedException(typeof(ArgumentException))]
    public void Divide_WithZeroDenominator_ThrowsException(int denominator) {
        _ = 10 / denominator;
    }
}
```

### Fixture Setup & TearDown Patterns

```csharp
using Hydrogen.NUnit;
using NUnit.Framework;

[TestFixture]
public class DatabaseTests : HydrogenTestBase {
    private IDatabase _database;
    private ITransaction _transaction;

    [OneTimeSetUp]
    public void ClassSetup() {
        // Runs once before all tests in this class
        Console.WriteLine("Test class initialization");
    }

    [SetUp]
    public void TestSetup() {
        // Runs before each test
        _database = new TestDatabase();
        _transaction = _database.BeginTransaction();
        
        // Create test data
        _database.Insert("Users", new[] {
            new ColumnValue("ID", 1),
            new ColumnValue("Name", "Alice")
        });
    }

    [TearDown]
    public void TestTeardown() {
        // Runs after each test
        _transaction?.Rollback();  // Rollback to clean state
        _database?.Dispose();
    }

    [OneTimeTearDown]
    public void ClassTeardown() {
        // Runs once after all tests
        Console.WriteLine("Test class cleanup");
    }

    [Test]
    public void Insert_WithValidData_CreatesRecord() {
        _database.Insert("Users", new[] {
            new ColumnValue("ID", 2),
            new ColumnValue("Name", "Bob")
        });

        var count = _database.ExecuteScalar<int>("SELECT COUNT(*) FROM Users");
        That.AreEqual(2, count);  // Alice + Bob
    }

    [Test]
    public void Update_WithValidData_ModifiesRecord() {
        _database.Update("Users",
            new[] { new ColumnValue("Name", "Alicia") },
            "WHERE ID = 1");

        var name = _database.ExecuteScalar<string>(
            "SELECT Name FROM Users WHERE ID = 1");
        
        That.AreEqual("Alicia", name);
    }

    [Test]
    public void Delete_WithValidId_RemovesRecord() {
        _database.ExecuteNonQuery("DELETE FROM Users WHERE ID = 1");

        var count = _database.ExecuteScalar<int>("SELECT COUNT(*) FROM Users");
        That.AreEqual(0, count);
    }
}
```

### Object Builders & Test Data Factories

```csharp
using Hydrogen.NUnit;
using NUnit.Framework;

// Custom builder for complex objects
public class UserBuilder {
    private int _id = 1;
    private string _name = "Test User";
    private string _email = "test@example.com";
    private bool _isActive = true;

    public UserBuilder WithId(int id) {
        _id = id;
        return this;
    }

    public UserBuilder WithName(string name) {
        _name = name;
        return this;
    }

    public UserBuilder WithEmail(string email) {
        _email = email;
        return this;
    }

    public UserBuilder AsInactive() {
        _isActive = false;
        return this;
    }

    public User Build() {
        return new User {
            Id = _id,
            Name = _name,
            Email = _email,
            IsActive = _isActive
        };
    }
}

[TestFixture]
public class UserServiceTests : HydrogenTestBase {
    private UserService _service;

    [SetUp]
    public void Setup() {
        _service = new UserService();
    }

    [Test]
    public void CreateUser_WithValidUser_ReturnsUserId() {
        // Arrange
        var user = new UserBuilder()
            .WithName("Alice")
            .WithEmail("alice@example.com")
            .Build();

        // Act
        int userId = _service.CreateUser(user);

        // Assert
        That.IsGreater(userId, 0);
    }

    [Test]
    public void CreateMultipleUsers_WithBuilders_CreatesAllUsers() {
        // Arrange
        var users = new[] {
            new UserBuilder().WithName("Alice").Build(),
            new UserBuilder().WithName("Bob").Build(),
            new UserBuilder().WithName("Charlie").Build()
        };

        // Act
        foreach (var user in users) {
            _service.CreateUser(user);
        }

        // Assert
        That.AreEqual(3, _service.GetUserCount());
    }

    [Test]
    public void SearchUsers_WithActiveFlag_ReturnsOnlyActiveUsers() {
        // Arrange
        _service.CreateUser(new UserBuilder().WithName("Alice").Build());
        _service.CreateUser(new UserBuilder().WithName("Bob").AsInactive().Build());

        // Act
        var activeUsers = _service.GetActiveUsers();

        // Assert
        That.AreEqual(1, activeUsers.Count);
        That.AreEqual("Alice", activeUsers[0].Name);
    }
}
```

### Mock Dependencies & Isolation

```csharp
using Hydrogen.NUnit;
using NUnit.Framework;
using Moq;

[TestFixture]
public class OrderServiceTests : HydrogenTestBase {
    private OrderService _service;
    private Mock<IPaymentProcessor> _mockPaymentProcessor;
    private Mock<IInventoryService> _mockInventory;

    [SetUp]
    public void Setup() {
        _mockPaymentProcessor = new Mock<IPaymentProcessor>();
        _mockInventory = new Mock<IInventoryService>();
        
        _service = new OrderService(
            _mockPaymentProcessor.Object,
            _mockInventory.Object);
    }

    [Test]
    public void PlaceOrder_WithValidData_ProcessesPayment() {
        // Arrange
        var order = new Order { Amount = 100.0m };
        _mockPaymentProcessor
            .Setup(p => p.ProcessPayment(It.IsAny<decimal>()))
            .Returns(true);

        // Act
        bool result = _service.PlaceOrder(order);

        // Assert
        That.IsTrue(result);
        _mockPaymentProcessor.Verify(
            p => p.ProcessPayment(100.0m),
            Times.Once);
    }

    [Test]
    public void PlaceOrder_WhenPaymentFails_DoesNotReduceInventory() {
        // Arrange
        var order = new Order { ItemId = 1, Quantity = 5 };
        _mockPaymentProcessor
            .Setup(p => p.ProcessPayment(It.IsAny<decimal>()))
            .Returns(false);

        // Act
        _service.PlaceOrder(order);

        // Assert
        _mockInventory.Verify(
            i => i.ReduceStock(It.IsAny<int>(), It.IsAny<int>()),
            Times.Never);
    }
}
```

### Performance & Timing Assertions

```csharp
using Hydrogen.NUnit;
using NUnit.Framework;

[TestFixture]
public class PerformanceTests : HydrogenTestBase {
    
    [Test]
    public void LargeListSort_CompletesWithinTimeLimit() {
        // Arrange
        var list = Enumerable.Range(0, 10000)
            .OrderByDescending(x => x)
            .ToList();

        // Act & Assert - must complete within 100ms
        That.CompletesWithin(() => {
            list.Sort();
        }, timeoutMs: 100);
    }

    [Test]
    public void SlowOperation_ExecutionTimeIsLogged() {
        // Act & Assert - measure execution time
        var elapsed = That.MeasureExecutionTime(() => {
            System.Threading.Thread.Sleep(500);
        });

        Console.WriteLine($"Execution time: {elapsed.TotalMilliseconds}ms");
        That.IsGreaterOrEqual(elapsed.TotalMilliseconds, 500);
        That.IsLess(elapsed.TotalMilliseconds, 600);  // Allow margin
    }

    [Test]
    public void MemoryAllocation_WithinReasonableLimits() {
        // Act
        var before = GC.GetTotalMemory(true);
        var list = Enumerable.Range(0, 100000).ToList();
        var after = GC.GetTotalMemory(true);

        // Assert - allocation should be reasonable
        long allocated = after - before;
        That.IsLess(allocated, 10_000_000);  // Less than 10MB
    }
}
```

### Collection & Exception Testing

```csharp
using Hydrogen.NUnit;
using NUnit.Framework;

[TestFixture]
public class CollectionTests : HydrogenTestBase {
    
    [Test]
    public void GetItems_ReturnsNonEmptyCollection() {
        var items = GetItems();
        That.IsNotEmpty(items);
    }

    [Test]
    public void FindItem_WithValidId_ReturnsCorrectItem() {
        var items = GetItems();
        var item = items.FirstOrDefault(x => x.Id == 2);
        
        That.IsNotNull(item);
        That.AreEqual("Item 2", item.Name);
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ProcessItem_WithNull_ThrowsArgumentNullException() {
        ProcessItem(null);
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException), 
        ExpectedMessage = "Item not found")]
    public void GetItem_WithInvalidId_ThrowsInvalidOperationException() {
        GetItem(-1);
    }

    [Test]
    public void ParseInteger_WithInvalidString_ReturnsFalseOnTryParse() {
        bool result = int.TryParse("not-a-number", out int value);
        That.IsFalse(result);
        That.AreEqual(0, value);
    }

    private List<Item> GetItems() => new() {
        new Item { Id = 1, Name = "Item 1" },
        new Item { Id = 2, Name = "Item 2" }
    };

    private Item GetItem(int id) =>
        GetItems().FirstOrDefault(x => x.Id == id) 
        ?? throw new InvalidOperationException("Item not found");

    private void ProcessItem(Item item) {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
    }
}

public class Item {
    public int Id { get; set; }
    public string Name { get; set; }
}
```

## 🏗️ Architecture

**HydrogenTestBase**: Abstract base class providing common test infrastructure.

**Assertion Helpers**: Type-safe `That` assertion utility with better error messages.

**Test Fixtures**: Builder patterns and object factories for test data.

**Performance Utilities**: Timing and memory measurement helpers.

**Mock Integration**: Simplified mock creation with Moq patterns.

## 📋 Best Practices

- **Test naming**: Use descriptive names following `Method_Scenario_ExpectedResult` pattern
- **AAA pattern**: Organize tests with Arrange, Act, Assert sections
- **Single assertion**: One logical assertion per test when possible
- **Isolation**: Use Setup/TearDown to ensure test independence
- **Mocking**: Mock external dependencies to test in isolation
- **Test data**: Use builders for complex test object creation
- **Cleanup**: Always clean up resources in TearDown
- **Readability**: Prioritize test clarity over code reuse

## 📊 Status & Compatibility

- **Version**: 2.0+
- **Framework**: NUnit 3.0+
- **.NET**: .NET 5.0+, .NET Framework 4.7+
- **Test Runner**: Visual Studio, ReSharper, NUnit Console, NCrunch

## 📦 Dependencies

- **NUnit**: 3.0+
- **Moq**: For mocking (optional but recommended)
- **Hydrogen.* libraries**: As needed for testing

## 📚 Related Projects

- [Hydrogen.NUnit.DB](../Hydrogen.NUnit.DB) - Database-specific test utilities
- [Hydrogen.Tests](../../tests/Hydrogen.Tests) - Test examples and patterns
- [Hydrogen.Data.Tests](../../tests/Hydrogen.Data.Tests) - Data layer test patterns
- All Hydrogen projects - Test these using Hydrogen.NUnit

## 📄 License & Author

**License**: [Refer to repository LICENSE](../../LICENSE)  
**Author**: Herman Schoenfeld, Sphere 10 Software (sphere10.com)  
**Copyright**: © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.
