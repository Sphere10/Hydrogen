<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# ⚙️ Hydrogen.Generators

**C# source generators** providing compile-time code generation for boilerplate reduction, plugin scaffolding, and compile-time optimization.

Hydrogen.Generators uses Roslyn analyzers to **generate code at compile time**, eliminating runtime reflection overhead and reducing boilerplate while maintaining type safety and compile-time checking.

## ⚡ 10-Second Example

```csharp
using Hydrogen.Generators;

// Create a class with a GenerateCode attribute
[GenerateCode]
public partial class UserModel {
    public string Name { get; set; }
    public int Age { get; set; }
}

// At compile time, the generator creates:
// - ToString() implementation
// - Equals/GetHashCode for value equality
// - Property validation scaffolding
// - Plugin registration boilerplate

// Use the generated code
var user = new UserModel { Name = "Alice", Age = 30 };
Console.WriteLine(user);  // Auto-generated ToString()
```

## 🏗️ Core Concepts

**Source Generators**: Roslyn-based code generation running at compile time.

**Compile-Time Boilerplate**: Eliminate repetitive code patterns automatically.

**Type-Safe Generation**: Generated code is checked by the C# compiler before runtime.

**Zero Runtime Overhead**: All generation happens during compilation.

**Plugin Scaffolding**: Generate plugin framework boilerplate and registration code.

## 🔧 Core Examples

### Value Type Code Generation

```csharp
using Hydrogen.Generators;

// Mark class for code generation
[GenerateCode]
public partial class Address {
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
}

// Generator creates:
// - ToString(): Returns formatted representation
// - Equals(object): Value equality comparison
// - GetHashCode(): Hash code for collections
// - == operator: Structural equality
// - != operator: Structural inequality

// Usage
var addr1 = new Address { 
    Street = "123 Main St", 
    City = "Boston", 
    State = "MA", 
    ZipCode = "02101" 
};

var addr2 = new Address { 
    Street = "123 Main St", 
    City = "Boston", 
    State = "MA", 
    ZipCode = "02101" 
};

// Value comparison (generated)
if (addr1 == addr2) {
    Console.WriteLine("Same address");  // Prints because values match
}

// Hash code for collections (generated)
var addressSet = new HashSet<Address> { addr1, addr2 };
Console.WriteLine(addressSet.Count);  // 1 (duplicates removed by value)
```

### Plugin Code Generation

```csharp
using Hydrogen.Generators;

// Mark plugin for scaffolding generation
[GeneratePluginCode("MyApp.Plugins")]
public partial class MyCustomPlugin : IPlugin {
    public string Name { get; set; }
    public string Version { get; set; }
    
    public void Initialize() {
        // Plugin initialization
    }
}

// Generator creates:
// - Plugin registration code
// - Dependency injection setup
// - Lifecycle management scaffolding
// - Event handler stubs

// Generated code enables:
var plugins = new PluginManager();
plugins.Register<MyCustomPlugin>();  // Scaffolding handles registration
```

### DTO & Entity Code Generation

```csharp
using Hydrogen.Generators;

// Mark as data transfer object
[GenerateDataTransferCode]
public partial class UserDto {
    public int Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}

// Generator creates:
// - Fluent builder pattern
// - ToEntity() conversion method
// - Clone() for deep copying
// - Serialization support
// - Validation scaffolding

// Usage of generated builder
var user = new UserDto.Builder()
    .WithId(1)
    .WithEmail("alice@example.com")
    .WithFullName("Alice Smith")
    .WithCreatedDate(DateTime.Now)
    .WithIsActive(true)
    .Build();

Console.WriteLine(user);  // Uses generated ToString()
```

### API Controller Scaffolding

```csharp
using Hydrogen.Generators;
using Microsoft.AspNetCore.Mvc;

// Mark for API generation
[GenerateApiController]
public partial class UsersController : ControllerBase {
    // Generator creates:
    // - GET /api/users
    // - POST /api/users
    // - GET /api/users/{id}
    // - PUT /api/users/{id}
    // - DELETE /api/users/{id}
    // - Model binding scaffolding
    // - Error handling boilerplate
    
    private List<UserDto> _users = new();
    
    [HttpGet]
    public IEnumerable<UserDto> GetAll() {
        return _users;
    }
    
    [HttpPost]
    public ActionResult<UserDto> Create([FromBody] CreateUserRequest request) {
        // Implementation
        return Created(nameof(GetById), new UserDto());
    }
}

// Generated scaffolding ensures:
// - Consistent routing
// - Standard error responses
// - Proper status codes
// - Validation attributes
```

