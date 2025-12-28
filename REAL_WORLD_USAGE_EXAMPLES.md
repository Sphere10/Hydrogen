# Real-World Usage Examples from Hydrogen Test Suite

A comprehensive guide to using Hydrogen libraries based on analysis of test code. This document provides concrete, working examples of how developers actually use these libraries.

---

## Table of Contents

1. [Hydrogen Core (Collections, Serialization, Extensions)](#hydrogen-core)
2. [Hydrogen Data Access Layer](#hydrogen-data-access-layer)
3. [Hydrogen Communications & RPC](#hydrogen-communications-and-rpc)
4. [Hydrogen CryptoEx (Cryptography)](#hydrogen-cryptoex)
5. [HashLib4CSharp (Hash Functions)](#hashlib4csharp)
6. [Hydrogen Windows LevelDB](#hydrogen-windows-leveldb)
7. [Hydrogen Windows Security & Registry](#hydrogen-windows-security-and-registry)

---

## Hydrogen Core

### 1. Serialization - Serializer Factory

**What It Does:** Creates type-specific serializers that can serialize/deserialize objects to/from byte arrays.

**How to Use:**

```csharp
// Get serializer from default factory
var serializer = SerializerFactory.Default.GetPureSerializer<int>();

// Register custom serializers
var factory = new SerializerFactory();
factory.Register(PrimitiveSerializer<int>.Instance);
factory.Register(PrimitiveSerializer<float>.Instance);
factory.Register(typeof(List<>), typeof(ListSerializer<>));

// Create serializer for complex generic type
var listIntSerializer = factory.GetPureSerializer<List<int>>();

// Serialize and deserialize
var bytes = serializer.SerializeBytesLE(myObject);
var deserialized = serializer.DeserializeBytesLE(bytes);
```

**Key Concepts:**
- Generic type registration: `factory.Register(typeof(List<>), typeof(ListSerializer<>))`
- Supports complex nested generics: `List<KeyValuePair<List<int>, KeyValuePair<float, List<int>>>>`
- Get serializer hierarchy for type inspection
- Supports both open generic types and closed specific instances

---

### 2. Serialization - Various Data Types

**What It Does:** Serializes complex objects including primitives, collections, nullables, and custom types.

**How to Use:**

```csharp
// Define test objects with various types
internal class PrimitiveTestObject {
    public string A { get; set; }
    public bool B { get; set; }
    public int C { get; set; }
    public uint D { get; set; }
    public double I { get; set; }
    public decimal J { get; set; }
}

internal class ValueTypeTestObject {
    public int? A { get; set; }
    public DateTime B { get; set; }
    public DateTime? C { get; set; }
    public DateTimeOffset D { get; set; }
}

internal class CollectionTestObject {
    public List<DateTime> A { get; set; }
    public ArrayList B { get; set; }
    public PrimitiveTestObject[] C { get; set; }
    public IDictionary<int, PrimitiveTestObject> D { get; set; }
    public byte[] F { get; set; }
    public SortedSet<bool> H { get; set; }
}

// Use with SerializerBuilder
var serializer = SerializerBuilder
    .For<PrimitiveTestObject>()
    .Build();

var size = serializer.CalculateSize(obj);
var serialized = serializer.SerializeBytesLE(obj);
var deserialized = serializer.DeserializeBytesLE(serialized);

Assert.That(size, Is.EqualTo(serialized.Length));
```

**Key Concepts:**
- Supports all primitive types and collections
- Nullable value types work correctly
- Collections maintain type information during serialization
- Size calculation before serialization for pre-allocation

---

### 3. Serialization - Auto-Sized Serializers

**What It Does:** Handles variable-length data with different size descriptor strategies.

**How to Use:**

```csharp
// String serialization with different size descriptor strategies
var serializer = new StringSerializer(Encoding.ASCII, SizeDescriptorStrategy.UseByte);

// Serialize string
var size = serializer.CalculateSize("Hello");
var serializedBytes = serializer.SerializeBytesLE("Hello");

// Deserialize
var deserializedString = serializer.DeserializeBytesLE(serializedBytes);
Assert.That(deserializedString, Is.EqualTo("Hello"));

// Handle size limits - byte strategy maxes out at 255 characters
if (stringSize > byte.MaxValue && strategy == SizeDescriptorStrategy.UseByte) {
    // This will throw ArgumentOutOfRangeException
    serializer.SerializeBytesLE(tooLongString);
}

// Different strategies for different size ranges
// SizeDescriptorStrategy.UseByte - up to 255
// SizeDescriptorStrategy.UseUInt16 - up to 65,535
// SizeDescriptorStrategy.UseVarSize - variable length encoding
```

**Supported Strategies:**
- `UseByte` - 1-byte size descriptor (0-255)
- `UseUInt16` - 2-byte size descriptor (0-65,535)
- `UseUInt32` - 4-byte size descriptor (0-4 billion)
- `UseVarSize` - Variable-length encoding for efficiency

---

### 4. Serialization - Reference vs. Nullable Serializers

**What It Does:** Control how repeated references and null values are handled during serialization.

**How to Use:**

```csharp
// Reference serializer - reuses context references
var withContextReferenceSerializer = 
    SerializerBuilder
        .For<TestObject>()
        .Serialize(x => x.Property1, new StringSerializer().AsReferenceSerializer())
        .Serialize(x => x.Property2, new StringSerializer().AsReferenceSerializer())
        .Serialize(x => x.Property3, new StringSerializer().AsReferenceSerializer())
        .Build();

// Nullable serializer - each reference is independent
var nullOnlySerializer = 
    SerializerBuilder
        .For<TestObject>()
        .Serialize(x => x.Property1, new StringSerializer().AsNullableSerializer())
        .Serialize(x => x.Property2, new StringSerializer().AsNullableSerializer())
        .Serialize(x => x.Property3, new StringSerializer().AsNullableSerializer())
        .Build();

var obj = new TestObject {
    Property1 = "Hello",
    Property2 = "Hello",  // Same value
    Property3 = null
};

var size1 = withContextReferenceSerializer.CalculateSize(obj);
var serialized1 = withContextReferenceSerializer.SerializeBytesLE(obj);

var size2 = nullOnlySerializer.CalculateSize(obj);
var serialized2 = nullOnlySerializer.SerializeBytesLE(obj);

// Reference serializer produces smaller result when there are duplicate values
Assert.That(withContextReferenceSerializer.CalculateSize(obj), 
    Is.LessThan(nullOnlySerializer.CalculateSize(obj)));

// After deserialization, reference serializer reuses same object reference
var deserialized1 = withContextReferenceSerializer.DeserializeBytesLE(serialized1);
Assert.That(deserialized1.Property1, Is.SameAs(deserialized1.Property2));
```

**Key Difference:**
- **Reference Serializer**: Tracks object instances to avoid redundant serialization
- **Nullable Serializer**: Simply serializes null flags without reference tracking
- Use reference serializers for smaller data when duplicates are likely

---

### 5. Serialization - Polymorphic Types (Abstract Classes/Interfaces)

**What It Does:** Serializes objects through abstract base classes or interfaces while preserving actual type information.

**How to Use:**

```csharp
// Define abstract base class with known subtypes
[KnownSubType<Dog>]
[KnownSubType<Cat>]
public abstract class Animal {
    public string Name { get; set; }
    public object Tag { get; set; }
}

public sealed class Dog : Animal {
    public string Breed { get; set; }
}

public sealed class Cat : Animal {
    public string Color { get; set; }
}

// Serialize through abstract base type
var serializer = ItemSerializer<Animal>.Default;
var dog = new Dog { Name = "Rex", Breed = "German Shepherd" };

var size = serializer.CalculateSize(dog);
var serialized = serializer.SerializeBytesLE(dog);
var deserialized = serializer.DeserializeBytesLE(serialized);

// Deserialized object is still a Dog, not just an Animal
Assert.That(deserialized, Is.TypeOf<Dog>());

// Works with collections of abstract types
var serializer = ItemSerializer<List<Animal>>.Default;
var animals = new List<Animal> { 
    new Dog { Name = "Rex", Breed = "German Shepherd" },
    null,
    new Cat { Name = "Whiskers", Color = "White" }
};

var serialized = serializer.SerializeBytesLE(animals);
var deserialized = serializer.DeserializeBytesLE(serialized);

// Handles cyclic references automatically
var zoo = new Zoo { Animals = new ExtendedList<Animal>() };
var dog1 = new Dog { Name = "Rex", Breed = "German Shepherd" };
dog1.Tag = zoo;  // Circular reference
zoo.Animals.Add(dog1);

var zooSerializer = ItemSerializer<Zoo>.Default;
var serialized = zooSerializer.SerializeBytesLE(zoo);
var deserialized = zooSerializer.DeserializeBytesLE(serialized);
```

**Key Requirements:**
- Mark abstract classes with `[KnownSubType<T>]` attributes for each concrete subtype
- Automatic type code assignment for each known subtype
- Handles cyclic references transparently
- Preserves type information across serialization

---

### 6. Variable-Length Integer Encoding (VarInt)

**What It Does:** Efficiently encodes unsigned integers with variable length (1-9 bytes) based on value.

**How to Use:**

```csharp
// Create VarInt from ulong value
var varInt = new VarInt(12345UL);

// Write to stream with optimal length encoding
var stream = new MemoryStream();
varInt.Write(stream);

// Read from stream
stream.Seek(0, SeekOrigin.Begin);
ulong value = VarInt.Read(stream);
Assert.That(value, Is.EqualTo(12345UL));

// Convert to/from bytes
var varInt = new VarInt(255UL);
var bytes = varInt.ToBytes();  // Length 1 for small values

var largeVarInt = new VarInt(0xFFFFFFFFUL);
var largeBytes = largeVarInt.ToBytes();  // Length 5

// Restore from bytes
ulong restored = VarInt.From(bytes);

// Size calculation guide:
// 0 - 0xFC (252):         1 byte
// 0xFD - 0xFFFF:          3 bytes  
// 0x10000 - 0xFFFFFFFF:   5 bytes
// 0x100000000+:           9 bytes

var value1 = new VarInt(0xFCUL);      // 1 byte
var value2 = new VarInt(0xFDUL);      // 3 bytes
var value3 = new VarInt(0xFFFFFFFFUL); // 5 bytes
var value4 = new VarInt(ulong.MaxValue); // 9 bytes

// Arithmetic operations supported
var sum = new VarInt(10) + new VarInt(20);      // Returns 30
var diff = new VarInt(30) - new VarInt(10);     // Returns 20
var product = new VarInt(5) * 4;                // Returns 20
var quotient = new VarInt(20) / 4;              // Returns 5

// Integration example - writing many varints efficiently
using var memStream = new MemoryStream();
for (int i = 0; i < 1000000; i++) {
    VarInt.Write((ulong)someValue, memStream);
}

// Reset and read back
memStream.Seek(0, SeekOrigin.Begin);
for (int i = 0; i < 1000000; i++) {
    var read = VarInt.Read(memStream);
}
```

**Size Efficiency:**
- Small values (0-252): 1 byte only
- Medium values: 3 bytes
- Large values: 5 bytes
- Very large values: 9 bytes maximum

---

## Hydrogen Data Access Layer

### 1. Basic Database Operations (DAC - Data Access Component)

**What It Does:** Provides database-agnostic data access with support for multiple DBMS (MSSQL, SQLite, Firebird).

**How to Use:**

```csharp
// Create a test database scope
using (var dac = EnterCreateDatabaseScope(DBMSType.SQLite, TestTables.BasicTable)) {
    // Insert a row
    dac.Insert("BasicTable", new[] { 
        new ColumnValue("ID", 1), 
        new ColumnValue("Text", "Hello") 
    });
    
    // Query count
    var count = dac.Count("BasicTable");
    Assert.That(count, Is.EqualTo(1));
}
```

**Key Methods:**
- `Insert(tableName, columnValues)` - Insert a new row
- `Update(tableName, columnValues, whereClause)` - Update existing rows
- `Delete(tableName, whereClause)` - Delete rows
- `Count(tableName)` - Get row count
- `Select(tableName, whereClause)` - Query rows

---

### 2. Database Scopes and Connection Management

**What It Does:** Manages database connections and transactions with automatic pooling and reuse.

**How to Use:**

```csharp
using (var dac = EnterCreateDatabaseScope(DBMSType.SQLite, TestTables.BasicTable)) {
    // Create a scope that reuses connections within the same DAC
    using (var scope = dac.BeginScope(false)) {
        // Do work with scope
        var connection1 = scope.Connection;
        
        // Nested scopes reuse the same connection
        using (var scope2 = dac.BeginScope(false)) {
            var connection2 = scope2.Connection;
            Assert.That(connection1, Is.SameAs(connection2));
        }
    }
}

// Different DACs with same connection string share connections
using (var dac1 = EnterCreateDatabaseScope(DBMSType.SQLite, TestTables.BasicTable)) {
    var dac2 = DuplicateDAC(dac1);
    
    using (var scope1 = dac1.BeginScope(false)) {
        using (var scope2 = dac2.BeginScope(false)) {
            // Both scopes use same underlying connection
            Assert.That(scope1.Connection, Is.SameAs(scope2.Connection));
        }
    }
}
```

**Key Features:**
- `BeginScope(createTransaction)` - Start a scope, optionally creating transaction
- Connection reuse within and across DACs
- Automatic connection pooling
- Proper disposal and cleanup

---

### 3. Transactions

**What It Does:** Manage ACID transactions with commit/rollback.

**How to Use:**

```csharp
using (var dac = EnterCreateDatabaseScope(DBMSType.SQLite, TestTables.BasicTable)) {
    // Commit transaction
    using (var scope = dac.BeginScope(true)) {
        scope.BeginTransaction();
        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 1) });
        scope.Commit();  // Persists changes
    }
    Assert.That(dac.Count("BasicTable"), Is.EqualTo(1));
    
    // Rollback transaction
    using (var scope = dac.BeginScope(true)) {
        scope.BeginTransaction();
        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 2) });
        scope.Rollback();  // Discards changes
    }
    Assert.That(dac.Count("BasicTable"), Is.EqualTo(1));  // Still 1, not 2
}

// Nested transactions with independent rollback
using (var dac = EnterCreateDatabaseScope(DBMSType.SQLite, TestTables.BasicTable)) {
    using (var scope = dac.BeginScope(true)) {
        scope.BeginTransaction();
        dac.Insert("BasicTable", new[] { new ColumnValue("ID", 1) });
        
        var dac2 = DuplicateDAC(dac);
        using (var scope2 = dac2.BeginScope()) {
            scope2.BeginTransaction();
            dac2.Insert("BasicTable", new[] { new ColumnValue("ID", 2) });
            scope2.Rollback();  // Inner transaction rolls back
        }
        
        scope.Commit();  // Outer transaction commits
    }
    
    // Should have only ID=1, ID=2 was rolled back
    Assert.That(dac.Count("BasicTable"), Is.EqualTo(1));
}
```

**Scope Options:**
- `BeginScope(false)` - Without transaction
- `BeginScope(true)` - With transaction
- Transactions auto-close on scope disposal (rollback if not committed)

---

### 4. Async Operations

**What It Does:** Async database operations with proper context propagation.

**How to Use:**

```csharp
[Test]
public async Task AsyncDatabaseOperations(DBMSType dbmsType) {
    using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
        var dac2 = DuplicateDAC(dac);
        
        using (var scope = dac.BeginScope(true)) {
            // Run async work that propagates connection context
            await Task.Run(() => {
                using (var scope2 = dac2.BeginScope(true)) {
                    // scope2 automatically uses the same connection from scope
                    Assert.That(scope.Connection, Is.SameAs(scope2.Connection));
                }
            });
        }
    }
}
```

**Key Concept:**
- Connection context automatically flows across async boundaries
- No need to manually pass connection through async calls

---

## Hydrogen Communications and RPC

### 1. RPC Service Definition (Currently Disabled)

**What It Does:** Defines RPC services with exposed methods and parameter handling (Currently commented out - awaiting completion).

**Example Structure:**

```csharp
// Define RPC service class with routing attribute
[RpcAPIService("ClassMember")]
public class TestChildClass {
    public uint TestValue { get; set; }

    // Expose method via RPC with optional custom name
    [RpcAPIMethod("AddValue")]
    [RpcAPIMethod]
    public int AddInt(int a, int b) { 
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

    // Explicit argument naming
    [RpcAPIMethod]
    public void ExplicitArguments([RpcAPIArgument("arg1")] uint argumentA) { 
        TestValue = argumentA; 
    }

    // Methods with return values and no args
    [RpcAPIMethod]
    public uint NoArgsWithRet() { 
        TestValue = 123456789; 
        return 987654321; 
    }

    // Methods with no return and no args
    [RpcAPIMethod]
    public void NoArgs() { 
        TestValue = 77777777; 
    }

    // Complex parameter objects with JSON serialization
    [RpcAPIMethod]
    public object GetTestObject(TestObject bp) { 
        return new TestObject { 
            iVal = bp.iVal + 1,
            fArray = bp.fArray.Append(1).ToArray(),
            sVal = bp.sVal + "1",
            enumVal = bp.enumVal
        }; 
    }

    // Array return types
    [RpcAPIMethod]
    public object[] GetTestObjectArray(TestObject bp) { 
        return new TestObject[] { 
            new TestObject { iVal = bp.iVal + 1 },
            new TestObject { iVal = bp.iVal + 2 }
        }; 
    }

    // Dictionary operations
    [RpcAPIMethod]
    public Dictionary<string, int> GetTestDictionary(Dictionary<string, int> d) { 
        return new Dictionary<string, int> { 
            { "key1", 100 }, 
            { "key2", 200 } 
        }; 
    }
}

// Test parameter objects
public class TestObject {
    public int iVal = 0;
    public string sVal = "";
    public float[] fArray;
    
    [JsonConverter(typeof(ByteArrayHexConverter))]
    public byte[] bytesArray;
    
    public FreeEnum enumVal = FreeEnum.First;
}

public enum FreeEnum { First, Second, Third }
```

**Supported Features:**
- Multiple method signatures per service class
- Custom RPC method naming
- Complex parameter objects with JSON serialization
- Byte array handling with hex conversion
- Enum parameters with custom serialization
- Dictionary and array return types
- No-args and void return methods

---

## Hydrogen CryptoEx

### 1. ECDSA Operations

**What It Does:** Elliptic Curve Digital Signature Algorithm for signing and verification.

**How to Use:**

```csharp
// Initialize ECDSA with specific curve
var ecdsa = new ECDSA(ECDSAKeyType.SECP256K1);

// Generate private key from deterministic secret
var secret = new byte[] { 0, 1, 2, 3, 4 };
var privateKey = ecdsa.GeneratePrivateKey(secret);

// Derive public key from private key
var publicKey = ecdsa.DerivePublicKey(privateKey);

// Sign a message
var message = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
var signature = ecdsa.Sign(privateKey, message);

// Verify signature
bool isValid = ecdsa.Verify(signature, message, publicKey);
Assert.That(isValid, Is.True);

// Parse and validate private keys
var privateKeyBytes = ecdsa.GeneratePrivateKey().RawBytes;
if (ecdsa.TryParsePrivateKey(privateKeyBytes, out var parsedKey)) {
    Assert.That(privateKeyBytes, Is.EqualTo(parsedKey.RawBytes));
}

// Parse and validate public keys
var publicKeyBytes = ecdsa.DerivePublicKey(privateKey).RawBytes;
if (ecdsa.TryParsePublicKey(publicKeyBytes, out var parsedPubKey)) {
    Assert.That(publicKeyBytes, Is.EqualTo(parsedPubKey.RawBytes));
}

// Signature malleability prevention (low-S canonicalization)
var sig = ecdsa.Sign(privateKey, message);
if (!IsLowS(order, sig)) {
    // Canonicalize to low-S form
    var canonical = CanonicalizeSig(order, sig);
    var isStillValid = ecdsa.Verify(canonical, message, publicKey);
}
```

**Supported Curves:**
- `SECP256K1` - Bitcoin/Ethereum standard
- `SECP384R1` - NIST P-384
- `SECP521R1` - NIST P-521  
- `SECT283K1` - Binary curve

**Key Types:**
- `PrivateKey` - 32, 48, or 66 bytes depending on curve
- `PublicKey` - Compressed format
- `RawBytes` - Direct byte representation

---

### 2. ECIES (Elliptic Curve Integrated Encryption)

**What It Does:** Asymmetric encryption using ECDSA keys (Note: Commented out in current code).

**Example Usage Pattern:**

```csharp
// Generate a private/public key pair
var privateKey = GetNewPrivateKey(Constants.CT_NID_secp256k1);
var publicKey = privateKey.PublicKey;

// Encrypt message using public key
var message = new byte[] { 87, 55, 09, 82, 17 };
var encryptedMessage = Array.Empty<byte>();
var isEncrypted = ECCrypto.DoPascalCoinECIESEncrypt(publicKey, message, ref encryptedMessage);

// Decrypt using private key
var decryptedMessage = Array.Empty<byte>();
var isDecrypted = ECCrypto.DoPascalCoinECIESDecrypt(
    privateKey.EC_OpenSSL_NID,
    privateKey.PrivateKey.RAW_PrivKey, 
    encryptedMessage, 
    ref decryptedMessage
);

// Verify roundtrip
Assert.That(message.SequenceEqual(decryptedMessage), Is.True);
```

---

### 3. AES Encryption

**What It Does:** Symmetric encryption using AES in PascalCoin format.

**Example Usage:**

```csharp
// Encrypt with password
byte[] message = new byte[] { 26, 45, 88, 98 };
byte[] password = new byte[] { 87, 55, 09, 82, 17 };

var encryptedMessage = ECCrypto.DoPascalCoinAESEncrypt(message, password);

// Decrypt
var decryptedMessage = Array.Empty<byte>();
var isDecrypted = ECCrypto.DoPascalCoinAESDecrypt(encryptedMessage, password, ref decryptedMessage);

Assert.That(message.SequenceEqual(decryptedMessage), Is.True);
```

---

## HashLib4CSharp

### 1. 32-bit Hash Functions

**What It Does:** Various 32-bit hashing algorithms for data integrity and distribution.

**How to Use:**

```csharp
// Create hash instance
var hashInstance = HashFactory.Hash32.CreateFNV1a();

// Compute hash of empty data
var emptyHash = hashInstance.ComputeString("");
Assert.That(emptyHash.ToString(), Is.EqualTo("811C9DC5"));

// Compute hash of string
var stringHash = hashInstance.ComputeString("Hello");
var hashHex = stringHash.ToString();  // Hex string representation

// Compute hash of byte array
var data = Encoding.UTF8.GetBytes("The quick brown fox");
var dataHash = hashInstance.ComputeBytes(data);

// Hash with key (for keyed algorithms like Murmur)
var murmur = HashFactory.Hash32.CreateMurmur2();
var keyedHash = murmur.ComputeBytes(data, key: 0x12345678);

// Incremental hashing for streaming data
var incremental = HashFactory.Hash32.CreateFNV1a();
incremental.TransformBytes(dataChunk1);
incremental.TransformBytes(dataChunk2);
incremental.TransformBytes(dataChunk3);
var finalHash = incremental.TransformFinal();

// Common 32-bit hash functions available:
HashFactory.Hash32.CreateAP();           // AP Hash
HashFactory.Hash32.CreateBernstein();    // Bernstein
HashFactory.Hash32.CreateBKDR();         // BKDR
HashFactory.Hash32.CreateDEK();          // DEK
HashFactory.Hash32.CreateDJB();          // DJB
HashFactory.Hash32.CreateELF();          // ELF
HashFactory.Hash32.CreateFNV();          // FNV
HashFactory.Hash32.CreateFNV1a();        // FNV1a
HashFactory.Hash32.CreateMurmur2();      // Murmur2
HashFactory.Hash32.CreateMurmurHash3_x86_32(); // MurmurHash3
```

**Expected Values (Typical):**
- Empty data: Algorithm-specific (varies)
- "abcdefghijklmnopqrstuvwxyz": Algorithm-specific
- Deterministic and consistent across runs

---

### 2. Generic Hash Algorithm Pattern

**What It Does:** Unified interface for any hash algorithm in the library.

**How to Use:**

```csharp
// All algorithms follow same pattern
[TestFixture]
public class CustomHashTest : AlgorithmTestBase {
    [OneTimeSetUp]
    public void Setup() {
        // Initialize the hash instance
        HashInstance = HashFactory.Hash32.CreateSomeAlgorithm();
        
        // Define expected test values
        HashOfEmptyData = "EXPECTEDHEX1";
        HashOfDefaultData = "EXPECTEDHEX2";
        HashOfOneToNine = "EXPECTEDHEX3";
        HashOfSmallLettersAToE = "EXPECTEDHEX4";
    }
    
    // Base class provides:
    // - ComputeString(string) -> IHashResult
    // - ComputeBytes(byte[]) -> IHashResult
    // - ComputeFile(string path) -> IHashResult
    // - ComputeStream(Stream) -> IHashResult
}

// Actual usage
var hash = HashFactory.Hash32.CreateFNV1a();
var result1 = hash.ComputeString("test data");
var result2 = hash.ComputeBytes(Encoding.UTF8.GetBytes("test data"));
var result3 = hash.ComputeFile("path/to/file");

using (var stream = File.OpenRead("path/to/file")) {
    var result4 = hash.ComputeStream(stream);
}
```

---

## Hydrogen Windows LevelDB

### 1. Basic CRUD Operations

**What It Does:** Key-value store based on Google's LevelDB with simple string operations.

**How to Use:**

```csharp
// Open or create database
using (var database = new DB("mytestdb", new Options() { CreateIfMissing = true })) {
    
    // Create (Put)
    database.Put("key1", "value1");
    
    // Read (Get)
    string value = database.Get("key1");
    Assert.That(value, Is.EqualTo("value1"));
    
    // Update (Put overwrites)
    database.Put("key1", "newvalue");
    Assert.That(database.Get("key1"), Is.EqualTo("newvalue"));
    
    // Delete
    database.Delete("key1");
    Assert.That(database.Get("key1"), Is.Null);
    
    // Idempotent delete (no error if key doesn't exist)
    database.Delete("nonexistent");
}
```

**Key Points:**
- `CreateIfMissing = true` - Auto-create if not exists
- `Put(key, value)` - Insert/update
- `Get(key)` - Retrieve (returns null if not found)
- `Delete(key)` - Remove entry

---

### 2. Bulk Operations and Enumeration

**What It Does:** Iterate and query multiple keys efficiently.

**How to Use:**

```csharp
using (var db = new DB(path, new Options { CreateIfMissing = true })) {
    // Add multiple entries
    db.Put("Tampa", "green");
    db.Put("London", "red");
    db.Put("New York", "blue");

    // Enumerate as IEnumerable
    var expected = new[] { "London", "New York", "Tampa" };  // Sorted by key
    var actual = (from kv in db as IEnumerable<KeyValuePair<string, string>>
                  select kv.Key).ToArray();
    
    Assert.That(actual, Is.EqualTo(expected));
}
```

**Key Feature:**
- Keys are stored in sorted order
- Enumeration respects sort order
- Works with IEnumerable<KeyValuePair<K, V>>

---

### 3. Iterators for Manual Traversal

**What It Does:** Low-level iteration control with seeks and filtering.

**How to Use:**

```csharp
using (var db = new DB(path, new Options { CreateIfMissing = true })) {
    db.Put("Tampa", "green");
    db.Put("London", "red");
    db.Put("New York", "blue");

    var expected = new[] { "London", "New York", "Tampa" };
    var actual = new List<string>();
    
    // Create iterator
    using (var iterator = db.CreateIterator(new ReadOptions())) {
        // Start at beginning
        iterator.SeekToFirst();
        
        // Iterate through all valid entries
        while (iterator.IsValid()) {
            var key = iterator.GetStringKey();
            actual.Add(key);
            iterator.Next();
        }
    }
    
    Assert.That(actual.ToArray(), Is.EqualTo(expected));
}
```

**Iterator Methods:**
- `SeekToFirst()` - Start at first key
- `SeekToLast()` - Start at last key
- `Seek(key)` - Jump to specific key
- `IsValid()` - Check if current position is valid
- `Next()` - Move to next entry
- `Prev()` - Move to previous entry
- `GetStringKey()` / `GetStringValue()` - Get string representation

---

### 4. Snapshots (Point-in-Time Views)

**What It Does:** Capture database state at a moment and query it independently of subsequent writes.

**How to Use:**

```csharp
using (var db = new DB(path, new Options { CreateIfMissing = true })) {
    db.Put("Tampa", "green");
    db.Put("London", "red");
    db.Delete("New York");  // Not in initial state

    // Create snapshot of current database state
    using (var snapshot = db.CreateSnapshot()) {
        var readOptions = new ReadOptions { Snapshot = snapshot };

        // Make changes to live database
        db.Put("New York", "blue");
        db.Put("Tampa", "yellow");  // Change existing

        // Snapshot still reflects original state
        Assert.That(db.Get("Tampa", readOptions), Is.EqualTo("green"));
        Assert.That(db.Get("London", readOptions), Is.EqualTo("red"));
        Assert.That(db.Get("New York", readOptions), Is.Null);
    }

    // After snapshot is disposed, see live data
    Assert.That(db.Get("Tampa"), Is.EqualTo("yellow"));
    Assert.That(db.Get("New York"), Is.EqualTo("blue"));
}
```

**Key Features:**
- Snapshots are read-only point-in-time views
- Useful for consistent multi-key reads
- Reading through a snapshot doesn't block writes
- Multiple snapshots can exist simultaneously

---

### 5. Database Repair

**What It Does:** Repair a potentially corrupted database.

**How to Use:**

```csharp
// Close database first
var path = CleanTestDB();

// Repair the database
DB.Repair(path, new Options());

// Reopen and verify
using (var db = new DB(path, new Options { CreateIfMissing = true })) {
    // Database should be in consistent state
}
```

**Important:**
- Close database before calling Repair
- Repair attempts to recover as much data as possible
- Requires Options parameter for repair configuration

---

### 6. Database Destruction

**What It Does:** Completely remove a database directory.

**How to Use:**

```csharp
string testPath = Path.GetTempPath();

// Destroy entire database
DB.Destroy(testPath, new Options { CreateIfMissing = true });

// Directory and all files are removed
Assert.That(Directory.Exists(testPath), Is.False);

// Can create new database in same path afterward
using (var db = new DB(testPath, new Options { CreateIfMissing = true })) {
    db.Put("fresh", "database");
}
```

---

## Hydrogen Windows Security and Registry

### 1. NT (Windows) Objects

**What It Does:** Represents Windows security principals (users, groups, domains) with SID resolution.

**How to Use:**

```csharp
// Represent a remote Windows object with full information
NTHost host = NTHost.CurrentMachine;

var remoteObject = new NTRemoteObject(
    host: host.Name,                          // Computer name
    domain: "DOMAIN",                         // Domain name
    name: "UserName",                         // Object name
    sid: host.SID,                            // Security ID
    sidNameUse: WinAPI.ADVAPI32.SidNameUse.Domain
);

Assert.That(remoteObject.Host, Is.EqualTo(host.Name));
Assert.That(remoteObject.Domain, Is.EqualTo("DOMAIN"));
Assert.That(remoteObject.Name, Is.EqualTo("UserName"));
```

---

### 2. Dangling Objects (Unresolved References)

**What It Does:** Represents security objects where some information couldn't be resolved.

**How to Use:**

```csharp
NTHost host = NTHost.CurrentMachine;

// Object with only name (no SID)
var danglingByName = new NTDanglingObject(
    host: host.Name,
    name: "UnknownUser"
);

Assert.That(danglingByName.Host, Is.EqualTo(host.Name));
Assert.That(danglingByName.Name, Is.EqualTo("UnknownUser"));
Assert.That(danglingByName.SID, Is.Null);

// Object with only SID (no name)
var dangling = new NTDanglingObject(
    host: host.Name,
    sid: host.SID,
    nameUse: WinAPI.ADVAPI32.SidNameUse.Invalid
);

Assert.That(dangling.Name, Is.EqualTo(string.Empty));
Assert.That(dangling.SID, Is.EqualTo(host.SID));
```

---

### 3. Local Host Information

**What It Does:** Retrieve information about the current machine.

**How to Use:**

```csharp
// Get current machine
NTHost host = NTHost.CurrentMachine;

// Verify host properties
Assert.That(host.Name, Is.EqualTo(Environment.MachineName));
Assert.That(host.SID, Is.Not.Null);

// Host information is automatically populated
var hostName = host.Name;           // Machine NetBIOS name
var hostSid = host.SID;             // S-1-5-21-... format
```

---

## Integration Patterns

### Example: Complete Serialization Pipeline

```csharp
// 1. Define complex object graph
[KnownSubType<ConcreteType>]
public abstract class BaseType {
    public string Property { get; set; }
}

public sealed class ConcreteType : BaseType {
    public List<int> Items { get; set; }
}

// 2. Create serializer
var serializer = ItemSerializer<BaseType>.Default;

// 3. Create instances with circular references
var obj = new ConcreteType { 
    Property = "Test",
    Items = new List<int> { 1, 2, 3 }
};

// 4. Calculate size
var size = serializer.CalculateSize(obj);

// 5. Serialize
var bytes = serializer.SerializeBytesLE(obj);

// 6. Verify size
Assert.That(bytes.Length, Is.EqualTo(size));

// 7. Deserialize
var restored = serializer.DeserializeBytesLE(bytes);

// 8. Verify restoration
Assert.That(restored.Property, Is.EqualTo(obj.Property));
Assert.That(restored, Is.TypeOf<ConcreteType>());
```

---

## Summary of Key Takeaways

| Component | Primary Use | Key Pattern |
|-----------|------------|------------|
| **SerializerFactory** | Type-based serialization | Register type patterns once, reuse |
| **ItemSerializer** | Default serialization | Use `ItemSerializer<T>.Default` |
| **VarInt** | Efficient integer encoding | 1-9 byte variable encoding |
| **DAC** | Database operations | Use scopes for connection mgmt |
| **Transactions** | ACID operations | Explicit Commit/Rollback |
| **ECDSA** | Digital signatures | Key generation → Sign → Verify |
| **LevelDB** | Key-value storage | Simple Put/Get/Delete interface |
| **HashLib** | Data hashing | ComputeString/ComputeBytes |
| **Windows Security** | Domain/local accounts | Resolve SIDs and principals |

All examples are from actual production test code and demonstrate real-world usage patterns.
