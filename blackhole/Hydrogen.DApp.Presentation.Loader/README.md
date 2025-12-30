# 🌐 Hydrogen.DApp.Presentation.Loader

**Blazor WebAssembly host application** for running Hydrogen DApp presentations in the browser with full service configuration and plugin support.

## 📋 Overview

`Hydrogen.DApp.Presentation.Loader` is a Blazor WebAssembly application that hosts the Hydrogen DApp presentation components. It provides the web application shell, service configuration, and runtime environment for running blockchain DApp UIs in modern browsers.

## 🏗️ Architecture

The application structure includes:

- **App.razor**: Main application shell and routing configuration
- **AppViewModel.cs**: Root application state and services
- **Program.cs**: WebAssembly entry point and dependency configuration
- **Services**: UI services (dialogs, wizards, navigation)
- **wwwroot**: Static assets, JavaScript, CSS, and service worker
- **Plugins**: Plugin loader for dynamic module loading

## 🚀 Key Features

- **Blazor WebAssembly**: Runs entirely in the browser
- **Service Workers**: Offline support and performance caching
- **Local Storage**: Client-side data persistence with Blazored.LocalStorage
- **Dynamic Plugin Loading**: Load DApp plugins at runtime
- **Responsive UI**: Bootstrap-based responsive design
- **Wizard Dialogs**: Multi-step dialog support
- **Modal Management**: Modal dialog framework

## 🔧 Usage

The application runs automatically when deployed. Customize through:

```csharp
// In Program.cs
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add Hydrogen services
builder.Services.AddHydrogenDAppPresentation();

// Add your plugins
builder.Services.AddYourPlugin();
```

## 📦 Dependencies

- **Hydrogen.DApp.Presentation**: Blazor component library
- **Hydrogen.DApp.Presentation.WidgetGallery**: Demo widget gallery
- **Microsoft.AspNetCore.Components.WebAssembly**: Blazor WebAssembly runtime
- **Blazored.LocalStorage**: Client-side storage

## ⚙️ Configuration

Configure application settings in `wwwroot/appsettings.json`:

```json
{
  "apiUrl": "https://api.example.com",
  "nodeUrl": "ws://localhost:8000",
  "cachePolicy": "aggressive"
}
```

## 🌐 Deployment

Build for production:

```bash
dotnet publish -c Release
```

The output in `bin/Release/net8.0/publish/wwwroot` contains the static site ready for hosting.

## 📄 Related Projects

- [Hydrogen.DApp.Presentation](./Hydrogen.DApp.Presentation) - Component library
- [Hydrogen.DApp.Presentation.WidgetGallery](./Hydrogen.DApp.Presentation.WidgetGallery) - Component showcase
- [Hydrogen.DApp.Node](../src/Hydrogen.DApp.Node) - Backend node API
- [Hydrogen.DApp.Core](../src/Hydrogen.DApp.Core) - Core DApp framework
