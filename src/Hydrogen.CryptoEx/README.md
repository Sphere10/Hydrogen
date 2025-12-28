# Hydrogen.CryptoEx

Extended cryptography library with blockchain-focused primitives and post-quantum research implementations.

## Overview
Hydrogen.CryptoEx extends the core cryptography in Hydrogen with additional algorithms (including SECP256k1), signature schemes, and utilities used in DApp components.

## Key features
- Elliptic-curve cryptography (including SECP256k1)
- Digital signatures and verification helpers
- Hashing variants and key derivation utilities
- Post-quantum research implementations used by the project

## Usage

### ECDSA signing

```csharp
using Hydrogen.CryptoEx.EC;
using System.Text;

var ecdsa = new ECDSA(ECDSAKeyType.SECP256K1);
var privateKey = ecdsa.GeneratePrivateKey();
var publicKey = ecdsa.DerivePublicKey(privateKey);

var message = Encoding.ASCII.GetBytes("hello");
var signature = ecdsa.Sign(privateKey, message);

bool ok = ecdsa.Verify(signature, message, publicKey);
```

## Dependencies
- Hydrogen
- BouncyCastle (crypto primitives)

## Related projects
- [Hydrogen](../Hydrogen)
- [Hydrogen.Consensus](../Hydrogen.Consensus)
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core)
