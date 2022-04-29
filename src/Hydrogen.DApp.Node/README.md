# Hydrogen Node

This project represents the hydrogen node.


## NOTES
- To ensure all Nuget dependencies are copied to the /bin folder add a `<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>` to the .csproj PropertyGroup element.

- In Visual Studio 2019+, launching the `Hydrogen.DApp.Host` project will not attach the debugger to the child `Hydrogen.DApp.Node` process. In order to auto-attach the debugger, follow these steps:
* Install the [Microsoft Child Process Debugging Power Tool](https://marketplace.visualstudio.com/items?itemName=vsdbgplat.MicrosoftChildProcessDebuggingPowerTool) extension.
* In `Debug` menu in Visual Studio, select `Other Debug Targets -> Child Process Debugging Settings`.
* Select `Enable child process debugging` and add `Hydrogen.DApp.Node.exe` into the list.
* Right click on the `Hydrogen.DApp.Node` and select `Properties -> Debug -> Enable native code debugging`.
