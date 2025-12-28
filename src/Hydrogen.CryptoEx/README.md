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

### ECDSA Digital Signatures

Generate keys, sign messages, and verify signatures with ECDSA:

```csharp
using Hydrogen.CryptoEx.EC;
using System.Text;

// Create ECDSA instance with SECP256K1 (Bitcoin curve)
var ecdsa = new ECDSA(ECDSAKeyType.SECP256K1);

// Generate a private key from a deterministic seed
var secret = new byte[] { 0, 1, 2, 3, 4 };
var privateKey = ecdsa.GeneratePrivateKey(secret);

// Derive the corresponding public key
var publicKey = ecdsa.DerivePublicKey(privateKey);

// Sign a message
var message = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
var signature = ecdsa.Sign(privateKey, message);

// Verify the signature
bool isValid = ecdsa.Verify(signature, message, publicKey); // true
```

### Supported Elliptic Curves

All curves are supported with the same interface:

```csharp
var ecdsa256 = new ECDSA(ECDSAKeyType.SECP256K1);      // 256-bit (Bitcoin)
var ecdsa384 = new ECDSA(ECDSAKeyType.SECP384R1);      // 384-bit
var ecdsa521 = new ECDSA(ECDSAKeyType.SECP521R1);      // 521-bit
var ecdsa283 = new ECDSA(ECDSAKeyType.SECT283K1);      // 283-bit (Binary curve)
```

### ECIES Asymmetric Encryption

Encrypt data with a public key and decrypt with the corresponding private key:

```csharp
using Hydrogen.CryptoEx.EC;
using System.Text;

var ecdsa = new ECDSA(ECDSAKeyType.SECP256K1);

// Generate keypair
var privateKey = ecdsa.GeneratePrivateKey();
var publicKey = ecdsa.DerivePublicKey(privateKey);

// Encrypt message with public key
var message = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
var encryptedData = ecdsa.IES.Encrypt(message, publicKey);

// Decrypt with private key
if (ecdsa.IES.TryDecrypt(encryptedData, out var decryptedData, privateKey)) {
	// Decrypted data matches original message
	bool success = message.SequenceEqual(decryptedData.ToArray()); // true
}
```

### Multiple Encryption Attempts

For testing randomized encryption (each encryption produces different ciphertext):

```csharp
var ecdsa = new ECDSA(ECDSAKeyType.SECP384R1);
var privateKey = ecdsa.GeneratePrivateKey();
var publicKey = ecdsa.DerivePublicKey(privateKey);

var message = Encoding.ASCII.GetBytes(RandomString(100));

// Encrypt multiple times
for (int i = 0; i < 1000; i++) {
	var encrypted = ecdsa.IES.Encrypt(message, publicKey);
    
	// Each encryption produces different ciphertext due to ECIES randomization
	if (ecdsa.IES.TryDecrypt(encrypted, out var decrypted, privateKey)) {
		// Verify decryption matches original
		bool match = message.SequenceEqual(decrypted.ToArray());
	}
}
```

## 📦 Dependencies

- **Hydrogen**: Core framework
- **BouncyCastle.Cryptography**: Cryptographic primitives
- Platform-specific cryptography libraries

## 📖 Documentation

See [Post-Quantum Cryptography: Abstract Merkle Signatures (AMS)](https://sphere10.com/tech/ams) for advanced signature scheme documentation.

## 🧮 Key Concepts

- **Private Key**: Used for signing and decryption - must be kept secret
- **Public Key**: Derived from private key, used for verification and encryption - can be shared
- **ECDSA**: Elliptic Curve Digital Signature Algorithm - signs messages with private key, verifies with public key
- **ECIES**: Elliptic Curve Integrated Encryption Scheme - encrypts with public key, decrypts with private key
- **Deterministic Generation**: Providing a seed produces the same keypair every time (useful for testing)
- **Non-Deterministic Encryption**: ECIES adds randomness, so each encryption of the same message produces different ciphertext

## 📄 Related Projects

- [Hydrogen](../Hydrogen) - Core framework
- [Hydrogen.Consensus](../Hydrogen.Consensus) - Consensus mechanisms using cryptography
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core) - DApp blockchain core

