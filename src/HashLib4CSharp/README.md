# HashLib4CSharp

Hashing and checksum library with a unified API for strings, files, streams, and byte arrays.

## Overview
Provides implementations for common cryptographic hashes and checksums used throughout Hydrogen.

## Usage

```csharp
using HashLib4CSharp.Base;

var hash = HashFactory.Crypto.CreateSHA256().ComputeString("hello world");
```

## Related projects
- [Hydrogen](../Hydrogen)
- [Hydrogen.CryptoEx](../Hydrogen.CryptoEx)