### Serialization Code Generation

```csharp
using Hydrogen.Generators;
using System.Text.Json;

// Mark for serialization generation
[GenerateSerializationCode]
public partial class ConfigSettings {
    public string DatabaseConnection { get; set; }
    public int MaxConnections { get; set; }
    public bool EnableLogging { get; set; }
}

// Generator creates custom serializers:
// - ToJson(): Serialize to JSON string
// - FromJson(string): Deserialize from JSON
// - ToXml(): Serialize to XML
// - FromXml(string): Deserialize from XML
// - Custom JsonConverter for System.Text.Json

// Usage
var settings = new ConfigSettings {
    DatabaseConnection = "Server=localhost;Database=mydb",
    MaxConnections = 100,
    EnableLogging = true
};

// Serialization (generated)
string json = settings.ToJson();
Console.WriteLine(json);

// Deserialization (generated)
var restored = ConfigSettings.FromJson(json);
Console.WriteLine($"Connection: {restored.DatabaseConnection}");
```

### Validation Code Generation

```csharp
using Hydrogen.Generators;
using System.ComponentModel.DataAnnotations;

// Mark for validation generation
[GenerateValidationCode]
public partial class UserRegistration {
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be 8-100 characters")]
    public string Password { get; set; }
    
    [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
    public int Age { get; set; }
}

// Generator creates:
// - Validate() method
// - ValidationResult collection method
// - Property-specific validators
// - Custom rule support

// Usage
var registration = new UserRegistration {
    Email = "invalid",
    Password = "short",
    Age = 15
};

var results = registration.Validate();
foreach (var error in results) {
    Console.WriteLine($"Error: {error.Property} - {error.Message}");
}
// Output:
// Error: Email - Invalid email format
// Error: Password - Password must be 8-100 characters
// Error: Age - Age must be between 18 and 120
```

## 🏗️ Architecture & Modules

**Source Generator Base**: Roslyn analyzer framework
- ISourceGenerator implementation
- Compilation context processing
- Syntax tree analysis
- Code output generation

**Code Generation Templates**: Pre-defined generation patterns
- ToString() and equality
- Serialization (JSON, XML)
- Validation rules
- Plugin scaffolding

**Symbol Analysis**: Type and symbol examination
- Attribute detection
- Property/field enumeration
- Inheritance chain analysis
- Interface implementation detection

**Generation Output**: Code formatting and output
- C# syntax formatting
- Indentation and spacing
- Namespace handling
- File structure generation

## 📦 Dependencies

- **Hydrogen**: Core framework
- **Microsoft.CodeAnalysis**: Roslyn compiler API
- **Microsoft.CodeAnalysis.CSharp**: C# language support

## ⚠️ Best Practices

- **Mark types appropriately**: Use correct `[Generate*]` attributes
- **Use partial classes**: Generated code works with partial class definitions
- **Compile often**: Run full rebuild to regenerate code after changes
- **Review generated output**: Check the obj folder for generated files
- **Keep manual and generated separate**: Don't edit generated code files
- **Version compatibility**: Generators require compatible Roslyn versions

## ✅ Status & Compatibility

- **Maturity**: Production-tested, stable for common patterns
- **.NET Target**: .NET 8.0+ (primary), .NET 6.0+ (compatible)
- **Generation Speed**: Zero-cost at runtime; compilation time minimal
- **Debugging**: Generated code is debuggable with proper configuration

## 📖 Related Projects

- [Hydrogen](../Hydrogen) - Core framework
- [Hydrogen.NET](../Hydrogen.NET) - Reflection-based alternatives
- [Hydrogen.Application](../Hydrogen.Application) - Application framework using generated code
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core) - Plugin system using generators

## ⚖️ License

Distributed under the **MIT NON-AI License**.

See the LICENSE file for full details. More information: [Sphere10 NON-AI-MIT License](https://sphere10.com/legal/NON-AI-MIT)

## 👤 Author

**Herman Schoenfeld** - Software Engineer

---

**Version**: 2.0+
