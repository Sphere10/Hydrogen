<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# Hydrogen.Data.NHibernate

NHibernate ORM integration for Hydrogen applications, providing object-relational mapping and advanced data access patterns.

## 📋 Overview

`Hydrogen.Data.NHibernate` integrates NHibernate ORM with Hydrogen.Data, providing a mature object-relational mapping solution for complex data models and advanced query scenarios.

## 🚀 Key Features

- **NHibernate Integration**: Full ORM support
- **Entity Mapping**: Flexible domain model mapping
- **LINQ Support**: LINQ-to-NHibernate queries
- **Lazy Loading**: Deferred loading of relationships
- **Caching**: First and second-level caching
- **Database-Agnostic**: Works with any NHibernate-supported database

## 🔧 Usage

Use NHibernate with Hydrogen:

```csharp
using Hydrogen.Data.NHibernate;

var sessionFactory = HydrogenNHibernateConfiguration.CreateSessionFactory();
using var session = sessionFactory.OpenSession();

var users = session.Query<User>()
    .Where(u => u.IsActive)
    .ToList();
```

## 📦 Dependencies

- **Hydrogen.Data**: Data abstraction layer
- **NHibernate**: ORM framework
- **Hydrogen.Linq**: Linq support

## 📄 Related Database Projects

- [Hydrogen.Data.Sqlite](../Hydrogen.Data.Sqlite) - SQLite
- [Hydrogen.Data.Firebird](../Hydrogen.Data.Firebird) - Firebird
- [Hydrogen.Data.MSSQL](../Hydrogen.Data.MSSQL) - SQL Server

## 📄 Related Projects

- [Hydrogen.Data](../Hydrogen.Data) - Data abstraction layer
