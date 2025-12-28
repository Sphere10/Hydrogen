<p align="center">
  <img  src="resources/branding/hydrogen-white-bg.gif" alt="HydrogenP2P logo">
</p>

# Hydrogen: P2P Framework

Copyright © Sphere 10 Software 2018 - Present

Hydrogen is a .NET-based framework for building scalable P2P blockchain applications. It is a broad framework that can be used for building  layer-1 blockchain-based systems as well as layer-2 extensions to those systems. 

The Hydrogen framework offers the following features:

- **Node**: a fully functional node with it's own console-based GUI. The node provides data-persistence for blockchain and consensus databases, networking layer for P2P protocols, key management for wallets, JSON APIs for integration and all the core functionality typically offered by a node.
- **GUI**: a Blazor-based GUI for interacting with the node. This piece includes a rich-set of widgets useful for composing applications.
- **In-protocol upgrades**: applications control how they are upgraded and can be upgraded automatically from the blockchain itself (typically through governance protocol).  Absolutely everything can be upgraded including the node and UI. 
- **Automatic Interoperability**: hydrogen applications can easily interoperate with one another with complex workflows and patterns (publish-subscribe, send-and-forget, sagas) via the Helium framework.
- **Plug-n-Play**:  plugins that can extend both the Node and/or GUI and which can be installed dynamically.
- **Extensible**: in addition to plugins, the the framework offers extension points at various layers of the architecture.
- **Cross-platform**: runs on any OS that supports .NET 5 framework.



## 📁 Project Structure

The Hydrogen framework consists of multiple interconnected projects organized as follows:

### Core Framework (`src/`)

| Project | Purpose |
|---------|---------|
| [**Hydrogen**](src/Hydrogen/README.md) | General-purpose core library with utilities for caching, collections, cryptography, serialization, streaming, and more |
| [**Hydrogen.Application**](src/Hydrogen.Application/README.md) | Application lifecycle, dependency injection, command-line interface, and presentation framework |
| [**Hydrogen.Communications**](src/Hydrogen.Communications/README.md) | Multi-protocol networking layer: TCP, UDP, WebSockets, RPC, and pipes |
| [**Hydrogen.CryptoEx**](src/Hydrogen.CryptoEx/README.md) | Extended cryptography: Bitcoin (SECP256k1), elliptic curves, hash functions, post-quantum algorithms |
| [**Hydrogen.Consensus**](src/Hydrogen.Consensus/README.md) | Blockchain consensus mechanisms and validation rules framework |
| [**Hydrogen.Data**](src/Hydrogen.Data/README.md) | Data access abstraction layer with ADO.NET enhancements, SQL query building, CSV support |
| [**HashLib4CSharp**](src/HashLib4CSharp/README.md) | Hashing library with support for MD5, SHA, BLAKE2, CRC, checksums, and more |

### DApp Framework (`src/`)

| Project | Purpose |
|---------|---------|
| [**Hydrogen.DApp.Core**](src/Hydrogen.DApp.Core/README.md) | Core blockchain and DApp framework: blocks, transactions, wallets, plugins, persistence |
| [**Hydrogen.DApp.Node**](src/Hydrogen.DApp.Node/README.md) | Full-featured blockchain node with terminal UI, consensus, networking, wallet, and JSON APIs |
| [**Hydrogen.DApp.Host**](src/Hydrogen.DApp.Host/README.md) | Host process for running DApp nodes as services |

### Presentation Layer (`blackhole/`)

| Project | Purpose |
|---------|---------|
| [**Hydrogen.DApp.Presentation**](blackhole/Hydrogen.DApp.Presentation/README.md) | Blazor component library with wizards, modals, grids, and UI components |
| [**Hydrogen.DApp.Presentation.Loader**](blackhole/Hydrogen.DApp.Presentation.Loader/README.md) | Blazor WebAssembly host for the presentation layer |
| [**Hydrogen.DApp.Presentation.WidgetGallery**](blackhole/Hydrogen.DApp.Presentation.WidgetGallery/README.md) | Component showcase and demonstration plugin |
| [**Hydrogen.DApp.Presentation2**](blackhole/Hydrogen.DApp.Presentation2/README.md) | Alternative presentation implementation with advanced patterns |
| [**Hydrogen.DApp.Presentation2.Loader**](blackhole/Hydrogen.DApp.Presentation2.Loader/README.md) | WebAssembly host for the alternative presentation |

### Database Implementations (`src/`)

| Project | Purpose |
|---------|---------|
| [**Hydrogen.Data.Sqlite**](src/Hydrogen.Data.Sqlite/README.md) | SQLite implementation for embedded databases |
| [**Hydrogen.Data.Firebird**](src/Hydrogen.Data.Firebird/README.md) | Firebird database implementation |
| [**Hydrogen.Data.MSSQL**](src/Hydrogen.Data.MSSQL/README.md) | Microsoft SQL Server implementation |
| [**Hydrogen.Data.NHibernate**](src/Hydrogen.Data.NHibernate/README.md) | NHibernate ORM integration |

