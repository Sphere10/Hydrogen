<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# ⛓️ Hydrogen.DApp.Node

**Reference blockchain node implementation** for Hydrogen DApps providing a complete, production-ready node with consensus, mining, network synchronization, and plugin support.

Hydrogen.DApp.Node is a **runnable blockchain node** that implements the full Hydrogen DApp protocol, connecting to peer networks, validating transactions and blocks, mining, and maintaining blockchain state while supporting plugin-based DApps.

## ⚡ 10-Second Example

```csharp
using Hydrogen.DApp.Node;

// Start a node with default configuration
var nodeConfig = new NodeConfiguration {
    NetworkId = 1,
    Port = 8080,
    DataDirectory = "./data",
    MiningEnabled = true
};

var node = new HydrogenNode(nodeConfig);
await node.StartAsync();

Console.WriteLine($"Node started: {node.NodeId}");
Console.WriteLine($"Listening on port {node.Port}");
Console.WriteLine($"Connected peers: {node.PeerCount}");

// Mining runs automatically
Console.WriteLine($"Mining: {node.IsMining}");
```

## 🏗️ Core Concepts

**Blockchain Node**: Complete implementation of blockchain protocol.

**Consensus Engine**: Pluggable consensus mechanism (PoW, PoS, etc.).

**Mining & Block Creation**: Proof-of-work mining with difficulty adjustment.

**Peer-to-Peer Network**: Connect to other nodes, synchronize state.

**Transaction Mempool**: Queue pending transactions for inclusion in blocks.

**Plugin Support**: Run blockchain DApps alongside core node functionality.

**State Persistence**: Persistent ledger storage with recovery support.

## 🔧 Core Examples

### Initialize & Start a Node

```csharp
using Hydrogen.DApp.Node;
using Hydrogen.DApp.Core;
using System.Net;

// Create node configuration
var config = new NodeConfiguration {
    NetworkId = 1,                          // Network identifier
    NetworkName = "hydrogen-testnet",
    NodeName = "my-node-1",
    DataDirectory = "./blockchain-data",
    Port = 8080,
    MaxPeers = 50,
    
    // Consensus settings
    ConsensusType = ConsensusType.ProofOfWork,
    TargetBlockTime = 10,                   // 10 seconds
    DifficultyAdjustmentInterval = 2016,    // blocks
    
    // Mining settings
    MiningEnabled = true,
    MinerAddress = "0xMinerAddress",
    MinerThreadCount = Environment.ProcessorCount,
    
    // Network settings
    SeedPeers = new[] {
        "127.0.0.1:8081",
        "127.0.0.1:8082"
    }
};

// Create and start node
var node = new HydrogenNode(config);

// Start background services
await node.StartAsync();

// Verify node is running
Console.WriteLine($"Node ID: {node.NodeId}");
Console.WriteLine($"Listening on {IPAddress.Loopback}:{node.Port}");
Console.WriteLine($"Blockchain Height: {node.BlockHeight}");
Console.WriteLine($"Current Difficulty: {node.CurrentDifficulty}");
```

### Connect to Network Peers

```csharp
using Hydrogen.DApp.Node;

var node = // ... initialized node ...

// Listen for peer connection events
node.PeerConnected += async (peer) => {
    Console.WriteLine($"Peer connected: {peer.EndPoint}");
    
    // Request peer's blockchain height
    var peerHeight = await node.GetPeerBlockHeightAsync(peer);
    Console.WriteLine($"  Height: {peerHeight}");
};

node.PeerDisconnected += (peer, reason) => {
    Console.WriteLine($"Peer disconnected: {peer.EndPoint} ({reason})");
};

// Connect to specific peer
await node.ConnectToPeerAsync("192.168.1.100", 8080);

// Get peer list
var peers = node.GetConnectedPeers();
Console.WriteLine($"Connected to {peers.Count} peers");

foreach (var peer in peers) {
    Console.WriteLine($"  {peer.EndPoint} - Height: {peer.BlockHeight}");
}
```

### Mining & Block Creation

