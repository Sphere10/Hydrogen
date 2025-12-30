<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# ⛓️ Hydrogen.Consensus

<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

**Blockchain consensus mechanism framework** providing pluggable consensus implementations, real-time difficulty adjustment, and distributed ledger validation infrastructure.

Hydrogen.Consensus enables **protocol-agnostic consensus** with support for Proof-of-Work variants, adaptive difficulty adjustment algorithms, block validation rules, and merkle-tree based proofs.

## ⚡ 10-Second Example

```csharp
using Hydrogen.Consensus;
using Hydrogen.CryptoEx;

// Initialize consensus mechanism
var consensusRules = new ConsensusRules {
    MaxBlockSize = 1_000_000,  // 1MB blocks
    TargetBlockTime = 600,      // 10 minutes
    DifficultyAdjustmentInterval = 2016  // Bitcoin-like
};

// Create block validator
var validator = new BlockValidator(consensusRules);

// Validate incoming block
var isValid = validator.ValidateBlock(newBlock, previousBlock);
if (isValid) {
    // Apply block to chain
    chain.AddBlock(newBlock);
    Console.WriteLine($"Block accepted: {newBlock.Hash}");
}

// Get current difficulty
var difficulty = validator.GetNextDifficulty(chain);
Console.WriteLine($"Next difficulty: {difficulty}");
```

## 🏗️ Core Concepts

**Consensus Rules**: Configurable parameters defining blockchain behavior (block size, block time, difficulty adjustment).

**Proof-of-Work**: Hash-based work proofs with adjustable difficulty targeting a desired block time.

**Real-Time Difficulty Adjustment**: Dynamically adjusts difficulty to maintain target block time regardless of hash rate changes.

**Block Validation**: Verifies blocks against consensus rules (size, timestamp, difficulty, previous hash reference).

**Merkle Tree Validation**: Ensures transaction integrity within blocks using merkle proofs.

## � Core Examples

### Block Validation

```csharp
using Hydrogen.Consensus;
using Hydrogen.CryptoEx;

// Define consensus rules
var rules = new ConsensusRules {
    MaxBlockSize = 1_000_000,              // 1MB
    MaxBlockWeight = 4_000_000,            // 4MB with witness data
    TargetBlockTime = 600,                 // 10 minutes (seconds)
    DifficultyAdjustmentInterval = 2016,   // Blocks between adjustment
    MaxDifficultyAdjustmentRatio = 4       // Max 4x change per adjustment
};

// Create validator
var validator = new BlockValidator(rules);

// Validate block structure
bool isStructureValid = validator.ValidateBlockStructure(block);

// Validate block against previous block
bool isChainValid = validator.ValidateBlock(block, previousBlock);

// Validate merkle tree
bool isMerkleValid = validator.ValidateMerkleTree(block);

// Validate proof-of-work
bool isPoWValid = validator.ValidateProofOfWork(block, difficulty);

if (isStructureValid && isChainValid && isMerkleValid && isPoWValid) {
    chain.AddBlock(block);
    Console.WriteLine("Block accepted");
} else {
    Console.WriteLine("Block rejected");
}
```

### Difficulty Adjustment

```csharp
using Hydrogen.Consensus;
using System;

var consensusRules = new ConsensusRules {
    TargetBlockTime = 600,  // Want 10-minute blocks
    DifficultyAdjustmentInterval = 2016  // Adjust every 2016 blocks
};

var validator = new BlockValidator(consensusRules);

// Calculate next difficulty based on recent block times
var blockTimings = chain.GetLastBlocks(2016)
    .Select(b => b.Timestamp)
    .ToList();

var nextDifficulty = validator.CalculateNextDifficulty(
    currentDifficulty: blockchain.CurrentDifficulty,
    actualBlockTime: blockTimings.Last() - blockTimings.First(),
    expectedBlockTime: 2016 * 600);  // 2016 blocks * 600 sec each

Console.WriteLine($"Current difficulty: {blockchain.CurrentDifficulty}");
Console.WriteLine($"Next difficulty: {nextDifficulty}");

// If hash rate increased, difficulty increases to maintain 10-min blocks
// If hash rate decreased, difficulty decreases to prevent long block times
```

### Block Production

```csharp
using Hydrogen.Consensus;
using Hydrogen.CryptoEx;

// Get pending transactions
var pendingTransactions = mempool.GetTransactions();

// Create new block
var newBlock = new Block {
    Version = 1,
    PreviousBlockHash = chain.LastBlock.Hash,
    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    Difficulty = validator.GetNextDifficulty(chain),
    Nonce = 0,
    Transactions = pendingTransactions.Take(1000).ToList()
};

// Calculate merkle root
newBlock.MerkleRoot = MerkleTree.CalculateRoot(newBlock.Transactions);

// Proof-of-Work mining: increment nonce until hash is below target
var target = GetTargetFromDifficulty(newBlock.Difficulty);
while (true) {
    var blockHash = ComputeBlockHash(newBlock);
    if (blockHash < target) {
        Console.WriteLine($"Block found: {blockHash}");
        chain.AddBlock(newBlock);
        break;
    }
    newBlock.Nonce++;
}
```

