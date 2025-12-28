# Hydrogen (core)

Hydrogen is a collection of utilities, data structures, serializers and streaming primitives used throughout the Sphere10 codebase. This README focuses on the core, test-driven features you will most commonly use: collections, serialization, streaming, and a few notable patterns extracted directly from the unit tests.

**Quick orientation**
- Language: C# targeting .NET 8+
- Primary value: well-tested, composable primitives (collections, serialization, streams, crypto)
- Source-of-truth for examples: the unit tests in `tests/*` (examples below mirror test patterns)

**Contents**
- Overview
- Collections (catalog + examples)
- Serialization (SerializerFactory / ItemSerializer patterns)
- Streams & storage primitives (ClusteredStreams, StreamMapped, PagedStream)
- Concurrency primitives (ProducerConsumerQueue, SynchronizedExtendedList)
- Notable utilities (VarInt, Result<T>)

---

**Overview**
Hydrogen exposes:
- Lightweight, high-performance collections (in-memory, stream-backed, transactional)
- A flexible binary serialization framework (`IItemSerializer<T>`, `SerializerFactory`, `SerializerBuilder`)
- Stream primitives tuned for clustered/transactional storage
- Utilities for cryptography, hashing and common helpers

All examples below are intentionally concise and reflect idioms used in the tests.

**Collections — catalog (common types)**
- `ExtendedList<T>` — feature-rich list used across tests and many components.
- `SynchronizedExtendedList<T>` — thread-safe variant of `ExtendedList<T>`.
- `ProducerConsumerQueue<T>` — bounded/unbounded producer-consumer queue with async producers/consumers.
- `TransactionalDictionary<TKey,TValue>` / `TransactionalList<T>` — transactional, stream-backed collections supporting commit/rollback semantics.
- `StreamMapped` collections (`IStreamMappedDictionary`, `StreamMappedList`) — cluster/stream-backed, disk/memory-backed collections.
- `ClusteredStreams` — underlying clustered stream primitive for stream-mapped containers.
- `PagedStream` / `PagedCollection<T>` — paged storage for large datasets.

Collections are heavily exercised by the tests under `tests/Hydrogen.Tests/Collections/**` and `tests/Hydrogen.Tests/ClusteredStreams/**`.

---

**Examples — collections**

ExtendedList and SynchronizedExtendedList (common patterns from tests):

```csharp
var list = new ExtendedList<string> { "a", "b", "c" };
list.Insert(1, "x");
foreach (var s in list) Console.WriteLine(s);

var sync = new SynchronizedExtendedList<int>();
sync.Add(1);
sync.Add(2);
// Safe to enumerate / mutate concurrently from multiple threads in tests
```

ProducerConsumerQueue (producer/consumer test pattern):

```csharp
using var q = new ProducerConsumerQueue<int>(int.MaxValue); // bounded/unbounded
// producers (async)
_ = Task.Run(async () => {
    for (int i = 0; i < 1000; i++) await q.PutAsync(i);
    q.CompleteAdding();
});

// consumer (async) — TakeManyAsync yields batches
await foreach (var batch in q.TakeManyAsync(batchSize: 128)) {
    foreach (var item in batch) Process(item);
}
```

TransactionalDictionary (test-driven transaction pattern):

```csharp
// Create a transactional dictionary (tests create using factory helpers)
using var txDict = new TransactionalDictionary<string, int>(...);
using (var scope = txDict.BeginScope()) {
    using (var txn = scope.BeginTransaction()) {
        txDict.Add("k", 1);
        txn.Commit();
    }
}
// After commit the state can be reloaded by opening the container again (test asserts behavior)
```

Stream-mapped / Clustered containers: the tests show `StreamMappedFactory.CreateDictionary*` and `StreamMappedFactory.CreateList*` to create on-disk or memory-backed containers with `ClusteredStreamsPolicy`.

---

