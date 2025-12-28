# Hydrogen (core)

Hydrogen is the core library of the framework. It provides collections, serialization, streaming, and utility primitives used throughout the solution.

## Focus areas
- Collections: in-memory, stream-backed, and transactional containers
- Serialization: `IItemSerializer<T>`, `SerializerFactory`, polymorphic and reference serializers
- Streams and storage: `ClusteredStreams`, `PagedStream`, and stream-mapped collections
- Utilities: `Result<T>`, `VarInt`, hashing helpers, async and threading utilities

## Examples

### Collections

```csharp
var list = new ExtendedList<string> { "a", "b", "c" };
list.Insert(1, "x");

using var queue = new ProducerConsumerQueue<int>(capacity: 1024);
await queue.PutAsync(42);
queue.CompleteAdding();
```

### Serialization

```csharp
var factory = new SerializerFactory();
factory.Register(typeof(string), new StringSerializer(SizeDescriptorStrategy.UseVarInt));

var serializer = factory.GetSerializer(typeof(ExtendedList<string>));
var bytes = serializer.Serialize(new ExtendedList<string> { "a", "b" });
```

### VarInt

```csharp
using var ms = new MemoryStream();
VarInt.Write(ms, 300);
ms.Position = 0;
var value = VarInt.Read(ms); // 300
```

## Tests as documentation
- `tests/Hydrogen.Tests/Collections/`
- `tests/Hydrogen.Tests/Serialization/`
- `tests/Hydrogen.Tests/ClusteredStreams/`
- `tests/Hydrogen.Tests/Values/VarIntTests.cs`

## Related projects
- [Hydrogen.Data](../Hydrogen.Data)
- [Hydrogen.Communications](../Hydrogen.Communications)
- [Hydrogen.CryptoEx](../Hydrogen.CryptoEx)
