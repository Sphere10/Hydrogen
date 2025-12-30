<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 💾 Hydrogen.Windows.LevelDB

**LevelDB integration for Windows** providing high-performance embedded key-value storage for blockchain data, indices, and persistent caches.

## 📋 Overview

`Hydrogen.Windows.LevelDB` integrates Google's LevelDB into Hydrogen applications, providing high-performance key-value storage suitable for blockchain ledgers, indices, and persistent caches on Windows platforms.

## 🚀 Key Features

- **LevelDB Integration**: Embedded key-value database
- **Fast Access**: Optimized for read-heavy workloads
- **Compression**: Snappy compression support for reduced storage
- **Batch Operations**: Atomic batch writes
- **Iteration**: Efficient key range iteration with LINQ support
- **Snapshots**: Consistent point-in-time snapshots for concurrent reads
- **Repair & Destroy**: Database maintenance utilities

## 🔧 Usage

### Basic CRUD Operations

Store and retrieve key-value data:

```csharp
using Hydrogen.Windows.LevelDB;

// Create/open database
using (var db = new DB("mydb", new Options { CreateIfMissing = true })) {
	// Write data
	db.Put("Tampa", "green");
	db.Put("London", "red");
	db.Put("New York", "blue");
    
	// Read data
	var value = db.Get("Tampa"); // Returns "green"
    
	// Delete data
	db.Delete("New York");
    
	// Check if exists
	if (db.Get("New York") == null) {
		// Key was deleted
	}
}
```

### Iterating Over Data

Iterate over all key-value pairs (automatically sorted by key):

```csharp
using (var db = new DB("mydb", new Options { CreateIfMissing = true })) {
	// Populate data
	db.Put("Tampa", "green");
	db.Put("London", "red");
	db.Put("New York", "blue");
    
	// Iterate from first key
	using (var iterator = db.CreateIterator(new ReadOptions())) {
		iterator.SeekToFirst();
		while (iterator.IsValid()) {
			var key = iterator.GetStringKey();
			var value = iterator.GetStringValue();
			Console.WriteLine($"{key}: {value}");
			iterator.Next();
		}
	}
    
	// Or use LINQ enumerable (convenient but iterates all keys)
	var allKeys = (from kv in db as IEnumerable<KeyValuePair<string, string>>
				   select kv.Key).ToList();
}
```

### Point-in-Time Snapshots

Create snapshots for consistent reads while data is being modified:

```csharp
using (var db = new DB("mydb", new Options { CreateIfMissing = true })) {
	db.Put("Tampa", "green");
	db.Put("London", "red");
    
	// Create snapshot before making changes
	using (var snapshot = db.CreateSnapshot()) {
		var readOptions = new ReadOptions { Snapshot = snapshot };
        
		// Update data in main database
		db.Put("New York", "blue");
		db.Delete("London");
        
		// Snapshot still sees the old state
		ClassicAssert.AreEqual(db.Get("New York", readOptions), null); // Not yet in snapshot
		ClassicAssert.AreEqual(db.Get("London", readOptions), "red");   // Still visible in snapshot
	}
    
	// Outside snapshot scope, we see the updated data
	ClassicAssert.AreEqual(db.Get("Tampa"), "yellow");      // Visible now
	ClassicAssert.IsNull(db.Get("London"));                  // Deleted
}
```

### Database Maintenance

Repair or destroy databases:

```csharp
// Repair a potentially corrupted database
DB.Repair("mydb", new Options());

// Completely destroy/delete a database
DB.Destroy("mydb", new Options { CreateIfMissing = true });
```

## 📦 Dependencies

- **Hydrogen**: Core framework library
- **LevelDB**: Native LevelDB database engine
- **Snappy**: Compression library

## 💡 Use Cases

- **Blockchain State**: Store blockchain blocks, transactions, and account state
- **Indices**: Create fast lookup indices for transaction history
- **Caching**: Persistent cache layer for frequently accessed data
- **Logging**: High-throughput event and transaction logging

## ⚠️ Important Notes

- Keys are stored in **sorted order** (lexicographic by default)
- **Snapshots don't require locking** - they're copy-on-write
- Always use `using` statements to ensure proper database cleanup
- **Single writer** - only one writer at a time, but multiple concurrent readers are supported

## 📄 Related Projects

- [Hydrogen.Data](../Hydrogen.Data) - Data access abstraction
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core) - Blockchain using storage
