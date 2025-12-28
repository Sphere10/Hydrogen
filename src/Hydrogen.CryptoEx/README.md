# Hydrogen.CryptoEx

Extended cryptography library providing specialized cryptographic implementations for blockchain applications, including post-quantum cryptography support and multi-signature schemes.

## 📋 Overview

`Hydrogen.CryptoEx` extends the core cryptographic capabilities of Hydrogen with blockchain-specific and post-quantum cryptographic algorithms. It provides implementations for digital signature schemes, key derivation, and advanced cryptographic primitives used in distributed systems.

## 🏗️ Architecture

The library is organized by cryptographic scheme families:

- **Bitcoin**: Bitcoin-specific cryptographic operations (SECP256k1)
- **EC (Elliptic Curve)**: General elliptic curve cryptography
- **CHF (Cryptographic Hash Functions)**: Advanced hashing algorithms
- **HF (Hash Functions)**: Standard hash function implementations
- **PascalCoin**: PascalCoin-specific cryptographic schemes
- **IES (Integrated Encryption Schemes)**: Encryption utilities
- **Misc**: Additional cryptographic utilities

## 🚀 Key Features

- **Post-Quantum Cryptography**: Support for quantum-resistant algorithms
- **Multiple Signature Schemes**: ECDSA, Winternitz, and other digital signatures
- **Blockchain Integration**: Bitcoin and PascalCoin algorithm support
- **Key Derivation**: Advanced key derivation functions
- **Hashing Variants**: Multiple hash function implementations
- **Hardware Acceleration**: Support for optimized cryptographic operations

## 🔧 Usage

Digital signature generation and verification:

```csharp
using Hydrogen.CryptoEx;

// SECP256k1 (Bitcoin)
var signer = new SECP256k1Signer();
var privateKey = signer.GeneratePrivateKey();
var signature = signer.Sign(message, privateKey);
var valid = signer.Verify(message, signature, publicKey);
```

## 📦 Dependencies

- **Hydrogen**: Core framework
- **BouncyCastle.Cryptography**: Cryptographic primitives
- Platform-specific cryptography libraries

## 📄 Documentation

See [Post-Quantum Cryptography: Abstract Merkle Signatures (AMS)](https://sphere10.com/tech/ams) for advanced signature scheme documentation.

## 📄 Related Projects

- [Hydrogen](../Hydrogen) - Core framework library
- [Hydrogen.Consensus](../Hydrogen.Consensus) - Consensus mechanisms using cryptography
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core) - DApp blockchain core
