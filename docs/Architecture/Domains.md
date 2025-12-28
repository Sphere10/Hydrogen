# Domains

The Hydrogen framework comprises of a variety of [modules](../Guidelines/3-tier-Architecture.md#module) and domains. A [domain](../Guidelines/3-tier-Architecture.md#domain) comprises of a collection of code artefacts that are all logically related in the abstraction they model. Whereas a module is a horizontal-slice of a single-tier, a domain can span across multiple tiers. This is so since a domain may comprise of UI, Business Logic and Data components.  A domain can be visualized as a vertical slice through the [3-tier architecture](../Guidelines/3-tier-Architecture.md). Modules are a .NET project that typically contain multiple domains each under a sub-folder.  Below are domains available in the Hydrogen framework.

## Hydrogen Framework Domains

Hydrogen Framework provides modules across all 3 tiers of the architecture. Of significance is the `Hydrogen` module which provides system-tier functionality that can be used anywhere in an application (from server-end to front-end including within Blazor web-assembly code). This module should be considered like an extension to the system .NET library.

| Domain            | Tiers              | Module / Location                                            | Functionality                                                |
| ----------------- | ------------------ | ------------------------------------------------------------ | ------------------------------------------------------------ |
| **Cache**         | System             | [Hydrogen/Cache](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Cache) | Caching abstractions, batch caching, caching policies (least-used, largest, smallest, etc), bulk caching, session caching. |
| **Collections**   | System             | [Hydrogen/Collections](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Collections) | Arrays, buffers, paged collections, transactional collections, stream mapped collections, transactional collections, observable collections, dictionary implementations (b-tree, etc) as well as merkle trees. |
| **Comparers**     | System             | [Hydrogen/Comparers](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Comparers) | Assortment of Comparers                                      |
| **Conversion**    | System             | [Hydrogen/Conversion](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Conversion) | Primarily Endian-based conversion and other.                 |
| **Cryptography**  | System             | [Hydrogen/Crypto](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Crypto) | Cryptographic primitives, checksums, encryption, key-derivation functions, post-quantum cryptography. |
| **Encoding**      | System             | [Hydrogen/Encoding](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Encoding) | Hexadecimal.                                                 |
| **Environment**   | System             | [Hydrogen/Environment](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Environment) | Support for OS and runtime-level concerns.                   |
| **Events**        | System             | [Hydrogen/Events](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Events) | Support for all things `event`.                              |
| **Exceptions**    | System             | [Hydrogen/Exceptions](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Exceptions) | Collection of exceptions and exception tools.                |
| **Extensions**    | System             | [Hydrogen/Extensions](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Extensions) | Vast collection of extension methods for .NET framework.     |
| **Functional**    | System             | [Hydrogen/Functional](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Functional) | Support for functional programming including tools for expressions, lambdas, operators. |
| **Introspection** | System             | [Hydrogen/Conversion](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Conversion) | Fast reflection library and tools.                           |
| **IO**            | System             | [Hydrogen/IO](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/IO) | Endian-aware IO, file stores and file-system tools.          |
| **IoC**           | System, Processing | [Hydrogen/IoC](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/IoC)<br />[Hydrogen.Application/ComponentRegistry](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen.Application/ComponentRegistry) | Framework-wide Inversion of Control container (uses [TinyIoC](https://github.com/grumpydev/TinyIoC) under the hood). |
| **Logging**       | System             | [Hydrogen/Logging](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Logging) | Support for all things logging.                              |
| **Math**          | System             | [Hydrogen/Maths](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Maths) | Math abstractions and tools, bloom filters, fixed point math, random number generators and other interesting math artefacts. |
| **Memory**        | System             | [Hydrogen/Memory](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Memory) | Minor tooling for memory unit conversion and other obscure tools. |
| **Misc**          | System             | [Hydrogen/Misc](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Misc) | Miscellaneous tools that don't belong in known domain.       |
| **Networking**    | System             | [Hydrogen/Network](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Network) | All things networking, a full P2P abstraction library (TODO) as well tools for URL's, Mimes and POP3. |
| **Objects**       | System             | [Hydrogen/Objects](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Objects) | Assortment of tools that deal with Objects.                  |
| **ObjectSpaces**  | System             | [Hydrogen/ObjectSpace](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/ObjectSpace) | A storage-engine for objects, used within Hydrogen.          |
| **Peripherals**   | System             | [Hydrogen/Peripherals](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Peripherals) | Tooling and abstractions for keyboard, mouse and hooks.      |
| **Scheduler**     | System             | [Hydrogen/Scheduler](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Scheduler) | A background scheduler to run code based on time rules.      |
| **Scopes**        | System             | [Hydrogen/Scopes](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Scopes) | Broad support for scope-based patterns.                      |
| **Serialization** | System             | [Hydrogen/Serialization](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Serialization) | Broad support for object serialization.                      |
| **Streams**       | System             | [Hydrogen/Streams](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Streams) | Tooling for Streams including bit streams, blocking streams, bounded streams, stream pipelines. |
| **Text**          | System             | [Hydrogen/Text](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Text) | All things text including parsers, inflectors, fast string builder, fluent regex builders, obscure html tools. |
| **TextWriters**   | System             | [Hydrogen/TextWriters](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/TextWriters) | Assortment of `TextWriter`'s including console, debug, file-based, etc. |
| **Threading**     | System             | [Hydrogen/Threading](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Threading) | Threading tools for multi-threaded applications.             |
| **Types**         | System             | [Hydrogen/Types](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Types) | Tools for `Type` including activation, resolution and switches. |
| **Values**        | System             | [Hydrogen/Values](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/Values) | Tools for futures, GUIDs, Enums, Results, Value Ranges.      |
| **XML**           | System             | [Hydrogen/XML](https://github.com/Sphere10/Hydrogen/tree/master/src/Hydrogen/XML) | Tools for XML including deep object serialization.           |



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
