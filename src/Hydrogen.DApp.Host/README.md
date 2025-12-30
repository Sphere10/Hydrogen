<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 🚀 Hydrogen.DApp.Host

**Host process for running Hydrogen DApp nodes** as standalone services with process management, lifecycle control, and graceful shutdown.

Hydrogen.DApp.Host provides the **entry point for blockchain nodes**, handling child process spawning, configuration loading, logging aggregation, and error recovery for production Hydrogen DApp deployments.

## ⚡ 10-Second Example

```csharp
using Hydrogen.DApp.Host;

// Program.cs - the host launcher
var hostBuilder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddSingleton<INodeHostService, NodeHostService>();
        services.AddHostedService<NodeHostService>();
    })
    .ConfigureLogging(logging => {
        logging.AddConsole();
        logging.AddFile("logs/node.log");
    });

var host = hostBuilder.Build();
await host.RunAsync();

// This launches Hydrogen.DApp.Node as child process
// Console output:
// info: Hydrogen.DApp.Node starting...
// info: Listening on port 8080
```

## 🏗️ Core Concepts

**Host Process**: Main process managing the lifecycle of child node process.

**Child Process Management**: Spawn and monitor Hydrogen.DApp.Node executable.

**Configuration Loading**: Load node settings from configuration files.

**Log Aggregation**: Collect and forward logs from child process.

**Graceful Shutdown**: Coordinated shutdown of host and child process.

**Error Handling & Recovery**: Catch critical errors and attempt recovery.

## 🔧 Core Examples

### Basic Node Host Startup

```csharp
using Hydrogen.DApp.Host;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

// Create and configure host
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => {
        // Register core services
        services.AddSingleton<INodeConfiguration, NodeConfiguration>();
        services.AddHostedService<DAppNodeHostService>();
    })
    .ConfigureLogging((context, logging) => {
        logging.AddConsole();
        logging.AddDebug();
    })
    .Build();

// Run host (blocks until shutdown)
await host.RunAsync();

// This will:
// 1. Load configuration from appsettings.json
// 2. Start the host
// 3. Spawn Hydrogen.DApp.Node.exe as child process
// 4. Monitor child process
// 5. Shutdown gracefully on CTRL+C
```

### Custom Node Configuration

```csharp
using Hydrogen.DApp.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) => {
        config
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables("HYDROGENNODE_");
    })
    .ConfigureServices((context, services) => {
        // Bind configuration to options
        services.Configure<NodeHostOptions>(context.Configuration.GetSection("Node"));
        
        // Register services
        services.AddSingleton<INodeHostService, NodeHostService>();
        services.AddHostedService<NodeLifecycleService>();
    });

var host = hostBuilder.Build();

// Access configuration
var nodeOptions = host.Services.GetRequiredService<IOptions<NodeHostOptions>>().Value;
Console.WriteLine($"Node Port: {nodeOptions.Port}");
Console.WriteLine($"Network: {nodeOptions.NetworkName}");
```

### appsettings.json Configuration

```json
{
  "Node": {
    "Port": 8080,
    "NetworkName": "testnet",
    "NetworkId": 1,
    "MaxConnections": 50,
    "DataDirectory": "./data",
    "LogLevel": "Information",
    "Consensus": {
      "Type": "ProofOfWork",
      "TargetBlockTime": 10
    },
    "Mining": {
      "Enabled": true,
      "MinerAddress": "0xMinerAddress",
      "ThreadCount": 4
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Hydrogen": "Debug"
    }
  }
}
```

### Process Lifecycle Management

```csharp
using Hydrogen.DApp.Host;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

public class NodeLifecycleService : IHostedService {
    private readonly INodeHostService _nodeHostService;
    private readonly ILogger<NodeLifecycleService> _logger;
    private Process _nodeProcess;

    public NodeLifecycleService(INodeHostService nodeHostService, ILogger<NodeLifecycleService> logger) {
        _nodeHostService = nodeHostService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Starting node host...");
        
        try {
            // Spawn child node process
            _nodeProcess = _nodeHostService.StartNodeProcess();
            _logger.LogInformation($"Node process started (PID: {_nodeProcess.Id})");
            
            // Wait for node to be ready
            await _nodeHostService.WaitForNodeReadyAsync(TimeSpan.FromSeconds(30), cancellationToken);
            _logger.LogInformation("Node is ready");
        } catch (Exception ex) {
            _logger.LogError(ex, "Failed to start node");
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Stopping node host...");
        
        try {
            // Graceful shutdown
            await _nodeHostService.GracefulShutdownAsync(TimeSpan.FromSeconds(10), cancellationToken);
            _logger.LogInformation("Node stopped gracefully");
        } catch (Exception ex) {
            _logger.LogError(ex, "Error during shutdown");
            
            // Force kill if graceful shutdown fails
            if (_nodeProcess != null && !_nodeProcess.HasExited) {
                _logger.LogWarning("Forcing node process termination");
                _nodeProcess.Kill();
            }
        }
    }
}
```

### Error Handling & Recovery