**Serialization — key patterns (tests are authoritative)**
Hydrogen uses `IItemSerializer<T>` implementations and a `SerializerFactory` to register and build serializer hierarchies. Common test-driven patterns:

1) Using `ItemSerializer<T>.Default` for simple types:

```csharp
var s = ItemSerializer<int>.Default;
var bytes = s.Serialize(42);
var n = s.Deserialize(bytes);
```

2) Building a serializer factory and registering custom serializers (reflecting `SerializerFactoryTests`):

```csharp
var factory = new SerializerFactory();
factory.Register(typeof(string), new StringSerializer(SizeDescriptorStrategy.UseVarInt));
factory.Register(typeof(TestObject), new TestObjectSerializer(factory));

// Request serializer for a constructed generic
var listSerializer = factory.GetSerializer(typeof(ExtendedList<TestObject>));
```

3) Reference serializers and shared-object graphs (from tests demonstrating reference preservation):

```csharp
// Make a string serializer behave as a reference serializer
var refStrSerializer = new StringSerializer().AsReferenceSerializer();
factory.Register(typeof(string), refStrSerializer);

// Serializing an object graph with repeated references preserves identity during deserialization
```

4) Polymorphic serialization (tests using KnownSubType/KnownType attributes):

```csharp
// ItemSerializer<Animal>.Default can properly serialize derived types
IItemSerializer<Animal> ser = ItemSerializer<Animal>.Default;
var list = new ExtendedList<Animal> { new Dog("Fido"), new Cat("Mittens") };
var bytes = ser.Serialize(list);
var copy = ser.Deserialize(bytes);
```

5) Custom ItemSerializerBase<T> implementations (example from tests):

```csharp
class TestObjectSerializer : ItemSerializerBase<TestObject> {
    public override void Serialize(ISerializationContext context, TestObject item) {
        StringSerializer.Write(context, item.Name, SizeDescriptorStrategy.UseVarInt);
        // ...other fields
    }
    public override TestObject Deserialize(IDeserializationContext context) {
        var name = StringSerializer.Read(context, SizeDescriptorStrategy.UseVarInt);
        return new TestObject(name);
    }
}
```

These patterns are visible in `tests/Hydrogen.Tests/Serialization/*`.

---

**VarInt / CVarInt — compact integer encoding (from VarIntTests)**
VarInt provides compact encoding for unsigned integers. Tests demonstrate writing to streams, converting to bytes and operator helpers.

```csharp
using (var ms = new MemoryStream()) {
    VarInt.Write(ms, 300);
    ms.Position = 0;
    var v = VarInt.Read(ms); // 300
}

var bytes = VarInt.ToBytes(12345);
var value = VarInt.From(bytes);

// VarInt supports arithmetic operators in tests (helper types)
```

---

**Streams & storage primitives**

ClusteredStreams (concurrency and storage policies): tests under `ClusteredStreams` exercise parallel inserts, parallel reads and mixed operations with different `ClusteredStreamsPolicy` values.

```csharp
using var root = new MemoryStream();
var streams = new ClusteredStreams(root, clusterSize: 4, policy: ClusteredStreamsPolicy.Default, autoLoad: true);
streams.Add(...);
// tests assert correctness after concurrent ops
```

Stream-mapped collections (`StreamMappedFactory.CreateDictionary*`, `CreateList*`) are used to create on-disk, cluster-aware dictionaries and lists. Tests cover `Load()`, persistence across reopen, and policy variations.

PagedStream / PagedCollection are used for large datasets to avoid loading everything into memory.

---

**LevelDB interoperability (Hydrogen.Windows.LevelDB tests)**
The LevelDB wrapper is exercised with basic CRUD + iterator/snapshot examples in tests:

```csharp
using var db = new DB(path);
db.Put(Encoding.UTF8.GetBytes("k"), Encoding.UTF8.GetBytes("v"));
var v = db.Get(Encoding.UTF8.GetBytes("k"));

using (var it = db.CreateIterator()) {
    it.SeekToFirst();
    while (it.IsValid()) {
        var key = it.Key();
        var val = it.Value();
        it.Next();
    }
}
```

