# Hydrogen.Windows.LevelDB

LevelDB integration for Windows applications, providing fast key-value storage for blockchain data and application state.

## Overview
Hydrogen.Windows.LevelDB wraps Google LevelDB with a .NET-friendly API for embedded key-value storage on Windows.

## Key features
- Embedded key-value database with ordered keys
- Fast read access and efficient batch writes
- Snapshots for consistent point-in-time reads
- Iterators for range scans
- Repair and destroy utilities

## Usage

### Basic CRUD

```csharp
using Hydrogen.Windows.LevelDB;

using var db = new DB("mydb", new Options { CreateIfMissing = true });
db.Put("Tampa", "green");
db.Put("London", "red");

var value = db.Get("Tampa"); // "green"
db.Delete("London");
```

### Iteration

```csharp
using var db = new DB("mydb", new Options { CreateIfMissing = true });

using var iterator = db.CreateIterator(new ReadOptions());
iterator.SeekToFirst();
while (iterator.IsValid()) {
    var key = iterator.GetStringKey();
    var val = iterator.GetStringValue();
    Console.WriteLine($"{key}: {val}");
    iterator.Next();
}
```

### Snapshots

```csharp
using var db = new DB("mydb", new Options { CreateIfMissing = true });
db.Put("Tampa", "green");

using var snapshot = db.CreateSnapshot();
var readOptions = new ReadOptions { Snapshot = snapshot };

db.Put("Tampa", "yellow");

var oldValue = db.Get("Tampa", readOptions); // "green"
var newValue = db.Get("Tampa");              // "yellow"
```

### Maintenance

```csharp
DB.Repair("mydb", new Options());
DB.Destroy("mydb", new Options { CreateIfMissing = true });
```

## Dependencies
- Hydrogen
- LevelDB native binaries
- Snappy compression

## Related projects
- [Hydrogen.Data](../Hydrogen.Data)
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core)
