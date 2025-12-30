<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 🧪 Hydrogen

**Lead Developer**: Herman Schoenfeld  
**Copyright**: © 2018-Present Herman Schoenfeld & Sphere 10 Software  
**License**: MIT NON-AI  
**Status**: Production-Ready (v2.0.2)

---

## 🚀 Quick Navigation

### Core Hydrogen Library Domains

**Data Structures & Collections**
- 📦 [Collections/](Collections/) – Extended lists, stream-mapped, recyclable, paged, observable, synchronized variants
- 🌳 [Merkle/](Merkle/) – Merkle-tree implementations: flat, simple, long, partial
- 🔗 [ClusteredStreams/](ClusteredStreams/) – Multi-stream storage, attachments, dynamic allocation

**Persistence & Serialization**
- 💾 [Serialization/](Serialization/) – Item serializers, polymorphism, references, versioning
- 📊 [ObjectSpaces/](ObjectSpaces/) – Typed dimensions, indexing, integrity proofs, transactions
- 🔄 [ObjectStream/](ObjectStream/) – Stream-backed object storage with indexes

**Transactions & Scoping**
- 🎯 [Scopes/](Scopes/) – Transactional boundaries, commit/rollback, isolation levels
- 💳 [Transactions/](Transactions/) – Transaction management and coordination

**Cryptography & Security**
- 🔐 [Crypto/](Crypto/) – Hashing, signatures, key derivation, VRF algorithms
- 🔑 [Protocol/](Protocol/) – Protocol abstractions and messaging

**Streams & I/O**
- 📝 [Streams/](Streams/) – Stream decorators, bounded, fragmented, extended memory
- 📄 [IO/](IO/) – File I/O operations and utilities

**Utilities & Extensions**
- ✂️ [Text/](Text/) – String extensions, formatting, case operations, validation
- 🔤 [Encoding/](Encoding/) – VarInt, CVarInt, compact integer encoding
- 🧠 [Memory/](Memory/) – Buffer operations, memory allocation, management
- ⚡ [Threading/](Threading/) – Synchronization, producer-consumer patterns
- 💾 [Cache/](Cache/) – Action caches, reaping policies, session management
- 🗄️ [Repository/](Repository/) – Repository pattern implementations
- 🔍 [Mapping/](Mapping/) – Object mapping and transformation
- 🎲 [Conversion/](Conversion/) – Type conversions and parsing
- 📐 [Maths/](Maths/) – Mathematical utilities
- 📅 [Values/](Values/) – Value types and structures
- 🏗️ [Comparers/](Comparers/) – Comparison and equality implementations

**Type System & Reflection**
- 🔎 [Introspection/](Introspection/) – Reflection and type analysis
- 📦 [Types/](Types/) – Type utilities and checks
- 🏷️ [Attributes/](Attributes/) – Custom attributes
- 📋 [Framework/](Framework/) – Framework abstractions
- 🔌 [Extensions/](Extensions/) – Framework extension methods

**Data & Resources**
- 🌍 [DataSource/](DataSource/) – Data source abstractions
- 🔌 [Peripherals/](Peripherals/) – Hardware peripherals access
- 🌐 [Network/](Network/) – Network utilities
- 🎯 [Environment/](Environment/) – Environment and system information
- ⚙️ [Misc/](Misc/) – Miscellaneous utilities
- 📦 [Objects/](Objects/) – Object utilities
- 🎛️ [Filter/](Filter/) – Filtering abstractions
- 📊 [Functional/](Functional/) – Functional programming utilities
- ♻️ [Disposables/](Disposables/) – Disposable pattern utilities
- 🔗 [Events/](Events/) – Event handling utilities
- ⚠️ [Exceptions/](Exceptions/) – Exception handling
- 📖 [Loadable/](Loadable/) – Loadable resource abstractions
- 📝 [Logging/](Logging/) – Logging utilities
- 💾 [Persistable/](Persistable/) – Persistence abstractions
- 🔧 [Saveable/](Saveable/) – Save/load patterns
- ⏰ [Scheduler/](Scheduler/) – Task scheduling
- 📏 [Sizing/](Sizing/) – Size calculations and measurements
- 🎯 [Spans/](Spans/) – Span and memory utilities
- 📝 [TextWriters/](TextWriters/) – Text writing abstractions

### Related Projects

**Data Access & Persistence**
- [Hydrogen.Data](../Hydrogen.Data/) – Database abstraction layer (SQL Server, SQLite, Firebird)
- [Hydrogen.Data.Sqlite](../Hydrogen.Data.Sqlite/) – SQLite-specific integration
- [Hydrogen.Data.MSSQL](../Hydrogen.Data.MSSQL/) – SQL Server integration
- [Hydrogen.Data.Firebird](../Hydrogen.Data.Firebird/) – Firebird integration
- [Hydrogen.Data.NHibernate](../Hydrogen.Data.NHibernate/) – NHibernate ORM integration
- [Hydrogen.Windows.LevelDB](../Hydrogen.Windows.LevelDB/) – High-performance embedded key-value store

**Networking & Communications**
- [Hydrogen.Communications](../Hydrogen.Communications/) – Multi-protocol networking (RPC, TCP, UDP, WebSockets)
- [Hydrogen.Web.AspNetCore](../Hydrogen.Web.AspNetCore/) – ASP.NET Core integration and middleware

**Cryptography & Consensus**
- [Hydrogen.CryptoEx](../Hydrogen.CryptoEx/) – Extended cryptography (ECDSA, ECIES, Schnorr, post-quantum)
- [Hydrogen.Consensus](../Hydrogen.Consensus/) – Blockchain consensus mechanisms

