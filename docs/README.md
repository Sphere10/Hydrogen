# Hydrogen Framework Documentation

**Copyright © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.**

Complete documentation for the Hydrogen framework — a comprehensive .NET 5+ framework for building blockchain applications, DApps, and distributed systems.

---

## 📖 Quick Navigation

### Essential Reading

- **[START-HERE.md](START-HERE.md)** — Begin here for framework overview and quick navigation
- **[DApp-Development-Guide.md](DApp-Development-Guide.md)** — Complete guide for building blockchain applications

### Architecture & Design

- **[Architecture/Hydrogen.md](Architecture/Hydrogen.md)** — Framework composition and core concepts
- **[Architecture/Runtime.md](Architecture/Runtime.md)** — Deployment model, lifecycle, and HAP system
- **[Architecture/Domains.md](Architecture/Domains.md)** — Catalog of all framework domains
- **[Guidelines/3-tier-Architecture.md](Guidelines/3-tier-Architecture.md)** — Architectural patterns and design principles
- **[Guidelines/Code-Styling.md](Guidelines/Code-Styling.md)** — Coding standards and conventions

### Component Documentation

- **[PresentationLayer/README.md](PresentationLayer/README.md)** — Blazor-based UI framework and components

---

## 🎯 By Role

### Developers

**Building a DApp?**
1. Start with [START-HERE.md](START-HERE.md)
2. Read [DApp-Development-Guide.md](DApp-Development-Guide.md)
3. Reference [src/Hydrogen.DApp.Core/README.md](../src/Hydrogen.DApp.Core/README.md)
4. Review [src/Hydrogen.DApp.Node/README.md](../src/Hydrogen.DApp.Node/README.md)

**Working with Collections?**
- [src/Hydrogen/README.md](../src/Hydrogen/README.md)
- [src/Hydrogen.Data/README.md](../src/Hydrogen.Data/README.md)

**Implementing Cryptography?**
- [src/Hydrogen.CryptoEx/README.md](../src/Hydrogen.CryptoEx/README.md)

**Building Networking/RPC?**
- [src/Hydrogen.Communications/README.md](../src/Hydrogen.Communications/README.md)

### Architects

**Designing a system?**
1. [Architecture/Hydrogen.md](Architecture/Hydrogen.md) — Understand framework composition
2. [Architecture/Runtime.md](Architecture/Runtime.md) — Learn deployment model
3. [Guidelines/3-tier-Architecture.md](Guidelines/3-tier-Architecture.md) — Study architectural patterns
4. [DApp-Development-Guide.md](DApp-Development-Guide.md) — Review integration patterns

**Planning infrastructure?**
- [Architecture/Runtime.md](Architecture/Runtime.md) — Node, Host, and HAP architecture
- [src/Hydrogen.DApp.Host/README.md](../src/Hydrogen.DApp.Host/README.md) — Host process management
- [src/Hydrogen.DApp.Node/README.md](../src/Hydrogen.DApp.Node/README.md) — Node architecture

### UI Developers

