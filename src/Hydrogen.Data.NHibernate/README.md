<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 💾 Hydrogen.Data.NHibernate

**NHibernate ORM integration** for Hydrogen.Data abstraction layer, providing object-relational mapping with support for multiple databases while maintaining LINQ query capabilities and advanced persistence patterns.

Hydrogen.Data.NHibernate bridges **NHibernate's powerful ORM capabilities** with Hydrogen.Data abstraction, enabling domain-driven design patterns, lazy loading, change tracking, and complex queries across SQLite, SQL Server, Firebird, and other supported databases.

## ⚡ 10-Second Example

```csharp
using Hydrogen.Data.NHibernate;
using NHibernate;

// Configure NHibernate with Hydrogen
var sessionFactory = new NHibernateSessionFactory()
    .UseSqlite("myapp.db")  // or UseSqlServer(), UseFirebird(), etc.
    .AddMapping(typeof(User).Assembly)
    .BuildSessionFactory();

// Open session and query
using (var session = sessionFactory.OpenSession()) {
    // LINQ query
    var users = session.Query<User>()
        .Where(u => u.Name.StartsWith("A"))
        .ToList();
    
    // HQL query
    var count = session.CreateQuery("SELECT COUNT(*) FROM User")
        .UniqueResult<int>();
}
```

## 🏗️ Core Concepts

**Object-Relational Mapping**: Automatic mapping between domain objects and database tables with type safety.

**Lazy Loading**: Deferred loading of related entities reducing database queries and memory footprint.

**Change Tracking**: Automatic detection of entity modifications and generation of appropriate UPDATE statements.

**LINQ Query Support**: Type-safe LINQ-to-NHibernate queries with SQL translation.

**Cascade Operations**: Automatic propagation of operations (insert, update, delete) across entity relationships.

## 🔧 Core Examples

### Configuration & Session Management

```csharp
using Hydrogen.Data.NHibernate;
using NHibernate;
using NHibernate.Cfg;

// SQLite database
var sqliteFactory = new NHibernateSessionFactory()
    .UseSqlite(":memory:")  // or "myapp.db" for file-based
    .AddMapping(typeof(User).Assembly)
    .BuildSessionFactory();

// SQL Server database
var sqlServerFactory = new NHibernateSessionFactory()
    .UseSqlServer("Server=.;Database=myapp;Integrated Security=true;")
    .AddMapping(typeof(User).Assembly)
    .BuildSessionFactory();

// Manual configuration
var config = new Configuration();
config.Configure();  // Reads from hibernate.cfg.xml
var sessionFactory = config.BuildSessionFactory();

// Open session and transaction
using (var session = sessionFactory.OpenSession()) {
    using (var transaction = session.BeginTransaction()) {
        try {
            var user = new User { Name = "Alice", Email = "alice@example.com" };
            session.Save(user);
            
            transaction.Commit();
        } catch {
            transaction.Rollback();
            throw;
        }
    }
}
```

### Entity Mapping & Persistence

```csharp
using Hydrogen.Data.NHibernate;
using NHibernate;
using NHibernate.Mapping.ByCode;

// Define domain model
public class User {
    public virtual int Id { get; set; }
    public virtual string Name { get; set; }
    public virtual string Email { get; set; }
    public virtual DateTime CreatedDate { get; set; }
}

public class Product {
    public virtual int Id { get; set; }
    public virtual string Name { get; set; }
    public virtual decimal Price { get; set; }
    public virtual ISet<Order> Orders { get; set; } = new HashSet<Order>();
}

public class Order {
    public virtual int Id { get; set; }
    public virtual User Customer { get; set; }
    public virtual Product Product { get; set; }
    public virtual int Quantity { get; set; }
    public virtual DateTime OrderDate { get; set; }
}

// Mapping configuration
var mapper = new ModelMapper();
mapper.Class<User>(cm => {
    cm.Id(u => u.Id, m => m.Generator(Generators.Native));
    cm.Property(u => u.Name);
    cm.Property(u => u.Email);
    cm.Property(u => u.CreatedDate);
});

mapper.Class<Product>(cm => {
    cm.Id(p => p.Id, m => m.Generator(Generators.Native));
    cm.Property(p => p.Name);
    cm.Property(p => p.Price);
});

mapper.Class<Order>(cm => {
    cm.Id(o => o.Id, m => m.Generator(Generators.Native));
    cm.ManyToOne(o => o.Customer, m => m.Column("UserID"));
    cm.ManyToOne(o => o.Product, m => m.Column("ProductID"));
    cm.Property(o => o.Quantity);
    cm.Property(o => o.OrderDate);
});
```

