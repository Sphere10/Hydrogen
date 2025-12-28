# HashLib4CSharp

HashLib4CSharp is a C# library that provides an easy to use interface for computing hashes and checksums of strings, files, streams, byte arrays and untyped data.

## 📋 Overview

`HashLib4CSharp` is a comprehensive hashing library providing implementations of multiple hash algorithms and checksums. It offers a unified interface for computing and verifying cryptographic hashes across various input sources.

## 🚀 Key Features

- **Multiple Algorithms**: MD5, SHA-1, SHA-2, SHA-3, BLAKE2, and many more
- **Multiple Input Types**: Strings, files, streams, byte arrays, untyped data
- **Checksum Algorithms**: CRC, Adler-32, and other checksum implementations
- **Fluent API**: Easy-to-use builder pattern for hash computation
- **Streaming Support**: Compute hashes for large files efficiently
- **Verification**: Built-in hash verification methods
- **Performance**: Optimized implementations with caching

## 🔧 Usage

Compute a hash:

```csharp
using HashLib4CSharp.Base;

// Hash a string
var hash = HashFactory.Crypto.CreateSHA256().ComputeString("hello world");

// Hash a file
var fileHash = HashFactory.Crypto.CreateSHA256().ComputeFile("myfile.bin");

// Hash a stream
using var stream = File.OpenRead("data.bin");
var streamHash = HashFactory.Crypto.CreateSHA256().ComputeStream(stream);
```

## 📦 Dependencies

- Core .NET Framework only

## 📄 Related Projects

- [Hydrogen](../Hydrogen) - Uses HashLib for cryptographic operations
- [Hydrogen.CryptoEx](../Hydrogen.CryptoEx) - Extended cryptography
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core) - Blockchain hashing
