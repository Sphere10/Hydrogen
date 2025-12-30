<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# ⛓️ Hydrogen.DApp.Core

**Core blockchain and distributed application framework** providing fundamental types, patterns, and infrastructure for building blockchain-based DApps with plugin support and in-protocol upgrades.

Hydrogen.DApp.Core is the **foundation for all Hydrogen DApps**, enabling developers to build blockchain systems with flexible transaction handling, multi-wallet support, plugin architecture, and governance-driven upgrades through blockchain state.

## ⚡ 10-Second Example

```csharp
using Hydrogen.DApp.Core;

// Define a blockchain application
public class MyBlockchainApp : Plugin {
    public override string Name => "MyBlockchainApp";
    public override Version Version => new Version(1, 0, 0);

    public override void OnInitialize() {
        // Setup blockchain features
    }

    public override void OnShutdown() {
        // Cleanup resources
    }
}

// Initialize and load the blockchain
var core = new DAppCore();
core.LoadPlugin<MyBlockchainApp>();
var app = core.GetPlugin<MyBlockchainApp>();

Console.WriteLine($"Running {app.Name} v{app.Version}");
```

## 🏗️ Core Concepts

**Blockchain State**: Immutable ledger tracking all transactions and state changes.

**Plugin Architecture**: Dynamically load blockchain applications; upgrade through governance.

**Transaction Framework**: Flexible transaction creation, validation, and processing.

**Wallet Management**: Multi-wallet support with key management and signing.

**Consensus Integration**: Interface with various consensus mechanisms (PoW, PoS, etc.).

**Peer Discovery**: Network synchronization and peer-to-peer communication.

**Event System**: React to blockchain events through subscription patterns.

## 🔧 Core Examples

### Initialize Blockchain Core

```csharp
using Hydrogen.DApp.Core;
using Hydrogen.DApp.Core.Configuration;

// Create configuration
var config = new BlockchainConfiguration {
    NetworkId = 1,
    ChainName = "MyChain",
    GenesisBlock = BlockFactory.CreateGenesis(),
    Consensus = new ProofOfWorkConsensus()
};

// Initialize core
var core = new DAppCore(config);

// Connect to network
await core.ConnectToNetworkAsync();

// Check blockchain state
var currentBlock = core.CurrentBlock;
var blockHeight = core.BlockHeight;
var difficulty = core.CurrentDifficulty;

Console.WriteLine($"Height: {blockHeight}, Difficulty: {difficulty}");
```

### Create & Register Plugins

```csharp
using Hydrogen.DApp.Core;

// Define custom application plugin
[Plugin("MyDApp", "1.0.0")]
public class MyDApp : Plugin {
    public override string Name => "MyDApp";
    public override Version Version => new Version(1, 0, 0);
    
    private IBlockchain _blockchain;
    
    public override void OnInitialize() {
        // Initialize plugin resources
        _blockchain = GetService<IBlockchain>();
        Console.WriteLine("MyDApp initialized");
    }
    
    public override void OnShutdown() {
        Console.WriteLine("MyDApp shutting down");
    }
    
    public void ProcessTransaction(Transaction tx) {
        _blockchain.ValidateTransaction(tx);
    }
}

// Register and use plugin
var core = new DAppCore();
core.RegisterPlugin<MyDApp>();

// Get plugin instance
var myApp = core.GetPlugin<MyDApp>();
myApp.ProcessTransaction(transaction);
```

### Multi-Wallet Account Management

```csharp
using Hydrogen.DApp.Core;
using Hydrogen.DApp.Core.Wallets;
using Hydrogen.CryptoEx;

// Create wallet
var wallet = new Wallet("MyWallet");

// Generate accounts with keys
var account1 = wallet.CreateAccount("Alice");
var account2 = wallet.CreateAccount("Bob");

// Get account details
var alice = wallet.GetAccount("Alice");
var privateKey = alice.PrivateKey;
var publicKey = alice.PublicKey;
var address = alice.Address;

Console.WriteLine($"Alice's Address: {address}");

// Sign transaction with account key
var transaction = new Transaction {
    From = alice.Address,
    To = bob.Address,
    Amount = 100,
    Nonce = 1
};

var signature = alice.SignData(transaction.ToBytes());
transaction.Signature = signature;

// Verify signature
bool isValid = PublicKey.VerifySignature(
    transaction.ToBytes(), 
    signature, 
    alice.PublicKey);

Console.WriteLine($"Signature valid: {isValid}");
```

### Transaction Creation & Validation

```csharp
using Hydrogen.DApp.Core;

var blockchain = core.GetBlockchain();

// Create transaction
var transaction = new Transaction {
    From = "0xAlice",
    To = "0xBob",
    Amount = 500,
    Fee = 1,
    Nonce = blockchain.GetAccountNonce("0xAlice") + 1,
    Timestamp = DateTime.UtcNow,
    Data = null
};

// Validate before broadcasting
var validation = blockchain.ValidateTransaction(transaction);

if (validation.IsValid) {
    // Add to mempool
    await blockchain.AddTransactionAsync(transaction);
    Console.WriteLine("Transaction accepted");
} else {
    Console.WriteLine($"Invalid: {validation.Error}");
}

// Track transaction
var txHash = transaction.GetHash();
var receipt = blockchain.GetTransactionReceipt(txHash);
Console.WriteLine($"Status: {receipt.Status}");
Console.WriteLine($"Gas Used: {receipt.GasUsed}");
```

### Block Creation & Mining

