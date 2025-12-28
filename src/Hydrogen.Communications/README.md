# Hydrogen.Communications

A comprehensive communication framework for building networked applications with support for multiple protocols, RPC, and real-time data exchange.

## 📋 Overview

`Hydrogen.Communications` provides a complete networking layer for Hydrogen applications, enabling peer-to-peer communication, remote procedure calls, and efficient data exchange across various transport protocols.

## 🏗️ Architecture

The library is organized into key protocol and utility modules:

- **Protocols**: Abstract protocol definitions and implementations
- **TCP**: TCP/IP socket communication layer
- **UDP**: UDP-based datagram communication
- **WebSockets**: WebSocket protocol implementation for real-time communication
- **RPC**: Remote Procedure Call infrastructure for service invocation
- **JSON**: JSON serialization utilities for message payloads
- **DataSource**: Data source abstractions for communication endpoints
- **EndPoint**: Network endpoint configuration and management
- **Pipes**: Named pipe communication support

## 🚀 Key Features

- **Multi-Protocol Support**: TCP, UDP, WebSockets, and pipes
- **RPC Framework**: Service-oriented remote procedure calls
- **Abstracted Communication**: Protocol-agnostic interfaces for flexibility
- **Data Serialization**: JSON support for message payloads
- **Endpoint Management**: Unified interface for managing connection endpoints
- **Async/Await Compatible**: Modern async communication patterns

## 🔧 Usage

Establish communication between nodes:

```csharp
using Hydrogen.Communications;

// TCP Communication
var endpoint = new TcpEndPoint("192.168.1.100", 8080);

// WebSocket Real-time communication
var wsEndpoint = new WebSocketEndPoint("ws://localhost:9000");
```

## 📦 Dependencies

- **Hydrogen**: Core framework utilities
- Custom serialization and protocol implementations

## 📄 Related Projects

- [Hydrogen](../Hydrogen) - Core framework library
- [Hydrogen.DApp.Node](../Hydrogen.DApp.Node) - Node implementation using communications
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core) - DApp core services
