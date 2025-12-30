<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 🔐 Hydrogen.CryptoEx

<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

**Extended cryptography library** providing specialized implementations for blockchain applications including post-quantum signature schemes, ECDSA, key derivation, and verifiable randomness (VRF).

Hydrogen.CryptoEx extends [Hydrogen](../Hydrogen)'s core cryptography with advanced primitives used in distributed systems and blockchain protocols, supporting **post-quantum resistance, multiple elliptic curves, and advanced digital signatures**.

## ⚡ 10-Second Example

```csharp
using Hydrogen.CryptoEx;
using Hydrogen.CryptoEx.EC;
using System.Text;

// ECDSA signing with Bitcoin curve
var signer = new ECDSA(ECDSAKeyType.SECP256K1);
var privateKey = signer.GeneratePrivateKey();  // Generates random key
var publicKey = signer.DerivePublicKey(privateKey);

// Sign a message
var message = Encoding.ASCII.GetBytes("Hello Blockchain");
var signature = signer.Sign(privateKey, message);

// Verify signature
bool isValid = signer.Verify(signature, message, publicKey);  // true

// ECIES encryption (public key encryption)
var encrypted = signer.IES.Encrypt(message, publicKey);
if (signer.IES.TryDecrypt(encrypted, out var decrypted, privateKey)) {
    Console.WriteLine(Encoding.ASCII.GetString(decrypted.ToArray()));  // "Hello Blockchain"
}
```

## 🏗️ Core Concepts

**Digital Signatures**: Sign with private key, verify with public key. ECDSA and post-quantum schemes available.

**Public-Key Encryption (ECIES)**: Encrypt with public key, decrypt with private key for confidential data exchange.

**Verifiable Random Functions (VRF)**: Deterministic random output with cryptographic proofs—used for leader election and consensus.

**Multiple Signature Schemes**: ECDSA (SECP256k1, SECP384R1, SECP521R1, SECT283K1), Winternitz One-Time Signatures (W-OTS), and quantum-resistant schemes (W-AMS).

**Key Derivation**: Deterministic key generation from seeds for reproducible key management.

## 🔧 Core Examples

### ECDSA Signatures with Deterministic Keys

```csharp
using Hydrogen.CryptoEx.EC;
using System.Text;

// Initialize with Bitcoin curve (SECP256k1)
var ecdsa = new ECDSA(ECDSAKeyType.SECP256K1);

// Deterministic key generation from seed (reproducible)
var seed = new byte[] { 0, 1, 2, 3, 4 };
var privateKey = ecdsa.GeneratePrivateKey(seed);
var publicKey = ecdsa.DerivePublicKey(privateKey);

// Sign message
var message = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
var signature = ecdsa.Sign(privateKey, message);

// Verify signature
bool isValid = ecdsa.Verify(signature, message, publicKey);  // true

// Note: Same seed always produces same privateKey and signature for testing
```

### Random Key Generation

```csharp
var ecdsa = new ECDSA(ECDSAKeyType.SECP256K1);

// Random key generation (non-deterministic)
var privateKey = ecdsa.GeneratePrivateKey();  // Generates random bytes
var publicKey = ecdsa.DerivePublicKey(privateKey);

// Each call produces different keys
var anotherPrivateKey = ecdsa.GeneratePrivateKey();
Console.WriteLine(privateKey.SequenceEqual(anotherPrivateKey));  // false
```

### ECIES: Public-Key Encryption

```csharp
using Hydrogen.CryptoEx.EC;
using System.Text;

var ecdsa = new ECDSA(ECDSAKeyType.SECP384R1);

// Generate keypair
var privateKey = ecdsa.GeneratePrivateKey();
var publicKey = ecdsa.DerivePublicKey(privateKey);

// Encrypt with public key
var plaintext = Encoding.ASCII.GetBytes("Confidential data");
var ciphertext = ecdsa.IES.Encrypt(plaintext, publicKey);

// Decrypt with private key
if (ecdsa.IES.TryDecrypt(ciphertext, out var decrypted, privateKey)) {
    string result = Encoding.ASCII.GetString(decrypted.ToArray());
    Console.WriteLine(result);  // "Confidential data"
}

// Each encryption is different (randomized) even for same message
var ciphertext2 = ecdsa.IES.Encrypt(plaintext, publicKey);
Console.WriteLine(ciphertext.SequenceEqual(ciphertext2));  // false
```