```csharp
using Hydrogen.DApp.Core;
using Hydrogen.DApp.Core.Mining;

var blockchain = core.GetBlockchain();
var miner = new ProofOfWorkMiner();

// Get pending transactions
var pendingTxs = blockchain.GetPendingTransactions();

// Create block
var block = new Block {
    Index = blockchain.BlockHeight + 1,
    Timestamp = DateTime.UtcNow,
    Transactions = pendingTxs,
    PreviousHash = blockchain.GetLastBlock().Hash,
    Miner = minerAddress,
    Nonce = 0
};

// Mine block (find valid nonce)
var minerResult = miner.Mine(block);

if (minerResult.IsMined) {
    // Add block to chain
    await blockchain.AddBlockAsync(block);
    Console.WriteLine($"Block {block.Index} mined!");
} else {
    Console.WriteLine("Mining failed");
}
```

### Blockchain Events & Subscriptions

```csharp
using Hydrogen.DApp.Core;

var blockchain = core.GetBlockchain();

// Subscribe to block events
blockchain.OnBlockAdded += (sender, block) => {
    Console.WriteLine($"Block added: {block.Index} with {block.Transactions.Count} transactions");
};

// Subscribe to transaction events
blockchain.OnTransactionAdded += (sender, tx) => {
    Console.WriteLine($"Transaction: {tx.From} -> {tx.To}: {tx.Amount}");
};

// Subscribe to state changes
blockchain.OnStateChanged += (sender, state) => {
    Console.WriteLine($"State updated: {state.Description}");
};

// Process transactions or mine blocks
// Events will fire automatically
await blockchain.ProcessAsync();
```

### Consensus & Difficulty Adjustment

```csharp
using Hydrogen.DApp.Core;

var blockchain = core.GetBlockchain();

// Get current difficulty
var currentDifficulty = blockchain.GetCurrentDifficulty();
Console.WriteLine($"Current Difficulty: {currentDifficulty}");

// Target block time
var targetBlockTime = TimeSpan.FromSeconds(10);  // 10 seconds

// Adjust difficulty based on recent blocks
var lastBlocks = blockchain.GetLastBlocks(100);
var avgBlockTime = CalculateAverageBlockTime(lastBlocks);

if (avgBlockTime > targetBlockTime) {
    // Blocks are taking too long, decrease difficulty
    var newDifficulty = currentDifficulty * 0.9;
    blockchain.SetDifficulty(newDifficulty);
} else if (avgBlockTime < targetBlockTime) {
    // Blocks are too fast, increase difficulty
    var newDifficulty = currentDifficulty * 1.1;
    blockchain.SetDifficulty(newDifficulty);
}

Console.WriteLine($"New Difficulty: {blockchain.GetCurrentDifficulty()}");
```

## 🏗️ Architecture & Domains

**Blockchain Domain**: Block, transaction, and chain management
- Block creation and validation
- Transaction processing and verification
- Chain integrity checking
- Genesis block initialization

**Wallet Domain**: Account and key management
- Multi-account wallets
- Key generation and storage
- Transaction signing
- Address generation

**Plugin System**: Dynamic application loading
- Plugin discovery and registration
- Lifecycle management (Initialize, Shutdown)
- Service injection to plugins
- In-protocol upgrades

**Network Domain**: Peer-to-peer synchronization
- Peer discovery and connection
- Block and transaction propagation
- State synchronization
- Network consensus

**Consensus Domain**: Consensus mechanism abstraction
- Pluggable consensus algorithms
- Difficulty adjustment
- Block validation rules
- Fork resolution

**Storage Domain**: Persistent state
- Block storage and retrieval
- Account state persistence
- Transaction history
- Index management

## 📦 Dependencies

- **Hydrogen**: Core framework
- **Hydrogen.Application**: Application lifecycle
- **Hydrogen.Communications**: Network communication
- **Hydrogen.CryptoEx**: Cryptographic primitives and hashing
- **Hydrogen.Data**: Persistent data access
- **BouncyCastle.Cryptography**: Cryptographic library
- **Newtonsoft.Json**: JSON serialization
- **SharpZipLib**: Compression support

## ⚠️ Best Practices

- **Validate all inputs**: Validate transactions and blocks before processing
- **Async operations**: Use async/await for network and I/O operations
- **Handle forks**: Implement proper fork resolution in consensus
- **Secure keys**: Never log or expose private keys; use secure storage
- **Rate limiting**: Implement DoS protection for network operations
- **State snapshots**: Periodically snapshot blockchain state for recovery
- **Monitor gas/fees**: Track computational costs and fee structures

## ✅ Status & Compatibility

- **Maturity**: Production-tested for blockchain applications
- **.NET Target**: .NET 8.0+ (primary), .NET 6.0+ (compatible)
- **Performance**: Optimized for high-throughput transaction processing
- **Scalability**: Designed for blockchain networks with thousands of nodes

## 📖 Related Projects

- [Hydrogen.DApp.Node](../Hydrogen.DApp.Node) - Reference node implementation
- [Hydrogen.DApp.Host](../Hydrogen.DApp.Host) - Node hosting and lifecycle
- [Hydrogen.Communications](../Hydrogen.Communications) - Network layer
- [Hydrogen.CryptoEx](../Hydrogen.CryptoEx) - Cryptographic functions
- [Hydrogen.Data](../Hydrogen.Data) - Persistence layer

## ⚖️ License

Distributed under the **MIT NON-AI License**.

See the LICENSE file for full details. More information: [Sphere10 NON-AI-MIT License](https://sphere10.com/legal/NON-AI-MIT)

## 👤 Author

**Herman Schoenfeld** - Software Engineer

---

**Version**: 2.0+