### Windows Desktop (`src/`)

| Project | Purpose |
|---------|---------|
| [**Hydrogen.Windows**](src/Hydrogen.Windows/README.md) | Windows platform integration: registry, services, event logging |
| [**Hydrogen.Windows.Forms**](src/Hydrogen.Windows.Forms/README.md) | Windows Forms UI framework and components |
| [**Hydrogen.Windows.Forms.Sqlite**](src/Hydrogen.Windows.Forms.Sqlite/README.md) | Windows Forms with SQLite data binding |
| [**Hydrogen.Windows.Forms.Firebird**](src/Hydrogen.Windows.Forms.Firebird/README.md) | Windows Forms with Firebird data binding |
| [**Hydrogen.Windows.Forms.MSSQL**](src/Hydrogen.Windows.Forms.MSSQL/README.md) | Windows Forms with SQL Server data binding |
| [**Hydrogen.Windows.LevelDB**](src/Hydrogen.Windows.LevelDB/README.md) | LevelDB integration for fast key-value storage |

### Web & Cross-Platform (`src/`)

| Project | Purpose |
|---------|---------|
| [**Hydrogen.Web.AspNetCore**](src/Hydrogen.Web.AspNetCore/README.md) | ASP.NET Core integration: middleware, filters, routing, forms |
| [**Hydrogen.Drawing**](src/Hydrogen.Drawing/README.md) | Cross-platform graphics and drawing utilities |
| [**Hydrogen.NET**](src/Hydrogen.NET/README.md) | .NET Framework-specific utilities and type introspection |
| [**Hydrogen.NETCore**](src/Hydrogen.NETCore/README.md) | .NET Core and modern .NET utilities |
| [**Hydrogen.iOS**](src/Hydrogen.iOS/README.md) | Xamarin.iOS integration for native iOS apps |
| [**Hydrogen.Android**](src/Hydrogen.Android/README.md) | Xamarin.Android integration for native Android apps |
| [**Hydrogen.macOS**](src/Hydrogen.macOS/README.md) | Xamarin.macOS integration for native macOS apps |

### Testing & Utilities (`src/` & `tests/`)

| Project | Purpose |
|---------|---------|
| [**Hydrogen.NUnit**](src/Hydrogen.NUnit/README.md) | NUnit testing framework and utilities |
| [**Hydrogen.NUnit.DB**](src/Hydrogen.NUnit.DB/README.md) | Database testing utilities |
| [**Hydrogen.Generators**](src/Hydrogen.Generators/README.md) | C# source generators for compile-time code generation |

## 📚 Documentation Links

### Architecture

1. [What is the Hydrogen Framework?](blackhole/docs/Architecture/Hydrogen.md)
2. [Hydrogen Runtime](blackhole/docs/Architecture/Runtime.md)
3. [Framework Domains](blackhole/docs/Architecture/Domains.md)

### Blockchain Technology

4. [Blockchain: Dynamic Merkle Trees](https://sphere10.com/tech/dynamic-merkle-trees)
5. [Blockchain: Real-Time Targeted Difficulty Adjustment Algorithm](blackhole/docs/Blockchain/rtt-asert.pdf)
6. [Post-Quantum Cryptography: Abstract Merkle Signatures (AMS)](https://sphere10.com/tech/ams)
7. [Post-Quantum Cryptography: Winternitz Abstracted Merkle Signatures (WAMS)](https://sphere10.com/tech/wams)
8. [Post-Quantum Cryptography: Faster and Smaller Winternitz Signatures](https://sphere10.com/tech/wots-sharp)

### Guidelines

9. [What is a 3-tier Architecture?](blackhole/docs/Guidelines/3-tier-Architecture.md)
10. [Code-Styling Guidelines](blackhole/docs/Guidelines/Code-Styling.md)

## 🔗 Quick Navigation

- **Getting Started**: See [Hydrogen.DApp.Node](src/Hydrogen.DApp.Node/README.md) to run a blockchain node
- **Web UI**: See [Hydrogen.DApp.Presentation.Loader](blackhole/Hydrogen.DApp.Presentation.Loader/README.md) for the web interface
- **Building DApps**: See [Hydrogen.DApp.Core](src/Hydrogen.DApp.Core/README.md) for core DApp development
- **Database Access**: See [Hydrogen.Data](src/Hydrogen.Data/README.md) for data layer patterns
- **Networking**: See [Hydrogen.Communications](src/Hydrogen.Communications/README.md) for network protocols