### Multiple Elliptic Curves

```csharp
using Hydrogen.CryptoEx.EC;

// All curves support same interface
var secp256k1 = new ECDSA(ECDSAKeyType.SECP256K1);    // 256-bit (Bitcoin)
var secp384r1 = new ECDSA(ECDSAKeyType.SECP384R1);    // 384-bit
var secp521r1 = new ECDSA(ECDSAKeyType.SECP521R1);    // 521-bit
var sect283k1 = new ECDSA(ECDSAKeyType.SECT283K1);    // 283-bit (Binary)

// Higher bit-length = more security but slower operations
var message = Encoding.ASCII.GetBytes("Test");

var key256 = secp256k1.GeneratePrivateKey();
var sig256 = secp256k1.Sign(key256, message);  // ~256-bit security

var key384 = secp384r1.GeneratePrivateKey();
var sig384 = secp384r1.Sign(key384, message);  // ~384-bit security
```

### Verifiable Random Functions (VRF)

```csharp
using Hydrogen.CryptoEx;
using Hydrogen.CryptoEx.EC;

// VRF creates deterministic but unpredictable outputs with proofs
var vrf = VRF.CreateCryptographicVRF(
    CHF.SHA2_256,  // Hash function
    DSS.ECDSA_SECP256k1);  // Signature scheme

// Generate keypair
var privateKey = Signers.GeneratePrivateKey(DSS.ECDSA_SECP256k1);
var nonce = 0UL;
var publicKey = Signers.DerivePublicKey(DSS.ECDSA_SECP256k1, privateKey, nonce);

// Generate VRF output with proof
var seed = new byte[] { 1, 2, 3, 4 };
var output = vrf.Run(seed, privateKey, nonce, out var proof);

// Verify VRF output independently
bool isProofValid = vrf.TryVerify(seed, output, proof, publicKey);  // true

// Output is deterministic for same seed
var output2 = vrf.Run(seed, privateKey, nonce, out var proof2);
Console.WriteLine(output.SequenceEqual(output2));  // true

// Use case: Leader election in consensus - prove randomly selected leader without manipulation
```

### Digital Signature Schemes (DSS)

```csharp
using Hydrogen.CryptoEx;
using System.Text;

// Available schemes with different security properties
var schemes = new[] {
    DSS.ECDSA_SECP256k1,      // Traditional: 256-bit ECC
    DSS.ECDSA_SECP384R1,      // Traditional: 384-bit ECC
    DSS.ECDSA_SECP521R1,      // Traditional: 521-bit ECC
    DSS.ECDSA_SECT283K1,      // Traditional: 283-bit Binary curve
    DSS.PQC_WAMS,             // Post-quantum: Winternitz AMS
    DSS.PQC_WAMSSharp         // Post-quantum: Sharp variant
};

var message = Encoding.ASCII.GetBytes("Important message");

foreach (var dss in schemes) {
    var privateKey = Signers.GeneratePrivateKey(dss);
    var publicKey = Signers.DerivePublicKey(dss, privateKey, 0UL);
    
    var signature = Signers.Sign(dss, privateKey, message);
    bool isValid = Signers.Verify(dss, publicKey, message, signature);
    
    Console.WriteLine($"{dss}: {isValid}");  // All true
}
```

## 📦 Cryptographic Primitives

### Supported Elliptic Curves (ECDSA)

