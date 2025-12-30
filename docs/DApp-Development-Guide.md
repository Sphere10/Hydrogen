# Hydrogen DApp Development Guide

**Copyright © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.**

## Table of Contents

1. [Introduction](#introduction)
2. [DApp Architecture](#dapp-architecture)
3. [Core Components](#core-components)
4. [Building Your DApp](#building-your-dapp)
5. [Plugin Architecture](#plugin-architecture)
6. [Creating Custom Domains](#creating-custom-domains)
7. [Consensus Integration](#consensus-integration)
8. [State Management](#state-management)
9. [Testing & Deployment](#testing--deployment)
10. [Best Practices](#best-practices)

---

## Introduction

### What is a Hydrogen DApp?

A **Hydrogen DApp (Decentralized Application)** is a blockchain-based application built on the Hydrogen framework that combines:

1. **Consensus Layer** - Agreement rules for validating transactions and blocks
2. **Processing Layer** - Business logic and state transitions
3. **Persistence Layer** - Blockchain and object spaces for state storage
4. **Presentation Layer** - Blazor-based GUI for user interaction
5. **Plugin System** - Extensibility for custom functionality

### Why Build on Hydrogen?

| Feature | Benefit |
|---------|---------|
| **In-Protocol Upgrades** | Applications upgrade themselves via blockchain consensus |
| **Plugin Architecture** | Extend functionality without recompiling core |
| **Consensus Abstraction** | Swap consensus mechanisms without changing app logic |
| **Multi-Database Support** | Use SQL Server, SQLite, Firebird simultaneously |
| **Rich Collections** | Merkle trees, transactional collections, paged access |
| **Cross-Platform** | Windows, macOS, Linux with single codebase |
| **Enterprise-Grade Security** | 100+ cryptographic algorithms, signature schemes, key derivation |

---

## DApp Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────┐
│   User Interface (Blazor-based GUI)     │
├─────────────────────────────────────────┤
│   API Layer (JSON RPC)                  │
├─────────────────────────────────────────┤
│   DApp Processing Layer                 │
│   ├── Consensus Rules                   │
│   ├── State Transitions                 │
│   └── Business Logic                    │
├─────────────────────────────────────────┤
│   Persistence Layer                     │
│   ├── Blockchain (Consensus Database)   │
│   ├── Object Spaces (State)              │
│   └── Wallet Data                       │
├─────────────────────────────────────────┤
│   P2P Networking Layer                  │
├─────────────────────────────────────────┤
│   Hydrogen Framework (Collections,      │
│   Crypto, Serialization, etc.)          │
└─────────────────────────────────────────┘
```

### The Three-Tier Structure

DApps follow the [3-Tier Architecture](Guidelines/3-tier-Architecture.md):

**Presentation Tier**
- Blazor web application
- Plugin system for UI extensions
- Real-time updates via WebSockets
- Responsive design for mobile

**Processing Tier**
- Transaction validation
- Block validation
- Consensus rule enforcement
- State machine logic
- Plugin processing hooks

**Data Tier**
- Blockchain database (immutable)
- Object spaces (consensus state)
- Transactional collections
- Wallet persistence

---

## Core Components

### 1. Hydrogen.DApp.Core

**Purpose**: Core blockchain functionality and state management

**Key Classes**:

```csharp
// Block and Transaction handling
public class Block {
    public long BlockNumber { get; set; }
    public byte[] PreviousBlockHash { get; set; }
    public DateTime Timestamp { get; set; }
    public byte[] MerkleRoot { get; set; }
    public List<Transaction> Transactions { get; set; }
    public byte[] Hash { get; set; }
}

public class Transaction {
    public byte[] TransactionHash { get; set; }
    public List<Input> Inputs { get; set; }
    public List<Output> Outputs { get; set; }
    public byte[] Data { get; set; }
}

// Consensus Database Interface
public interface IConsensusDatabase {
    void StoreBlock(Block block);
    Block GetBlock(long blockNumber);
    Block GetLatestBlock();
    void ApplyTransaction(Transaction transaction);
}

// Wallet Management
public interface IWallet {
    byte[] GetPublicKey(byte[] privateKey);
    byte[] SignTransaction(Transaction tx, byte[] privateKey);
    bool VerifySignature(byte[] signature, byte[] transaction, byte[] publicKey);
}

// Plugin System
public interface IDAppPlugin {
    string Name { get; }
    Version Version { get; }
    void Initialize(IPluginContext context);
    void OnBlockAdded(Block block);
    void OnTransactionReceived(Transaction transaction);
}
```

### 2. Hydrogen.DApp.Node

**Purpose**: Full blockchain node with P2P networking and consensus

**Key Responsibilities**:
- Synchronizing blockchain with peers
- Mining or validating blocks (depending on consensus)
- Processing transaction mempool
- Hosting JSON RPC APIs
- Managing plugin lifecycle
- Persisting state

**Starting a Node**:

```csharp
using Hydrogen.DApp.Node;

var nodeConfig = new NodeConfiguration {
    Port = 8000,
    NetworkId = "MyDApp",
    GenesisBlockHash = "...",
    MaxBlockSize = 1_000_000,
    TargetBlockTime = 60  // seconds
};

var node = new BlockchainNode(nodeConfig);
await node.Start();

// Node automatically connects to peers, syncs blockchain,
// and begins consensus validation
```

### 3. Hydrogen.DApp.Host

**Purpose**: Process lifecycle management and application upgrading

**Key Features**:
- Loads HAP (Hydrogen Application Package)
- Manages Node and GUI processes
- Detects upgrade blocks on blockchain
- Archives and deploys new HAPs
- Handles inter-process communication via anonymous pipes

### 4. Hydrogen.DApp.Presentation

**Purpose**: Blazor-based GUI framework

**Includes**:
- Component library (buttons, grids, dialogs, etc.)
- Plugin system for UI extensions
- Real-time data binding
- Responsive layout system
- Theme support

---

## Building Your DApp

### Step 1: Define Your Consensus Rules

```csharp
using Hydrogen.DApp.Core;
using Hydrogen.CryptoEx;

public class MyConsensusRules : ConsensusRulesBase {
    
    private const long TargetBlockTime = 600;  // 10 minutes
    private const int DifficultyAdjustmentInterval = 2016;
    
    public override ValidationResult ValidateBlock(Block block, Block previousBlock) {
        // Validate proof-of-work
        if (!ValidateProofOfWork(block, GetCurrentDifficulty()))
            return ValidationResult.Failure("Invalid proof-of-work");
        
        // Validate timestamp
        if (block.Timestamp <= previousBlock.Timestamp)
            return ValidationResult.Failure("Block timestamp must be after previous block");
        
        // Validate merkle root
        var calculatedRoot = CalculateMerkleRoot(block.Transactions);
        if (!calculatedRoot.SequenceEqual(block.MerkleRoot))
            return ValidationResult.Failure("Invalid merkle root");
        
        // Validate all transactions
        foreach (var tx in block.Transactions) {
            var result = ValidateTransaction(tx);
            if (!result.IsValid)
                return result;
        }
        
        return ValidationResult.Success();
    }
    
    public override ValidationResult ValidateTransaction(Transaction tx) {
        // Custom transaction validation logic
        // Check signatures, balances, nonces, etc.
        return ValidationResult.Success();
    }
    
    public override byte[] GetCurrentDifficulty() {
        // Implement difficulty adjustment logic
        // Return target difficulty for next block
        return CalculateDifficulty();
    }
}
```

### Step 2: Define Your State Model

```csharp
using Hydrogen.ObjectSpace;

// Define your application state that lives in object space
public class ApplicationState {
    public Dictionary<byte[], AccountBalance> Balances { get; set; }
    public Dictionary<byte[], AccountNonce> Nonces { get; set; }
    public Dictionary<string, Asset> Assets { get; set; }
    public long TotalSupply { get; set; }
}

public class AccountBalance {
    public byte[] AccountAddress { get; set; }
    public long Balance { get; set; }
    public DateTime LastUpdate { get; set; }
}
```

### Step 3: Create Consensus Stream Handlers

```csharp
using Hydrogen.DApp.Core;

public class ConsensusStreamProcessor {
    
    private readonly ApplicationState _state;
    private readonly IWallet _wallet;
    
    public void ProcessTransaction(Transaction tx) {
        // Validate transaction signature
        if (!VerifySignature(tx))
            throw new InvalidTransactionException("Invalid signature");
        
        // Apply state changes
        var sender = ExtractSender(tx);
        var receiver = ExtractReceiver(tx);
        var amount = ExtractAmount(tx);
        
        if (!_state.Balances.ContainsKey(sender))
            _state.Balances[sender] = new AccountBalance { Balance = 0 };
        
        if (!_state.Balances.ContainsKey(receiver))
            _state.Balances[receiver] = new AccountBalance { Balance = 0 };
        
        _state.Balances[sender].Balance -= amount;
        _state.Balances[receiver].Balance += amount;
        
        // Increment nonce for replay protection
        _state.Nonces[sender].Nonce++;
    }
    
    public void ProcessBlock(Block block) {
        foreach (var tx in block.Transactions) {
            ProcessTransaction(tx);
        }
    }
    
    private bool VerifySignature(Transaction tx) {
        var signatureBytes = ExtractSignature(tx);
        var signer = ExtractSender(tx);
        var publicKey = _wallet.GetPublicKey(signer);
        return _wallet.VerifySignature(signatureBytes, GetTransactionData(tx), publicKey);
    }
}
```

### Step 4: Create Your DApp Class

```csharp
using Hydrogen.DApp.Core;

public class MyDApp : DAppBase {
    
    private readonly ConsensusStreamProcessor _processor;
    private readonly ApplicationState _state;
    
    public override string Name => "MyDApp";
    public override Version Version => new Version(1, 0, 0);
    
    public override void Initialize(IConsensusDatabase consensusDb, IPluginHost pluginHost) {
        base.Initialize(consensusDb, pluginHost);
        
        _state = new ApplicationState();
        _processor = new ConsensusStreamProcessor(_state);
        
        // Load genesis state
        LoadGenesisState();
    }
    
    public override ValidationResult ValidateTransaction(Transaction tx) {
        // Validate transaction format
        if (tx.Inputs.Count == 0)
            return ValidationResult.Failure("Transaction must have inputs");
        
        if (tx.Outputs.Count == 0)
            return ValidationResult.Failure("Transaction must have outputs");
        
        // Validate sender has sufficient balance
        var sender = ExtractSender(tx);
        var amount = ExtractAmount(tx);
        
        if (_state.Balances[sender].Balance < amount)
            return ValidationResult.Failure("Insufficient balance");
        
        // Check for replay attacks
        var nonce = ExtractNonce(tx);
        if (nonce != _state.Nonces[sender].Nonce)
            return ValidationResult.Failure("Invalid nonce");
        
        return ValidationResult.Success();
    }
    
    public override ValidationResult ValidateBlock(Block block, Block previousBlock) {
        // Validate block structure
        if (block.PreviousBlockHash == null || block.PreviousBlockHash.Length != 32)
            return ValidationResult.Failure("Invalid previous block hash");
        
        if (!block.PreviousBlockHash.SequenceEqual(previousBlock.Hash))
            return ValidationResult.Failure("Block hash chain broken");
        
        // Validate transactions
        foreach (var tx in block.Transactions) {
            var result = ValidateTransaction(tx);
            if (!result.IsValid)
                return result;
        }
        
        return ValidationResult.Success();
    }
    
    public override void ApplyBlock(Block block) {
        foreach (var tx in block.Transactions) {
            _processor.ProcessTransaction(tx);
        }
    }
    
    private void LoadGenesisState() {
        // Initialize with genesis accounts and distribution
        var genesisAccount = new byte[] { /* genesis address bytes */ };
        _state.Balances[genesisAccount] = new AccountBalance {
            Balance = 21_000_000_000_000_000,  // 21 million with 8 decimals
            LastUpdate = DateTime.UtcNow
        };
        _state.TotalSupply = 21_000_000_000_000_000;
    }
}
```

---

## Plugin Architecture

### Creating a Custom Plugin

Plugins extend both Node and GUI without requiring core recompilation.

```csharp
using Hydrogen.DApp.Core;

public class MyCustomPlugin : IDAppPlugin {
    
    public string Name => "MyCustomPlugin";
    public Version Version => new Version(1, 0, 0);
    
    private IPluginContext _context;
    
    public void Initialize(IPluginContext context) {
        _context = context;
        
        // Register message handlers
        context.RegisterMessageHandler("custom.action", OnCustomAction);
        
        // Register RPC methods
        context.RegisterRpcMethod("customMethod", CustomMethod);
        
        // Subscribe to blockchain events
        context.BlockchainEvents.OnBlockAdded += OnBlockAdded;
        context.BlockchainEvents.OnTransactionReceived += OnTransactionReceived;
    }
    
    public void OnBlockAdded(Block block) {
        // Custom logic when block is added
        Console.WriteLine($"Block {block.BlockNumber} added");
    }
    
    public void OnTransactionReceived(Transaction tx) {
        // Custom logic when transaction is received
        Console.WriteLine($"Transaction received: {tx.TransactionHash.ToHex()}");
    }
    
    public Response OnCustomAction(Message message) {
        // Handle custom messages
        return new Response { Status = "Success" };
    }
    
    public object CustomMethod(params object[] args) {
        // Expose custom RPC method
        return "Custom response";
    }
}
```

### Loading Plugins

```csharp
var pluginHost = new PluginHost();

// Load plugin from assembly
var assembly = Assembly.LoadFrom("path/to/MyCustomPlugin.dll");
pluginHost.LoadPlugin(assembly.GetType("MyCustomPlugin.MyCustomPlugin"));

// Plugins can also be loaded from blockchain (automatic deployment)
```

---

## Creating Custom Domains

### Example: Creating a Token Domain

```csharp
using Hydrogen.DApp.Core;

// Define token data structure
public class Token {
    public byte[] TokenId { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public byte Decimals { get; set; }
    public long TotalSupply { get; set; }
    public byte[] Owner { get; set; }
    public Dictionary<byte[], long> Balances { get; set; }
    public Dictionary<byte[], Dictionary<byte[], long>> Allowances { get; set; }
}

// Define operations
public enum TokenOperation {
    Create,
    Transfer,
    TransferFrom,
    Approve,
    Burn,
    Mint
}

// Define token transaction
public class TokenTransaction : Transaction {
    public TokenOperation Operation { get; set; }
    public Token Token { get; set; }
    public byte[] From { get; set; }
    public byte[] To { get; set; }
    public long Amount { get; set; }
    public byte[] Spender { get; set; }
    public long Allowance { get; set; }
}

// Implement token processor
public class TokenProcessor {
    
    private Dictionary<byte[], Token> _tokens = new();
    
    public ValidationResult Transfer(TokenTransaction tx) {
        // Validate token exists
        if (!_tokens.ContainsKey(tx.Token.TokenId))
            return ValidationResult.Failure("Token not found");
        
        var token = _tokens[tx.Token.TokenId];
        
        // Validate sender has sufficient balance
        if (token.Balances[tx.From] < tx.Amount)
            return ValidationResult.Failure("Insufficient token balance");
        
        // Update balances
        token.Balances[tx.From] -= tx.Amount;
        if (!token.Balances.ContainsKey(tx.To))
            token.Balances[tx.To] = 0;
        token.Balances[tx.To] += tx.Amount;
        
        return ValidationResult.Success();
    }
    
    public ValidationResult CreateToken(TokenTransaction tx) {
        // Create new token
        var token = new Token {
            TokenId = GenerateTokenId(),
            Name = tx.Token.Name,
            Symbol = tx.Token.Symbol,
            Decimals = tx.Token.Decimals,
            TotalSupply = tx.Token.TotalSupply,
            Owner = tx.From,
            Balances = new Dictionary<byte[], long> { { tx.From, tx.Token.TotalSupply } },
            Allowances = new Dictionary<byte[], Dictionary<byte[], long>>()
        };
        
        _tokens[token.TokenId] = token;
        return ValidationResult.Success();
    }
}
```

---

## Consensus Integration

### Implementing Proof-of-Work

```csharp
using Hydrogen.CryptoEx;
using System.Numerics;

public class ProofOfWorkValidator {
    
    private const int DifficultyBits = 20;  // Difficulty target
    
    public bool ValidateProofOfWork(Block block) {
        var blockHash = ComputeBlockHash(block);
        var difficulty = GetDifficultyTarget();
        
        return BigInteger.Parse(blockHash) < difficulty;
    }
    
    public Block MineBlock(Block candidate) {
        var difficulty = GetDifficultyTarget();
        var nonce = 0UL;
        
        while (true) {
            candidate.Nonce = nonce;
            var hash = ComputeBlockHash(candidate);
            
            if (BigInteger.Parse(hash) < difficulty) {
                candidate.Hash = System.Convert.FromHexString(hash);
                return candidate;
            }
            
            nonce++;
            
            // Yield CPU periodically
            if (nonce % 1000000 == 0)
                System.Threading.Thread.Sleep(0);
        }
    }
    
    private BigInteger GetDifficultyTarget() {
        // Difficulty = 2^256 / (target * 2^32)
        var maxTarget = BigInteger.Parse("00000000FFFF0000000000000000000000000000000000000000000000000000", System.Globalization.NumberStyles.HexNumber);
        return maxTarget >> DifficultyBits;
    }
    
    private string ComputeBlockHash(Block block) {
        using (var hasher = System.Security.Cryptography.SHA256.Create()) {
            var blockData = SerializeBlock(block);
            var hash = hasher.ComputeHash(blockData);
            return System.Convert.ToHexString(hash);
        }
    }
    
    private byte[] SerializeBlock(Block block) {
        // Serialize block for hashing
        using (var ms = new System.IO.MemoryStream()) {
            using (var bw = new System.IO.BinaryWriter(ms)) {
                bw.Write(block.BlockNumber);
                bw.Write(block.PreviousBlockHash);
                bw.Write(block.Timestamp.Ticks);
                bw.Write(block.MerkleRoot);
                bw.Write(block.Nonce);
                return ms.ToArray();
            }
        }
    }
}
```

### Implementing Proof-of-Stake

```csharp
public class ProofOfStakeValidator {
    
    private Dictionary<byte[], long> _stakes = new();
    
    public bool ValidateProofOfStake(Block block) {
        // Verify block producer has adequate stake
        var producer = ExtractProducer(block);
        
        if (!_stakes.ContainsKey(producer))
            return false;
        
        return _stakes[producer] >= MinimumStake;
    }
    
    public Block ProposeBlock(Block candidate, byte[] validatorAddress) {
        // Only validators with stake can propose blocks
        if (!_stakes.ContainsKey(validatorAddress) || 
            _stakes[validatorAddress] < MinimumStake)
            throw new InvalidOperationException("Insufficient stake");
        
        candidate.Hash = ComputeBlockHash(candidate);
        return candidate;
    }
    
    public void AddStake(byte[] address, long amount) {
        if (!_stakes.ContainsKey(address))
            _stakes[address] = 0;
        _stakes[address] += amount;
    }
}
```

---

## State Management

### Using Object Spaces for Distributed State

```csharp
using Hydrogen.ObjectSpace;

// Define your consensus state
public class DAppState : ConsensusState {
    
    public ObjectSpace<AccountData> Accounts { get; set; }
    public ObjectSpace<TransactionRecord> Transactions { get; set; }
    public ObjectSpace<SmartContractState> Contracts { get; set; }
    
    public override void ApplyBlock(Block block) {
        foreach (var tx in block.Transactions) {
            var account = Accounts.Get(ExtractSender(tx));
            account.Nonce++;
            account.LastUpdate = DateTime.UtcNow;
            Accounts.Update(account);
            
            Transactions.Add(new TransactionRecord {
                Hash = tx.TransactionHash,
                BlockNumber = block.BlockNumber,
                Timestamp = block.Timestamp
            });
        }
    }
}

// Object Space provides:
// - CRUD operations
// - Consensus validation
// - Merkle tree proofs
// - Point-in-time snapshots
```

---

## Testing & Deployment

### Unit Testing Consensus Rules

```csharp
using NUnit.Framework;

[TestFixture]
public class ConsensusRulesTests {
    
    private MyConsensusRules _rules;
    private MyDApp _dApp;
    
    [SetUp]
    public void Setup() {
        _rules = new MyConsensusRules();
        _dApp = new MyDApp();
    }
    
    [Test]
    public void ValidBlock_Accepts() {
        var block = CreateValidBlock();
        var previous = CreateGenesisBlock();
        
        var result = _rules.ValidateBlock(block, previous);
        
        Assert.IsTrue(result.IsValid);
    }
    
    [Test]
    public void InvalidProofOfWork_Rejects() {
        var block = CreateValidBlock();
        block.Nonce = 999999;  // Wrong nonce
        
        var result = _rules.ValidateBlock(block, CreateGenesisBlock());
        
        Assert.IsFalse(result.IsValid);
    }
    
    [Test]
    public void DuplicateTransaction_Rejects() {
        var tx = CreateTransaction();
        var block = new Block {
            Transactions = new[] { tx, tx }  // Duplicate
        };
        
        var result = _rules.ValidateBlock(block, CreateGenesisBlock());
        
        Assert.IsFalse(result.IsValid);
    }
}
```

### Integration Testing with Full Node

```csharp
[TestFixture]
public class IntegrationTests {
    
    private BlockchainNode _node;
    
    [SetUp]
    public async Task Setup() {
        var config = new NodeConfiguration {
            Port = 8001,
            InMemoryDatabase = true  // Use in-memory for testing
        };
        
        _node = new BlockchainNode(config);
        await _node.Start();
    }
    
    [Test]
    public async Task SubmitTransaction_MinedIntoBlock() {
        var tx = CreateTransaction();
        
        await _node.SubmitTransaction(tx);
        
        // Wait for mining
        await Task.Delay(TimeSpan.FromSeconds(1));
        
        // Verify transaction in blockchain
        var block = _node.GetLatestBlock();
        Assert.IsTrue(block.Transactions.Any(t => t.TransactionHash.SequenceEqual(tx.TransactionHash)));
    }
    
    [TearDown]
    public async Task Teardown() {
        await _node.Stop();
    }
}
```

### Deployment Checklist

Before deploying to production:

- [ ] All consensus rules thoroughly tested
- [ ] Transaction validation comprehensive
- [ ] Plugin system tested
- [ ] Performance benchmarks acceptable
- [ ] Security audit completed
- [ ] Genesis block finalized
- [ ] Network parameters set
- [ ] Plugin registry configured
- [ ] API endpoints documented
- [ ] Monitoring setup
- [ ] Backup/recovery procedures
- [ ] Upgrade path for future versions

---

## Best Practices

### 1. Consensus Rule Safety

```csharp
// ✅ DO: Return descriptive validation results
public ValidationResult ValidateTransaction(Transaction tx) {
    if (tx.Inputs.Count == 0)
        return ValidationResult.Failure("Transaction must have inputs");
    
    if (TotalInputAmount(tx.Inputs) < TotalOutputAmount(tx.Outputs))
        return ValidationResult.Failure("Insufficient input amount");
    
    return ValidationResult.Success();
}

// ❌ DON'T: Silent failures
public ValidationResult ValidateTransaction(Transaction tx) {
    try {
        // validation logic
        return ValidationResult.Success();
    } catch {
        return ValidationResult.Failure("Validation error");  // Too vague
    }
}
```

### 2. Plugin Safety

```csharp
// ✅ DO: Validate plugin signatures
public void LoadPlugin(string pluginPath) {
    var signature = GetPluginSignature(pluginPath);
    if (!VerifySignature(signature, AllowedPublicKeys))
        throw new SecurityException("Plugin not signed by trusted authority");
    
    LoadAndExecutePlugin(pluginPath);
}

// ❌ DON'T: Load unsigned plugins
public void LoadPlugin(string pluginPath) {
    var assembly = Assembly.LoadFrom(pluginPath);
    // No signature verification
}
```

### 3. State Consistency

```csharp
// ✅ DO: Atomic state updates
public void ProcessBlock(Block block) {
    using (var transaction = _db.BeginTransaction()) {
        try {
            foreach (var tx in block.Transactions) {
                ProcessTransaction(tx);
            }
            transaction.Commit();
        } catch {
            transaction.Rollback();
            throw;
        }
    }
}

// ❌ DON'T: Partial updates
public void ProcessBlock(Block block) {
    foreach (var tx in block.Transactions) {
        ProcessTransaction(tx);  // No rollback if one fails
    }
}
```

### 4. Performance Optimization

```csharp
// ✅ DO: Batch operations
public void ProcessTransactions(List<Transaction> transactions) {
    using (var batch = _db.CreateBatch()) {
        foreach (var tx in transactions) {
            var account = _state.Accounts.Get(ExtractSender(tx));
            account.Balance -= GetAmount(tx);
            batch.Add(account);
        }
        _db.WriteBatch(batch);  // Single write
    }
}

// ❌ DON'T: Individual operations
public void ProcessTransactions(List<Transaction> transactions) {
    foreach (var tx in transactions) {
        var account = _state.Accounts.Get(ExtractSender(tx));
        account.Balance -= GetAmount(tx);
        _db.SaveAccount(account);  // Multiple writes
    }
}
```

### 5. Error Recovery

```csharp
// ✅ DO: Graceful degradation
public ValidationResult SyncBlockchain() {
    try {
        return FullSync();
    } catch (NetworkException) {
        // Fall back to last known good state
        return RestoreLastSnapshot();
    } catch (ConsensusException ex) {
        // Log and alert
        Logger.Error($"Consensus violation: {ex}");
        return ValidationResult.Failure("Consensus check failed");
    }
}

// ❌ DON'T: Hard failures
public void SyncBlockchain() {
    FullSync();  // Crashes on any error
}
```

---

## Resources & References

- [Hydrogen.DApp.Core README](../src/Hydrogen.DApp.Core/README.md)
- [Hydrogen.DApp.Node README](../src/Hydrogen.DApp.Node/README.md)
- [Consensus Documentation](Architecture/Runtime.md)
- [Code Styling Guidelines](Guidelines/Code-Styling.md)

---

**Version**: 1.0  
**Last Updated**: December 2025  
**Author**: Sphere 10 Software