```csharp
using Hydrogen.DApp.Node;

var node = // ... initialized node ...

// Monitor mining events
node.BlockMined += (block) => {
    Console.WriteLine($"Block mined: #{block.Index}");
    Console.WriteLine($"  Hash: {block.Hash}");
    Console.WriteLine($"  Transactions: {block.Transactions.Count}");
    Console.WriteLine($"  Miner: {block.Miner}");
    Console.WriteLine($"  Nonce: {block.Nonce}");
    Console.WriteLine($"  Difficulty: {block.Difficulty}");
};

// Get mining stats
var miningStats = node.GetMiningStats();
Console.WriteLine($"Mining enabled: {miningStats.IsEnabled}");
Console.WriteLine($"Hash rate: {miningStats.HashesPerSecond:F2} h/s");
Console.WriteLine($"Blocks mined: {miningStats.BlocksMined}");
Console.WriteLine($"Total hashes: {miningStats.TotalHashes}");

// Stop mining
await node.StopMiningAsync();

// Resume mining
await node.StartMiningAsync();
```

### Transaction Processing

```csharp
using Hydrogen.DApp.Node;
using Hydrogen.DApp.Core;

var node = // ... initialized node ...

// Create transaction
var tx = new Transaction {
    From = "0xAliceAddress",
    To = "0xBobAddress",
    Amount = 100,
    Fee = 1,
    Nonce = await node.GetAccountNonceAsync("0xAliceAddress"),
    Timestamp = DateTime.UtcNow,
    Data = null
};

// Sign transaction (client-side)
var signature = SignTransaction(tx, alicePrivateKey);
tx.Signature = signature;

// Broadcast to node
var result = await node.BroadcastTransactionAsync(tx);

if (result.IsAccepted) {
    Console.WriteLine($"Transaction accepted: {result.TransactionHash}");
} else {
    Console.WriteLine($"Transaction rejected: {result.Error}");
}

// Monitor mempool
node.TransactionAdded += (tx) => {
    Console.WriteLine($"Mempool: +{tx.GetHash()}");
};

node.TransactionRemoved += (txHash) => {
    Console.WriteLine($"Mempool: -{txHash}");
};

// Get mempool state
var mempoolTxs = node.GetPendingTransactions();
Console.WriteLine($"Pending transactions: {mempoolTxs.Count}");
```

### Blockchain State & Query

```csharp
using Hydrogen.DApp.Node;

var node = // ... initialized node ...

// Get blockchain info
var blockchainInfo = node.GetBlockchainInfo();
Console.WriteLine($"Height: {blockchainInfo.BlockHeight}");
Console.WriteLine($"Difficulty: {blockchainInfo.CurrentDifficulty}");
Console.WriteLine($"Total transactions: {blockchainInfo.TotalTransactions}");
Console.WriteLine($"Accounts: {blockchainInfo.AccountCount}");

// Get block by height
var block = node.GetBlockByHeight(12345);
Console.WriteLine($"Block {block.Index}: {block.Hash}");
Console.WriteLine($"Timestamp: {block.Timestamp}");
Console.WriteLine($"Transactions: {block.Transactions.Count}");

// Get transaction by hash
var txHash = "0x123abc...";
var transaction = node.GetTransactionByHash(txHash);
Console.WriteLine($"Transaction: {transaction.From} -> {transaction.To}: {transaction.Amount}");

// Get transaction receipt
var receipt = node.GetTransactionReceipt(txHash);
Console.WriteLine($"Status: {receipt.Status}");
Console.WriteLine($"Block: {receipt.BlockHeight}");
Console.WriteLine($"Gas used: {receipt.GasUsed}");

// Query account balance
var address = "0xAliceAddress";
var balance = await node.GetAccountBalanceAsync(address);
Console.WriteLine($"{address} balance: {balance}");

// Get account nonce (transaction count)
var nonce = await node.GetAccountNonceAsync(address);
Console.WriteLine($"Nonce: {nonce}");
```

### Plugin Management