**Building a UI?**
1. [PresentationLayer/README.md](PresentationLayer/README.md) — Component library and patterns
2. [src/Hydrogen.DApp.Presentation/README.md](../src/Hydrogen.DApp.Presentation/README.md) — Presentation framework
3. [DApp-Development-Guide.md](DApp-Development-Guide.md#plugin-architecture) — Plugin system

---

## 📂 Documentation Structure

```
docs/
├── README.md (you are here)
├── START-HERE.md
├── DApp-Development-Guide.md
├── Architecture/
│   ├── Hydrogen.md
│   ├── Runtime.md
│   ├── Domains.md
│   └── resources/
├── Guidelines/
│   ├── 3-tier-Architecture.md
│   ├── Code-Styling.md
│   └── resources/
├── Education/
│   └── README.md
└── PresentationLayer/
    ├── README.md
    ├── Hydrogen-Requirements.md
    ├── Design/
    └── resources/
```

---

## 🔍 By Topic

| Topic | Documentation |
|-------|-----------------|
| **Framework Overview** | [Architecture/Hydrogen.md](Architecture/Hydrogen.md) |
| **DApp Development** | [DApp-Development-Guide.md](DApp-Development-Guide.md) |
| **Deployment & Runtime** | [Architecture/Runtime.md](Architecture/Runtime.md) |
| **Architecture Patterns** | [Guidelines/3-tier-Architecture.md](Guidelines/3-tier-Architecture.md) |
| **Code Standards** | [Guidelines/Code-Styling.md](Guidelines/Code-Styling.md) |
| **Domain Catalog** | [Architecture/Domains.md](Architecture/Domains.md) |
| **UI Components** | [PresentationLayer/README.md](PresentationLayer/README.md) |
| **Collections & Data** | [src/Hydrogen/README.md](../src/Hydrogen/README.md) |
| **Cryptography** | [src/Hydrogen.CryptoEx/README.md](../src/Hydrogen.CryptoEx/README.md) |
| **Consensus** | [DApp-Development-Guide.md](DApp-Development-Guide.md#consensus-integration) |
| **Networking** | [src/Hydrogen.Communications/README.md](../src/Hydrogen.Communications/README.md) |

---

## 📚 Core Domains

The Hydrogen framework provides 30+ domains across multiple categories:

### Collections & Data Structures
- Collections (maps, lists, sets, trees)
- Serialization and deserialization
- Data access (SQL, NoSQL, file-based)
- Object spaces and consensus streams

### Cryptography & Security
- Signatures (ECDSA, EdDSA, DSS, RSA)
- Encryption (AES, RSA, ECC)
- Hashing (SHA256, Keccak, Blake)
- Key derivation and management
- Zero-knowledge proofs

### Networking & Communication
- P2P protocols
- JSON RPC services
- WebSocket support
- Message routing

### Blockchain & Consensus
- Block validation
- Transaction processing
- Consensus mechanisms (PoW, PoS)
- Merkle trees and proofs
- Smart contracts

### Application Framework
- Dependency injection
- Configuration management
- Plugin architecture
- Lifecycle management
- Event system

See [Architecture/Domains.md](Architecture/Domains.md) for the complete catalog.

---

## 🚀 Getting Started

### First Time?

1. Read [START-HERE.md](START-HERE.md) for orientation
2. Review [Architecture/Hydrogen.md](Architecture/Hydrogen.md) for framework overview
3. Choose your path:
   - **Building a DApp**: [DApp-Development-Guide.md](DApp-Development-Guide.md)
   - **Designing a system**: [Guidelines/3-tier-Architecture.md](Guidelines/3-tier-Architecture.md)
   - **Building a UI**: [PresentationLayer/README.md](PresentationLayer/README.md)

### Project-Specific Documentation

Each project in `/src` has its own README with examples and usage patterns:

**Core Framework**
- [Hydrogen](../src/Hydrogen/README.md) — Collections, utilities, core types
- [Hydrogen.NET](../src/Hydrogen.NET/README.md) — .NET-specific utilities
- [Hydrogen.NETCore](../src/Hydrogen.NETCore/README.md) — .NET Core extensions

**Data & Storage**
- [Hydrogen.Data](../src/Hydrogen.Data/README.md) — Database abstraction
- [Hydrogen.Data.Sqlite](../src/Hydrogen.Data.Sqlite/README.md) — SQLite implementation
- [Hydrogen.Data.MSSQL](../src/Hydrogen.Data.MSSQL/README.md) — SQL Server implementation
- [Hydrogen.Data.Firebird](../src/Hydrogen.Data.Firebird/README.md) — Firebird implementation

**Cryptography**
- [Hydrogen.CryptoEx](../src/Hydrogen.CryptoEx/README.md) — Advanced cryptography

**DApp Framework**
- [Hydrogen.DApp.Core](../src/Hydrogen.DApp.Core/README.md) — Blockchain core
- [Hydrogen.DApp.Node](../src/Hydrogen.DApp.Node/README.md) — Node implementation
- [Hydrogen.DApp.Host](../src/Hydrogen.DApp.Host/README.md) — Host management
- [Hydrogen.DApp.Presentation](../src/Hydrogen.DApp.Presentation/README.md) — UI framework

**UI & Presentation**
- [Hydrogen.DApp.Presentation](../src/Hydrogen.DApp.Presentation/README.md) — Blazor components
- [Hydrogen.DApp.Presentation.Loader](../src/Hydrogen.DApp.Presentation.Loader/README.md) — Web app loader

**Networking**
- [Hydrogen.Communications](../src/Hydrogen.Communications/README.md) — RPC and messaging
- [Hydrogen.Web.AspNetCore](../src/Hydrogen.Web.AspNetCore/README.md) — ASP.NET Core integration

**Platform-Specific**
- [Hydrogen.Windows](../src/Hydrogen.Windows/README.md) — Windows integration
- [Hydrogen.Drawing](../src/Hydrogen.Drawing/README.md) — Graphics and drawing

---

## ❓ Common Questions

**Where do I start?**
→ [START-HERE.md](START-HERE.md)

**How do I build a DApp?**
→ [DApp-Development-Guide.md](DApp-Development-Guide.md)

**What's the framework architecture?**
→ [Architecture/Hydrogen.md](Architecture/Hydrogen.md)

**How does deployment work?**
→ [Architecture/Runtime.md](Architecture/Runtime.md)

**What design patterns should I use?**
→ [Guidelines/3-tier-Architecture.md](Guidelines/3-tier-Architecture.md)

**How do I structure my code?**
→ [Guidelines/Code-Styling.md](Guidelines/Code-Styling.md)

**What UI components are available?**
→ [PresentationLayer/README.md](PresentationLayer/README.md)

---

## 📖 Reading Guide

### For Understanding the Framework

1. [Architecture/Hydrogen.md](Architecture/Hydrogen.md) — What Hydrogen is and how it's organized
2. [Architecture/Domains.md](Architecture/Domains.md) — What domains are available
3. [Guidelines/3-tier-Architecture.md](Guidelines/3-tier-Architecture.md) — How to structure applications

### For Building Applications

1. [DApp-Development-Guide.md](DApp-Development-Guide.md) — Complete DApp development reference
2. Project-specific READMEs in `/src` — Domain-specific guidance
3. [Guidelines/Code-Styling.md](Guidelines/Code-Styling.md) — Code standards

### For System Design

1. [Architecture/Hydrogen.md](Architecture/Hydrogen.md) — Framework composition
2. [Architecture/Runtime.md](Architecture/Runtime.md) — Deployment and runtime model
3. [Guidelines/3-tier-Architecture.md](Guidelines/3-tier-Architecture.md) — Architectural patterns
4. [DApp-Development-Guide.md](DApp-Development-Guide.md) — Integration patterns

### For UI Development

1. [PresentationLayer/README.md](PresentationLayer/README.md) — Component library
2. [src/Hydrogen.DApp.Presentation/README.md](../src/Hydrogen.DApp.Presentation/README.md) — Framework details

---

## 🔗 Related Resources

- **[GitHub Repository](https://github.com/Sphere10/Hydrogen)** — Source code
- **[NuGet Packages](https://www.nuget.org/packages?q=Hydrogen)** — Published packages
- **[Source Code Documentation](../src/README.md)** — Project structure

---

**Version**: 2.0  
**Last Updated**: December 2025  
**Author**: Sphere 10 Software
