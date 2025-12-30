<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 📡 Hydrogen.Communications

<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

**Multi-protocol networking and RPC framework for .NET 8.0+** providing type-safe remote procedure calls, protocol abstraction, and distributed system communication patterns.

Hydrogen.Communications enables **seamless remote service invocation** through attribute-based RPC definitions with automatic JSON serialization, type conversion, and protocol agnosticism.

## ⚡ 10-Second Example

```csharp
using Hydrogen.Communications.RPC;
using System.Text;

// Define RPC service with attributes
[RpcAPIService("calculator")]
public class CalcService {
    [RpcAPIMethod]
    public int Add(int a, int b) => a + b;
    
    [RpcAPIMethod]
    public string Greet([RpcAPIArgument("name")] string userName) 
        => $"Hello, {userName}!";
}

// Invoke remotely (framework handles serialization)
var result = await rpcClient.InvokeAsync<int>("calculator.Add", new { a = 5, b = 3 });
Console.WriteLine(result);  // 8

var greeting = await rpcClient.InvokeAsync<string>("calculator.Greet", new { name = "World" });
Console.WriteLine(greeting);  // "Hello, World!"
```

## 🏗️ Core Concepts

**RPC Service**: Method marked with `[RpcAPIService]` and `[RpcAPIMethod]` attributes that can be invoked remotely.

**Method Parameters**: Parameters automatically serialized from JSON with support for primitives, objects, collections, and enums.

**Type-Safe Invocation**: Call remote methods as if they were local, with automatic serialization and deserialization.

**Protocol Agnostic**: RPC framework separates protocol implementation from service definition—swap transports without changing service code.

**Custom Naming**: Use `[RpcAPIArgument("name")]` to map C# parameter names to JSON field names for API flexibility.

## 🔧 Core Examples

### Primitive Type Services

```csharp
using Hydrogen.Communications.RPC;

// Define RPC service with multiple primitive type methods
[RpcAPIService("math")]
public class MathService {
    [RpcAPIMethod]
    public int Add(int a, int b) => a + b;
    
    [RpcAPIMethod]
    public uint AddUnsigned(uint a, uint b) => a + b;
    
    [RpcAPIMethod]
    public float Multiply(float a, float b) => a * b;
    
    [RpcAPIMethod]
    public double Divide(double a, double b) => a / b;
    
    [RpcAPIMethod]
    public string Concatenate(string a, string b) => a + b;
    
    [RpcAPIMethod]
    public bool IsGreater(int a, int b) => a > b;
}

// JSON-RPC invocation format:
// {"jsonrpc": "2.0", "method": "math.Add", "params": {"a": 5, "b": 3}, "id": 1}
// Response: {"jsonrpc": "2.0", "result": 8, "id": 1}
```

### Custom Parameter Naming

```csharp
using Hydrogen.Communications.RPC;

// Use [RpcAPIArgument] to map C# names to JSON field names
[RpcAPIService("strings")]
public class StringService {
    [RpcAPIMethod]
    public string Concat(
        [RpcAPIArgument("first")] string text1,
        [RpcAPIArgument("second")] string text2) 
        => text1 + text2;
    
    [RpcAPIMethod]
    public int MixedTypes(
        [RpcAPIArgument("num")] int number,
        [RpcAPIArgument("text")] string str) 
        => number + str.Length;
}

// JSON-RPC invocation:
// {"jsonrpc": "2.0", "method": "strings.Concat", 
//  "params": {"first": "Hello", "second": "World"}, "id": 1}
// Response: {"jsonrpc": "2.0", "result": "HelloWorld", "id": 1}
```

### Complex Objects & Collections

```csharp
using Hydrogen.Communications.RPC;
using System.Collections.Generic;
using System.Text.Json.Serialization;

// Define complex types for RPC parameters
public class Person {
    public string Name { get; set; }
    public int Age { get; set; }
    public string[] Tags { get; set; }
}

[RpcAPIService("people")]
public class PeopleService {
    [RpcAPIMethod]
    public string GetInfo(Person person) 
        => $"{person.Name} is {person.Age}, tags: {string.Join(",", person.Tags)}";
    
    [RpcAPIMethod]
    public int CountTags(Person person) 
        => person.Tags?.Length ?? 0;
    
    [RpcAPIMethod]
    public List<Person> FilterByAge(List<Person> people, int minAge) 
        => people.FindAll(p => p.Age >= minAge);
}

// JSON-RPC invocation:
// {"jsonrpc": "2.0", "method": "people.GetInfo", 
//  "params": {"person": {"name": "Alice", "age": 30, "tags": ["dev", "blockchain"]}}, "id": 1}
```

### Enum Serialization