| Curve | Bits | Type | Use Case |
|-------|------|------|----------|
| SECP256K1 | 256 | Prime | Bitcoin, Ethereum, blockchain |
| SECP384R1 | 384 | Prime | Higher security threshold |
| SECP521R1 | 521 | Prime | Maximum traditional security |
| SECT283K1 | 283 | Binary | Specialized/legacy |

### Supported Hash Functions

- **SHA-2 Family**: SHA256, SHA512
- **SHA-3 Family**: SHA3-256, SHA3-512
- **BLAKE2**: BLAKE2b-256, BLAKE2b-512
- **Specialized**: RIPEMD160, MurmurHash3

### Supported Signature Schemes

**Traditional (Vulnerable to Quantum)**:
- ECDSA with multiple curves
- One-time signatures (W-OTS)

**Post-Quantum Resistant**:
- Winternitz AMS (W-AMS) - quantum-resistant alternatives
- W-AMS-Sharp - optimized variant

## 🔧 Advanced Usage

### Bitcoin & Blockchain Algorithms

Bitcoin-specific cryptography is available through `DSS.ECDSA_SECP256k1` with specialized tools in the `Bitcoin/` namespace for compatibility and integration.

## ⚠️ Security Considerations

- **Private Keys**: Must be kept secret. Never log, transmit unencrypted, or store in plain text.
- **Seed-Based Keys**: Use cryptographically secure RNG for seeds in production.
- **Post-Quantum Migration**: Current ECDSA is vulnerable to quantum computing. Use W-AMS family for long-term security.
- **Signature Verification**: Always verify signatures independently before trusting data.
- **ECIES Randomization**: Do NOT assume encrypted data is identical for same plaintext/key pair.
- **Hash Collisions**: Use SHA-3 or BLAKE2 for cryptographic binding; MurmurHash only for non-security use.

## 🔌 Architecture Layers

- **EC (Elliptic Curve)**: `ECDSA` class for signature and encryption operations
- **Bitcoin**: Bitcoin-specific algorithms and compatibility
- **CHF (Cryptographic Hash Functions)**: Advanced hash implementations
- **VRF**: Verifiable random functions for consensus and selection
- **Signers**: Unified interface for all signature schemes (ECDSA, W-OTS, W-AMS)
- **IES**: Integrated encryption scheme (ECIES) for public-key encryption
- **PascalCoin**: PascalCoin-specific cryptographic operations

## ✅ Status & Maturity

- **ECDSA & Hash Functions**: Production-tested, stable
- **Post-Quantum Schemes (W-AMS)**: Reference implementations; audit before production cryptographic use
- **.NET Target**: .NET 8.0+ (primary), .NET Standard 2.0 for some components
- **Thread Safety**: Hash functions and signature verification are thread-safe; key generation typically single-threaded per instance
- **Performance**: ECDSA faster than post-quantum schemes; post-quantum trades speed for quantum resistance

## 📖 Related Projects

- [Hydrogen](../Hydrogen) - Core framework with basic cryptography
- [Hydrogen.Consensus](../Hydrogen.Consensus) - Consensus mechanisms using VRF and signatures
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core) - Blockchain DApp core with cryptographic security
- [Hydrogen.Communications](../Hydrogen.Communications) - Network protocols with cryptographic authentication

## 🏆 Advanced Topics

See **Post-Quantum Cryptography: Abstract Merkle Signatures (AMS)** documentation for theoretical foundation of W-AMS scheme and proof of quantum resistance.

## 📦 Dependencies

- **Hydrogen**: Core framework
- **BouncyCastle.Cryptography**: Cryptographic primitives (EC, ECDSA, Hashing)
- **.NET 8.0+**: Modern cryptography APIs

## ⚖️ License

Distributed under the **MIT NON-AI License**.

See the LICENSE file for full details. More information: [Sphere10 NON-AI-MIT License](https://sphere10.com/legal/NON-AI-MIT)

## 👤 Author

**Herman Schoenfeld** - Software Engineer

---

**Version**: 2.0+

