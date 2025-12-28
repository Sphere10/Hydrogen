# Hydrogen

A comprehensive utility library for full-stack .NET 8.0+ applications. Hydrogen provides robust abstractions, extensions, and tools for collections, serialization, cryptography, streaming, memory management, and much more.

**Developed by [Sphere 10 Software](https://sphere10.com)**

## 📋 Key Features

### Collections & Data Structures
- **Extensive Collections Framework**: Advanced data structures including paged collections, file-mapped collections, sorted collections, and custom iterators
- **Paged Collections**: Handle large datasets without loading everything into memory
- **Sorted Collections**: Maintain sorted order while supporting fast lookups
- **File-Mapped Collections**: Use disk storage for extremely large collections

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
