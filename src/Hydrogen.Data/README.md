# Hydrogen.Data

Data access and database abstraction layer with ADO.NET helpers, SQL building, and CSV support.

## Overview
Hydrogen.Data provides consistent APIs for executing queries, managing transactions, and mapping results across multiple database providers.

## Key features
- ADO.NET helpers and extensions
- Query building utilities
- Schema inspection helpers
- CSV import/export utilities
- Provider abstractions for multiple DB engines

## Usage

```csharp
using Hydrogen.Data;

using (var dac = CreateDatabaseScope(DBMSType.SQLite)) {
    dac.Insert("Users", new[] {
        new ColumnValue("ID", 1),
        new ColumnValue("Name", "Alice"),
        new ColumnValue("Email", "alice@example.com")
    });

    var users = dac.Select("Users");

    dac.Update("Users", new[] {
        new ColumnValue("Name", "Alice Smith")
    }, "WHERE ID = 1");

    dac.Delete("Users", "WHERE ID = 1");
}
```

## Database providers
- [Hydrogen.Data.Sqlite](../Hydrogen.Data.Sqlite)
- [Hydrogen.Data.Firebird](../Hydrogen.Data.Firebird)
- [Hydrogen.Data.MSSQL](../Hydrogen.Data.MSSQL)
- [Hydrogen.Data.NHibernate](../Hydrogen.Data.NHibernate)

## Related projects
- [Hydrogen](../Hydrogen)
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core)