**Desktop & Platform Integration**
- [Hydrogen.Windows](../Hydrogen.Windows/) – Windows utilities (registry, services, events, security)
- [Hydrogen.Windows.Forms](../Hydrogen.Windows.Forms/) – WinForms component integration
- [Hydrogen.Windows.Forms.Sqlite](../Hydrogen.Windows.Forms.Sqlite/) – SQLite integration for WinForms
- [Hydrogen.Windows.Forms.MSSQL](../Hydrogen.Windows.Forms.MSSQL/) – SQL Server integration for WinForms
- [Hydrogen.Windows.Forms.Firebird](../Hydrogen.Windows.Forms.Firebird/) – Firebird integration for WinForms
- [Hydrogen.Drawing](../Hydrogen.Drawing/) – Graphics and drawing utilities

**Application Framework**
- [Hydrogen.Application](../Hydrogen.Application/) – Lifecycle management, DI, configuration
- [Hydrogen.Application.Settings](../Hydrogen.Application.Settings/) – Persistent settings management

**Testing & Quality**
- [Hydrogen.NUnit](../Hydrogen.NUnit/) – NUnit testing utilities
- [Hydrogen.NUnit.DB](../Hydrogen.NUnit.DB/) – Database testing utilities

**Cross-Platform & Generators**
- [Hydrogen.NET](../Hydrogen.NET/) – .NET platform abstractions
- [Hydrogen.NETCore](../Hydrogen.NETCore/) – .NET Core-specific implementations
- [Hydrogen.Android](../Hydrogen.Android/) – Android platform integration
- [Hydrogen.iOS](../Hydrogen.iOS/) – iOS platform integration
- [Hydrogen.macOS](../Hydrogen.macOS/) – macOS platform integration
- [Hydrogen.Generators](../Hydrogen.Generators/) – Code generation utilities

**Blockchain & DApps**
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core/) – Decentralized app core functionality
- [Hydrogen.DApp.Host](../Hydrogen.DApp.Host/) – DApp hosting infrastructure
- [Hydrogen.DApp.Node](../Hydrogen.DApp.Node/) – DApp node implementation

### Test Suites

**Core Library Tests**
- [Hydrogen.Tests](../../tests/Hydrogen.Tests/) – Comprehensive test suite for all Hydrogen domains

**Related Project Tests**
- [Hydrogen.Communications.Tests](../../tests/Hydrogen.Communications.Tests/) – Networking and protocol tests
- [Hydrogen.CryptoEx.Tests](../../tests/Hydrogen.CryptoEx.Tests/) – Extended cryptography tests
- [Hydrogen.Data.Tests](../../tests/Hydrogen.Data.Tests/) – Database integration tests
- [Hydrogen.Windows.Tests](../../tests/Hydrogen.Windows.Tests/) – Windows utilities tests
- [Hydrogen.Windows.LevelDB.Tests](../../tests/Hydrogen.Windows.LevelDB.Tests/) – LevelDB integration tests
- [Hydrogen.DApp.Core.Tests](../../tests/Hydrogen.DApp.Core.Tests/) – DApp core tests
- [Hydrogen.NET.Tests](../../tests/Hydrogen.NET.Tests/) – .NET platform tests
- [Hydrogen.NETCore.Tests](../../tests/Hydrogen.NETCore.Tests/) – .NET Core tests
- [HashLib4CSharp.Tests](../../tests/HashLib4CSharp.Tests/) – Cryptographic hash tests

---

## 📋 Project Overview

Hydrogen is a **low-level, high-performance .NET utility library** providing composable data structures and persistence primitives. It excels at scenarios requiring fine-grained control over memory, serialization, and transactional semantics—think blockchain systems, embedded databases, high-volume analytics, and custom storage layers.

Unlike general-purpose libraries, Hydrogen doesn't provide application frameworks or abstractions. Instead, it offers:

- **50+ collection types** (extended lists, stream-mapped, paged, recyclable, merkle-aware)
- **Advanced serialization framework** (polymorphism, references, versioning, constant-size encoding)
- **Transactional ACID primitives** (scopes, streams, collections with commit/rollback)
- **Merkle-tree implementations** (flat, simple, long, partial) for integrity proofs
- **Cryptographic utilities** (hashing, signatures, key derivation, VRF, post-quantum schemes)
- **Clustered streams** (multi-stream storage, dynamic allocation, attachments)
- **Thread-safe concurrent collections** and producer-consumer patterns
- **50+ string, enumerable, task, stream, and type extension methods**

**Key Attributes**
- **Language**: C# targeting .NET 8+ (with .NET Standard 2.0 compatibility where applicable)
- **Dependencies**: Zero external dependencies for core functionality (optional: BouncyCastle, Newtonsoft.Json)
- **Platform Support**: Windows, Linux, macOS, iOS, Android
- **Philosophy**: Composable, explicit, performance-conscious, extensible, correct
- **Tests**: [Comprehensive test suite](../../tests/Hydrogen.Tests/) with 25+ subsystems and 2000+ tests
- **Maturity**: Production-ready (v2.0.2) with battle-tested core subsystems

## 🎨 Design Philosophy

### Core Principles

**Composability**: The library is structured around small, focused abstractions that compose predictably. Decorators, adapters, and interfaces allow developers to layer functionality without tight coupling.

**Explicit Control**: Hydrogen favors explicitness over magic. Memory allocation strategies, serialization formats, caching policies, and locking semantics are configurable rather than hidden behind opaque defaults.

