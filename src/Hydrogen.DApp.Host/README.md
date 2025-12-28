# Hydrogen.DApp.Host

Host process for running Hydrogen DApp nodes as standalone services, managing the lifecycle and providing entry points for node execution.

## 📋 Overview

`Hydrogen.DApp.Host` is a lightweight host application that launches and manages Hydrogen DApp nodes. It serves as the entry point for running blockchain nodes, handling process management, logging, and graceful shutdown.

## 🏗️ Key Responsibilities

- **Process Lifecycle**: Start, monitor, and shutdown node processes
- **Child Process Management**: Launch and attach to Hydrogen.DApp.Node
- **Configuration**: Load and apply node configuration
- **Logging**: Forward and aggregate node logs
- **Error Handling**: Catch and report critical errors

## 🚀 Usage

Launch a node:

```bash
# Run the host, which will spawn Hydrogen.DApp.Node as a child process
dotnet Hydrogen.DApp.Host.dll
```

## 🔧 Debugging

To debug the child node process in Visual Studio:

1. Install the [Microsoft Child Process Debugging Power Tool](https://marketplace.visualstudio.com/items?itemName=vsdbgplat.MicrosoftChildProcessDebuggingPowerTool)
2. In Visual Studio Debug menu: **Other Debug Targets → Child Process Debugging Settings**
3. Enable child process debugging and add `Hydrogen.DApp.Node.exe`
4. Right-click on `Hydrogen.DApp.Node` project → **Properties → Debug → Enable native code debugging**

## 📦 Dependencies

- **Hydrogen.Application**: Application framework
- **Hydrogen.DApp.Core**: DApp core functionality
- **Microsoft.Extensions.DependencyInjection**: Service injection

## ⚙️ Configuration

Node configuration is defined in embedded resource files and can be customized through application settings.

## 📄 Related Projects

- [Hydrogen.DApp.Node](../Hydrogen.DApp.Node) - Node implementation
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core) - DApp framework
- [Hydrogen.Application](../Hydrogen.Application) - Application lifecycle
