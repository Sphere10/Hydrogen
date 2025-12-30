# Hydrogen Domains Reference

## Table of Contents

1. [What are Domains](#what-are-domains)
2. [Domain Organization](#domain-organization)
3. [Hydrogen Framework Domains](#hydrogen-framework-domains)
4. [Helium Framework Domains](#helium-framework-domains)
5. [Hydrogen DApp Domains](#hydrogen-dapp-domains)
6. [Domain Dependencies](#domain-dependencies)
7. [Extending with Custom Domains](#extending-with-custom-domains)

---

## What are Domains

A **domain** is a logical grouping of code and functionality that represents a specific abstraction or concept. Unlike modules (which are horizontal slices within a single tier), domains are **vertical slices** that typically span multiple architectural tiers.

### Key Characteristics

- **Encapsulation**: Domains encapsulate all code related to a specific concept
- **Multi-Tier**: A domain may include presentation, processing, communications, and data components
- **Responsibility**: Each domain has a clear, single responsibility
- **Organization**: Within modules, domains are typically organized as sub-folders

### Domain vs. Module

| Aspect | Module | Domain |
|--------|--------|--------|
| **Definition** | Horizontal slice within one tier | Vertical slice across tiers |
| **Scope** | Single tier | Multiple tiers |
| **Project** | Often a .NET project | Folder/namespace within project |
| **Coupling** | Modules depend on tiers | Domains integrate features |

---

## Domain Organization

### Naming Convention

Domains follow the pattern: `Company.Product.Tier.Domain`

**Examples**:
- `Sphere10.Hydrogen.System.Collections`
- `Sphere10.Hydrogen.Data.ObjectSpaces`
- `Sphere10.Hydrogen.DApp.Consensus`

### Filesystem Organization

Domains are typically organized within modules by sub-folders:

```
Hydrogen/
├── Cache/
│   ├── ICacheProvider.cs
│   ├── Policies/
│   └── BatchCache.cs
├── Collections/
│   ├── BTree.cs
│   ├── MerkleTree.cs
│   └── Merkle/
└── Crypto/
    ├── Hashing/
    ├── Signing/
    └── KeyDerivation/
```

---

## Hydrogen Framework Domains

Hydrogen Framework provides **35+ domains** organized by system tier. These domains are foundational and contain **zero blockchain dependencies**.

### System Tier Domains

System tier provides foundational functionality used by all other tiers.

#### Cache Domain
**Purpose**: Caching strategies and policies for in-memory data storage

**Key Components**:
- `ICacheProvider`: Base caching interface
- `LRUCache`: Least-recently-used eviction policy
- `LFUCache`: Least-frequently-used eviction policy
- `FIFOCache`: First-in-first-out eviction
- `BatchCache`: Bulk caching operations
- `SessionCache`: Request/session-scoped caching

**Typical Usage**: Application-level caching, query result caching, object pooling

#### Collections Domain
**Purpose**: Advanced data structures for efficient data management

**Key Components**:
- `BTree<K,V>`: Ordered key-value tree structure
- `MerkleTree<T>`: Cryptographically verified tree structure
- `PagedList<T>`: Memory-efficient pageable collection
- `TransactionalCollection<T>`: ACID collection operations
- `ObservableCollection<T>`: Change notification collection
- `BloomFilter<T>`: Probabilistic set membership testing
- `BitArray`: Space-efficient bit manipulation

**Typical Usage**: Database indices, cryptographic verification, large dataset handling

#### Comparers Domain
**Purpose**: Custom comparison implementations for specialized types

**Key Components**:
- Custom equality comparers
- Specialized sorting comparers
- Type-aware comparison logic

#### Conversion Domain
**Purpose**: Type conversion and data transformation utilities

**Key Components**:
- Endian conversion (big-endian ↔ little-endian)
- Numeric type conversion
- Custom conversion strategies
- `IEndianConvertible`: Endian-aware interface

#### Cryptography Domain
**Purpose**: 100+ cryptographic algorithms and primitives

**Sub-Domains**:
- **Hashing**: MD5, SHA (all variants), BLAKE2, Whirlpool, RIPEMD, Tiger, Groestl, CRC, checksums
- **Digital Signatures**: ECDSA, EdDSA, RSA, GOST R 34.10
- **Encryption**: AES, ChaCha20, symmetric/asymmetric schemes
- **Key Derivation**: Scrypt, PBKDF2, Argon2, bcrypt
- **Bitcoin Cryptography**: SECP256k1 curves and Bitcoin-specific functions

**Typical Usage**: Password hashing, digital signatures, encrypted storage, blockchain operations

#### Encoding Domain
**Purpose**: Data encoding and decoding strategies

**Key Components**:
- Hexadecimal encoding/decoding
- Base64 support
- Custom encoding schemes
- `IEncodable`: Standard encoding interface

#### Environment Domain
**Purpose**: OS and runtime environment abstraction

**Key Components**:
- Platform detection (Windows, macOS, Linux)
- Runtime information
- System resource queries
- OS-specific utilities

#### Events Domain
**Purpose**: Event handling and pub-sub patterns

**Key Components**:
- `IEvent`: Standard event interface
- `IEventPublisher`: Event broadcasting
- `IEventSubscriber`: Event consumption
- Weak event patterns for memory efficiency

#### Exceptions Domain
**Purpose**: Custom exception types and exception utilities

**Key Components**:
- Framework-specific exceptions
- Exception utilities
- Exception aggregation
- Structured exception information

#### Extensions Domain
**Purpose**: Extension methods for .NET framework types

**Key Components**:
- 200+ extension methods for common types
- String, collection, type extensions
- Functional programming extensions
- Performance-optimized implementations

#### Functional Domain
**Purpose**: Functional programming support

**Key Components**:
- Lambda expression utilities
- Function composition
- Currying support
- Monadic patterns
- Result/Option types

#### Introspection Domain
**Purpose**: Fast reflection and type introspection

**Key Components**:
- Cached type information
- Constructor/method delegates
- Generic type handling
- Performance-optimized reflection

#### IO Domain
**Purpose**: Input/output and file system utilities

**Key Components**:
- Endian-aware I/O streams
- File store abstractions
- Filesystem utilities
- Memory-mapped files
- Structured binary I/O

#### IoC Domain
**Purpose**: Inversion of Control container and dependency injection

**Key Components**:
- `IComponentRegistry`: Component registration
- `IComponentFactory`: Component resolution
- Singleton, transient, scoped lifetimes
- Property injection, constructor injection

#### Logging Domain
**Purpose**: Flexible logging framework

**Key Components**:
- `ILogger`: Standard logging interface
- Multiple output targets (console, file, debug)
- Log levels (Trace, Debug, Info, Warning, Error, Fatal)
- Structured logging
- Performance considerations

#### Math Domain
**Purpose**: Mathematical utilities and algorithms

**Key Components**:
- `BigDecimal`: Arbitrary precision decimal
- `Rational`: Rational number support
- `FixedPoint`: Fixed-point arithmetic
- Random number generators
- Matrix/vector operations
- Bloom filters
- Statistical functions

#### Memory Domain
**Purpose**: Memory management and optimization utilities

**Key Components**:
- Memory unit conversion (bytes, KB, MB, GB)
- Memory allocation tracking
- GC optimization utilities

#### Misc Domain
**Purpose**: Miscellaneous utilities not belonging to other domains

**Key Components**:
- General-purpose helpers
- Specialized utilities
- One-off implementations

#### Networking Domain
**Purpose**: Network communication and protocols

**Key Components**:
- TCP/UDP abstractions
- WebSocket support
- URL parsing and manipulation
- MIME type handling
- POP3 email protocol
- P2P network abstractions

#### Objects Domain
**Purpose**: Object manipulation and reflection utilities

**Key Components**:
- Object cloning (deep, shallow)
- Object comparison
- Object serialization helpers
- Property access utilities
- Dynamic object creation

#### ObjectSpaces Domain
**Purpose**: Distributed storage engine for objects

**Key Components**:
- `IObjectSpace`: Persistent object interface
- Transaction support
- Distributed indexing
- Object query language
- Consensus database integration

#### Peripherals Domain
**Purpose**: Hardware peripheral abstraction

**Key Components**:
- Keyboard input handling
- Mouse tracking
- Hotkey registration
- Peripheral device abstraction

#### Scheduler Domain
**Purpose**: Background task scheduling

**Key Components**:
- Time-based scheduling
- Recurring task support
- Cron-like expressions
- Task queue management
- Background worker pools

#### Scopes Domain
**Purpose**: Scope management and context patterns

**Key Components**:
- Request scope
- Transaction scope
- Custom scope implementation
- Scope cleanup handlers
- Nested scope support

#### Serialization Domain
**Purpose**: Object serialization and deserialization

**Key Components**:
- Binary serialization format
- JSON serialization
- XML serialization
- Streaming serialization
- Custom type handling
- Version migration
- Compressor integration

#### Streams Domain
**Purpose**: Advanced stream implementations and utilities

**Key Components**:
- `BitStream`: Bit-level stream manipulation
- `BlockingStream`: Thread-safe blocking operations
- `BoundedStream`: Length-limited streams
- `DeltaStream`: Delta compression
- Stream pipelines
- Stream filters and transformers

#### Text Domain
**Purpose**: Text processing and manipulation

**Key Components**:
- Parser combinators
- Inflector (singular/plural conversion)
- Fast string builder
- Fluent regex builder
- HTML utilities
- Template engine

#### TextWriters Domain
**Purpose**: Custom text writer implementations

**Key Components**:
- `ConsoleTextWriter`: Colored console output
- `DebugTextWriter`: Debug output
- `FileTextWriter`: File logging
- `NullTextWriter`: No-op writer
- `ChainTextWriter`: Multi-target writing

#### Threading Domain
**Purpose**: Threading and concurrency utilities

**Key Components**:
- Thread-safe collections
- Lock patterns
- Async/await utilities
- Task schedulers
- Cancellation tokens
- Synchronization primitives

#### Types Domain
**Purpose**: Type manipulation and activation

**Key Components**:
- Type activation factories
- Generic type handling
- Type resolution
- Type switches
- Constructor invocation
- Method/property access

#### Values Domain
**Purpose**: Value type utilities and patterns

**Key Components**:
- `Future<T>`: Lazy value computation
- `Result<T>`: Success/failure result
- `Option<T>`: Nullable type alternative
- GUID utilities
- Enum utilities
- Value range handling

#### XML Domain
**Purpose**: XML processing and serialization

**Key Components**:
- Deep object serialization to XML
- XML schema support
- XPath queries
- XML validation
- Custom type XML serialization

---

## Helium Framework Domains

Helium provides **distributed messaging and orchestration** functionality.

### Publishing & Distribution Domain
**Purpose**: Message publishing and subscriber management

**Key Components**:
- Pub-Sub bus
- Message routing
- Topic management
- Subscriber registry
- Dead letter queues
- Message serialization

### Messaging Domain
**Purpose**: Message definitions and protocols

**Key Components**:
- Message envelope
- Message headers
- Message metadata
- Correlation IDs
- Causation tracking

### Sagas Domain
**Purpose**: Distributed transaction orchestration

**Key Components**:
- Saga definition and execution
- Compensation logic
- State management
- Timeout handling
- Saga persistence

### Event Sourcing Domain
**Purpose**: Event-based state reconstruction

**Key Components**:
- Event store
- Event streams
- Event versioning
- Snapshot management
- Event replay

### Error Handling Domain
**Purpose**: Distributed error handling and recovery

**Key Components**:
- Retry policies
- Circuit breakers
- Compensation logic
- Error aggregation
- Fault tolerance

---

## Hydrogen DApp Domains

Hydrogen DApp adds **blockchain and consensus-specific** domains.

### Consensus Domain
**Purpose**: Blockchain consensus rules and validation

**Key Components**:
- Consensus rule definitions
- Block validation logic
- Transaction validation
- Fork resolution
- Consensus state machine

### Cryptography (DApp-Specific)
**Purpose**: Blockchain-specific cryptographic operations

**Key Components**:
- Bitcoin curve (SECP256k1) operations
- Digital signature schemes
- Hash functions for blockchain
- Merkle proof generation/verification

### Wallet Domain
**Purpose**: Key management and wallet functionality

**Key Components**:
- Key generation and storage
- Private key management
- Address generation
- Account abstraction
- Multi-signature support
- Hardware wallet integration

### Blockchain Domain
**Purpose**: Block and transaction data structures

**Key Components**:
- Block definition
- Block header
- Transaction structure
- Transaction pool (mempool)
- Block builder
- Chain validation

### Mining Domain
**Purpose**: Block production and proof-of-work

**Key Components**:
- Miner implementation
- Hash target calculation
- Difficulty adjustment
- Nonce generation
- Mining rewards

### Networking (DApp-Specific)
**Purpose**: P2P network protocols for blockchain

**Key Components**:
- Node discovery (Kademlia)
- Peer management
- Block propagation
- Transaction propagation
- Peer-to-peer messaging

### Persistence (DApp-Specific)
**Purpose**: Blockchain data storage

**Key Components**:
- Block store
- Transaction store
- State database
- Index management
- Data synchronization
- Archive functions

### Plugins (DApp)
**Purpose**: Plugin architecture for extending nodes and GUI

**Key Components**:
- Plugin loader
- Plugin lifecycle
- Plugin dependencies
- Plugin versioning
- Plugin sandbox

### Object Spaces (DApp)
**Purpose**: Decentralized consensus state database

**Key Components**:
- Object space implementation
- Merkle-based verification
- Index management
- Query support
- Transaction support

---

## Domain Dependencies

### Dependency Flow

```
Hydrogen DApp Domains
    ↓
    Depends on
    ↓
Helium Domains
    ↓
    Depends on
    ↓
Hydrogen Framework Domains
    ↓
    Zero external dependencies
    ↓
.NET Framework
```

### Example Dependencies

**Wallet Domain** depends on:
- Cryptography Domain (key generation, signing)
- Collections Domain (key storage)
- Persistence Domain (wallet state)
- Serialization Domain (wallet export/import)

**Consensus Domain** depends on:
- Cryptography Domain (proof verification)
- Math Domain (difficulty calculation)
- Blockchain Domain (block validation)
- Networking Domain (peer validation)

---

## Extending with Custom Domains

### Creating a Custom Domain

1. **Identify the Abstraction**: What concept/responsibility does your domain represent?
2. **Choose Your Tier**: Where does it fit in the architecture? (Presentation, Processing, Data, Communications, System)
3. **Name Your Domain**: Follow `Company.Product.Tier.YourDomain` convention
4. **Organize Files**: Create domain folder with logical sub-folders
5. **Implement Interfaces**: Use framework interfaces (ICache, ILogger, etc.)
6. **Document Publicly**: Write clear, API documentation

### Custom Domain Example

```csharp
// Namespace: Sphere10.MyApp.Processing.Analytics
namespace Sphere10.MyApp.Processing.Analytics {

    // Domain-level interface
    public interface IAnalyticsService {
        void TrackEvent(string eventName, Dictionary<string, object> properties);
        IEnumerable<AnalyticsReport> GenerateReports(DateTime from, DateTime to);
    }

    // Implementation
    public class AnalyticsService : IAnalyticsService {
        private readonly ILogger _logger;
        private readonly ICache<string, List<AnalyticsEvent>> _eventCache;
        
        public AnalyticsService(ILogger logger, ICacheProvider cacheProvider) {
            _logger = logger;
            _eventCache = cacheProvider.CreateCache<string, List<AnalyticsEvent>>();
        }
        
        public void TrackEvent(string eventName, Dictionary<string, object> properties) {
            // Implementation
        }
        
        public IEnumerable<AnalyticsReport> GenerateReports(DateTime from, DateTime to) {
            // Implementation
        }
    }

    // Data object (in Data tier)
    public class AnalyticsEvent {
        public string EventName { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
```

### Integration Pattern

```csharp
// Register in IoC container
container.Register<IAnalyticsService, AnalyticsService>();

// Inject into other domains
public class UserService {
    private readonly IAnalyticsService _analytics;
    
    public UserService(IAnalyticsService analytics) {
        _analytics = analytics;
    }
    
    public void LoginUser(User user) {
        _analytics.TrackEvent("User.Login", new {
            UserId = user.Id,
            Timestamp = DateTime.UtcNow
        });
    }
}
```

---

## Domain Best Practices

### DO:
✅ Keep domains focused on a single responsibility
✅ Use framework interfaces for integration
✅ Document public APIs and contracts
✅ Provide clear examples and usage patterns
✅ Consider performance implications
✅ Use dependency injection for testability

### DON'T:
❌ Mix concerns from multiple domains in one class
❌ Create circular dependencies between domains
❌ Bypass frameworks abstractions
❌ Skip documentation and API design
❌ Ignore error handling and edge cases
❌ Create overly granular micro-domains

---

## Related Documentation

- **[Hydrogen.md](Hydrogen.md)** - Framework overview and composition
- **[Runtime.md](Runtime.md)** - Deployment model and lifecycle
- **[3-tier Architecture](../Guidelines/3-tier-Architecture.md)** - Tier definitions
- **[Code Styling](../Guidelines/Code-Styling.md)** - Coding standards

---

## Copyright & Attribution

Copyright © 2018-Present Herman Schoenfeld & Sphere 10 Software

All rights reserved.