**Performance-Conscious**: Many components are optimized for batch operations, memory locality, and reduced allocations. The library provides both in-memory and stream-backed variants of collections to accommodate different performance/capacity tradeoffs.

**Extensibility**: Core abstractions like `IItemSerializer<T>`, `IExtendedList<T>`, and `ITransactionalScope` are designed to be implemented or decorated by user code. The library provides building blocks rather than closed systems.

**Correctness**: Transaction-aware data structures emphasize ACID semantics where applicable. Merkle tree implementations prioritize cryptographic correctness. Thread-safety guarantees are explicit and documented.

### ❌ Non-Goals

- **High-level Application Frameworks**: Hydrogen does not provide MVC frameworks, dependency injection containers, or application scaffolding.
- **Platform Abstractions**: The library does not abstract away platform-specific APIs beyond what .NET Standard requires.
- **Opinionated Workflows**: While the library enables patterns like repositories and transactional scopes, it does not enforce specific architectural patterns.

## 🗂️ Domains Covered

### Collections

Hydrogen provides an extensive suite of collection types that extend beyond the standard .NET collections:

- **Extended Lists and Collections**: Interfaces like `IExtendedList<T>` and `IExtendedCollection<T>` support range-based operations (batch reads, writes, insertions, deletions) for improved performance when working with large datasets.
- **Stream-Mapped Collections**: Collections such as `StreamMappedList<T>`, `StreamMappedDictionary<TKey, TValue>`, and `StreamMappedHashSet<T>` persist their data to streams, enabling collections that exceed available memory while maintaining list/dictionary/set semantics.
- **Recyclable Lists**: `IRecyclableList<T>` and its implementations maintain a pool of reusable indices for deleted items, optimizing scenarios with frequent insertions and deletions.
- **Paged Collections**: Both memory-paged and file-paged lists partition data into pages, supporting arbitrarily large datasets with configurable memory footprints.
- **Observable Collections**: Observable variants of standard collections expose events for monitoring mutations, supporting use cases like change tracking and logging.
- **Synchronized Collections**: Thread-safe variants like `SynchronizedExtendedList<T>`, `SynchronizedDictionary<TKey, TValue>`, and `ProducerConsumerQueue<T>` for concurrent scenarios.
- **Specialized Data Structures**: Bloom filters, binary heaps, circular lists, bounded lists, and bidirectional dictionaries address specific algorithmic needs.

### 🔗 Clustered Streams

The `ClusteredStreams` subsystem provides a sophisticated mechanism for managing multiple logical streams within a single underlying stream. This enables:

- **Multi-Stream Storage**: A single file or stream can host multiple independent logical streams, each with its own lifecycle and addressing.
- **Dynamic Allocation**: Streams grow and shrink dynamically, with clusters allocated and linked as needed.
- **Metadata Storage**: Headers and metadata are stored alongside stream data, supporting features like indexing and merkle-tree integration.
- **Attachments**: Pluggable `IClusteredStreamsAttachment` components allow behaviors like indexing, merkle-tree maintenance, and key storage to be composed declaratively.

This architecture underpins the library's stream-mapped collections and object spaces, providing a flexible foundation for custom persistence schemes.

### 🌌 Object Spaces

Object spaces abstract the storage and retrieval of typed objects across multiple "dimensions" (logical tables). Key capabilities include:

- **Typed Dimensions**: Each dimension stores objects of a specific type, with configurable serialization and indexing strategies.
- **Indexing**: Automatic or custom indexes accelerate lookups by projected keys. Both unique and non-unique indexes are supported.
- **Merkle-Tree Integration**: Optional merkle-tree attachments provide cryptographic proofs of data integrity.
- **ACID Transactions**: Object spaces can participate in transactional scopes, supporting commit/rollback semantics.

Object spaces are suitable for lightweight embedded databases, event stores, and other scenarios requiring structured persistence without a full database engine.

### 🌳 Merkle Trees

Hydrogen includes multiple merkle-tree implementations optimized for different use cases:

- **Flat Merkle Trees**: Store all nodes in contiguous memory, optimized for fast random access and proof generation.
- **Simple Merkle Trees**: Lazily compute parent nodes, suitable for smaller trees where memory is less constrained.
- **Long Merkle Trees**: Designed for very large datasets, retaining only sub-root hashes in memory while supporting append and proof operations.
- **Partial Merkle Trees**: Maintain only a subset of nodes, useful for constructing and verifying multi-item proofs.

These implementations integrate with collections, enabling `IMerkleList<T>`, `IMerkleDictionary<TKey, TValue>`, and `IMerkleSet<T>` variants that maintain cryptographic integrity proofs alongside their data.

### 🔐 Cryptography

The library provides cryptographic primitives and utilities:

