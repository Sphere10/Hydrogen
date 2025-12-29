# Hydrogen

## Project Overview

Hydrogen is a comprehensive .NET utility library that provides low-level building blocks and advanced data structures for high-performance, data-intensive applications. It extends the capabilities of the standard .NET library with specialized collections, cryptographic primitives, stream abstractions, serialization frameworks, and transactional data structures. The library is designed for scenarios requiring fine-grained control over memory layout, persistence strategies, and computational efficiency—particularly in domains like blockchain infrastructure, distributed systems, embedded databases, and high-throughput data processing.

What distinguishes Hydrogen from general-purpose utility libraries is its focus on data structure internals and storage primitives. Rather than providing application-level frameworks, Hydrogen offers foundational components that enable developers to build custom persistence layers, implement merkle-tree-based verification systems, manage clustered stream architectures, and construct transactional collections with ACID guarantees—all while maintaining explicit control over serialization, memory management, and concurrency semantics.

**Quick Orientation**
- **Language**: C# targeting .NET 8+
- **Primary Value**: Well-tested, composable primitives (collections, serialization, streams, crypto)
- **Source-of-Truth**: Unit tests in `tests/*` demonstrate real-world patterns
- **Architecture**: Modular design with clean separation of concerns

## Design Philosophy

### Core Principles

**Composability**: The library is structured around small, focused abstractions that compose predictably. Decorators, adapters, and interfaces allow developers to layer functionality without tight coupling.

**Explicit Control**: Hydrogen favors explicitness over magic. Memory allocation strategies, serialization formats, caching policies, and locking semantics are configurable rather than hidden behind opaque defaults.

**Performance-Conscious**: Many components are optimized for batch operations, memory locality, and reduced allocations. The library provides both in-memory and stream-backed variants of collections to accommodate different performance/capacity tradeoffs.

**Extensibility**: Core abstractions like `IItemSerializer<T>`, `IExtendedList<T>`, and `ITransactionalScope` are designed to be implemented or decorated by user code. The library provides building blocks rather than closed systems.

**Correctness**: Transaction-aware data structures emphasize ACID semantics where applicable. Merkle tree implementations prioritize cryptographic correctness. Thread-safety guarantees are explicit and documented.

### Non-Goals

- **High-level Application Frameworks**: Hydrogen does not provide MVC frameworks, dependency injection containers, or application scaffolding.
- **Platform Abstractions**: The library does not abstract away platform-specific APIs beyond what .NET Standard requires.
- **Opinionated Workflows**: While the library enables patterns like repositories and transactional scopes, it does not enforce specific architectural patterns.

## Domains Covered

### Collections

Hydrogen provides an extensive suite of collection types that extend beyond the standard .NET collections:

- **Extended Lists and Collections**: Interfaces like `IExtendedList<T>` and `IExtendedCollection<T>` support range-based operations (batch reads, writes, insertions, deletions) for improved performance when working with large datasets.
- **Stream-Mapped Collections**: Collections such as `StreamMappedList<T>`, `StreamMappedDictionary<TKey, TValue>`, and `StreamMappedHashSet<T>` persist their data to streams, enabling collections that exceed available memory while maintaining list/dictionary/set semantics.
- **Recyclable Lists**: `IRecyclableList<T>` and its implementations maintain a pool of reusable indices for deleted items, optimizing scenarios with frequent insertions and deletions.
- **Paged Collections**: Both memory-paged and file-paged lists partition data into pages, supporting arbitrarily large datasets with configurable memory footprints.
- **Observable Collections**: Observable variants of standard collections expose events for monitoring mutations, supporting use cases like change tracking and logging.
- **Synchronized Collections**: Thread-safe variants like `SynchronizedExtendedList<T>`, `SynchronizedDictionary<TKey, TValue>`, and `ProducerConsumerQueue<T>` for concurrent scenarios.
- **Specialized Data Structures**: Bloom filters, binary heaps, circular lists, bounded lists, and bidirectional dictionaries address specific algorithmic needs.

### Clustered Streams

The `ClusteredStreams` subsystem provides a sophisticated mechanism for managing multiple logical streams within a single underlying stream. This enables:

- **Multi-Stream Storage**: A single file or stream can host multiple independent logical streams, each with its own lifecycle and addressing.
- **Dynamic Allocation**: Streams grow and shrink dynamically, with clusters allocated and linked as needed.
- **Metadata Storage**: Headers and metadata are stored alongside stream data, supporting features like indexing and merkle-tree integration.
- **Attachments**: Pluggable `IClusteredStreamsAttachment` components allow behaviors like indexing, merkle-tree maintenance, and key storage to be composed declaratively.

This architecture underpins the library's stream-mapped collections and object spaces, providing a flexible foundation for custom persistence schemes.

### Object Spaces

Object spaces abstract the storage and retrieval of typed objects across multiple "dimensions" (logical tables). Key capabilities include:

- **Typed Dimensions**: Each dimension stores objects of a specific type, with configurable serialization and indexing strategies.
- **Indexing**: Automatic or custom indexes accelerate lookups by projected keys. Both unique and non-unique indexes are supported.
- **Merkle-Tree Integration**: Optional merkle-tree attachments provide cryptographic proofs of data integrity.
- **ACID Transactions**: Object spaces can participate in transactional scopes, supporting commit/rollback semantics.

Object spaces are suitable for lightweight embedded databases, event stores, and other scenarios requiring structured persistence without a full database engine.

### Merkle Trees

Hydrogen includes multiple merkle-tree implementations optimized for different use cases:

- **Flat Merkle Trees**: Store all nodes in contiguous memory, optimized for fast random access and proof generation.
- **Simple Merkle Trees**: Lazily compute parent nodes, suitable for smaller trees where memory is less constrained.
- **Long Merkle Trees**: Designed for very large datasets, retaining only sub-root hashes in memory while supporting append and proof operations.
- **Partial Merkle Trees**: Maintain only a subset of nodes, useful for constructing and verifying multi-item proofs.

These implementations integrate with collections, enabling `IMerkleList<T>`, `IMerkleDictionary<TKey, TValue>`, and `IMerkleSet<T>` variants that maintain cryptographic integrity proofs alongside their data.

### Cryptography

The library provides cryptographic primitives and utilities:

- **Hashing**: Wrappers for standard hash functions (SHA-256, SHA-512, SHA-3, etc.) plus specialized implementations like BLAKE2b and MurMur3.
- **Digital Signatures**: Abstractions for signature schemes, including stateless schemes (ECDSA, EdDSA, Schnorr) and post-quantum candidates (W-OTS, W-OTS#, AMS).
- **Key Derivation**: PBKDF2 and custom key derivation functions.
- **Verifiable Random Functions (VRF)**: Primitives for generating cryptographically verifiable random outputs.
- **Data Protection**: Secure memory handling and encryption utilities for sensitive data.

### Serialization

Hydrogen's serialization framework is designed for efficiency, control, and extensibility:

- **Item Serializers**: The `IItemSerializer<T>` abstraction enables custom serialization logic for any type. Serializers can be composed, decorated, and registered in a `SerializerFactory`.
- **Polymorphic Serialization**: Support for serializing and deserializing class hierarchies with type discrimination using `[KnownSubType]` attributes.
- **Reference Handling**: Automatic tracking and resolution of object references and cycles within a serialization context.
- **Constant-Size Serialization**: Specialized serializers for fixed-width data, enabling efficient indexing and random access.
- **Versioning**: Support for versioned serialization strategies, allowing schemas to evolve over time.

The framework integrates deeply with the library's collections and storage primitives, ensuring that persistence strategies are explicit and customizable.

### Transactions

The transactional subsystem provides ACID guarantees for in-memory and file-backed data structures:

- **Transactional Scopes**: `ITransactionalScope` defines a protocol for commit/rollback operations. Context-aware scopes allow nested transactions and isolation.
- **Transactional Collections**: `TransactionalList<T>`, `TransactionalDictionary<TKey, TValue>`, and `TransactionalHashSet<T>` provide ACID semantics over persistent storage.
- **Transactional Streams**: `TransactionalStream` wraps a stream with commit/rollback capabilities, enabling atomic multi-operation updates.
- **File Transactions**: `FileTransaction` and `FileTransactionScope` coordinate file-system operations within a transactional boundary.

These primitives enable building robust, crash-recoverable data stores without relying on external database engines.

### Caching

The caching subsystem offers flexible, policy-driven caching mechanisms:

- **Action Caches**: Populate cache entries on-demand using delegates.
- **Bulk-Fetch Caches**: Refresh all entries in a single operation when any entry becomes stale.
- **Session Caches**: Expire entries based on last-access time, suitable for session-style semantics.
- **Reaping Policies**: Both isolated and pooled reaper implementations manage capacity constraints across single or multiple cache instances.

### Protocol and Communication

The protocol subsystem facilitates structured, bidirectional communication between peers:

- **Protocol Definition**: Define protocols with typed messages, commands, requests, responses, and handshakes.
- **Orchestration**: `ProtocolOrchestrator` manages message dispatch, handshake workflows, and request/response correlation.
- **Extensibility**: Handlers for commands, requests, responses, and handshakes can be implemented via interfaces or delegates.

This framework is suitable for building custom RPC mechanisms, control protocols, or peer-to-peer communication layers.

### Streams and I/O

Hydrogen extends .NET's stream abstractions with specialized implementations:

- **Bounded Streams**: Restrict read/write operations to a defined segment of an underlying stream.
- **Fragmented Streams**: Compose multiple disparate byte fragments into a single logical stream.
- **Extended Memory Streams**: Use `IBuffer` as the backing store instead of a contiguous byte array, enabling arbitrarily large in-memory streams with paging support.
- **Transactional Streams**: Wrap streams with commit/rollback semantics.
- **Decorator Streams**: Read-only, write-only, non-closing, and profiling stream wrappers.

### Logging

A flexible, composable logging framework:

- **Logger Abstractions**: `ILogger` defines a simple, level-based logging interface.
- **Decorators**: Prefix, timestamp, thread-ID, and synchronization decorators compose to customize log output.
- **Sinks**: Built-in loggers for console, debug output, files, and custom delegates.
- **Multicast Logging**: Route log messages to multiple sinks simultaneously.

### Threading and Concurrency

Utilities for managing concurrency and synchronization:

- **Custom Synchronization Primitives**: `ProducerConsumerLock`, `NonReentrantLock`, `FastLock`, and minimal semaphore implementations.
- **Serial Thread Pool**: Execute actions serially on a background thread, with configurable lifecycle policies.
- **Critical Sections**: `Critical<T>` and `CriticalObject` encapsulate objects with lock-based access.
- **Producer-Consumer Queue**: `ProducerConsumerQueue<T>` provides bounded/unbounded thread-safe queuing with async support.

### Scheduling

A job scheduling framework with support for various triggers:

- **Job Definitions**: Wrap actions or async functions as jobs.
- **Schedules**: Interval-based, day-of-week, and day-of-month schedules trigger job execution.
- **Policies**: Control job behavior on failure, completion, and rescheduling.

### Extensions & Utilities

**50+ Extension Methods** covering:
- `StringExtensions`: Truncation, case handling, validation, parsing, formatting
- `EnumerableExtensions`: Filtering, grouping, transformation, batching
- `TaskExtensions`: Async utilities, timeout handling, retry logic
- `StreamExtensions`: I/O operations, reading/writing helpers
- `TypeExtensions`: Reflection helpers, type resolution
- And many more...

## Key Concepts

### Extended Lists

`IExtendedList<T>` extends the standard `IList<T>` interface with range-based operations: `ReadRange`, `UpdateRange`, `InsertRange`, and `RemoveRange`. These methods accept long indices and counts, supporting collections larger than 2GB. Implementations are expected to optimize batch operations rather than iterating element-by-element.

### Stream-Mapped Collections

Stream-mapped collections persist their data to streams using serializers and cluster-based storage. They behave like standard collections but their contents reside on disk (or any stream) rather than entirely in memory. This enables collections to scale beyond available RAM while maintaining familiar APIs.

### Object Streams

`ObjectStream<T>` is a low-level primitive for storing a sequence of serialized objects in a stream, along with metadata and indexes. It underpins stream-mapped collections and object spaces, providing features like:

- Recyclable item slots (deleted items can be reused)
- Pluggable indexes for fast lookups
- Merkle-tree integration for integrity proofs
- Metadata tracking (timestamps, checksums, etc.)

### Serialization Context

`SerializationContext` tracks object references and cycles during serialization/deserialization. When a reference-type object is serialized, the context checks if it has been seen before. If so, a reference marker is emitted rather than re-serializing the object. This enables correct handling of cyclic graphs and repeated references.

### Transactional Scopes

`ITransactionalScope` defines a protocol for ACID transactions:

- `BeginTransaction()`: Start a new transaction.
- `CommitTransaction()`: Persist changes.
- `RollbackTransaction()`: Discard changes.

Context-aware scopes (subclasses of `ContextScope`) track active transactions within the call context, enabling nested transactions and isolation semantics. Transactional collections and streams implement `ITransactionalObject` to participate in these scopes.

### Merkle Coordinates and Proofs

A `MerkleCoordinate` identifies a node within a merkle tree by its level and index. Merkle proofs are represented as sequences of `MerkleNode` instances, which can be verified against a root hash to confirm the presence and position of specific leaves. The library's merkle implementations expose methods for generating proofs and verifying them efficiently.

### Decorators and Adapters

Many components follow the decorator pattern, allowing behavior to be layered:

- `StreamDecorator`: Wrap a stream to add logging, profiling, or transaction support.
- `ListDecorator<T>`: Augment list behavior without reimplementing the entire interface.
- `ItemSerializerDecorator<T>`: Transform serialization logic (e.g., add null-handling or reference-tracking).

Adapters convert between related interfaces (e.g., `IList<T>` to `IExtendedList<T>`) to integrate external code with Hydrogen's abstractions.

## Typical Use Cases

### Well-Suited Scenarios

- **Embedded Databases**: Build lightweight, file-based data stores with ACID transactions, indexing, and querying without a full database engine.
- **Blockchain and Distributed Ledgers**: Merkle-tree primitives and cryptographic utilities simplify integrity verification and proof generation.
- **High-Volume Data Processing**: Stream-backed collections and batch-optimized operations enable processing datasets larger than available memory.
- **Custom Persistence Layers**: Fine-grained control over serialization, storage layout, and transactional semantics.
- **Cryptographic Applications**: Post-quantum signature schemes and VRF implementations support advanced security requirements.
- **Protocol Implementations**: Structured communication frameworks for RPC, control protocols, or peer-to-peer messaging.
- **LevelDB Integration**: Native Windows LevelDB wrapper for high-performance key-value storage.

### Less-Suited Scenarios

- **Simple CRUD Applications**: If standard Entity Framework or Dapper suffice, Hydrogen's low-level primitives may introduce unnecessary complexity.
- **Web APIs with Standard ORMs**: The library does not integrate with ASP.NET or Entity Framework out-of-the-box.
- **UI-Centric Applications**: Hydrogen focuses on data structures and persistence, not UI frameworks or bindings.
- **Rapid Prototyping**: The library's emphasis on explicitness and control trades off against rapid development convenience.

## Architecture Overview

Hydrogen's architecture is organized into largely independent subsystems that compose through well-defined interfaces:

1. **Collections Layer**: Extended list and collection interfaces define the foundation. Implementations range from simple in-memory structures to complex stream-mapped and paged variants.

2. **Storage Layer**: Clustered streams provide the underlying mechanism for multi-stream persistence. Object streams layer serialization, indexing, and metadata on top of clustered streams.

3. **Serialization Framework**: A registry-based system (`SerializerFactory`) maps types to serializers. Serializers compose via decorators for features like null-handling, polymorphism, and reference-tracking.

4. **Transactional Framework**: Transactional scopes coordinate commit/rollback across multiple objects. Collections, streams, and object spaces implement `ITransactionalObject` to participate.

5. **Merkle Trees**: Separate implementations provide different tradeoffs. Merkle-aware collections integrate tree maintenance into their mutation operations.

6. **Utilities and Extensions**: Comparers, operators, logging, threading, and I/O utilities provide cross-cutting functionality without coupling to core abstractions.

Data typically flows from application code through collections or object spaces, which delegate to object streams for persistence. Object streams use clustered streams for storage and serializers for encoding. Transactional scopes coordinate mutations, and merkle trees provide integrity proofs where enabled.

### Configuration

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

## Getting Started

### Installation

Add via NuGet:

```bash
dotnet add package Hydrogen
```

Or reference the compiled assembly directly in your project.

### Basic Examples

**Working with Result Types:**

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

**ExtendedList and SynchronizedExtendedList:**

```csharp
var list = new ExtendedList<string> { "a", "b", "c" };
list.Insert(1, "x");
foreach (var s in list) Console.WriteLine(s);

var sync = new SynchronizedExtendedList<int>();
sync.Add(1);
sync.Add(2);
// Safe to enumerate / mutate concurrently from multiple threads
```

**ProducerConsumerQueue (Producer/Consumer Pattern):**

```csharp
using var q = new ProducerConsumerQueue<int>(int.MaxValue); // bounded/unbounded

// Producers (async)
_ = Task.Run(async () => {
    for (int i = 0; i < 1000; i++) 
        await q.PutAsync(i);
    q.CompleteAdding();
});

// Consumer (async) — TakeManyAsync yields batches
await foreach (var batch in q.TakeManyAsync(batchSize: 128)) {
    foreach (var item in batch) Process(item);
}
```

**Stream-Mapped List:**

```csharp
using Hydrogen;
using System.IO;

var fileStream = new FileStream("data.bin", FileMode.Create, FileAccess.ReadWrite);
var list = new StreamMappedList<string>(
    fileStream,
    ItemSerializer.Default<string>(),
    autoLoad: false
);

list.Add("Hello");
list.Add("World");
list.Save(); // Persist changes to disk

Console.WriteLine(list[0]); // "Hello"
```

**Transactional Dictionary:**

```csharp
using Hydrogen;

// Create a transactional dictionary
using var txDict = new TransactionalDictionary<string, int>(...);
using (var scope = txDict.BeginScope()) {
    using (var txn = scope.BeginTransaction()) {
        txDict.Add("k", 1);
        txn.Commit();
    }
}
// After commit the state is durable
```

**Merkle-Tree List:**

```csharp
using Hydrogen;
using System.Security.Cryptography;

var hasher = new HashAlgorithmAdapter(SHA256.Create());
var merkleList = new MerkleListAdapter<string>(
    new ExtendedList<string>(),
    new ActionHasher<string>(s => hasher.ComputeHash(Encoding.UTF8.GetBytes(s)))
);

merkleList.Add("Item1");
merkleList.Add("Item2");

byte[] rootHash = merkleList.MerkleTree.Root;
var proof = merkleList.MerkleTree.GenerateProof(0); // Proof for first item
```

### Serialization Examples

**Using ItemSerializer<T>.Default for Simple Types:**

```csharp
var s = ItemSerializer<int>.Default;
var bytes = s.Serialize(42);
var n = s.Deserialize(bytes);
```

**Building a Serializer Factory:**

```csharp
var factory = new SerializerFactory();
factory.Register(typeof(string), new StringSerializer(SizeDescriptorStrategy.UseVarInt));
factory.Register(typeof(TestObject), new TestObjectSerializer(factory));

// Request serializer for a constructed generic
var listSerializer = factory.GetSerializer(typeof(ExtendedList<TestObject>));
```

**Reference Serializers and Shared Object Graphs:**

```csharp
// Make a string serializer behave as a reference serializer
var refStrSerializer = new StringSerializer().AsReferenceSerializer();
factory.Register(typeof(string), refStrSerializer);

// Serializing an object graph with repeated references preserves identity
```

**Polymorphic Serialization:**

```csharp
// ItemSerializer<Animal>.Default can properly serialize derived types
IItemSerializer<Animal> ser = ItemSerializer<Animal>.Default;
var list = new ExtendedList<Animal> { new Dog("Fido"), new Cat("Mittens") };
var bytes = ser.Serialize(list);
var copy = ser.Deserialize(bytes);
```

**Custom ItemSerializerBase<T> Implementation:**

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

### VarInt / CVarInt — Compact Integer Encoding

```csharp
using (var ms = new MemoryStream()) {
    VarInt.Write(ms, 300);
    ms.Position = 0;
    var v = VarInt.Read(ms); // 300
}

var bytes = VarInt.ToBytes(12345);
var value = VarInt.From(bytes);
```

### Hashing

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

```csharp
using Hydrogen;

var text = "Hello World";

// Formatting
var padded = text.PadToLength(20);
var truncated = text.Truncate(5);

// Validation
bool isNumeric = "12345".IsNumeric();
bool isAlpha = "abc".IsAlpha();

// Case operations
var camelCase = "hello_world".ToCamelCase();
var pascalCase = "hello_world".ToPascalCase();
```

### LevelDB Integration (Windows)

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

## Extensibility & Customization

### Implementing Custom Serializers

Implement `IItemSerializer<T>` to define serialization logic:

```csharp
public class MyTypeSerializer : IItemSerializer<MyType> {
    public bool SupportsNull => false;
    
    public long CalculateSize(SerializationContext context, MyType item) {
        // Return byte size
    }
    
    public void Serialize(MyType item, EndianBinaryWriter writer, SerializationContext context) {
        // Write to stream
    }
    
    public MyType Deserialize(EndianBinaryReader reader, SerializationContext context) {
        // Read from stream
        return new MyType();
    }
}
```

Register with `SerializerFactory`:

```csharp
SerializerFactory.Default.Register(new MyTypeSerializer());
```

### Decorating Serializers

Add null-handling or reference-tracking:

```csharp
var baseSerializer = new PrimitiveSerializer<int>();
var nullableSerializer = new WithNullSubstitutionSerializer<int>(baseSerializer, -1);
var referencedSerializer = new ReferenceSerializer<MyClass>(baseSerializer);
```

### Custom Indexes on Object Streams

Implement `IProjectionIndex<TItem, TKey>` to define custom indexing logic:

```csharp
public class MyIndex : ProjectionIndexBase<MyType, string> {
    protected override string ProjectKey(MyType item) => item.Name;
    protected override IEqualityComparer<string> Comparer => EqualityComparer<string>.Default;
    
    // Implement abstract members for storage and lookup
}
```

Attach to an `ObjectStream<T>`:

```csharp
var objectStream = new ObjectStream<MyType>(clusteredStreams, serializer);
var index = new MyIndex(objectStream);
objectStream.RegisterIndex(index);
```

### Custom Transactional Scopes

Subclass `TransactionalScopeBase`:

```csharp
public class MyTransactionalScope : TransactionalScopeBase {
    protected override void OnBeginTransaction() {
        // Initialize transaction state
    }
    
    protected override void OnCommitTransaction() {
        // Persist changes
    }
    
    protected override void OnRollbackTransaction() {
        // Discard changes
    }
}
```

### Extending Collections

Subclass `ExtendedListDecorator<T>` or `RangedListBase<T>` to add custom behavior:

```csharp
public class LoggingList<T> : ExtendedListDecorator<T> {
    public LoggingList(IExtendedList<T> inner) : base(inner) { }
    
    public override void Add(T item) {
        Console.WriteLine($"Adding {item}");
        base.Add(item);
    }
}
```

## Threading / Performance / Safety Notes

### Thread Safety

- **Most collections are NOT thread-safe by default.** Use `SynchronizedList<T>`, `SynchronizedDictionary<TKey, TValue>`, `SynchronizedExtendedList<T>`, or wrap with `ConcurrentStream` where concurrent access is required.
- **Transactional scopes are single-threaded.** Transactions are isolated per call context; concurrent transactions require separate scope instances.
- **Caches**: Thread-safety depends on implementation. `SynchronizedRepository<T>` and `SynchronizedLogger` provide synchronized wrappers.
- **ProducerConsumerQueue<T>**: Fully thread-safe for concurrent producers and consumers.

### Performance Considerations

- **Range Operations**: Prefer `ReadRange`, `UpdateRange`, and `InsertRange` over element-by-element operations for large datasets.
- **Paging Configuration**: Tune page sizes and in-memory page counts for memory-paged and file-paged collections based on access patterns.
- **Serialization**: Constant-size serializers enable efficient random access. Variable-size serializers require sequential scans.
- **Merkle Trees**: Flat merkle trees optimize for proof generation but consume O(n log n) memory. Long merkle trees trade memory for computation.
- **ClusteredStreams**: Default cluster size is 4KB; adjust based on typical object sizes and access patterns.

### Safety Constraints

- **Stream Position Management**: Many components assume exclusive control over stream position. Concurrent stream access without synchronization is unsafe.
- **Serialization Context**: Contexts are NOT thread-safe. Use separate contexts per thread or synchronize access.
- **Disposal**: Disposable resources (streams, transactions, scopes) must be disposed to release locks and persist changes. Use `using` statements.

## Status & Maturity

Hydrogen is a mature library that has evolved over multiple years. Core subsystems (collections, serialization, transactions, merkle trees) are stable and production-tested. Some components (post-quantum cryptography, protocol orchestration) may be less battle-tested and should be evaluated carefully for production use.

### Compatibility

- **Target Framework**: .NET 8.0+ (primary), with support for .NET Standard 2.0 where applicable
- **Backward Compatibility**: The library does not guarantee API stability across major versions. Serialization formats may evolve, requiring migration strategies for persistent data.
- **Platform Support**: Windows, Linux, macOS, iOS (via Xamarin/MAUI), Android (via Xamarin/MAUI)

### Experimental Components

- Post-quantum signature schemes (W-OTS, AMS) are reference implementations. Audit and validate before use in production cryptographic systems.
- Some advanced merkle-tree variants and indexing strategies are optimized for specific use cases and may have edge-case limitations.

### Platform-Specific Packages

- `Hydrogen.Windows` - Windows-specific utilities
- `Hydrogen.Windows.Forms` - WinForms integration
- `Hydrogen.Windows.LevelDB` - Native LevelDB wrapper
- `Hydrogen.Application` - Cross-platform application framework
- `Hydrogen.Communications` - Networking and protocol layers
- `Hydrogen.Web.AspNetCore` - ASP.NET Core integration
- `Hydrogen.CryptoEx` - Extended cryptography (ECDSA, ECIES, etc.)

## Where to Look Next

### Tests & Code Locations

The unit tests serve as comprehensive examples of real-world usage patterns:

- **Serialization**: `tests/Hydrogen.Tests/Serialization/*` (SerializerFactoryTests, PolymorphicSerializerTests)
- **VarInt**: `tests/Hydrogen.Tests/Values/VarIntTests.cs`
- **Clustered/Stream-Mapped**: `tests/Hydrogen.Tests/ClusteredStreams/*` and `tests/Hydrogen.Tests/Collections/StreamMapped/*`
- **Transactional Collections**: `tests/Hydrogen.Tests/Collections/Transactional/*`
- **Producer/Consumer**: `tests/Hydrogen.Tests/Collections/ProducerConsumerQueueTest.cs`
- **LevelDB Integration**: `tests/Hydrogen.Windows.LevelDB.Tests/LevelDBTests.cs`
- **Extended Lists**: `tests/Hydrogen.Tests/Collections/Lists/ExtendedListTests.cs`
- **Result<T>**: `tests/Hydrogen.Tests/Misc/ResultTests.cs`

## Dependencies

- **.NET 8.0** or higher (primary target)
- **No external dependencies** for core functionality
- **Optional**: BouncyCastle for advanced cryptography
- **Optional**: Newtonsoft.Json for JSON support
- **Optional**: Microsoft.Extensions.DependencyInjection for DI integration

## License

Distributed under the **MIT NON-AI License**.

This license encourages ethical AI development and prevents use in certain AI/ML contexts without explicit permission. See the LICENSE file for full details.

More information: [https://sphere10.com/legal/NON-AI-MIT](https://sphere10.com/legal/NON-AI-MIT)

## Resources

- **GitHub**: [Sphere10/Hydrogen](https://github.com/Sphere10/Hydrogen)
- **Website**: [sphere10.com](https://sphere10.com)
- **Company**: Sphere 10 Software Pty Ltd

## Author

**Herman Schoenfeld** - Lead Developer

---

**Version**: 2.0.2