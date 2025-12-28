# Hydrogen.Data.MSSQL

Microsoft SQL Server implementation for Hydrogen.Data, providing enterprise SQL Server database access.

## 📋 Overview

`Hydrogen.Data.MSSQL` provides SQL Server implementation of the Hydrogen.Data abstraction layer, enabling enterprise-grade database access for large-scale Hydrogen applications.

## 🚀 Key Features

- **SQL Server Integration**: Full MSSQL support
- **Enterprise Scale**: High availability and performance
- **Azure SQL**: Azure SQL Database compatibility
- **Advanced Features**: Computed columns, indexed views, etc.
- **Full-Text Search**: FTS support for text queries
- **Async Operations**: Async queries and connections

## 🔧 Usage

Connect to SQL Server:

```csharp
using Hydrogen.Data.MSSQL;

var connectionString = "Server=.;Database=myapp;Trusted_Connection=true;";
var dataLayer = new MssqlDataLayer(connectionString);
var data = dataLayer.ExecuteQuery("SELECT * FROM Products");
```

## 📦 Dependencies

- **Hydrogen.Data**: Data abstraction layer
- **System.Data.SqlClient** or **Microsoft.Data.SqlClient**: SQL Server provider

## 📄 Related Database Projects

- [Hydrogen.Data.Sqlite](../Hydrogen.Data.Sqlite) - SQLite for embedded scenarios
- [Hydrogen.Data.Firebird](../Hydrogen.Data.Firebird) - Firebird
- [Hydrogen.Data.NHibernate](../Hydrogen.Data.NHibernate) - NHibernate ORM

## 📄 Related Projects

- [Hydrogen.Data](../Hydrogen.Data) - Data abstraction
- [Hydrogen.Windows.Forms.MSSQL](../Hydrogen.Windows.Forms.MSSQL) - Windows Forms SQL Server binding
