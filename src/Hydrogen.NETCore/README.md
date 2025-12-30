<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 🌐 Hydrogen.NETCore

**.NET Core and modern .NET runtime utilities** providing configuration integration, dependency injection support, async patterns, and built-in service discovery for .NET 5+ applications.

Hydrogen.NETCore bridges Hydrogen with Microsoft's **modern .NET ecosystem**, enabling seamless integration with `IServiceCollection`, configuration systems, and async/await patterns while providing utilities for performance monitoring and diagnostics.

## ⚡ 10-Second Example

```csharp
using Hydrogen;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

// Configure services using standard DI container
var services = new ServiceCollection();
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Add Hydrogen services
services.AddSingleton<IConfiguration>(config);
services.AddHydrogenServices();  // Extension method

// Build and use
var provider = services.BuildServiceProvider();
var logger = provider.GetRequiredService<ILogger>();
logger.Info("Initialized");
```

## 🏗️ Core Concepts

**Service Collection Extensions**: Extension methods for registering Hydrogen services in IServiceCollection following .NET conventions.

**Configuration Integration**: Bridge between Hydrogen configuration and Microsoft.Extensions.Configuration for seamless settings management.

**Async Patterns**: Modern async/await utilities and async initialization patterns for .NET Core applications.

**Dependency Injection**: Full support for Microsoft dependency injection container with scope management.

**Performance Diagnostics**: Real-time performance monitoring, memory profiling, and diagnostic metrics collection.

## 🔧 Core Examples

### Service Registration & Dependency Injection

```csharp
using Hydrogen;
using Hydrogen.NETCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// Create service collection
var services = new ServiceCollection();

// Register custom services
services.AddSingleton<IUserRepository, UserRepository>();
services.AddTransient<IUserService, UserService>();
services.AddScoped<IUnitOfWork, UnitOfWork>();

// Hydrogen service registration
services.TryAddSingleton<IApplicationContext, ApplicationContext>();
services.TryAddSingleton<ILogger, ConsoleLogger>();

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Request services with dependency resolution
var userService = serviceProvider.GetRequiredService<IUserService>();
var users = userService.GetAllUsers();

// Service scoping for database contexts
using (var scope = serviceProvider.CreateScope()) {
    var scopedService = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    scopedService.Users.Add(new User { Name = "John" });
    await scopedService.SaveChangesAsync();
}
```

### Configuration & Settings Management

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Build configuration from multiple sources
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
    .AddEnvironmentVariables()
    .AddInMemoryCollection(new Dictionary<string, string> {
        { "Application:Name", "MyApp" },
        { "Application:Version", "1.0" }
    })
    .Build();

// Create service collection with configuration
var services = new ServiceCollection();
services.AddSingleton(configuration);

// Bind configuration sections to strongly-typed options
services.Configure<DatabaseSettings>(configuration.GetSection("Database"));
services.Configure<AppSettings>(configuration.GetSection("Application"));

var provider = services.BuildServiceProvider();

// Access configuration
var dbSettings = provider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
Console.WriteLine($"Connection: {dbSettings.ConnectionString}");

var appSettings = provider.GetRequiredService<IOptions<AppSettings>>().Value;
Console.WriteLine($"App: {appSettings.Name} v{appSettings.Version}");
```

### Async Initialization & Startup Patterns

```csharp
using Hydrogen;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Hosted service for initialization
public class InitializationService : IHostedService {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public InitializationService(IServiceProvider serviceProvider, ILogger logger) {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        _logger.Info("Initialization service starting...");
        
        using (var scope = _serviceProvider.CreateScope()) {
            var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationContext>();
            await dbContext.InitializeAsync();
        }
        
        _logger.Info("Initialization complete");
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        _logger.Info("Initialization service stopping");
        return Task.CompletedTask;
    }
}

// Register in Host builder
var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) => {
        services.AddSingleton<ILogger, ConsoleLogger>();
        services.AddSingleton<IApplicationContext, ApplicationContext>();
        services.AddHostedService<InitializationService>();
    })
    .Build();

await host.RunAsync();
```

### Performance Monitoring & Diagnostics

```csharp
using Hydrogen;
using Hydrogen.NETCore;
using System.Diagnostics;

// Create diagnostics collector
var diagnostics = new PerformanceDiagnostics();

// Measure operation timing
diagnostics.StartMeasure("database-query");
var results = await FetchDataAsync();
diagnostics.StopMeasure("database-query");