- **Hashing**: Wrappers for standard hash functions (SHA-256, SHA-512, SHA-3, etc.) plus specialized implementations like BLAKE2b and MurMur3.
- **Digital Signatures**: Abstractions for signature schemes, including stateless schemes (ECDSA, EdDSA, Schnorr) and post-quantum candidates (W-OTS, W-OTS#, AMS).
- **Key Derivation**: PBKDF2 and custom key derivation functions.
- **Verifiable Random Functions (VRF)**: Primitives for generating cryptographically verifiable random outputs.
- **Data Protection**: Secure memory handling and encryption utilities for sensitive data.

### 📦 Serialization

Hydrogen's serialization framework is designed for efficiency, control, and extensibility:

- **Item Serializers**: The `IItemSerializer<T>` abstraction enables custom serialization logic for any type. Serializers can be composed, decorated, and registered in a `SerializerFactory`.
- **Polymorphic Serialization**: Support for serializing and deserializing class hierarchies with type discrimination using `[KnownSubType]` attributes.
- **Reference Handling**: Automatic tracking and resolution of object references and cycles within a serialization context.
- **Constant-Size Serialization**: Specialized serializers for fixed-width data, enabling efficient indexing and random access.
- **Versioning**: Support for versioned serialization strategies, allowing schemas to evolve over time.

The framework integrates deeply with the library's collections and storage primitives, ensuring that persistence strategies are explicit and customizable.

### ⚙️ Transactions

The transactional subsystem provides ACID guarantees for in-memory and file-backed data structures:

- **Transactional Scopes**: `ITransactionalScope` defines a protocol for commit/rollback operations. Context-aware scopes allow nested transactions and isolation.
- **Transactional Collections**: `TransactionalList<T>`, `TransactionalDictionary<TKey, TValue>`, and `TransactionalHashSet<T>` provide ACID semantics over persistent storage.
- **Transactional Streams**: `TransactionalStream` wraps a stream with commit/rollback capabilities, enabling atomic multi-operation updates.
- **File Transactions**: `FileTransaction` and `FileTransactionScope` coordinate file-system operations within a transactional boundary.

These primitives enable building robust, crash-recoverable data stores without relying on external database engines.

### 💾 Caching

The caching subsystem offers flexible, policy-driven caching mechanisms:

- **Action Caches**: Populate cache entries on-demand using delegates.
- **Bulk-Fetch Caches**: Refresh all entries in a single operation when any entry becomes stale.
- **Session Caches**: Expire entries based on last-access time, suitable for session-style semantics.
- **Reaping Policies**: Both isolated and pooled reaper implementations manage capacity constraints across single or multiple cache instances.

### 📡 Protocol and Communication

The protocol subsystem facilitates structured, bidirectional communication between peers:

- **Protocol Definition**: Define protocols with typed messages, commands, requests, responses, and handshakes.
- **Orchestration**: `ProtocolOrchestrator` manages message dispatch, handshake workflows, and request/response correlation.
- **Extensibility**: Handlers for commands, requests, responses, and handshakes can be implemented via interfaces or delegates.

This framework is suitable for building custom RPC mechanisms, control protocols, or peer-to-peer communication layers.

### 📲 Streams and I/O

Hydrogen extends .NET's stream abstractions with specialized implementations:

- **Bounded Streams**: Restrict read/write operations to a defined segment of an underlying stream.
- **Fragmented Streams**: Compose multiple disparate byte fragments into a single logical stream.
- **Extended Memory Streams**: Use `IBuffer` as the backing store instead of a contiguous byte array, enabling arbitrarily large in-memory streams with paging support.
- **Transactional Streams**: Wrap streams with commit/rollback semantics.
- **Decorator Streams**: Read-only, write-only, non-closing, and profiling stream wrappers.

### 📝 Logging

A flexible, composable logging framework:

- **Logger Abstractions**: `ILogger` defines a simple, level-based logging interface.
- **Decorators**: Prefix, timestamp, thread-ID, and synchronization decorators compose to customize log output.
- **Sinks**: Built-in loggers for console, debug output, files, and custom delegates.
- **Multicast Logging**: Route log messages to multiple sinks simultaneously.

### 🔄 Threading and Concurrency

Utilities for managing concurrency and synchronization:

- **Custom Synchronization Primitives**: `ProducerConsumerLock`, `NonReentrantLock`, `FastLock`, and minimal semaphore implementations.
- **Serial Thread Pool**: Execute actions serially on a background thread, with configurable lifecycle policies.
- **Critical Sections**: `Critical<T>` and `CriticalObject` encapsulate objects with lock-based access.
- **Producer-Consumer Queue**: `ProducerConsumerQueue<T>` provides bounded/unbounded thread-safe queuing with async support.

### ⏰ Scheduling

A job scheduling framework with support for various triggers:

- **Job Definitions**: Wrap actions or async functions as jobs.
- **Schedules**: Interval-based, day-of-week, and day-of-month schedules trigger job execution.
- **Policies**: Control job behavior on failure, completion, and rescheduling.

### 🛠️ Extensions & Utilities

**50+ Extension Methods** covering:
- `StringExtensions`: Truncation, case handling, validation, parsing, formatting
- `EnumerableExtensions`: Filtering, grouping, transformation, batching
- `TaskExtensions`: Async utilities, timeout handling, retry logic
- `StreamExtensions`: I/O operations, reading/writing helpers
- `TypeExtensions`: Reflection helpers, type resolution
- And many more...

## 💡 Key Concepts

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

## 🎯 Typical Use Cases

### Well-Suited Scenarios

- **Embedded Databases**: Build lightweight, file-based data stores with ACID transactions, indexing, and querying without a full database engine.
- **Blockchain and Distributed Ledgers**: Merkle-tree primitives and cryptographic utilities simplify integrity verification and proof generation.
- **High-Volume Data Processing**: Stream-backed collections and batch-optimized operations enable processing datasets larger than available memory.
- **Custom Persistence Layers**: Fine-grained control over serialization, storage layout, and transactional semantics.
- **Cryptographic Applications**: Post-quantum signature schemes and VRF implementations support advanced security requirements.
- **Protocol Implementations**: Structured communication frameworks for RPC, control protocols, or peer-to-peer messaging.
- **LevelDB Integration**: Native Windows LevelDB wrapper for high-performance key-value storage.

### 📊 Less-Suited Scenarios

- **Simple CRUD Applications**: If standard Entity Framework or Dapper suffice, Hydrogen's low-level primitives may introduce unnecessary complexity.
- **Web APIs with Standard ORMs**: The library does not integrate with ASP.NET or Entity Framework out-of-the-box.
- **UI-Centric Applications**: Hydrogen focuses on data structures and persistence, not UI frameworks or bindings.
- **Rapid Prototyping**: The library's emphasis on explicitness and control trades off against rapid development convenience.

## 🏗️ Architecture Overview

Hydrogen's architecture is organized into largely independent subsystems that compose through well-defined interfaces:

1. **Collections Layer**: Extended list and collection interfaces define the foundation. Implementations range from simple in-memory structures to complex stream-mapped and paged variants.

2. **Storage Layer**: Clustered streams provide the underlying mechanism for multi-stream persistence. Object streams layer serialization, indexing, and metadata on top of clustered streams.

3. **Serialization Framework**: A registry-based system (`SerializerFactory`) maps types to serializers. Serializers compose via decorators for features like null-handling, polymorphism, and reference-tracking.

4. **Transactional Framework**: Transactional scopes coordinate commit/rollback across multiple objects. Collections, streams, and object spaces implement `ITransactionalObject` to participate.

5. **Merkle Trees**: Separate implementations provide different tradeoffs. Merkle-aware collections integrate tree maintenance into their mutation operations.

6. **Utilities and Extensions**: Comparers, operators, logging, threading, and I/O utilities provide cross-cutting functionality without coupling to core abstractions.

Data typically flows from application code through collections or object spaces, which delegate to object streams for persistence. Object streams use clustered streams for storage and serializers for encoding. Transactional scopes coordinate mutations, and merkle trees provide integrity proofs where enabled.

### ⚡ Configuration

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

## 🚀 Getting Started

### Installation

**NuGet Packages:**

```bash
# Core library
dotnet add package Hydrogen

# Platform-specific (optional)
dotnet add package Hydrogen.Windows              # Windows utilities
dotnet add package Hydrogen.Windows.Forms        # WinForms integration
dotnet add package Hydrogen.Windows.LevelDB      # Native LevelDB wrapper
dotnet add package Hydrogen.CryptoEx             # Extended cryptography (ECDSA, ECIES)
dotnet add package Hydrogen.Communications       # Networking & protocols
dotnet add package Hydrogen.Web.AspNetCore       # ASP.NET Core integration
```

Or reference compiled assemblies directly in your project.

### 10-Second Example

```csharp
using Hydrogen;

// Extended list with range operations
var list = new ExtendedList<int> { 1, 2, 3 };
list.InsertRange(1, new[] { 10, 20 });  // Batch insert at index 1
Console.WriteLine(string.Join(", ", list));  // 1, 10, 20, 2, 3

// Stream-backed list (persists to disk)
var fileStream = new FileStream("data.bin", FileMode.Create, FileAccess.ReadWrite);
var streamList = new StreamMappedList<string>(fileStream, ItemSerializer.Default<string>());
streamList.Add("Hello");
streamList.Save();

// Transactional dictionary
var txDict = new TransactionalDictionary<string, int>();
using (var scope = txDict.BeginScope()) {
    using (var txn = scope.BeginTransaction()) {
        txDict["key"] = 42;
        txn.Commit();  // Persists atomically
    }
}
```

### 💻 Collections Examples

**Extended Lists with Range Operations:**

```csharp
using Hydrogen;

// In-memory extended list with batch operations
var list = new ExtendedList<int>();
list.InsertRange(0, new[] { 1, 2, 3 });     // Batch insert
list.UpdateRange(1, new[] { 20, 30 });      // Batch update
var chunk = list.ReadRange(0, 2);           // Batch read
list.RemoveRange(1, 2);                     // Batch remove

// Thread-safe variant
var syncList = new SynchronizedExtendedList<string>();
syncList.Add("Safe");
syncList.Add("from");
syncList.Add("multiple");
syncList.Add("threads");
```

**Paged Collections (Memory or File-Backed):**

```csharp
// Paged list with 4 in-memory pages, 256 items per page
// Spills to disk automatically as needed
var pagedList = new MemoryPagedList<MyObject>(
    pageCount: 4,
    itemsPerPage: 256,
    itemSerializer: ItemSerializer.Default<MyObject>()
);

// Or file-paged variant for explicit disk storage
var filePagedList = new FilePagedList<MyObject>(
    stream,
    pageSize: 4096,
    itemSerializer: ItemSerializer.Default<MyObject>()
);
```

**Error Handling with Result<T>:**

```csharp
// Success case
var success = Result<int>.From(42);
if (success) {
    var value = (int)success;  // Implicit cast
    Console.WriteLine($"Success: {value}");
}

// Error case
var error = Result<int>.Error("Operation failed", "Context details");
if (!error) {
    foreach (var msg in error.Errors) Console.WriteLine(msg);
}

// Serializable
var json = Tools.Json.WriteToString(success);
var restored = Tools.Json.ReadFromString<Result<int>>(json);
```
**Producer-Consumer Queue (Async Concurrent Pattern):**

```csharp
using var queue = new ProducerConsumerQueue<DataItem>(capacity: 1000);

// Producer task (async)
_ = Task.Run(async () => {
    for (int i = 0; i < 10_000; i++) {
        await queue.PutAsync(new DataItem { Id = i, Data = /* ... */ });
    }
    queue.CompleteAdding();  // Signal no more items
});

// Consumer task (async) — TakeManyAsync yields batches for efficiency
await foreach (var batch in queue.TakeManyAsync(batchSize: 100)) {
    await ProcessBatch(batch);
}
```

### 💾 Persistence Examples

**Stream-Mapped Collections (Disk-Backed):**

```csharp
using Hydrogen;
using System.IO;

// List that persists to file, exceeding available memory
var fileStream = new FileStream("inventory.dat", FileMode.Create, FileAccess.ReadWrite);
var inventory = new StreamMappedList<Product>(
    fileStream,
    ItemSerializer.Default<Product>(),
    autoLoad: false
);

inventory.Add(new Product { Id = 1, Name = "Widget" });
inventory.Add(new Product { Id = 2, Name = "Gadget" });
inventory.Save();  // Flush to disk

// Later, reload from disk
fileStream.Seek(0, SeekOrigin.Begin);
var reloaded = new StreamMappedList<Product>(fileStream, ItemSerializer.Default<Product>(), autoLoad: true);
Console.WriteLine(reloaded[0].Name);  // "Widget"
```

**Transactional Collections (ACID Semantics):**

```csharp
// Transactional dictionary backed by file
var persistedDict = new TransactionalDictionary<string, Account>();

using (var scope = persistedDict.BeginScope()) {
    using (var txn = scope.BeginTransaction()) {
        persistedDict["acc001"] = new Account { Balance = 1000 };
        persistedDict["acc002"] = new Account { Balance = 500 };
        txn.Commit();  // Atomic commit to disk
    }
    // If exception occurs, automatic rollback
}

// Verify persistence
using (var scope2 = persistedDict.BeginScope()) {
    using (var txn2 = scope2.BeginTransaction()) {
        var acc1 = persistedDict["acc001"];  // Data persisted
        Console.WriteLine(acc1.Balance);    // 1000
    }
}
```

### 🌳 Merkle Tree Examples

**Merkle List with Integrity Proofs:**

```csharp
using Hydrogen;
using System.Security.Cryptography;

// Create a merkle-aware list with SHA-256
var hasher = new HashAlgorithmAdapter(SHA256.Create());
var merkleList = new FlatMerkleList<string>(
    ItemSerializer.Default<string>(),
    hasher
);

merkleList.Add("Block 1");
merkleList.Add("Block 2");
merkleList.Add("Block 3");

// Get root hash (commitment to entire list)
byte[] rootHash = merkleList.MerkleTree.Root;

// Generate proof that \"Block 2\" is at index 1
var proof = merkleList.MerkleTree.GenerateProof(1);

// Verify proof independently
var isValid = merkleList.MerkleTree.VerifyProof(
    merkleList.GetItemHash(1),
    1,
    proof,
    rootHash
);
Console.WriteLine($"Proof valid: {isValid}");  // true
```

**Merkle Dictionary (Multiple Keys):**

```csharp
var merkleDictionary = new MerkleListAdapter<KeyValuePair<string, int>>(
    new ExtendedList<KeyValuePair<string, int>>(),
    hasher
);

merkleDictionary.Add(new KeyValuePair<string, int>("Alice", 100));
merkleDictionary.Add(new KeyValuePair<string, int>("Bob", 50));

// Prove integrity of multi-item state
var multiProof = merkleDictionary.MerkleTree.GenerateMultiProof(new[] { 0, 1 });
```
### 📦 Serialization Examples

**Built-in Serializers (Default):**

```csharp
using Hydrogen;

// Simple type serialization
var intSerializer = ItemSerializer<int>.Default;
byte[] bytes = intSerializer.Serialize(42);
int restored = intSerializer.Deserialize(bytes);

// Supports complex types automatically
var listSerializer = ItemSerializer<ExtendedList<string>>.Default;
var list = new ExtendedList<string> { "a", "b", "c" };
var serialized = listSerializer.Serialize(list);
var deserialized = listSerializer.Deserialize(serialized);
```

**Custom Serializer Factory with Type Registration:**

```csharp
var factory = new SerializerFactory();

// Register primitives with specific strategies
factory.Register(
    typeof(string),
    new StringSerializer(SizeDescriptorStrategy.UseVarInt)
);

// Register custom type
factory.Register(
    typeof(MyObject),
    new MyObjectSerializer(factory)
);

// Retrieve and use
var serializer = factory.GetSerializer(typeof(MyObject));
var data = serializer.Serialize(myObj);
```

**Polymorphic Serialization (Inheritance Support):**

```csharp
// Animal is abstract; Dog and Cat inherit from it
// Mark subtypes with [KnownSubType]
[KnownSubType(typeof(Dog))]
[KnownSubType(typeof(Cat))]
public abstract class Animal { /* ... */ }

// Default serializer automatically handles polymorphism
var animalSerializer = ItemSerializer<Animal>.Default;
var animals = new ExtendedList<Animal> { 
    new Dog("Fido"), 
    new Cat("Mittens") 
};

byte[] bytes = animalSerializer.Serialize(animals);
var restored = animalSerializer.Deserialize(bytes);
Console.WriteLine(restored[0].GetType());  // Dog ✓
Console.WriteLine(restored[1].GetType());  // Cat ✓
```

**Reference-Tracked Serialization (Graph Preservation):**

```csharp
// When serializing object graphs with repeated references
// or cycles, use reference serializers to preserve identity

class Node { 
    public string Value { get; set; }
    public Node Next { get; set; }
}

var factory = new SerializerFactory();
var refSerializer = new NodeSerializer().AsReferenceSerializer();
factory.Register(typeof(Node), refSerializer);

// Circular linked list: A -> B -> A
var a = new Node { Value = "A" };
var b = new Node { Value = "B", Next = a };
a.Next = b;

byte[] data = refSerializer.Serialize(a);
var restored = refSerializer.Deserialize(data);
// Identity preserved: restored.Next.Next == restored ✓
```

**Compact Integer Encoding (VarInt/CVarInt):**

```csharp
using Hydrogen;

// VarInt: Variable-length signed integers (more compact for small numbers)
using (var ms = new MemoryStream()) {
    VarInt.Write(ms, 300);
    ms.Position = 0;
    int v = VarInt.Read(ms);  // 300
    // 300 encoded as 3 bytes instead of 4
}

// CVarInt: Compact unsigned, extreme compression for typical ranges
var bytes = CVarInt.ToBytes(10000);  // Few bytes only
var value = CVarInt.From(bytes);

// Typical usage in custom serializers
class CompactSerializer : ItemSerializerBase<int> {
    public override void Serialize(ISerializationContext context, int item) {
        CVarInt.Write(context.Writer, (ulong)item);
    }
    
    public override int Deserialize(IDeserializationContext context) {
        return (int)CVarInt.Read(context.Reader);
    }
}
```

**Cryptographic Hashing:**

```csharp
using Hydrogen;

var data = "Hello, World!";

// Standard hash functions
byte[] sha256 = Tools.Hashing.SHA256(data);
byte[] sha512 = Tools.Hashing.SHA512(data);
byte[] blake2b = Tools.Hashing.BLAKE2b(data);
byte[] murmurhash = Tools.Hashing.MurmurHash3(data);

// Hash files
byte[] fileHash = Tools.Hashing.SHA256File("path/to/file.bin");

// Compute multiple simultaneously
var hashes = Tools.Hashing.ComputeMultipleHashes(data, CHF.SHA2_256, CHF.SHA3_256);
```
// Hash files
byte[] fileHash = Tools.Hashing.SHA256File("path/to/file.bin");

// Compute multiple simultaneously
var hashes = Tools.Hashing.ComputeMultipleHashes(data, CHF.SHA2_256, CHF.SHA3_256);
```

### 📝 String Extensions (50+ helpers)

```csharp
using Hydrogen;

var text = "Hello World";

// Formatting & validation
var padded = text.PadToLength(20);                      // Pad or truncate to exact length
var truncated = text.Truncate(5);                       // Truncate with ellipsis
bool isEmpty = text.IsNullOrEmpty();
bool isWhitespace = "   ".IsNullOrWhiteSpace();

// Type checking
bool isNumeric = "12345".IsNumeric();
bool isAlpha = "abc".IsAlpha();
bool isAlphaNumeric = "abc123".IsAlphaNumeric();
bool isHex = "DEADBEEF".IsHex();

// Case conversion
var camelCase = "hello_world".ToCamelCase();           // helloWorld
var pascalCase = "hello_world".ToPascalCase();         // HelloWorld
var snakeCase = "HelloWorld".ToSnakeCase();            // hello_world

// Parsing & extraction
var (success, number) = "42".TryParseInt();
var guid = "550e8400-e29b-41d4-a716-446655440000".TryParseGuid();
var words = "The quick brown fox".SplitOnWhitespace();

// Splitting & joining
var lines = "line1\nline2\nline3".ToLines();
var csv = new[] { "a", "b", "c" }.JoinWith(", ");
```

**LevelDB Integration (High-Performance Key-Value Store):**

```csharp
using Hydrogen.Windows.LevelDB;

// Open database
using var db = new DB("./mydata");

// Basic operations
var key = Encoding.UTF8.GetBytes("user:42");
var value = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(user));
db.Put(key, value);

