# Domains

A [domain]() consists of a collection of classes that are all logically related in the functionality they provide. A domain may span across multiple tiers of the [3-tier architecture](3-tier Architecture.md), then it can be considered as a vertical slice of the 3-tier architecture. Modules (.NET projects) typically contain multiple domains each segregated in a sub-folder.  Below are domains available in the Hydrogen framework.

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

   



Hydrogen is composed of the following domains.



| Domain       | Sub-Domain        | Modules                                                      | Notes |
| ------------ | ----------------- | ------------------------------------------------------------ | ----- |
| Framework    | IoC               | [Sphere10.Framework/IoC](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/IoC)<br />[Sphere10.Framework.Application/ComponentRegistry](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework.Application/ComponentRegistry) |       |
| Framework    | Collections       | [Sphere10.Framework/Collections](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Collections) |       |
| Cryptography | Base Abstractions |                                                              |       |
| Cryptography | Logging           |                                                              |       |
| Cryptography | Serialization     |                                                              |       |
| Consensus    | Blockchain        |                                                              |       |
| Consensus    | Merkle Trees      |                                                              |       |
| Consensus    |                   |                                                              |       |
| Consensus    |                   |                                                              |       |
| Consensus    |                   |                                                              |       |
| Consensus    |                   |                                                              |       |
|              |                   |                                                              |       |





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

   

