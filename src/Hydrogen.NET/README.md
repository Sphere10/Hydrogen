<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 🔧 Hydrogen.NET

**CLR and .NET Framework runtime utilities** providing type introspection, reflection helpers, dynamic code generation, and cross-framework compatibility abstractions.

Hydrogen.NET enables **advanced metaprogramming** through reflection APIs, **runtime type discovery**, **dynamic code generation**, and **framework detection** for building robust .NET applications that work across Framework and modern .NET versions.

## ⚡ 10-Second Example

```csharp
using Hydrogen;
using Hydrogen.CodeBuilder;

// Generate C# code at runtime
var classBuilder = new ClassBuilder {
    Name = "Generated_User",
    Namespace = "MyApp.Models",
    BaseTypeName = "BaseDomainModel"
};

classBuilder.AddField(ProtectionLevel.Private, "string", "name");
classBuilder.AddProperty(ProtectionLevel.Public, "string", "Name", true, true);

// Generate source code as string
string generatedCode = classBuilder.ToString();
Console.WriteLine(generatedCode);  // Full C# class definition
```

## 🏗️ Core Concepts

**ClassBuilder**: Fluent API for generating C# classes at runtime with properties, fields, methods, and inheritance.

**Reflection Extensions**: Enhanced reflection utilities for discovering types, methods, properties with filtering and caching.

**Type Discovery**: Find all types implementing an interface or decorated with a specific attribute.

**Process Sentry**: Runtime process environment monitoring, host detection, and environment introspection.

**Math Runtime**: Advanced mathematical expression evaluation and computation.

## 🔧 Core Examples

### Code Generation with ClassBuilder

```csharp
using Hydrogen.CodeBuilder;

// Create a class builder
var classBuilder = new ClassBuilder {
    Name = "Product",
    Namespace = "MyStore",
    ProtectionLevel = ProtectionLevel.Public
};

// Add fields
classBuilder.AddField(
    ProtectionLevel.Private,
    "string",
    "_productName");

// Add properties with getter/setter
classBuilder.AddProperty(
    ProtectionLevel.Public,
    "string",
    "ProductName",
    true,  // has getter
    true   // has setter
);

classBuilder.AddProperty(
    ProtectionLevel.Public,
    "decimal",
    "Price",
    true,
    true);

// Generate complete C# source code
string sourceCode = classBuilder.ToString();

Console.WriteLine("Generated Class:");
Console.WriteLine(sourceCode);
/* Output:
public class Product {
    private string _productName;
    
    public string ProductName {
        get { return _productName; }
        set { _productName = value; }
    }
    
    public decimal Price { get; set; }
}
*/
```

### Type Discovery & Reflection

```csharp
using Hydrogen;
using System.Reflection;

// Find all types in loaded assemblies that implement an interface
var handlerTypes = typeof(IEventHandler)
    .Assembly
    .GetTypes()
    .Where(t => typeof(IEventHandler).IsAssignableFrom(t))
    .ToList();

// Find types with specific attributes
var auditedTypes = AppDomain.CurrentDomain
    .GetAssemblies()
    .SelectMany(a => a.GetTypes())
    .Where(t => t.GetCustomAttribute<AuditableAttribute>() != null)
    .ToList();

// Get all properties of a type
var productProperties = typeof(Product)
    .GetProperties(
        BindingFlags.Public | 
        BindingFlags.Instance)
    .ToList();

foreach (var prop in productProperties) {
    Console.WriteLine($"Property: {prop.Name} ({prop.PropertyType.Name})");
}
```

### Dynamic Assembly Loading

```csharp
using System.Reflection;

// Load assembly from file path
var assembly = Assembly.LoadFrom("path/to/MyAssembly.dll");

// Get all types from loaded assembly
var types = assembly.GetTypes();

// Find specific type
var targetType = assembly.GetType("MyNamespace.MyClass");

// Instantiate dynamically using Activator
var instance = Activator.CreateInstance(targetType);

// Call method dynamically
var method = targetType.GetMethod("MyMethod");
var result = method.Invoke(instance, null);  // No parameters
```

### Custom Attributes & Metadata

