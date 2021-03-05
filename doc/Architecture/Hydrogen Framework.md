# Framework Architecture



The hydrogen framework is composed of 3 sub-frameworks.

| Framework           | Naming Conventions   | Purpose                                                      |
| ------------------- | -------------------- | ------------------------------------------------------------ |
| Sphere 10 Framework | Sphere10.Framework.* | These are a vast collection of modules for general-purpose usage across all [tiers](3-tier Architecture.md) of an application. Whilst integral to Hydrogen they can also be used stand-alone for non-Hydrogen applications. |
| Helium              | Sphere10.Helium.*    | The Helium framework provides an [enterprise service bus](https://en.wikipedia.org/wiki/Enterprise_service_bus) implementation (modelled after the popular NServiceBus framework) which enables Hydrogen applications to interoperate with one another. Like Sphere 10 Framework, Helium can also be used as a stand-alone framework for non-Hydrogen applications. |
| Hydrogen            | Sphere10.Hydrogen.*  | These modules comprise the P2P blockchain functionality including the node, UI and core engine. The Hydrogen framework depends on both the Sphere 10 and Helium frameworks and thus they are considered part of the "Hydrogen framework". Technically, they are separate frameworks addressing separate domains. |

Each sub-framework within the Hydrogen framework addresses a specific set of [domains](3-tier Architecture.md#Domains) relevant to the applications.  Many of these domains primarily exist to support other domains in the frameworks. Some domains are specifically intended for consumption by framework consumers. Technically, all domains in all the sub-frameworks are available for consumption by developers if they so choose.

## Sphere 10 Framework Domains

Sphere 10 Framework provides modules across all 3 tiers of the architecture. Of significance is the `Sphere10.Framework` module which provides system-tier functionality that can be used anywhere in an application (from server-end to front-end including within Blazor web-assembly code). This module should be considered like an extension to the system .NET library.

### System-tier 

| Domain        | Module/Area                                                  | Functionality                                                |
| ------------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| Cache         | [Sphere10.Framework/Cache](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Cache) | Caching abstractions, batch caching, caching policies (least-used, largest, smallest, etc), bulk caching, session caching. |
| Collections   | [Sphere10.Framework/Collections](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Collections) | Arrays, buffers, paged collections, transactional collections, stream mapped collections, transactional collections, observable collections, dictionary implementations (b-tree, etc) as well as merkle trees. |
| Comparers     |                                                              | Assortment of Comparers                                      |
| Conversion    |                                                              | Primarily Endian-based conversion and other.                 |
| Cryptography  |                                                              | Cryptographic primitives, checksums, encryption, key-derivation functions, post-quantum cryptography. |
| Encoding      |                                                              | Hexadecimal.                                                 |
| Environment   |                                                              | Support for OS and runtime-level concerns.                   |
| Events        |                                                              | Support for all things event.                                |
| Exceptions    |                                                              | Collection of exceptions and exception tools.                |
| Extensions    |                                                              | Vast collection of extension methods for .NET framework.     |
| Functional    |                                                              | Support for functional programming including tools for expressions, lambdas, operators. |
| Introspection |                                                              | Fast reflection library and tools.                           |
| IO            |                                                              | Endian-aware IO, file stores and file-system tools.          |
| IoC           | [Sphere10.Framework/IoC](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/IoC)<br />[Sphere10.Framework.Application/ComponentRegistry](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework.Application/ComponentRegistry) | Framework-wide Inversion of Control container (using TinyIoC under the hood). |
| Logging       |                                                              | Support for all things logging.                              |
| Math          |                                                              | Math abstractions and tools, bloom filters, fixed point math, random number generators and other interesting math artefacts. |
| Memory        |                                                              | Minor tooling for memory unit conversion and other obscure tools. |
| Misc          |                                                              | Miscellaneous tools that don't belong in known domain.       |
| Networking    |                                                              | All things networking, a full P2P abstraction library (TODO) as well tools for URL's, Mimes and POP3. |
| Objects       |                                                              | Assortment of tools that deal with Objects.                  |
| ObjectSpaces  |                                                              | A storage-engine for objects, used within Hydrogen.          |
| Peripherals   |                                                              | Tooling and abstractions for keyboard, mouse and hooks.      |
| Scheduler     |                                                              | A background scheduler to run code based on time rules.      |
| Scopes        |                                                              | Broad support for scope-based patterns.                      |
| Serialization |                                                              | Broad support for object serialization.                      |
| Streams       |                                                              | Tooling for Streams including bit streams, blocking streams, bounded streams, stream pipelines. |
| Text          |                                                              | All things text including parsers, inflectors, fast string builder, fluent regex builders, obscure html tools. |
| TextWriters   |                                                              | Assortment of TextWriter's including console, debug, file-based, etc. |
| Threading     |                                                              | Support for multi-threaded applications.                     |
| Types         |                                                              | Tools for `Type` including activation, resolution and switches. |
| Values        |                                                              | Tools for Futures, GUIDs, Enums, Results, Value Ranges.      |
| XML           |                                                              | Tools for XML including deep object serialization.           |



## Hydrogen Domains

| Domain       | Module/Area                                                  | Notes |
| ------------ | ------------------------------------------------------------ | ----- |
| Framework    | [Sphere10.Framework/IoC](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/IoC)<br />[Sphere10.Framework.Application/ComponentRegistry](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework.Application/ComponentRegistry) |       |
| Framework    | [Sphere10.Framework/Collections](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Collections) |       |
| Cryptography |                                                              |       |
| Cryptography |                                                              |       |
| Cryptography |                                                              |       |
| Consensus    |                                                              |       |
| Consensus    |                                                              |       |
| Consensus    |                                                              |       |
| Consensus    |                                                              |       |
| Consensus    |                                                              |       |
| Consensus    |                                                              |       |
|              |                                                              |       |

## 

## Helium Domains



## Sphere 10 Framework

1. IoC

2. Collections

   1. ExtendedList
   2. ObservableList
   3. PagedList
   4. MemoryPagedList
   5. FileMappedList
   6. Transactional

3. Cryptography

   1. Base libraries
   2. Post-quantum Cryptography (PQC) using Abstracted Merkle Signature Scheme (AMS).
   3. ECDSA

4. Logging

5. Serialization

   

## Hydrogen Core

1. Blockchain

2. Object Spaces

3. Consensus

4. Node 

   

## Hydrogen UI

1. Wizards

2. Grids

3. Mainframe

   

