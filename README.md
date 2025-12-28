<p align="center">
  <img src="resources/branding/hydrogen-white-bg.gif" alt="Hydrogen logo">
</p>

# Hydrogen: Comprehensive .NET Application Framework

Copyright (c) Sphere 10 Software 2018 - Present

Hydrogen is a mature .NET framework providing a foundation for building full-stack applications across desktop, mobile, and web platforms. It originated in blockchain systems and has grown into a general-purpose framework with strong primitives for data, networking, cryptography, and UI.

## What Hydrogen provides
- Core utilities: collections, serialization, streaming, caching, and diagnostics
- Data access: DB-agnostic abstractions with providers for SQLite, SQL Server, Firebird, and NHibernate
- Cryptography: hashing, signatures, and specialized algorithms for blockchain use cases
- Networking: TCP/UDP/RPC utilities and protocol abstractions
- UI: Windows Forms components plus ASP.NET Core integration

## Solutions
- `src/Hydrogen (Win).sln`: Windows and full-featured solution including UI and DApp projects
- `src/Hydrogen (CrossPlatform).sln`: cross-platform subset without Windows UI projects

## Project structure
- `src/`: libraries and applications
- `tests/`: unit/integration tests
- `utils/`: internal tools and test harnesses
- `recipes/`: sample projects and protocol recipes
- `resources/`: shared assets (branding, fonts, presentations)
- `docs/`: architecture and guidelines

## Core projects

### Core framework and utilities

| Project | Purpose |
|---------|---------|
| [Hydrogen](src/Hydrogen/README.md) | Core utilities: collections, serialization, streams, hashing |
| [Hydrogen.Application](src/Hydrogen.Application/README.md) | Application lifecycle, DI, settings, CLI support |
| [Hydrogen.Communications](src/Hydrogen.Communications/README.md) | RPC and protocol abstractions |
| [Hydrogen.Generators](src/Hydrogen.Generators/README.md) | C# source generators |
| [HashLib4CSharp](src/HashLib4CSharp/README.md) | Hashing and checksum algorithms |

### Cryptography and consensus

| Project | Purpose |
|---------|---------|
| [Hydrogen.CryptoEx](src/Hydrogen.CryptoEx/README.md) | Extended crypto and blockchain-focused primitives |
| [Hydrogen.Consensus](src/Hydrogen.Consensus/README.md) | Consensus abstractions and validation rules |

### Data access and persistence

| Project | Purpose |
|---------|---------|
| [Hydrogen.Data](src/Hydrogen.Data/README.md) | Data abstraction and SQL helpers |
| [Hydrogen.Data.Sqlite](src/Hydrogen.Data.Sqlite/README.md) | SQLite provider |
| [Hydrogen.Data.Firebird](src/Hydrogen.Data.Firebird/README.md) | Firebird provider |
| [Hydrogen.Data.MSSQL](src/Hydrogen.Data.MSSQL/README.md) | SQL Server provider |
| [Hydrogen.Data.NHibernate](src/Hydrogen.Data.NHibernate/README.md) | NHibernate integration |

### Desktop and Windows

| Project | Purpose |
|---------|---------|
| [Hydrogen.Windows](src/Hydrogen.Windows/README.md) | Windows platform utilities |
| [Hydrogen.Windows.Forms](src/Hydrogen.Windows.Forms/README.md) | WinForms UI framework |
| [Hydrogen.Windows.Forms.Sqlite](src/Hydrogen.Windows.Forms.Sqlite/README.md) | WinForms + SQLite binding |
| [Hydrogen.Windows.Forms.Firebird](src/Hydrogen.Windows.Forms.Firebird/README.md) | WinForms + Firebird binding |
| [Hydrogen.Windows.Forms.MSSQL](src/Hydrogen.Windows.Forms.MSSQL/README.md) | WinForms + SQL Server binding |
| [Hydrogen.Windows.LevelDB](src/Hydrogen.Windows.LevelDB/README.md) | LevelDB wrapper for Windows |

### Web and cross-platform

| Project | Purpose |
|---------|---------|
| [Hydrogen.Web.AspNetCore](src/Hydrogen.Web.AspNetCore/README.md) | ASP.NET Core integration |
| [Hydrogen.Drawing](src/Hydrogen.Drawing/README.md) | Drawing helpers |
| [Hydrogen.NET](src/Hydrogen.NET/README.md) | .NET Framework utilities |
| [Hydrogen.NETCore](src/Hydrogen.NETCore/README.md) | .NET Core and modern .NET utilities |
| [Hydrogen.iOS](src/Hydrogen.iOS/README.md) | iOS platform integration |
| [Hydrogen.Android](src/Hydrogen.Android/README.md) | Android platform integration |
| [Hydrogen.macOS](src/Hydrogen.macOS/README.md) | macOS platform integration |

### Blockchain and DApps

| Project | Purpose |
|---------|---------|
| [Hydrogen.DApp.Core](src/Hydrogen.DApp.Core/README.md) | DApp and blockchain core types |
| [Hydrogen.DApp.Node](src/Hydrogen.DApp.Node/README.md) | Node implementation |
| [Hydrogen.DApp.Host](src/Hydrogen.DApp.Host/README.md) | Node host process |

## Tests

Tests live under `tests/` and mirror the main projects (core, data, crypto, communications, Windows, and DApp components).

## Presentation layer (archived/experimental)

Projects under `blackhole/` contain older or experimental presentation layers:
- `blackhole/Hydrogen.DApp.Presentation`
- `blackhole/Hydrogen.DApp.Presentation.Loader`
- `blackhole/Hydrogen.DApp.Presentation.WidgetGallery`
- `blackhole/Hydrogen.DApp.Presentation2`
- `blackhole/Hydrogen.DApp.Presentation2.Loader`

## Documentation

Architecture and guidelines live under `docs/`:
- `docs/Architecture/Hydrogen.md`
- `docs/Architecture/Runtime.md`
- `docs/Architecture/Domains.md`
- `docs/Guidelines/3-tier-Architecture.md`
- `docs/Guidelines/Code-Styling.md`

## Quick navigation
- Getting started with the node: `src/Hydrogen.DApp.Node/README.md`
- Core framework: `src/Hydrogen/README.md`
- Data access: `src/Hydrogen.Data/README.md`
- Networking/RPC: `src/Hydrogen.Communications/README.md`
- Windows UI: `src/Hydrogen.Windows.Forms/README.md`