```csharp
// Define custom attribute
[AttributeUsage(AttributeTargets.Class)]
public class DataContractAttribute : Attribute {
    public string ContractName { get; set; }
}

// Apply to class
[DataContract(ContractName = "User")]
public class User {
    public string Name { get; set; }
}

// Discover and use
var userType = typeof(User);
var attr = userType.GetCustomAttribute<DataContractAttribute>();

if (attr != null) {
    Console.WriteLine($"Contract: {attr.ContractName}");  // "User"
}

// Get all classes with this attribute
var contractTypes = AppDomain.CurrentDomain
    .GetAssemblies()
    .SelectMany(a => a.GetTypes())
    .Where(t => t.GetCustomAttribute<DataContractAttribute>() != null)
    .ToList();
```

### Environment Detection & Process Monitoring

```csharp
using Hydrogen;

// Detect .NET runtime version
var runtimeVersion = Environment.Version;
var frameworkVersion = System.Diagnostics.FileVersionInfo
    .GetVersionInfo(typeof(object).Module.FullyQualifiedName)
    .ProductVersion;

Console.WriteLine($".NET Version: {runtimeVersion}");
Console.WriteLine($"Framework: {frameworkVersion}");

// Platform detection
var isPlatformWindows = System.Runtime.InteropServices
    .RuntimeInformation.IsOSPlatform(
        System.Runtime.InteropServices.OSPlatform.Windows);

var isPlatformLinux = System.Runtime.InteropServices
    .RuntimeInformation.IsOSPlatform(
        System.Runtime.InteropServices.OSPlatform.Linux);

Console.WriteLine($"Windows: {isPlatformWindows}");
Console.WriteLine($"Linux: {isPlatformLinux}");
```

## 🏗️ Architecture & Modules

**CodeBuilder Module**: Fluent API for programmatic C# code generation
- ClassBuilder: Generate classes with fields, properties, methods
- MethodBuilder: Define method signatures and implementations
- FieldBuilder: Create typed fields with initialization
- Supports nested types, inheritance, interfaces

**Reflection Module**: Enhanced type system introspection
- Extension methods on Type, Assembly, MethodInfo
- Attribute discovery and filtering
- Member access and invocation helpers
- Performance optimizations with caching

**Environment Module**: Runtime information and process monitoring
- ProcessSentry: Monitor current process state
- .NET version and platform detection
- Host and machine information
- Assembly and module introspection

**Math Runtime**: Expression evaluation and computation
- Parse and evaluate mathematical expressions
- Function support (sin, cos, sqrt, pow, etc.)
- Variable substitution
- Error handling for invalid expressions

## 📦 Dependencies

- **Hydrogen**: Core framework
- **System.Reflection**: .NET reflection APIs (.NET built-in)
- **System.CodeDom**: Code generation support (legacy, used internally)

## ⚠️ Best Practices

- **Reflection Performance**: Cache type lookups to avoid repeated expensive reflection
- **Code Generation**: Use ClassBuilder for simple dynamic code; consider Source Generators for complex compile-time scenarios
- **Assembly Loading**: Be careful with loading untrusted assemblies; consider sandboxing
- **Type Safety**: When using Activator.CreateInstance, verify constructor parameters match
- **Error Handling**: Reflection exceptions (MethodAccessException, MissingMethodException) should be handled gracefully
- **Thread Safety**: Reflection caches should be thread-safe when used in concurrent scenarios

## ✅ Status & Compatibility

- **Maturity**: Production-tested, core APIs stable
- **.NET Target**: .NET 8.0+ (primary), .NET Framework 4.7+ (legacy support)
- **Thread Safety**: Reflection utilities are thread-safe; use locks when modifying mutable builder state
- **Performance**: Type discovery operations benefit from caching; use cached results when possible

## 📖 Related Projects

- [Hydrogen.NETCore](../Hydrogen.NETCore) - .NET Core specific utilities and configuration
- [Hydrogen.Windows](../Hydrogen.Windows) - Windows-specific APIs
- [Hydrogen.Application](../Hydrogen.Application) - Application framework using reflection
- [Hydrogen.Generators](../Hydrogen.Generators) - Compile-time code generation alternative

## ⚖️ License

Distributed under the **MIT NON-AI License**.

See the LICENSE file for full details. More information: [Sphere10 NON-AI-MIT License](https://sphere10.com/legal/NON-AI-MIT)

## 👤 Author

**Herman Schoenfeld** - Software Engineer

---

**Version**: 2.0+
