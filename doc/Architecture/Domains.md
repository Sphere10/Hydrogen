# Domains

A [domain](3-tier Architecture.md#domain) consists of a collection of [modules](#3-tier Architecture.md#module) that are all logically related in the abstraction they model. A domain can span across multiple tiers depending on the functionality it provides (i.e. user interface, business component, etc). A domain can be visualized as a vertical slice through the [3-tier architecture. Modules (.NET projects) typically contain multiple domains each segregated in a sub-folder.  Below are domains available in the Hydrogen framework.

## Sphere 10 Framework Domains

Sphere 10 Framework provides modules across all 3 tiers of the architecture. Of significance is the `Sphere10.Framework` module which provides system-tier functionality that can be used anywhere in an application (from server-end to front-end including within Blazor web-assembly code). This module should be considered like an extension to the system .NET library.

| Domain        | Tiers              | Module/Area                                                  | Functionality                                                |
| ------------- | ------------------ | ------------------------------------------------------------ | ------------------------------------------------------------ |
| Cache         | System             | [Sphere10.Framework/Cache](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Cache) | Caching abstractions, batch caching, caching policies (least-used, largest, smallest, etc), bulk caching, session caching. |
| Collections   | System             | [Sphere10.Framework/Collections](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/Collections) | Arrays, buffers, paged collections, transactional collections, stream mapped collections, transactional collections, observable collections, dictionary implementations (b-tree, etc) as well as merkle trees. |
| Comparers     | System             |                                                              | Assortment of Comparers                                      |
| Conversion    | System             |                                                              | Primarily Endian-based conversion and other.                 |
| Cryptography  | System             |                                                              | Cryptographic primitives, checksums, encryption, key-derivation functions, post-quantum cryptography. |
| Encoding      | System             |                                                              | Hexadecimal.                                                 |
| Environment   | System             |                                                              | Support for OS and runtime-level concerns.                   |
| Events        | System             |                                                              | Support for all things event.                                |
| Exceptions    | System             |                                                              | Collection of exceptions and exception tools.                |
| Extensions    | System             |                                                              | Vast collection of extension methods for .NET framework.     |
| Functional    | System             |                                                              | Support for functional programming including tools for expressions, lambdas, operators. |
| Introspection | System             |                                                              | Fast reflection library and tools.                           |
| IO            | System             |                                                              | Endian-aware IO, file stores and file-system tools.          |
| IoC           | System, Processing | [Sphere10.Framework/IoC](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework/IoC)<br />[Sphere10.Framework.Application/ComponentRegistry](https://github.com/Sphere10/Hydrogen/tree/master/src/Sphere10.Framework.Application/ComponentRegistry) | Framework-wide Inversion of Control container (using TinyIoC under the hood). |
| Logging       | System             |                                                              | Support for all things logging.                              |
| Math          | System             |                                                              | Math abstractions and tools, bloom filters, fixed point math, random number generators and other interesting math artefacts. |
| Memory        | System             |                                                              | Minor tooling for memory unit conversion and other obscure tools. |
| Misc          | System             |                                                              | Miscellaneous tools that don't belong in known domain.       |
| Networking    | System             |                                                              | All things networking, a full P2P abstraction library (TODO) as well tools for URL's, Mimes and POP3. |
| Objects       | System             |                                                              | Assortment of tools that deal with Objects.                  |
| ObjectSpaces  | System             |                                                              | A storage-engine for objects, used within Hydrogen.          |
| Peripherals   | System             |                                                              | Tooling and abstractions for keyboard, mouse and hooks.      |
| Scheduler     | System             |                                                              | A background scheduler to run code based on time rules.      |
| Scopes        | System             |                                                              | Broad support for scope-based patterns.                      |
| Serialization | System             |                                                              | Broad support for object serialization.                      |
| Streams       | System             |                                                              | Tooling for Streams including bit streams, blocking streams, bounded streams, stream pipelines. |
| Text          | System             |                                                              | All things text including parsers, inflectors, fast string builder, fluent regex builders, obscure html tools. |
| TextWriters   | System             |                                                              | Assortment of TextWriter's including console, debug, file-based, etc. |
| Threading     | System             |                                                              | Support for multi-threaded applications.                     |
| Types         | System             |                                                              | Tools for `Type` including activation, resolution and switches. |
| Values        | System             |                                                              | Tools for Futures, GUIDs, Enums, Results, Value Ranges.      |
| XML           | System             |                                                              | Tools for XML including deep object serialization.           |



## Hydrogen Domains

| Domain | Module/Area | Notes |
| ------ | ----------- | ----- |
|        |             |       |
|        |             |       |
|        |             |       |
|        |             |       |
|        |             |       |
|        |             |       |
|        |             |       |
|        |             |       |
|        |             |       |
|        |             |       |
|        |             |       |
|        |             |       |

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

   

