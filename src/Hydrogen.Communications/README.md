# Hydrogen.Communications

Networking and RPC utilities for Hydrogen applications.

## Overview
Provides protocol abstractions and an attribute-based RPC framework for defining strongly-typed services.

## Key features
- Attribute-based RPC services and methods
- JSON serialization for request/response payloads
- Supports complex parameters, collections, and enums
- Protocol abstraction for different transports

## Usage

```csharp
using Hydrogen.Communications.RPC;

[RpcAPIService("math")]
public class MathService {
    [RpcAPIMethod]
    public int Add(int a, int b) => a + b;

    [RpcAPIMethod]
    public string Concat([RpcAPIArgument("left")] string left,
                         [RpcAPIArgument("right")] string right) => left + right;
}
```

## Related projects
- [Hydrogen](../Hydrogen)
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core)