var retrieved = db.Get(key);
if (retrieved != null) {
    var restored = JsonSerializer.Deserialize<User>(Encoding.UTF8.GetString(retrieved));
}

// Batch operations (atomic)
using (var batch = db.CreateBatch()) {
    for (int i = 0; i < 1000; i++) {
        batch.Put(Encoding.UTF8.GetBytes($"key:{i}"), Encoding.UTF8.GetBytes($"value:{i}"));
    }
    db.Write(batch);
}

// Iteration & range queries
using (var iterator = db.CreateIterator()) {
    iterator.SeekToFirst();
    while (iterator.IsValid()) {
        var k = Encoding.UTF8.GetString(iterator.Key());
        var v = Encoding.UTF8.GetString(iterator.Value());
        Console.WriteLine($"{k} = {v}");
        iterator.Next();
    }
}
```

}
```

## 🔧 Extensibility & Customization

The library's design encourages extending core abstractions rather than modifying built-in types. Here are the main extension points:

### Custom Serializers

Implement `IItemSerializer<T>` (or inherit `ItemSerializerBase<T>`) to define custom serialization logic:

```csharp
public class UserSerializer : ItemSerializerBase<User> {
    private readonly IItemSerializer<string> _stringSerializer;
    private readonly IItemSerializer<int> _intSerializer;
    
    public UserSerializer(SerializerFactory factory) {
        _stringSerializer = factory.GetSerializer<string>();
        _intSerializer = factory.GetSerializer<int>();
    }
    
    public override void Serialize(ISerializationContext context, User user) {
        _stringSerializer.Serialize(context, user.Name);
        _intSerializer.Serialize(context, user.Age);
    }
    
    public override User Deserialize(IDeserializationContext context) {
        var name = _stringSerializer.Deserialize(context);
        var age = _intSerializer.Deserialize(context);
        return new User { Name = name, Age = age };
    }
}

// Register and use
var factory = new SerializerFactory();
factory.Register(typeof(User), new UserSerializer(factory));
var serialized = factory.GetSerializer<User>().Serialize(user);
```

