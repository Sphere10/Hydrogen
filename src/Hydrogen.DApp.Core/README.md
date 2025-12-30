# ⛓️ Hydrogen.DApp.Core

**Core blockchain and distributed application framework** providing fundamental types, patterns, and infrastructure for building blockchain-based DApps with plugin support.

## 📋 Overview

`Hydrogen.DApp.Core` is the foundation for all distributed applications (DApps) built on Hydrogen. It provides essential types, protocols, and patterns for blockchain systems including transactions, blocks, wallets, and plugin management.

## 🏗️ Architecture

The library encompasses several critical domains:

- **Blockchain**: Block, transaction, and chain management
- **Wallet**: Key management, account handling, and signing
- **Plugins**: Plugin discovery, loading, and lifecycle management
- **Applications (Apps)**: DApp framework and application hosting
- **Persistence**: Data storage and retrieval abstractions
- **Networking**: Peer discovery and synchronization
- **Consensus**: Integration with consensus mechanisms

## 🚀 Key Features

- **Plugin Architecture**: Dynamically load and manage blockchain applications
- **In-Protocol Upgrades**: Applications upgraded through blockchain governance
- **Multi-Wallet Support**: Manage multiple wallets and accounts
- **Transaction Framework**: Flexible transaction creation and validation
- **Blockchain State**: Track and manage blockchain state
- **Event System**: React to blockchain events
- **Async Operations**: Full async/await support

## 🔧 Usage

Create a simple DApp:

```csharp
using Hydrogen.DApp.Core;

// Define your DApp
public class MyDApp : Plugin {
    public override void OnInitialize() {
        // Setup your application
    }
    
    public override void OnShutdown() {
        // Cleanup
    }
}

// Load and run
var appCore = new DAppCore();
appCore.LoadPlugin<MyDApp>();
```

## 📦 Dependencies

- **Hydrogen**: Core framework
- **Hydrogen.Application**: Application lifecycle
- **Hydrogen.Communications**: Network communication
- **Hydrogen.CryptoEx**: Cryptographic primitives
- **Hydrogen.Data**: Data persistence
- **BouncyCastle.Cryptography**: Cryptographic library
- **Newtonsoft.Json**: JSON serialization
- **SharpZipLib**: Compression support

## 📖 Architecture Documentation

See [What is the Hydrogen Framework?](../../docs/Architecture/Hydrogen.md) and [Framework Domains](../../docs/Architecture/Domains.md) for comprehensive architecture details.

## 📄 Related Projects

- [Hydrogen.DApp.Node](../Hydrogen.DApp.Node) - Reference node implementation
- [Hydrogen.Communications](../Hydrogen.Communications) - Network layer
- [Hydrogen.CryptoEx](../Hydrogen.CryptoEx) - Cryptographic functions