Snapshots and repair/destroy patterns are covered in corresponding tests.

---

**Concurrency & test patterns**
- Tests often use `SynchronizedExtendedList<T>` in producer/consumer tests to accumulate results safely across threads.
- `ProducerConsumerQueue<T>` tests show `PutAsync`, `CompleteAdding`, `TakeManyAsync` patterns for efficient batching.

---

**Notable utilities**
- `Result<T>`: used pervasively for operations that may fail; tests use `Result<T>.From(value)` and `.Error(...)` patterns.
- `Tools.Json` helpers are used in tests for quick JSON roundtrips.
- `Tools.Hashing` and `CryptoEx` libraries are demonstrated by ECDSA/ECIES unit tests in `tests/Hydrogen.CryptoEx.Tests`.

---

**Guidance & recommendations (from test behavior)**n- Prefer `ItemSerializer<T>.Default` for primitive types and `SerializerFactory` for composed or open generic types.
- Use reference serializers when you need de-duplication / identity preservation across object graphs.
- For large or disk-backed datasets, prefer `StreamMapped` containers and `ClusteredStreams` with an appropriate `ClusteredStreamsPolicy`.
- Use transactional collections when you need atomic multi-step updates that can be rolled back (tests verify commit/reload behavior).

---

**Where to look next (tests & code locations)**
- Serialization patterns: `tests/Hydrogen.Tests/Serialization/*` (SerializerFactoryTests, PolymorphicSerializerTests)
- VarInt: `tests/Hydrogen.Tests/Values/VarIntTests.cs`
- Clustered / stream-mapped containers: `tests/Hydrogen.Tests/ClusteredStreams/*` and `tests/Hydrogen.Tests/Collections/StreamMapped/*`
- Transactional collections: `tests/Hydrogen.Tests/Collections/Transactional/*`
- Producer/consumer and concurrent list usage: `tests/Hydrogen.Tests/Collections/ProducerConsumerQueueTest.cs`
- LevelDB integration: `tests/Hydrogen.Windows.LevelDB.Tests/LevelDBTests.cs`

---

If you want, I can:
- Expand any example into a complete, buildable snippet.
- Add a short API reference table enumerating constructors and primary methods for each collection type.
- Run a targeted grep across tests to gather one-line usage examples per collection (useful for the README index).

Would you like me to commit this README, or first expand a specific example into a runnable snippet? 
### Serialization
- **Comprehensive Serialization**: Flexible binary serialization with context-aware item serializers and support for checksums
- **Polymorphic Support**: Automatically serialize derived types
- **Custom Serializers**: Register custom serializers for specific types
- **Checksums**: Built-in validation to detect corruption

### Cryptography
- **Enterprise-Grade Cryptography**: Hashing, digital signatures, encryption, key derivation functions
- **Multiple Algorithms**: SHA-256, SHA-512, SHA-3, BLAKE2, and more
- **Digital Signatures**: ECDSA, EdDSA, and other schemes
- **Key Management**: Safe key generation and derivation

### Streaming & Memory
- **Advanced Streaming**: Clustered, transactional, and paged stream implementations
- **Memory Efficiency**: Efficient memory handling with paged structures and reinterpreted arrays
- **Memory Metrics**: Track and analyze memory usage

### Extensions & Utilities
- **50+ Extension Methods**: For strings, collections, types, tasks, streams, and more
- **Async Utilities**: Modern async/await helpers
- **Text Processing**: String formatting, parsing, and manipulation
- **Logging Framework**: Integrated logging with multiple outputs

## 🚀 Quick Start

### Installation

Add via NuGet:
```bash
dotnet add package Hydrogen
```

### Basic Usage

Working with Result types for error handling:

