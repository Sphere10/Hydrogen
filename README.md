<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

<p align="center">
  <img  src="resources/branding/hydrogen-white-bg.gif" alt="Hydrogen logo">
</p>

# 🧪 Hydrogen: Comprehensive .NET Application Framework

Copyright © Herman Schoenfeld, Sphere 10 Software 2018 - Present

**A mature, production-ready .NET framework** providing a complete foundation for building full-stack applications across desktop, mobile, and web platforms. Originally designed for blockchain systems, Hydrogen has evolved into a comprehensive general-purpose framework offering robust abstractions, advanced data structures, cryptographic primitives, and utilities for high-performance .NET development.

## 🎯 What Hydrogen Provides

**Core Foundation**
- **Unified Architecture**: Consistent patterns for application lifecycle, dependency injection, configuration, and component lifecycle across all platforms
- **Enterprise Data Access**: Abstracted data layer with support for multiple database engines (SQL Server, SQLite, Firebird, NHibernate) and advanced query building
- **Advanced Cryptography**: Comprehensive cryptographic implementations including post-quantum algorithms, digital signatures, and multiple hashing algorithms
- **Multi-Protocol Networking**: TCP, UDP, WebSockets, and RPC frameworks for building distributed systems
- **Rich Serialization**: Flexible binary serialization, JSON support, and streaming implementations

**Application Development**
- **Desktop UI Framework**: Full-featured Windows Forms component library with data binding, validation, and plugin support
- **Web UI**: Blazor-based component library with wizards, modals, grids, and responsive layouts for modern web applications
- **Cross-Platform**: Run applications on Windows, macOS, iOS, Android, or .NET Core/5+
- **Plugin Architecture**: Dynamic plugin loading and lifecycle management for extensible applications

**Specialized Features**
- **Memory Efficiency**: Advanced collections, paged data structures, and streaming for handling large datasets
- **Graphics & Drawing**: Cross-platform drawing utilities and image manipulation
- **Performance**: Caching, connection pooling, and optimized algorithms
- **Testing**: Comprehensive testing framework and utilities for unit and integration testing



## � Project Structure

The Hydrogen framework consists of **45+ projects** organized by category within `src/`, `tests/`, and `utils/`:

### 🔧 Core Framework & Utilities

| Project | Purpose |
|---------|---------|
| [**Hydrogen**](src/Hydrogen/README.md) | General-purpose core library with utilities for caching, collections, cryptography, serialization, streaming, and more |
| [**Hydrogen.Application**](src/Hydrogen.Application/README.md) | Application lifecycle, dependency injection, command-line interface, and presentation framework |
| [**Hydrogen.Communications**](src/Hydrogen.Communications/README.md) | Multi-protocol networking layer: TCP, UDP, WebSockets, RPC, and pipes |
| [**Hydrogen.Generators**](src/Hydrogen.Generators/README.md) | C# source generators for compile-time code generation |
| [**HashLib4CSharp**](src/HashLib4CSharp/README.md) | Hashing library with support for MD5, SHA, BLAKE2, CRC, checksums, and more |

### 🔐 Cryptography & Security

| Project | Purpose |
|---------|---------|
| [**Hydrogen.CryptoEx**](src/Hydrogen.CryptoEx/README.md) | Extended cryptography: Bitcoin (SECP256k1), elliptic curves, hash functions, post-quantum algorithms |
| [**Hydrogen.Consensus**](src/Hydrogen.Consensus/README.md) | Blockchain consensus mechanisms and validation rules framework |

### 💾 Data Access & Persistence

| Project | Purpose |
|---------|---------|
| [**Hydrogen.Data**](src/Hydrogen.Data/README.md) | Data access abstraction layer with ADO.NET enhancements, SQL query building, CSV support |
| [**Hydrogen.Data.Sqlite**](src/Hydrogen.Data.Sqlite/README.md) | SQLite implementation for embedded databases |
| [**Hydrogen.Data.Firebird**](src/Hydrogen.Data.Firebird/README.md) | Firebird database implementation |
| [**Hydrogen.Data.MSSQL**](src/Hydrogen.Data.MSSQL/README.md) | Microsoft SQL Server implementation |
| [**Hydrogen.Data.NHibernate**](src/Hydrogen.Data.NHibernate/README.md) | NHibernate ORM integration |

### 🖥️ Desktop & Windows

| Project | Purpose |
|---------|---------|
| [**Hydrogen.Windows**](src/Hydrogen.Windows/README.md) | Windows platform integration: registry, services, event logging |
| [**Hydrogen.Windows.Forms**](src/Hydrogen.Windows.Forms/README.md) | Windows Forms UI framework and components |
| [**Hydrogen.Windows.Forms.Sqlite**](src/Hydrogen.Windows.Forms.Sqlite/README.md) | Windows Forms with SQLite data binding |
| [**Hydrogen.Windows.Forms.Firebird**](src/Hydrogen.Windows.Forms.Firebird/README.md) | Windows Forms with Firebird data binding |
| [**Hydrogen.Windows.Forms.MSSQL**](src/Hydrogen.Windows.Forms.MSSQL/README.md) | Windows Forms with SQL Server data binding |
| [**Hydrogen.Windows.LevelDB**](src/Hydrogen.Windows.LevelDB/README.md) | LevelDB integration for fast key-value storage |

### 🌐 Web & Cross-Platform

