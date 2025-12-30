# 📡 Hydrogen.Communications

**Multi-protocol networking framework for .NET 8.0+** with support for RPC (Remote Procedure Call) APIs, protocol abstraction, and distributed system communication patterns.

*Developed by [Sphere 10 Software](https://sphere10.com)*

## 🚀 Key Features

### 🔄 RPC (Remote Procedure Call) Framework
- **Attribute-Based Service Definition**: Use `[RpcAPIService]` and `[RpcAPIMethod]` attributes to expose methods remotely
- **Type-Safe Communication**: Strongly-typed parameters and return values
- **JSON Serialization**: Automatic JSON serialization of complex types
- **Custom Parameter Names**: Use `[RpcAPIArgument]` to specify explicit parameter names for JSON requests

### 🟠 Protocol Abstraction
- **Protocol Agnostic**: Support for multiple transport protocols
- **Network Communication**: Built on top of standard .NET networking
- **Message Serialization**: Flexible serialization for RPC messages

### 💫 Advanced Type Support
- **Complex Objects**: Serialize/deserialize custom objects as RPC parameters
- **Arrays & Collections**: Support for arrays, dictionaries, and generic collections
- **Enums**: Both string and numeric enum serialization
- **Byte Arrays**: Special handling for binary data with hex encoding

## 🚀 Quick Start

### 📦 Installation

Add via NuGet:
```bash
dotnet add package Hydrogen.Communications
```

### 🕣 Defining RPC Services

```csharp
using Hydrogen;
using Hydrogen.Communications.RPC;

// Define a simple RPC service with multiple methods
[RpcAPIService("math")]
public class MathService {
	[RpcAPIMethod]
	public int Add(int a, int b) {
		return a + b;
	}

	[RpcAPIMethod]
	public uint AddUInt(uint a, uint b) {
		return a + b;
	}

	[RpcAPIMethod]
	public float AddFloat(float a, float b) {
		return a + b;
	}

	[RpcAPIMethod]
	public double AddDouble(double a, double b) {
		return a + b;
	}

	[RpcAPIMethod]
	public string ConcatString(string a, string b) {
		return a + b;
	}
}

// Use explicit parameter names in JSON requests
[RpcAPIService("advanced")]
public class AdvancedService {
	[RpcAPIMethod]
	public string AddStrings([RpcAPIArgument("s1")] string str1, [RpcAPIArgument("s2")] string str2) {
		return str1 + str2;
	}

	[RpcAPIMethod]
	public uint Add2Different([RpcAPIArgument("arg1")] uint unsigned, [RpcAPIArgument("arg2")] int signed) {
		return unsigned + (uint)signed;
	}
}
```