### Serializer Decorators

Wrap serializers to add cross-cutting concerns (null-handling, encryption, compression, etc.):

```csharp
// Add null-substitution
var baseSerializer = ItemSerializer<int>.Default;
var nullableSerializer = new WithNullSubstitutionSerializer<int>(
    baseSerializer, 
    defaultValue: -1  // Use -1 when null
);

// Chain decorators
var encrypted = new EncryptedSerializer<MyType>(nullableSerializer, encryptionKey);
var compressed = new CompressedSerializer<MyType>(encrypted);
```

### Custom Indexes on Object Streams

Implement `IProjectionIndex<TItem, TKey>` to add custom indexing strategies:

```csharp
public class LastNameIndex : ProjectionIndexBase<Person, string> {
    public LastNameIndex(ObjectStream<Person> objectStream) 
        : base(objectStream) { }
    
    public override string ProjectKey(Person item) => item.LastName;
    
    protected override void OnIndexAdded(Person item, long index) {
        // Store index mapping
    }
    
    public override long? TryGetIndex(string lastName) {
        // Lookup by last name
        return _indexStore.TryGetValue(lastName, out var idx) ? idx : null;
    }
}

// Attach to ObjectStream
var objectStream = new ObjectStream<Person>(clusteredStreams, serializer);
var index = new LastNameIndex(objectStream);
objectStream.RegisterIndex(index);

// Query via index
var personIndex = index.TryGetIndex("Smith");
var person = objectStream[personIndex.Value];
```