### LINQ Queries & HQL

```csharp
using Hydrogen.Data.NHibernate;

var sessionFactory = new NHibernateSessionFactory()
    .UseSqlite(":memory:")
    .AddMapping(typeof(User).Assembly)
    .BuildSessionFactory();

using (var session = sessionFactory.OpenSession()) {
    // LINQ queries (type-safe)
    var usersNamedAlice = session.Query<User>()
        .Where(u => u.Name == "Alice")
        .ToList();

    var recentUsers = session.Query<User>()
        .Where(u => u.CreatedDate > DateTime.Now.AddDays(-7))
        .OrderByDescending(u => u.CreatedDate)
        .Take(10)
        .ToList();

    var expensiveProducts = session.Query<Product>()
        .Where(p => p.Price > 100m)
        .Select(p => new { p.Name, p.Price })
        .ToList();

    // HQL queries (Hibernate Query Language)
    var hqlUsers = session.CreateQuery("SELECT u FROM User u WHERE u.Name LIKE :name")
        .SetParameter("name", "A%")
        .List<User>();

    var hqlCount = session.CreateQuery("SELECT COUNT(u) FROM User u")
        .UniqueResult<int>();

    // Aggregation
    var avgPrice = session.Query<Product>()
        .Average(p => p.Price);

    var maxPrice = session.Query<Product>()
        .Max(p => p.Price);

    var groupedOrders = session.Query<Order>()
        .GroupBy(o => o.Customer)
        .Select(g => new {
            Customer = g.Key.Name,
            OrderCount = g.Count(),
            TotalAmount = g.Sum(o => o.Quantity * o.Product.Price)
        })
        .ToList();
}
```

### Relationship Management & Cascading

```csharp
using Hydrogen.Data.NHibernate;

var sessionFactory = new NHibernateSessionFactory()
    .UseSqlite(":memory:")
    .AddMapping(typeof(User).Assembly)
    .BuildSessionFactory();

using (var session = sessionFactory.OpenSession()) {
    using (var transaction = session.BeginTransaction()) {
        // Create user with orders
        var user = new User { Name = "Bob", Email = "bob@example.com" };
        
        var laptop = new Product { Name = "Laptop", Price = 999.99m };
        var mouse = new Product { Name = "Mouse", Price = 29.99m };
        
        var order1 = new Order { 
            Customer = user, 
            Product = laptop, 
            Quantity = 1, 
            OrderDate = DateTime.Now 
        };
        
        var order2 = new Order { 
            Customer = user, 
            Product = mouse, 
            Quantity = 2, 
            OrderDate = DateTime.Now 
        };
        
        // Save user (orders cascade-saved if configured)
        session.Save(user);
        session.Save(order1);
        session.Save(order2);
        
        transaction.Commit();
    }
}

using (var session = sessionFactory.OpenSession()) {
    // Load user with lazy-loaded orders
    var user = session.Get<User>(1);
    
    // Access related entity (triggers lazy load)
    var orders = session.Query<Order>()
        .Where(o => o.Customer.Id == user.Id)
        .ToList();
    
    // Update scenario
    var orderToDelete = orders.First();
    session.Delete(orderToDelete);  // If cascade configured, may affect user
    
    session.Flush();  // Flush changes to database
}
```

### Transactions & Batch Operations