```csharp
using Hydrogen.DApp.Host;
using Microsoft.Extensions.Hosting;

public class NodeMonitoringService : IHostedService {
    private readonly INodeHostService _nodeHostService;
    private readonly ILogger<NodeMonitoringService> _logger;
    private Timer _monitoringTimer;

    public NodeMonitoringService(INodeHostService nodeHostService, ILogger<NodeMonitoringService> logger) {
        _nodeHostService = nodeHostService;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Starting node monitoring...");
        
        // Monitor child process every 5 seconds
        _monitoringTimer = new Timer(async _ => await MonitorNodeHealth(), null, 
            TimeSpan.Zero, TimeSpan.FromSeconds(5));
        
        return Task.CompletedTask;
    }

    private async Task MonitorNodeHealth() {
        try {
            var isHealthy = await _nodeHostService.CheckNodeHealthAsync();
            
            if (!isHealthy) {
                _logger.LogWarning("Node health check failed");
                
                // Attempt recovery
                _logger.LogInformation("Attempting node recovery...");
                await _nodeHostService.RestartNodeAsync();
                _logger.LogInformation("Node restarted");
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error monitoring node health");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        _monitoringTimer?.Dispose();
        return Task.CompletedTask;
    }
}
```

### Logging & Output Capture

```csharp
using Hydrogen.DApp.Host;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((context, logging) => {
        // Console logging
        logging.AddConsole();
        
        // File logging
        logging.AddFile(new FileLoggingOptions {
            Path = "logs/host.log",
            RetainedFileCountLimit = 10,
            FileSizeLimit = 10 * 1024 * 1024  // 10MB
        });
        
        // Debug output
        logging.AddDebug();
        
        // Set log levels
        logging.SetMinimumLevel(LogLevel.Information);
    })
    .ConfigureServices((context, services) => {
        services.AddHostedService<DAppNodeHostService>();
        
        // Custom log processor to capture child process output
        services.AddSingleton<ILogProcessor, ChildProcessLogProcessor>();
    })
    .Build();

// Run with automatic output capture
await host.RunAsync();
```

### Child Process Debugging Setup

```csharp
using Hydrogen.DApp.Host;
using System.Diagnostics;

// Program.cs - Configure for debugging
var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

if (isDevelopment && Debugger.IsAttached) {
    // Enable child process debugging attachment
    var processStartInfo = new ProcessStartInfo {
        FileName = "Hydrogen.DApp.Node.exe",
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = false  // Show window for debugging
    };
    
    // Note: To debug child process:
    // 1. Install Microsoft Child Process Debugging Power Tool extension
    // 2. Debug menu -> Other Debug Targets -> Child Process Debugging Settings
    // 3. Enable child process debugging and add Hydrogen.DApp.Node.exe
    // 4. Right-click Hydrogen.DApp.Node project -> Properties -> Debug -> Enable native debugging
}
```

## 🏗️ Architecture & Responsibilities

**Process Lifecycle**: Start, monitor, shutdown node processes
- Child process spawning
- Process state tracking
- Exit code handling
- Process resource cleanup

**Configuration Management**: Load and apply node settings
- Configuration file parsing
- Environment variable override
- Settings validation
- Dynamic reload support

**Logging Infrastructure**: Aggregate and forward logs
- Console output capture
- File rotation
- Log level filtering
- Structured logging

**Error Handling**: Catch and report critical errors
- Exception logging
- Error recovery attempts
- Graceful degradation
- Health checks

**Service Integration**: Integration with .NET Generic Host
- IHostedService implementation
- Dependency injection
- Configuration provider
- Logging framework

## 📦 Dependencies

- **Hydrogen**: Core framework
- **Hydrogen.Application**: Application lifecycle
- **Hydrogen.DApp.Core**: DApp core functionality
- **Microsoft.Extensions.Hosting**: Generic host pattern
- **Microsoft.Extensions.DependencyInjection**: Service injection
- **Microsoft.Extensions.Configuration**: Configuration system
- **Microsoft.Extensions.Logging**: Logging abstraction

## ⚠️ Best Practices

- **Graceful shutdown**: Always implement proper shutdown sequences
- **Process monitoring**: Implement health checks to detect failures
- **Configuration isolation**: Keep host and node configuration separate
- **Log rotation**: Configure log file rotation to prevent disk overflow
- **Resource cleanup**: Properly dispose process handles and file streams
- **Error recovery**: Implement automatic restart with backoff strategy
- **Permission management**: Ensure proper Windows permissions for service installation

## ✅ Status & Compatibility

- **Maturity**: Production-tested for node hosting
- **.NET Target**: .NET 8.0+ (primary), .NET 6.0+ (compatible)
- **Platform**: Windows primary, cross-platform via Generic Host
- **Reliability**: Designed for 24/7 uptime requirements

## 📖 Related Projects

- [Hydrogen.DApp.Node](../Hydrogen.DApp.Node) - Node implementation
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core) - DApp framework
- [Hydrogen.Application](../Hydrogen.Application) - Application framework
- [Hydrogen.Communications](../Hydrogen.Communications) - Network layer

## ⚖️ License

Distributed under the **MIT NON-AI License**.

See the LICENSE file for full details. More information: [Sphere10 NON-AI-MIT License](https://sphere10.com/legal/NON-AI-MIT)

## 👤 Author

**Herman Schoenfeld** - Software Engineer

---

**Version**: 2.0+