### Real-Time Difficulty Adjustment (RTTA)

```csharp
using Hydrogen.Consensus;
using System;

// Real-Time Targeted Adjustment (RTTA) - adjusts every block, not every N blocks
var rttaValidator = new RTTAValidator {
    TargetBlockTime = 600,              // 10 minutes
    MaxAdjustmentPercent = 10,          // Max 10% change per block
    WindowSize = 144                    // Last 144 blocks for averaging
};

// Each new block triggers difficulty adjustment
var lastBlocks = chain.GetLastBlocks(144);
var avgBlockTime = lastBlocks
    .Zip(lastBlocks.Skip(1), (a, b) => b.Timestamp - a.Timestamp)
    .Average();

var nextDifficulty = rttaValidator.AdjustDifficulty(
    currentDifficulty: blockchain.CurrentDifficulty,
    actualAverageBlockTime: avgBlockTime,
    targetBlockTime: 600);

// Advantage: Quickly responds to hash rate changes
// Prevents "difficulty bomb" when mining rigs turn on/off
```

### Validation Rule Composition

```csharp
using Hydrogen.Consensus;

// Create composable validators
var structureValidator = new BlockStructureValidator();
var chainValidator = new BlockChainValidator();
var merkleValidator = new MerkleTreeValidator();
var powValidator = new ProofOfWorkValidator();
var transactionValidator = new TransactionValidator();

// Validate block through all rules
var validators = new[] {
    structureValidator.Validate(block),
    chainValidator.Validate(block, previousBlock),
    merkleValidator.Validate(block),
    powValidator.Validate(block),
    transactionValidator.ValidateAll(block.Transactions)
};

bool isValid = validators.All(v => v.IsValid);
if (!isValid) {
    var failures = validators.Where(v => !v.IsValid).ToList();
    Console.WriteLine($"Validation failed: {string.Join(", ", failures.Select(v => v.Reason))}");
}
```

## 🎯 Consensus Mechanisms Supported

**Proof-of-Work (PoW)**: Requires solving computational puzzles. Difficulty adjusts to maintain target block time.

**Real-Time Targeted Adjustment (RTTA)**: Adjusts difficulty every block instead of fixed intervals; responds quickly to hash rate changes.

**Proof-of-Stake Variants** (infrastructure provided): Framework supports plugging in PoS rules.

**Hybrid Consensus**: Mix PoW and other mechanisms with custom validation rules.

## 🏗️ Architecture

- **Consensus Rules**: `ConsensusRules` class defines parameters
- **Validators**: Specialized validators for different aspects (structure, chain, PoW, transactions)
- **Difficulty Calculator**: Computes next difficulty based on block times and target
- **Block Factory**: Creates well-formed blocks adhering to consensus rules
- **Rule Composer**: Combines multiple validators into comprehensive validation pipeline

## ⚠️ Design Considerations

- **Block Time Target**: Set target to match network security goals (Bitcoin: 10min, Ethereum: 12s)
- **Difficulty Adjustment**: Slower adjustment (Bitcoin) more stable but slower response; faster (Ethereum) more responsive but volatile
- **Validation Order**: Check structure first (cheap), then chain (medium), then PoW (expensive)
- **Timestamp Validation**: Prevent block timestamp manipulation with median-time-past rules
- **Transaction Ordering**: Some consensus systems require specific transaction ordering

## 📖 Documentation

- [Real-Time Targeted Difficulty Adjustment (RTTA)](../../docs/Blockchain/rtt-asert.pdf) - Algorithm details
- [Proof-of-Work Fundamentals](../../docs/Blockchain/pow.md) - PoW security analysis
- [Block Validation Rules](../../docs/Blockchain/validation.md) - Comprehensive validation rules

## 📦 Dependencies

- **Hydrogen**: Core framework
- **Hydrogen.CryptoEx**: Cryptographic primitives (hashing, signatures)
- **Hydrogen.DApp.Core**: Blockchain core types (Block, Transaction, Chain)

## ✅ Status & Maturity

- **Proof-of-Work**: Production-tested, stable
- **RTTA**: Reference implementation, audit-recommended before production
- **.NET Target**: .NET 8.0+ (primary)
- **Thread Safety**: Validators generally immutable; chain modifications require external synchronization
- **Performance**: Validation optimized; PoW verification is CPU-intensive by design

## 📄 Related Projects

- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core) - Blockchain core types and chain management
- [Hydrogen.DApp.Node](../Hydrogen.DApp.Node) - Node implementation using consensus layer
- [Hydrogen.CryptoEx](../Hydrogen.CryptoEx) - Cryptographic primitives (hashing, signatures)
- [Hydrogen.Consensus](../Hydrogen.Consensus) - This library

## ⚖️ License

Distributed under the **MIT NON-AI License**.

See the LICENSE file for full details. More information: [Sphere10 NON-AI-MIT License](https://sphere10.com/legal/NON-AI-MIT)

## 👤 Author

**Herman Schoenfeld** - Software Engineer

---

**Version**: 2.0+