```csharp
using Hydrogen.DApp.Node;
using Hydrogen.DApp.Core;

var node = // ... initialized node ...

// Define plugin
[Plugin("CustomDApp", "1.0.0")]
public class CustomDApp : Plugin {
    public override string Name => "CustomDApp";
    
    public override void OnInitialize() {
        Console.WriteLine("CustomDApp initialized");
    }
    
    public void OnTransactionReceived(Transaction tx) {
        Console.WriteLine($"Received transaction: {tx.GetHash()}");
    }
}

// Load plugin in node
node.LoadPlugin<CustomDApp>();

// Get loaded plugins
var plugins = node.GetLoadedPlugins();
foreach (var plugin in plugins) {
    Console.WriteLine($"Plugin: {plugin.Name} v{plugin.Version}");
}

// Disable plugin (for upgrades)
await node.UnloadPluginAsync<CustomDApp>();
```

### Synchronization & Catching Up

```csharp
using Hydrogen.DApp.Node;

var node = // ... initialized node ...

// Monitor sync progress
node.SyncStarted += () => {
    Console.WriteLine("Sync started");
};

node.BlocksSync += (blocksDownloaded) => {
    var syncProgress = (node.BlockHeight / (double)node.MaxPeerHeight) * 100;
    Console.WriteLine($"Sync progress: {syncProgress:F1}% ({node.BlockHeight}/{node.MaxPeerHeight})");
};

node.SyncCompleted += () => {
    Console.WriteLine("Sync completed!");
};

// Check if node is synchronized
bool isSynced = node.IsSynchronized;
Console.WriteLine($"Synchronized: {isSynced}");

// Get sync status
var syncStatus = node.GetSyncStatus();
Console.WriteLine($"Local height: {syncStatus.LocalHeight}");
Console.WriteLine($"Max peer height: {syncStatus.MaxPeerHeight}");
Console.WriteLine($"Synced blocks: {syncStatus.BlocksSynced}");
```

## 🏗️ Architecture & Components

**Node Service**: Main node coordinator
- Lifecycle management
- Component initialization
- Event publishing
- Public API

**Consensus Engine**: Pluggable consensus implementation
- Block validation
- Difficulty adjustment
- Fork resolution
- Finality tracking

**Mining Module**: Block creation and PoW
- Transaction selection
- Nonce search
- Hash computation
- Block broadcasting

**Network Module**: P2P communication
- Peer discovery
- Connection management
- Message routing
- Block/transaction sync

**Storage Module**: Persistent state
- Block storage
- Account state
- Transaction history
- State snapshots

**Plugin System**: DApp loading and lifecycle
- Plugin discovery
- Dependency injection
- Lifecycle events
- Service integration

## 📦 Dependencies

- **Hydrogen**: Core framework
- **Hydrogen.DApp.Core**: DApp framework and types
- **Hydrogen.Application**: Application lifecycle
- **Hydrogen.Communications**: Network communication
- **Hydrogen.CryptoEx**: Cryptographic operations
- **Hydrogen.Data**: Persistent storage

## ⚠️ Best Practices

- **Port Forwarding**: Configure firewall and port forwarding for peer connectivity
- **Disk Space**: Ensure sufficient disk space for blockchain growth
- **Network Bandwidth**: Monitor bandwidth for high-volume networks
- **Graceful Shutdown**: Always use `StopAsync()` for clean shutdown
- **Backup State**: Regularly backup blockchain data
- **Monitor Peers**: Keep track of peer health and connections
- **Validate Input**: All external input is validated before processing
- **Security**: Run with minimal required permissions

## ✅ Status & Compatibility

- **Maturity**: Production-ready for blockchain networks
- **.NET Target**: .NET 8.0+ (primary)
- **Consensus Pluggable**: Supports PoW, PoS, and custom implementations
- **Performance**: Optimized for thousands of transactions per second

## 📖 Related Projects

- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core) - Core blockchain framework
- [Hydrogen.DApp.Host](../Hydrogen.DApp.Host) - Node hosting and process management
- [Hydrogen.Communications](../Hydrogen.Communications) - Network layer
- [Hydrogen.CryptoEx](../Hydrogen.CryptoEx) - Cryptographic functions

## ⚖️ License

Distributed under the **MIT NON-AI License**.

See the LICENSE file for full details. More information: [Sphere10 NON-AI-MIT License](https://sphere10.com/legal/NON-AI-MIT)

## 👤 Author

**Herman Schoenfeld** - Software Engineer

---

**Version**: 2.0+