```csharp
using Hydrogen.Communications.RPC;

public enum Status { Active = 1, Inactive = 2, Pending = 3 }

[RpcAPIService("status")]
public class StatusService {
    [RpcAPIMethod]
    public string GetStatusName(Status status) => status.ToString();
    
    [RpcAPIMethod]
    public bool IsActive(Status status) => status == Status.Active;
    
    [RpcAPIMethod]
    public Status[] GetAllStatuses() => new[] { Status.Active, Status.Inactive, Status.Pending };
}

// JSON-RPC supports both numeric and string enum representation
// {"jsonrpc": "2.0", "method": "status.IsActive", "params": {"status": "Active"}, "id": 1}
// or
// {"jsonrpc": "2.0", "method": "status.IsActive", "params": {"status": 1}, "id": 1}
```

### Byte Array Handling

```csharp
using Hydrogen.Communications.RPC;
using System.Text.Json.Serialization;

public class DataPayload {
    public string Name { get; set; }
    [JsonPropertyName("data_hex")]
    public byte[] Data { get; set; }  // Serialized as hex string in JSON
}

[RpcAPIService("data")]
public class DataService {
    [RpcAPIMethod]
    public int GetLength(DataPayload payload) 
        => payload.Data?.Length ?? 0;
    
    [RpcAPIMethod]
    public string GetDataHash(DataPayload payload) 
        => Tools.Hashing.SHA256Hex(payload.Data);
}

// JSON-RPC invocation with hex-encoded bytes:
// {"jsonrpc": "2.0", "method": "data.GetLength", 
//  "params": {"payload": {"name": "test", "data_hex": "0102030405"}}, "id": 1}
// Response: {"jsonrpc": "2.0", "result": 5, "id": 1}
```

## 🎯 Service Registration & Discovery

Services are automatically discovered via reflection from assemblies containing `[RpcAPIService]` attributes. The framework builds a method registry mapping RPC method names (service.method) to implementations.

**Service Namespace**: `[RpcAPIService("namespace")]` organizes methods under a namespace for API organization.

**Method Overloading**: Not supported in RPC due to JSON-based parameter passing; use different method names or combine parameters into complex types.

## 🔌 Protocol Abstraction

The RPC framework is protocol-agnostic:
- **Transport**: Pluggable transport layer (HTTP, WebSocket, TCP, named pipes)
- **Serialization**: JSON by default; can implement `IMessageSerializer` for alternatives
- **Message Format**: JSON-RPC 2.0 compatible request/response structure

Services defined with attributes work unchanged across different transport implementations.

## 🏗️ Architecture

- **RPC Service Definition**: `[RpcAPIService]`, `[RpcAPIMethod]`, `[RpcAPIArgument]` attributes
- **Service Registry**: Automatic discovery and method registration
- **Parameter Binding**: JSON → C# object conversion with type safety
- **Response Serialization**: C# return values → JSON
- **Protocol Handlers**: Transport-specific implementations (HTTP, WebSocket, etc.)

## ⚠️ Design Considerations

- **Stateless Services**: Design services as stateless when possible for distributed invocation
- **Exception Handling**: Exceptions are serialized and returned as JSON-RPC error responses
- **Parameter Validation**: Validate parameters in service methods; null checks recommended
- **Return Type Support**: Primitive types, objects, collections, enums, byte arrays supported
- **Async Support**: Methods can be async (Task, Task<T>) for non-blocking invocation

## 📖 Related Projects

- [Hydrogen](../Hydrogen) - Core framework
- [Hydrogen.Web.AspNetCore](../Hydrogen.Web.AspNetCore) - ASP.NET Core RPC transport implementation
- [Hydrogen.Application](../Hydrogen.Application) - Application framework with RPC support
- [Hydrogen.Communications](../Hydrogen.Communications) - This library
- [Hydrogen.DApp.Node](../Hydrogen.DApp.Node) - Blockchain node with RPC API

## ✅ Status & Maturity

- **RPC Framework**: Core functionality stable, production-tested
- **.NET Target**: .NET 8.0+ (primary)
- **Thread Safety**: Service methods should be thread-safe if called concurrently
- **Performance**: JSON serialization overhead minimal for typical RPC payload sizes
- **Protocol Variants**: JSON-RPC 2.0 compatible; extending for additional protocols supported

## 📦 Dependencies

- **Hydrogen**: Core framework
- **Newtonsoft.Json**: JSON serialization (handles custom converters for hex, enums)
- **System.Reflection**: Service discovery and method invocation
- **System.Net**: Networking utilities (protocol-specific implementations)

## ⚖️ License

Distributed under the **MIT NON-AI License**.

See the LICENSE file for full details. More information: [Sphere10 NON-AI-MIT License](https://sphere10.com/legal/NON-AI-MIT)

## 👤 Author

**Herman Schoenfeld** - Software Engineer

---

**Version**: 2.0+