```csharp
using Hydrogen.Data.NHibernate;

var sessionFactory = new NHibernateSessionFactory()
    .UseSqlite(":memory:")
    .AddMapping(typeof(User).Assembly)
    .BuildSessionFactory();

using (var session = sessionFactory.OpenSession()) {
    // Batch insert
    using (var transaction = session.BeginTransaction()) {
        for (int i = 1; i <= 1000; i++) {
            var user = new User { 
                Name = $"User{i}", 
                Email = $"user{i}@example.com",
                CreatedDate = DateTime.Now 
            };
            session.Save(user);
            
            // Flush every 100 records to manage memory
            if (i % 100 == 0) {
                session.Flush();
                session.Clear();
            }
        }
        transaction.Commit();
    }
    
    // Bulk update using HQL
    using (var transaction = session.BeginTransaction()) {
        var updateCount = session.CreateQuery(
            "UPDATE User u SET u.Email = :newEmail WHERE u.Name LIKE :pattern")
            .SetParameter("newEmail", "updated@example.com")
            .SetParameter("pattern", "User1%")
            .ExecuteUpdate();
        
        Console.WriteLine($"Updated {updateCount} records");
        transaction.Commit();
    }
    
    // Bulk delete using HQL
    using (var transaction = session.BeginTransaction()) {
        var deleteCount = session.CreateQuery(
            "DELETE FROM User u WHERE u.CreatedDate < :cutoffDate")
            .SetParameter("cutoffDate", DateTime.Now.AddYears(-1))
            .ExecuteUpdate();
        
        Console.WriteLine($"Deleted {deleteCount} records");
        transaction.Commit();
    }
}
```

## 🏗️ Architecture

**NHibernateSessionFactory**: Factory for creating NHibernate sessions configured for Hydrogen.Data.

**Mapping Configuration**: Supports both fluent and attribute-based entity mappings.

**Query Providers**: LINQ provider and HQL support for flexible querying.

**Session Management**: Automatic session and transaction lifecycle management.

**Provider Abstraction**: Support for multiple database backends through dialect configuration.

## 📋 Best Practices

- **Lazy loading**: Use lazy loading for one-to-many relationships to reduce query complexity
- **N+1 queries**: Use `fetch` in LINQ or `join` in HQL to prevent N+1 query problems
- **Session scope**: Keep sessions short-lived; dispose after work completes
- **Flush strategically**: Call `Flush()` to sync changes; `Clear()` to free memory in batch operations
- **Cache warmly**: Use NHibernate's query cache for frequently executed queries
- **Stateless sessions**: Use stateless sessions for bulk operations without change tracking
- **Mapping clarity**: Define clear cascade and relationship boundaries
- **Performance monitoring**: Enable SQL logging to identify inefficient queries

## 📊 Status & Compatibility

- **Version**: 2.0+
- **Framework**: .NET 5.0+, .NET Framework 4.7+
- **NHibernate Version**: 5.0+
- **Supported Databases**: SQLite, SQL Server, Firebird, PostgreSQL, MySQL, Oracle (via dialect)
- **Performance**: ORM-level optimization with lazy loading and query optimization

## 📦 Dependencies

- **Hydrogen.Data**: Data abstraction layer
- **NHibernate**: ORM framework
- **Database provider**: SQLite, SQL Server, Firebird, etc.
- **.NET Standard 2.1+**: Cross-platform compatibility

## 📚 Related Projects

- [Hydrogen.Data](../Hydrogen.Data) - Core data abstraction layer
- [Hydrogen.Data.Sqlite](../Hydrogen.Data.Sqlite) - SQLite support for NHibernate
- [Hydrogen.Data.MSSQL](../Hydrogen.Data.MSSQL) - SQL Server support for NHibernate
- [Hydrogen.Data.Firebird](../Hydrogen.Data.Firebird) - Firebird support for NHibernate
- [Hydrogen.Tests](../../tests/Hydrogen.Tests) - Test patterns and examples

## 📄 License & Author

**License**: [Refer to repository LICENSE](../../LICENSE)  
**Author**: Herman Schoenfeld, Sphere 10 Software (sphere10.com)  
**Copyright**: © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.