```csharp
using Hydrogen;

// Create a success result
var success = Result<int>.From(42);
if (success) {
    var value = (int)success; // Cast to value
}

// Create an error result
var error = Result<int>.Error("Operation failed", "Additional context");
if (!error) {
    foreach (var message in error.Errors) {
        Console.WriteLine(message);
    }
}

// Serialize to JSON
var json = Tools.Json.WriteToString(success);
var deserialized = Tools.Json.ReadFromString<Result<int>>(json);
```

### Collections

Working with advanced collection types:

```csharp
using Hydrogen;

// Paged collections for large data
var pagedList = new PagedCollection<int>();
pagedList.Add(new[] { 1, 2, 3, 4, 5 });

foreach (var page in pagedList.Pages) {
    foreach (var item in page) {
        Console.WriteLine(item);
    }
}

// Sorted collections with custom comparers
var sortedSet = new SortedDictionary<string, int>();
sortedSet.Add("Charlie", 3);
sortedSet.Add("Alice", 1);
sortedSet.Add("Bob", 2);

// Iterates in sorted order
foreach (var kvp in sortedSet) {
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
}
```

### Hashing

Compute hashes of various data:

```csharp
using Hydrogen;

var data = "Hello, World!";
var hash = Tools.Hashing.SHA256(data);

// Hash file
var fileHash = Tools.Hashing.SHA256File("path/to/file");

// Compute multiple hash types
var md5 = Tools.Hashing.MD5(data);
var sha1 = Tools.Hashing.SHA1(data);
var sha512 = Tools.Hashing.SHA512(data);
```

### Text Extensions

Powerful string utilities:

```csharp
using Hydrogen;

var text = "Hello World";

// Formatting
var padded = text.PadToLength(20);
var truncated = text.Truncate(5);

// Validation
bool isNumeric = "12345".IsNumeric();
bool isAlpha = "abc".IsAlpha();

// Parsing
var parts = "a,b,c".Split(",");
var lines = multilineString.Split(Environment.NewLine);

// Case operations
var camelCase = "hello_world".ToCamelCase();
var pascalCase = "hello_world".ToPascalCase();
```

### Async Utilities

Work with async operations:

```csharp
using Hydrogen;
using System.Threading.Tasks;

// Retry logic
var result = await AsyncTools.RetryAsync(
    () => FaultyOperation(),
    maxAttempts: 3,
    delay: TimeSpan.FromSeconds(1)
);

// Timeout handling
var task = Task.Delay(1000);
var completed = await task.WithTimeout(TimeSpan.FromSeconds(5));

// Task composition
var tasks = new[] { Task1(), Task2(), Task3() };
await Task.WhenAll(tasks);
```

## 📚 Detailed Guides

### Working with Serialization

```csharp
using Hydrogen;

// Custom serializer for a type
var serializer = new IntSerializer();
var bytes = serializer.Serialize(42);
var value = serializer.Deserialize(bytes);

// Polymorphic serialization
var baseObject = (Base)new Derived { Value = 10 };
var serializedBytes = Tools.Serialization.Serialize(baseObject);
var deserialized = Tools.Serialization.Deserialize<Base>(serializedBytes);
```

### Memory Management

```csharp
using Hydrogen;

// Reinterpreted arrays (view one type as another)
var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
var ints = bytes.Reinterpret<int>();  // View bytes as 32-bit integers

// Paged streams for large data
using (var pagedStream = new PagedStream()) {
    pagedStream.Write(largeByteArray);
    pagedStream.Seek(0);
    var data = pagedStream.ReadAllBytes();
}
```

## 🎯 Architecture

The library is organized into functional areas:

| Module | Purpose |
|--------|---------|
| **Collections** | Data structures and collection implementations |
| **Serialization** | Binary and JSON serialization |
| **Crypto** | Hashing, signatures, encryption |
| **Streams** | Advanced streaming and memory-mapped implementations |
| **Text** | String utilities, formatters, parsers |
| **Extensions** | Helper methods for built-in types |
| **Threading** | Async utilities and thread helpers |
| **Logging** | Logging framework and outputs |