| Project | Purpose |
|---------|---------|
| [**Hydrogen.Web.AspNetCore**](src/Hydrogen.Web.AspNetCore/README.md) | ASP.NET Core integration: middleware, filters, routing, forms |
| [**Hydrogen.Drawing**](src/Hydrogen.Drawing/README.md) | Cross-platform graphics and drawing utilities |
| [**Hydrogen.NET**](src/Hydrogen.NET/README.md) | .NET Framework-specific utilities and type introspection |
| [**Hydrogen.NETCore**](src/Hydrogen.NETCore/README.md) | .NET Core and modern .NET utilities |
| [**Hydrogen.iOS**](src/Hydrogen.iOS/README.md) | Xamarin.iOS integration for native iOS apps |
| [**Hydrogen.Android**](src/Hydrogen.Android/README.md) | Xamarin.Android integration for native Android apps |
| [**Hydrogen.macOS**](src/Hydrogen.macOS/README.md) | Xamarin.macOS integration for native macOS apps |

### ⛓️ Blockchain & DApps

| Project | Purpose |
|---------|---------|
| [**Hydrogen.DApp.Core**](src/Hydrogen.DApp.Core/README.md) | Core blockchain and DApp framework: blocks, transactions, wallets, plugins, persistence |
| [**Hydrogen.DApp.Node**](src/Hydrogen.DApp.Node/README.md) | Full-featured blockchain node with terminal UI, consensus, networking, wallet, and JSON APIs |
| [**Hydrogen.DApp.Host**](src/Hydrogen.DApp.Host/README.md) | Host process for running DApp nodes as services |

## ✅ Test Projects

The `tests/` directory contains **2000+ comprehensive tests** covering all framework subsystems:


| Test Project | Purpose |
|--------------|---------|
| **HashLib4CSharp.Tests** | Tests for hashing algorithms |
| **Hydrogen.Communications.Tests** | Networking and RPC tests |
| **Hydrogen.CryptoEx.Tests** | Cryptography implementation tests |
| **Hydrogen.DApp.Core.Tests** | DApp framework tests |
| **Hydrogen.Data.Tests** | Database access layer tests |
| **Hydrogen.NET.Tests** | .NET framework utility tests |
| **Hydrogen.NETCore.Tests** | .NET Core utility tests |
| **Hydrogen.Tests** | Core framework tests |
| **Hydrogen.Windows.LevelDB.Tests** | LevelDB integration tests |
| **Hydrogen.Windows.Tests** | Windows platform tests |

## 🎨 Presentation & UI Layer

The `blackhole/` directory contains the Blazor-based presentation layer and alternative implementations:

| Project | Purpose |
|---------|---------|
| [**Hydrogen.DApp.Presentation**](blackhole/Hydrogen.DApp.Presentation/README.md) | Blazor component library with wizards, modals, grids, and UI components |
| [**Hydrogen.DApp.Presentation.Loader**](blackhole/Hydrogen.DApp.Presentation.Loader/README.md) | Blazor WebAssembly host for the presentation layer |
| [**Hydrogen.DApp.Presentation.WidgetGallery**](blackhole/Hydrogen.DApp.Presentation.WidgetGallery/README.md) | Component showcase and demonstration plugin |
| [**Hydrogen.DApp.Presentation2**](blackhole/Hydrogen.DApp.Presentation2/README.md) | Alternative presentation implementation with advanced patterns |
| [**Hydrogen.DApp.Presentation2.Loader**](blackhole/Hydrogen.DApp.Presentation2.Loader/README.md) | WebAssembly host for the alternative presentation |

## � Documentation & Learning

### 🏗️ Architecture

1. [What is the Hydrogen Framework?](docs/Architecture/Hydrogen.md)
2. [Hydrogen Runtime](docs/Architecture/Runtime.md)
3. [Framework Domains](docs/Architecture/Domains.md)

### ⛓️ Blockchain Technology

4. [Blockchain: Dynamic Merkle Trees](https://sphere10.com/tech/dynamic-merkle-trees)
5. [Blockchain: Real-Time Targeted Difficulty Adjustment Algorithm](blackhole/docs/Blockchain/rtt-asert.pdf)
6. [Post-Quantum Cryptography: Abstract Merkle Signatures (AMS)](https://sphere10.com/tech/ams)
7. [Post-Quantum Cryptography: Winternitz Abstracted Merkle Signatures (WAMS)](https://sphere10.com/tech/wams)
8. [Post-Quantum Cryptography: Faster and Smaller Winternitz Signatures](https://sphere10.com/tech/wots-sharp)

### 📋 Guidelines

9. [What is a 3-tier Architecture?](docs/Guidelines/3-tier-Architecture.md)
10. [Code-Styling Guidelines](docs/Guidelines/Code-Styling.md)

## � Quick Navigation & Resources

- **Getting Started**: See [Hydrogen.DApp.Node](src/Hydrogen.DApp.Node/README.md) to run a blockchain node
- **Web UI**: See [Hydrogen.DApp.Presentation.Loader](blackhole/Hydrogen.DApp.Presentation.Loader/README.md) for the web interface
- **Building DApps**: See [Hydrogen.DApp.Core](src/Hydrogen.DApp.Core/README.md) for core DApp development
- **Database Access**: See [Hydrogen.Data](src/Hydrogen.Data/README.md) for data layer patterns
- **Networking**: See [Hydrogen.Communications](src/Hydrogen.Communications/README.md) for network protocols