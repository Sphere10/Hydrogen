# Domains

A [domain](../Guidelines/3-tier-Architecture.md#domain) consists of a collection of [modules](../Guidelines/3-tier-Architecture.md#module) that are all logically related in the abstraction they model. A domain can span across multiple tiers depending on the functionality they provide (i.e. user interface, business component, etc). A domain can be visualized as a vertical slice through the [3-tier architecture](../Guidelines/3-tier-Architecture.md). Modules are a .NET project that typically contain multiple domains each under a sub-folder.  Below are domains available in the Hydrogen framework.

## Sphere 10 Framework Domains

Sphere 10 Framework provides modules across all 3 tiers of the architecture. Of significance is the `Sphere10.Framework` module which provides system-tier functionality that can be used anywhere in an application (from server-end to front-end including within Blazor web-assembly code). This module should be considered like an extension to the system .NET library.

| Domain            | Tiers              | Module / Location                                            | Functionality                                                |
| ----------------- | ------------------ | ------------------------------------------------------------ | ------------------------------------------------------------ |
| **Cache**         | System             | [Sphere10.Framework/Cache](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Cache) | Caching abstractions, batch caching, caching policies (least-used, largest, smallest, etc), bulk caching, session caching. |
| **Collections**   | System             | [Sphere10.Framework/Collections](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Collections) | Arrays, buffers, paged collections, transactional collections, stream mapped collections, transactional collections, observable collections, dictionary implementations (b-tree, etc) as well as merkle trees. |
| **Comparers**     | System             | [Sphere10.Framework/Comparers](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Comparers) | Assortment of Comparers                                      |
| **Conversion**    | System             | [Sphere10.Framework/Conversion](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Conversion) | Primarily Endian-based conversion and other.                 |
| **Cryptography**  | System             | [Sphere10.Framework/Crypto](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Crypto) | Cryptographic primitives, checksums, encryption, key-derivation functions, post-quantum cryptography. |
| **Encoding**      | System             | [Sphere10.Framework/Encoding](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Encoding) | Hexadecimal.                                                 |
| **Environment**   | System             | [Sphere10.Framework/Environment](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Environment) | Support for OS and runtime-level concerns.                   |
| **Events**        | System             | [Sphere10.Framework/Events](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Events) | Support for all things `event`.                              |
| **Exceptions**    | System             | [Sphere10.Framework/Exceptions](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Exceptions) | Collection of exceptions and exception tools.                |
| **Extensions**    | System             | [Sphere10.Framework/Extensions](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Extensions) | Vast collection of extension methods for .NET framework.     |
| **Functional**    | System             | [Sphere10.Framework/Functional](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Functional) | Support for functional programming including tools for expressions, lambdas, operators. |
| **Introspection** | System             | [Sphere10.Framework/Conversion](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Conversion) | Fast reflection library and tools.                           |
| **IO**            | System             | [Sphere10.Framework/IO](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/IO) | Endian-aware IO, file stores and file-system tools.          |
| **IoC**           | System, Processing | [Sphere10.Framework/IoC](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/IoC)<br />[Sphere10.Framework.Application/ComponentRegistry](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework.Application/ComponentRegistry) | Framework-wide Inversion of Control container (uses [TinyIoC](https://github.com/grumpydev/TinyIoC) under the hood). |
| **Logging**       | System             | [Sphere10.Framework/Logging](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Logging) | Support for all things logging.                              |
| **Math**          | System             | [Sphere10.Framework/Maths](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Maths) | Math abstractions and tools, bloom filters, fixed point math, random number generators and other interesting math artefacts. |
| **Memory**        | System             | [Sphere10.Framework/Memory](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Memory) | Minor tooling for memory unit conversion and other obscure tools. |
| **Misc**          | System             | [Sphere10.Framework/Misc](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Misc) | Miscellaneous tools that don't belong in known domain.       |
| **Networking**    | System             | [Sphere10.Framework/Network](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Network) | All things networking, a full P2P abstraction library (TODO) as well tools for URL's, Mimes and POP3. |
| **Objects**       | System             | [Sphere10.Framework/Objects](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Objects) | Assortment of tools that deal with Objects.                  |
| **ObjectSpaces**  | System             | [Sphere10.Framework/ObjectSpace](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/ObjectSpace) | A storage-engine for objects, used within Hydrogen.          |
| **Peripherals**   | System             | [Sphere10.Framework/Peripherals](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Peripherals) | Tooling and abstractions for keyboard, mouse and hooks.      |
| **Scheduler**     | System             | [Sphere10.Framework/Scheduler](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Scheduler) | A background scheduler to run code based on time rules.      |
| **Scopes**        | System             | [Sphere10.Framework/Scopes](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Scopes) | Broad support for scope-based patterns.                      |
| **Serialization** | System             | [Sphere10.Framework/Serialization](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Serialization) | Broad support for object serialization.                      |
| **Streams**       | System             | [Sphere10.Framework/Streams](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Streams) | Tooling for Streams including bit streams, blocking streams, bounded streams, stream pipelines. |
| **Text**          | System             | [Sphere10.Framework/Text](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Text) | All things text including parsers, inflectors, fast string builder, fluent regex builders, obscure html tools. |
| **TextWriters**   | System             | [Sphere10.Framework/TextWriters](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/TextWriters) | Assortment of `TextWriter`'s including console, debug, file-based, etc. |
| **Threading**     | System             | [Sphere10.Framework/Threading](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Threading) | Threading tools for multi-threaded applications.             |
| **Types**         | System             | [Sphere10.Framework/Types](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Types) | Tools for `Type` including activation, resolution and switches. |
| **Values**        | System             | [Sphere10.Framework/Values](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Values) | Tools for futures, GUIDs, Enums, Results, Value Ranges.      |
| **XML**           | System             | [Sphere10.Framework/XML](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/XML) | Tools for XML including deep object serialization.           |



## Hydrogen Domains

| Domain            | Tier               | Module/Area | Notes                                                        |
| ----------------- | ------------------ | ----------- | ------------------------------------------------------------ |
| **Host**          | Presentation, Core |             | Handles the execution and lifecycle of Hydrogen application packages |
| **GUI Core**      | Presentation       |             | The core Blazor-based UI. UI Skeleton, bootstrap, navigation, modals, etc |
| **Wizards**       | Presentation       |             | Collection of components to compose wizard UI flows via Blazor |
| **Grids**         | Presentation       |             | Collection of components for grids in Blazor                 |
| **Plugins**       | Presentation, Core |             | Plugin extensions for node and GUI                           |
| **Node**          | Presentation, Core |             | The node daemon/service                                      |
| **Consensus**     | Core               |             | Components for consensus rules                               |
| **Cryptography**  | Core               |             | Digital signature schemes for ECDSA and PQC                  |
| **Object Spaces** | Core               |             | Decentralized database technology                            |
| **Blockchain**    | Core               |             | Consensus-stream based blockchain components                 |
| **Wallet**        | Core               |             | Managing and organizing user keys                            |
| **Networking**    | Core               |             | P2P networking library                                       |

 

## Helium Domains

**Conceptual Overview (Technical)**

This is a technial overview that shows the basic internal components of the Helium system.
![ConceptualOverview](https://user-images.githubusercontent.com/79815312/116169619-79548c00-a748-11eb-8448-ff332d33b445.jpeg)