## 📦 Dependencies

- .NET 8.0 or higher
- No external dependencies for core functionality
- Optional: BouncyCastle for advanced cryptography
- Optional: Newtonsoft.Json for JSON support

## 🔗 Cross-Platform

Hydrogen is designed for .NET 8.0+ which runs on:
- Windows
- Linux
- macOS
- iOS (via Xamarin)
- Android (via Xamarin)

## 📄 Related Projects

- [Hydrogen.Application](../Hydrogen.Application) - Application framework
- [Hydrogen.Data](../Hydrogen.Data) - Data access layer
- [Hydrogen.Communications](../Hydrogen.Communications) - Networking layer
- [Hydrogen.CryptoEx](../Hydrogen.CryptoEx) - Extended cryptography

- `StringExtensions`: Truncation, case handling, validation, parsing
- `EnumerableExtensions`: Filtering, grouping, transformation
- `TaskExtensions`: Async utilities
- `StreamExtensions`: I/O operations
- `TypeExtensions`: Reflection helpers
- And many more...

### **Streams** (`Hydrogen.Streams`)
Specialized streaming implementations:
- Clustered streams for structured data
- Transactional page buffering
- Memory-based streams
- Stream composition and decoration

### **Memory Management** (`Hydrogen.Memory`)
Efficient memory handling:
- Byte array buffers
- Memory metrics and analysis
- Reinterpreted arrays

### **Threading & Scheduling**
- Thread-safe collections
- Scheduler implementations
- Synchronization utilities

## 🔧 Advanced Features

### Merkle Trees & Spatial Indexing
```csharp
// Integrate Merkle trees for data integrity verification
// Spatial indexing for efficient data location
```

### Clustered Storage
```csharp
// Use clustered streams for optimized read/write operations
// Configurable cluster sizes (default: 4KB)
```

### Lazy Loading & Futures
```csharp
var moduleConfigs = Tools.Values.Future.LazyLoad(() => 
    /* expensive operation */
);
```

## 🎯 Platform Support

- ✅ **.NET 8.0** (primary target)
- ✅ **Windows** (Console, WinForms, Desktop)
- ✅ **Web** (ASP.NET Core)
- ✅ **Mobile** (iOS, Android via Xamarin/MAUI)
- ✅ **macOS**

Platform-specific packages available:
- `Hydrogen.Windows`
- `Hydrogen.Windows.Forms`
- `Hydrogen.Application` (cross-platform app framework)
- `Hydrogen.Communications` (networking)
- `Hydrogen.Web.AspNetCore`

## 📚 Configuration

Default settings can be customized via `HydrogenDefaults`:

```csharp
public static class HydrogenDefaults 
{
    // Streams
    public const int DefaultBufferOperationBlockSize = 32768;
    
    // Clustering
    public const int ClusterSize = 4096; // 4KB
    
    // Hashing
    public const CHF HashFunction = CHF.SHA2_256;
    
    // And more...
}
```

## 🏗️ Architecture

Hydrogen follows clean architecture principles:
- **Modular Design**: Each concern in dedicated namespace/project
- **Extension Methods**: Non-invasive API enhancement
- **Factory & Decorator Patterns**: Flexible object creation and behavior modification
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection integration
- **Thread Safety**: Thread-safe implementations where needed

## 📝 License

Distributed under the **MIT NON-AI License** - see LICENSE file for details.

This license encourages ethical AI development and prevents use in certain AI/ML contexts. More information at [https://sphere10.com/legal/NON-AI-MIT](https://sphere10.com/legal/NON-AI-MIT)

## 🔗 Resources

- **GitHub**: [Sphere10/Framework](https://github.com/Sphere10/Framework)
- **Website**: [sphere10.com](https://sphere10.com)
- **Company**: [Sphere 10 Software Pty Ltd](https://sphere10.com)

## 👨‍💻 Author

**Herman Schoenfeld** - Lead Developer

---

**Version**: 2.0.2