### Custom Transactional Scopes

Subclass `TransactionalScopeBase` to implement custom transaction semantics:

```csharp
public class FileBackedTransactionalScope : TransactionalScopeBase {
    private readonly FileStream _logFile;
    private List<Operation> _operations = new();
    
    protected override void OnBeginTransaction() {
        _operations.Clear();
        // Write transaction start marker to log
    }
    
    protected override void OnCommitTransaction() {
        // Flush all operations to file atomically
        _logFile.Write(Encoding.UTF8.GetBytes("[COMMIT]"));
        _logFile.Flush();
    }
    
    protected override void OnRollbackTransaction() {
        _operations.Clear();
        // Discard pending operations
    }
}

// Use in transactional collections
var dict = new TransactionalDictionary<string, int>();
using (var scope = new FileBackedTransactionalScope()) {
    using (var txn = scope.BeginTransaction()) {
        dict["key"] = 42;
        txn.Commit();
    }
}
```

### Extending Collections

Decorate existing collections to add custom behavior:

```csharp
// Add logging to list operations
public class LoggingList<T> : ExtendedListDecorator<T> {
    private readonly ILogger _logger;
    
    public LoggingList(IExtendedList<T> inner, ILogger logger) : base(inner) {
        _logger = logger;
    }
    
    public override void Add(T item) {
        _logger.Info($"Adding {item}");
        base.Add(item);
    }
    
    public override void InsertRange(long index, T[] items) {
        _logger.Info($"Inserting {items.Length} items at {index}");
        base.InsertRange(index, items);
    }
}

// Use transparently
var baseList = new ExtendedList<int>();
IExtendedList<int> logged = new LoggingList<int>(baseList, logger);
logged.Add(42);  // "Adding 42" logged
```

