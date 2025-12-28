# Hydrogen.DApp.Node

Node implementation for Hydrogen-based blockchains.

## Notes
- To ensure NuGet dependencies are copied to the `/bin` folder, add `<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>` to the project file.
- In Visual Studio 2019+, launching `Hydrogen.DApp.Host` does not attach the debugger to the child `Hydrogen.DApp.Node` process by default.

## Debugging child process in Visual Studio
1. Install the [Microsoft Child Process Debugging Power Tool](https://marketplace.visualstudio.com/items?itemName=vsdbgplat.MicrosoftChildProcessDebuggingPowerTool).
2. In Visual Studio, go to **Debug -> Other Debug Targets -> Child Process Debugging Settings**.
3. Enable child process debugging and add `Hydrogen.DApp.Node.exe`.
4. Right-click `Hydrogen.DApp.Node` -> **Properties -> Debug** and enable native code debugging.