// Get performance metrics
var metrics = diagnostics.GetMetrics();
foreach (var metric in metrics) {
    Console.WriteLine($"{metric.Name}: {metric.ElapsedMilliseconds}ms");
}

// Monitor memory usage
long initialMemory = GC.GetTotalMemory(true);
var largeList = new List<object>();
for (int i = 0; i < 100000; i++) {
    largeList.Add(new object());
}
long finalMemory = GC.GetTotalMemory(false);
long allocated = finalMemory - initialMemory;

Console.WriteLine($"Memory allocated: {allocated / 1024.0}KB");
```

### Async Task Coordination

```csharp
using Hydrogen;
using System.Threading.Tasks;

// Execute multiple async operations in parallel
var tasks = new List<Task<User>> {
    FetchUserAsync(1),
    FetchUserAsync(2),
    FetchUserAsync(3)
};

// Wait for all to complete
var users = await Task.WhenAll(tasks);

// Process results
foreach (var user in users) {
    Console.WriteLine($"User: {user.Name}");
}

// Handle first completed result
Task<string> task1 = FetchDataAsync(source1);
Task<string> task2 = FetchDataAsync(source2);
var firstResult = await Task.WhenAny(task1, task2);
var data = firstResult.Result;

// Cancel operations with timeout
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
try {
    await LongRunningOperationAsync(cts.Token);
} catch (OperationCanceledException) {
    Console.WriteLine("Operation timed out");
}
```

## 🏗️ Architecture & Modules

**Service Collection Extensions**: Methods for registering Hydrogen types with IServiceCollection
- AddHydrogenServices: Register core Hydrogen services
- AddLogging: Logger integration
- AddConfiguration: Configuration integration

**Configuration Module**: Integration with Microsoft.Extensions.Configuration
- JSON, XML, INI file support
- Environment variable override
- Options pattern support
- Configuration builder patterns

**Async Patterns**: Modern async/await support
- Task coordination (WhenAll, WhenAny)
- CancellationToken support
- Async initialization patterns
- Task cancellation and timeouts

**Diagnostics Module**: Performance monitoring and metrics
- Operation timing and profiling
- Memory usage monitoring
- Performance metrics collection
- Performance counters integration

**Hosted Services**: Integration with IHostedService for background tasks
- Application lifecycle management
- Graceful startup/shutdown
- Background task coordination
- Service registration patterns

## 📦 Dependencies

- **Hydrogen**: Core framework
- **Hydrogen.NET**: .NET framework utilities
- **Microsoft.Extensions.DependencyInjection**: Dependency injection abstraction
- **Microsoft.Extensions.Configuration**: Configuration system
- **Microsoft.Extensions.Logging.Abstractions**: Logging abstraction
- **Microsoft.Extensions.Hosting**: Generic host pattern

## ⚠️ Best Practices

- **Use IOptions for configuration**: Prefer injecting `IOptions<T>` over raw configuration
- **Proper scope management**: Always use `using` with service scopes
- **Async all the way**: Use async APIs consistently, not mixed with sync
- **CancellationToken support**: Pass cancellation tokens to long-running operations
- **Dependency injection**: Prefer constructor injection to service locator pattern
- **Lazy initialization**: Use IHostedService for complex initialization logic

## ✅ Status & Compatibility

- **Maturity**: Production-tested, core APIs stable
- **.NET Target**: .NET 8.0+ (primary), .NET 6.0+ (compatible)
- **Thread Safety**: Service scopes provide thread-local isolation; reuse provider across threads
- **Performance**: Dependency resolution is fast; cache required services when used frequently

## 📖 Related Projects

- [Hydrogen.NET](../Hydrogen.NET) - .NET Framework utilities
- [Hydrogen.Application](../Hydrogen.Application) - Application lifecycle framework
- [Hydrogen.Web.AspNetCore](../Hydrogen.Web.AspNetCore) - ASP.NET Core integration
- [Hydrogen.Communications](../Hydrogen.Communications) - Network communication using async patterns

## ⚖️ License

Distributed under the **MIT NON-AI License**.

See the LICENSE file for full details. More information: [Sphere10 NON-AI-MIT License](https://sphere10.com/legal/NON-AI-MIT)

## 👤 Author

**Herman Schoenfeld** - Software Engineer

---

**Version**: 2.0+