## ⚠️ Threading / Performance / Safety Notes

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

## ✅ Status & Maturity

Hydrogen is a mature library that has evolved over multiple years. Core subsystems (collections, serialization, transactions, merkle trees) are stable and production-tested. Some components (post-quantum cryptography, protocol orchestration) may be less battle-tested and should be evaluated carefully for production use.

### Compatibility

- **Target Framework**: .NET 8.0+ (primary), with support for .NET Standard 2.0 where applicable
- **Backward Compatibility**: The library does not guarantee API stability across major versions. Serialization formats may evolve, requiring migration strategies for persistent data.
- **Platform Support**: Windows, Linux, macOS, iOS (via Xamarin/MAUI), Android (via Xamarin/MAUI)

### 🧪 Experimental Components

- Post-quantum signature schemes (W-OTS, AMS) are reference implementations. Audit and validate before use in production cryptographic systems.
- Some advanced merkle-tree variants and indexing strategies are optimized for specific use cases and may have edge-case limitations.

### 📦 Platform-Specific Packages

- `Hydrogen.Windows` - Windows-specific utilities
- `Hydrogen.Windows.Forms` - WinForms integration
- `Hydrogen.Windows.LevelDB` - Native LevelDB wrapper
- `Hydrogen.Application` - Cross-platform application framework
- `Hydrogen.Communications` - Networking and protocol layers
- `Hydrogen.Web.AspNetCore` - ASP.NET Core integration
- `Hydrogen.CryptoEx` - Extended cryptography (ECDSA, ECIES, etc.)

##  Dependencies

- **.NET 8.0** or higher (primary target)
- **No external dependencies** for core functionality
- **Optional**: BouncyCastle for advanced cryptography
- **Optional**: Newtonsoft.Json for JSON support
- **Optional**: Microsoft.Extensions.DependencyInjection for DI integration

## ⚖️ License

Distributed under the **MIT NON-AI License**.

This license encourages ethical AI development and prevents use in certain AI/ML contexts without explicit permission. See the LICENSE file for full details.

More information: [Sphere10 NON-AI-MIT License](https://sphere10.com/legal/NON-AI-MIT)

## 📖 Resources

- **GitHub**: [Sphere10/Hydrogen](https://github.com/Sphere10/Hydrogen)
- **Website**: [sphere10.com](https://sphere10.com)
- **Company**: Sphere 10 Software Pty Ltd

## 👤 Author

**Herman Schoenfeld** - Lead Developer

---

**Version**: 2.0.2