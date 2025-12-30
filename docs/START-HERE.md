# Hydrogen Framework - Documentation Guide

**Welcome to the Hydrogen Framework Documentation!**

This is your starting point for understanding the Hydrogen framework, its architecture, and how to use it.

## Quick Navigation

### 📚 Essential Documentation

**New to Hydrogen?** Start here:
1. [What is Hydrogen?](Architecture/Hydrogen.md#what-is-hydrogen) - Overview and core concepts
2. [Framework Composition](Architecture/Hydrogen.md#framework-composition) - Three sub-frameworks explained
3. [3-Tier Architecture Guide](Guidelines/3-tier-Architecture.md) - Architectural principles

**Understanding the System?** Read these:
- [Domains Reference](Architecture/Domains.md) - Complete catalog of all framework domains
- [Runtime Model](Architecture/Runtime.md) - Deployment, lifecycle, and upgrade mechanisms
- [Code Styling Guide](Guidelines/Code-Styling.md) - Coding standards and best practices

**Learning Blockchain Concepts?**
- [What is Blockchain?](Education/What-is-Blockchain.md) - Comprehensive blockchain education
- [Blockchain Consensus](Architecture/Runtime.md#consensus-mechanisms) - Consensus mechanisms in Hydrogen
- [DApp Development](DApp-Development-Guide.md) - Building DApps on Hydrogen

### 🎯 By Use Case

| Use Case | Start Here |
|----------|-----------|
| **Building a Blockchain DApp** | [Hydrogen.DApp.Core README](../src/Hydrogen.DApp.Core/README.md) |
| **Creating a Node** | [Hydrogen.DApp.Node README](../src/Hydrogen.DApp.Node/README.md) |
| **Building a UI/GUI** | [Hydrogen.DApp.Presentation README](../src/Hydrogen.DApp.Presentation/README.md) |
| **Using Collections & Data** | [Hydrogen.Data README](../src/Hydrogen.Data/README.md) |
| **Cryptography & Security** | [Hydrogen.CryptoEx README](../src/Hydrogen.CryptoEx/README.md) |
| **Networking & RPC** | [Hydrogen.Communications README](../src/Hydrogen.Communications/README.md) |
| **General Utilities** | [Hydrogen Core README](../src/Hydrogen/README.md) |

### 📂 Documentation Structure

```
docs/
├── Architecture/
│   ├── Hydrogen.md           → Framework overview & vision
│   ├── Domains.md            → Catalog of all domains
│   ├── Runtime.md            → Deployment model & lifecycle
│   └── resources/            → Diagrams and images
├── Guidelines/
│   ├── 3-tier-Architecture.md → Architectural patterns
│   ├── Code-Styling.md       → Coding standards
│   └── resources/            → Config files & images
├── Education/
│   ├── README.md             → Learning resources hub
│   ├── What-is-Blockchain.md → Blockchain fundamentals
│   └── resources/            → Educational diagrams
├── Helium/
│   ├── README.md             → ESB framework overview
│   └── resources/            → Helium diagrams
├── PresentationLayer/
│   ├── Hydrogen-Requirements.md → UI requirements & design
│   ├── Design/               → UI design specifications
│   └── resources/            → UI mockups & designs
└── START-HERE.md             ← You are here
```

## Documentation Topics

### Architecture & Design (docs/Architecture/)

**[Hydrogen.md](Architecture/Hydrogen.md)**
- What is Hydrogen and why it exists
- Three sub-frameworks (Hydrogen, Helium, Hydrogen.DApp)
- Key features and capabilities
- Architecture layers and deployment model
- Use cases and when to use Hydrogen

**[Domains.md](Architecture/Domains.md)**
- Complete reference of 30+ framework domains
- Domain purpose and locations
- Which domains to use for specific tasks
- Domain relationships and dependencies

**[Runtime.md](Architecture/Runtime.md)**
- Hydrogen Application Package (HAP) specification
- Host, Node, and GUI architecture
- Application folder structure
- HAP lifecycle and state transitions
- Host Protocol for inter-process communication
- Upgrade mechanisms (in-protocol upgrades)

### Guidelines & Standards (docs/Guidelines/)

**[3-Tier Architecture](Guidelines/3-tier-Architecture.md)**
- Presentation, Processing, Data tiers
- Ancillary tiers (Communications, Data Objects, System)
- Domain vs Module vs Framework concepts
- Naming conventions
- Design patterns for tier separation

**[Code Styling](Guidelines/Code-Styling.md)**
- Brace placement and indentation
- Naming conventions (PascalCase, snake_case, etc.)
- Member ordering in classes
- Namespace organization
- Comment style
- Performance considerations

### Education (docs/Education/)

**[What is Blockchain?](Education/What-is-Blockchain.md)**
- Blockchain fundamentals
- Proof-of-Work and Proof-of-Stake
- Mining, difficulty, and hash power
- Consensus mechanisms
- Transaction models (UTXO vs Account)
- Digital signatures and cryptography
- Network attacks and mitigation
- Wallet security (hot/cold)

### Framework-Specific (docs/Helium/, docs/PresentationLayer/)

**Helium ESB Framework**
- Enterprise service bus patterns
- Message routing and distribution
- Publish-Subscribe and Request-Response
- Distributed sagas and event sourcing

**Presentation Layer**
- Blazor component architecture
- UI framework requirements
- Widget library
- Responsive design
- Plugin architecture for UI extensions

## Quick Answer to Common Questions

**Q: What is Hydrogen?**  
A: Hydrogen is a comprehensive .NET framework for building distributed applications, originally designed for blockchain but applicable to any sophisticated system.

**Q: Should I use Hydrogen for my project?**  
A: If you need advanced data structures, cryptography, multi-database support, or cross-platform capabilities—yes. See [Use Cases](Architecture/Hydrogen.md#use-cases).

**Q: How do I build a blockchain on Hydrogen?**  
A: Start with [Hydrogen.DApp.Core](../src/Hydrogen.DApp.Core/README.md), review [Consensus documentation](Architecture/Runtime.md#consensus-mechanisms), and follow [DApp Development Guide](DApp-Development-Guide.md).

**Q: Where are the code examples?**  
A: Each project's README in `/src/` contains extensive examples. Also check `/tests/` for working test patterns.

**Q: How do I understand the architecture?**  
A: Read [3-Tier Architecture](Guidelines/3-tier-Architecture.md) first, then [Domains](Architecture/Domains.md), then specific domain documentation.

**Q: What are the coding standards?**  
A: See [Code Styling Guide](Guidelines/Code-Styling.md) for comprehensive guidelines.

**Q: How does in-protocol upgrading work?**  
A: See [Runtime Model - Upgrade Mechanism](Architecture/Runtime.md#upgrade-mechanism).

**Q: Can I use Hydrogen without blockchains?**  
A: Absolutely! The base `Hydrogen.*` framework has no blockchain dependencies. Use just the parts you need.

## For Different Roles

### 👨‍💻 Developers
1. Read [3-Tier Architecture](Guidelines/3-tier-Architecture.md)
2. Review [Code Styling Guidelines](Guidelines/Code-Styling.md)
3. Explore domain-specific README files in `/src/`
4. Check test files in `/tests/` for code patterns

### 🏗️ Architects
1. Understand [Hydrogen Framework Overview](Architecture/Hydrogen.md)
2. Study [3-Tier Architecture & Design Patterns](Guidelines/3-tier-Architecture.md)
3. Review [Domains Reference](Architecture/Domains.md)
4. Examine [Runtime Model](Architecture/Runtime.md)

### 🔗 DApp/Blockchain Builders
1. Start with [What is Blockchain?](Education/What-is-Blockchain.md)
2. Review [Hydrogen DApp Framework Components](Architecture/Hydrogen.md#3-hydrogen-dapp-framework)
3. Study [Runtime Model - HAP Lifecycle](Architecture/Runtime.md#hap-lifecycle)
4. Explore [Hydrogen.DApp.Core](../src/Hydrogen.DApp.Core/README.md)

### 🎨 UI/Frontend Developers
1. Review [Presentation Layer Requirements](PresentationLayer/Hydrogen-Requirements.md)
2. Check [Blazor Component Guide](../src/Hydrogen.DApp.Presentation/README.md)
3. Study responsive design in [Design docs](PresentationLayer/Design/)
4. Explore widget library examples

### 📚 System/Database Designers
1. Review [Data Tier Documentation](Guidelines/3-tier-Architecture.md#tier-3-data)
2. Explore [Hydrogen.Data README](../src/Hydrogen.Data/README.md)
3. Check database-specific READMEs (Sqlite, MSSQL, Firebird, NHibernate)
4. Understand object spaces for distributed state

## Learning Paths

### Path 1: General .NET Development
```
3-Tier Architecture 
  → Domains Overview
  → Code Styling
  → Project-specific READMEs
  → Working Code Examples
```

### Path 2: Blockchain DApp Development
```
What is Blockchain
  → Hydrogen Framework Overview
  → Runtime Model
  → Hydrogen.DApp.Core
  → Hydrogen.DApp.Node
  → DApp Development Guide
  → Building Your First DApp
```

### Path 3: Distributed Systems
```
Hydrogen Overview
  → Helium Framework
  → Messaging Patterns
  → Event Sourcing
  → Building Distributed Systems
```

## Key Concepts to Understand

| Concept | Learn Here |
|---------|-----------|
| **Domain** | [3-Tier Architecture - Domains](Guidelines/3-tier-Architecture.md#domain) |
| **Module** | [3-Tier Architecture - Modules](Guidelines/3-tier-Architecture.md#module) |
| **3-Tier Architecture** | [Complete Guide](Guidelines/3-tier-Architecture.md) |
| **HAP (Hydrogen Application Package)** | [Runtime Model](Architecture/Runtime.md#hydrogen-application-package-hap) |
| **Host Protocol** | [Runtime Model - Host Protocol](Architecture/Runtime.md#host-protocol) |
| **Plugin System** | [Runtime Model - Plugins](Architecture/Runtime.md#plugin-system) |
| **Consensus Mechanisms** | [Runtime Model & Blockchain Ed](Education/What-is-Blockchain.md) |
| **Merkle Trees** | [Collections Domain](Architecture/Domains.md#collections) & [Education](Education/What-is-Blockchain.md#merkle-root) |

## Contributing to Documentation

Found an issue or want to improve documentation?
- Review [Contributing Guidelines](../CONTRIBUTING.md)
- Check current documentation structure
- Maintain consistency with existing style
- Update related cross-references
- Include code examples where relevant

## Document Versions & Updates

**Current Documentation Version**: 2.0+  
**Last Updated**: December 2025  
**Maintained By**: Herman Schoenfeld & Sphere 10 Software  

Documentation is maintained alongside code. When code changes, documentation is updated accordingly.

## Copyright & License

Copyright © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.

Licensed under the MIT NON-AI License. See LICENSE file for details.

---

**Ready to dive in?** Start with [What is Hydrogen?](Architecture/Hydrogen.md)
