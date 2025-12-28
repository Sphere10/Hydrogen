# Hydrogen

A comprehensive, production-grade utility library for full-stack .NET 8.0+ applications. Hydrogen provides robust abstractions, extensions, and tools for collections, serialization, cryptography, streaming, memory management, and much more.

**Developed by [Sphere 10 Software](https://sphere10.com)** and battle-tested in commercial mobile, desktop, web, and enterprise applications.

## 📋 Key Features

- **Extensive Collections Framework**: Advanced data structures including paged collections, file-mapped collections, sorted collections, and custom iterators
- **Comprehensive Serialization**: Flexible binary serialization with context-aware item serializers and support for checksums
- **Enterprise-Grade Cryptography**: Hashing, digital signatures, encryption, key derivation functions, and quantum-resistant algorithms
- **Advanced Streaming**: Clustered, transactional, and paged stream implementations with support for complex data patterns
- **Memory Management**: Efficient memory handling with paged collections, reinterpreted arrays, and memory metrics
- **Full-Stack Utilities**: 50+ extension methods for strings, collections, types, tasks, streams, and more
- **Multi-Platform Support**: Optimized for mobile, desktop, and web applications
- **Modern .NET Practices**: Async/await support, dependency injection integration, and latest C# features

## 🚀 Quick Start

### Installation

```bash
dotnet add package Hydrogen
```

Or via NuGet Package Manager:
```
Install-Package Hydrogen
```

### Basic Usage

```csharp
using Hydrogen;

// String extensions
string text = "Hello World";
string truncated = text.Truncate(5); // "Hello…"
int count = text.CountSubstring("l"); // 3

// Collection utilities
var arr = Tools.Collection.GenerateArray(10, i => i * 2);
var partitions = Tools.Collection.Partition(100, 10);

// Binary search
var list = new List<int> { 1, 3, 5, 7, 9 };
long index = Tools.Collection.BinarySearch(list, 5, 0, list.Count - 1, 
    (search, item) => search.CompareTo(item));
```

## 📦 Core Modules

### **Collections** (`Hydrogen.Collections`)
Advanced collection types and utilities:
- Array operations and 2D arrays
- Dictionary, list, and set extensions
- Bit collections and buffer collections
- Graph structures and iterators
- Paged collections for memory efficiency
- Stream-backed collections

### **Serialization** (`Hydrogen.Serialization`)
Flexible binary serialization framework:
- Custom item serializers with context support
- Checksum validation
- Nullable type handling
- Collection and specialized type serializers
- Factory pattern for dynamic serialization

### **Cryptography** (`Hydrogen.Crypto`)
Production-ready cryptographic utilities:
- Checksums and hashing
- Digital signature schemes
- Encryption and key management
- Key derivation functions (KDF)
- Pseudo-random number generation
- Quantum-resistant algorithms support

### **Extensions**
50+ extension methods for common types:
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

*Hydrogen powers production applications handling millions of transactions daily across multiple platforms.*
