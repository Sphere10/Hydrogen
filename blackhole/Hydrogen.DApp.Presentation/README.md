<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 🎨 Hydrogen.DApp.Presentation

**Blazor component library** providing rich, reusable UI components for building responsive web interfaces for blockchain DApps.

## 📋 Overview

`Hydrogen.DApp.Presentation` is a Razor component library providing a rich set of reusable Blazor components for building user interfaces for Hydrogen DApps. It includes dialogs, wizards, modals, grids, and other common UI patterns needed for blockchain applications.

## 🏗️ Architecture

The library is organized into functional component categories:

- **Components**: Reusable Blazor UI components (buttons, inputs, etc.)
- **Wizard**: Multi-step wizard/form framework for complex workflows
- **Modal**: Modal dialog implementations
- **Services**: UI services (dialog service, wizard service, etc.)
- **ViewModels**: Base view model classes for component logic
- **Models**: UI data models and state containers
- **Events**: Custom event handlers and delegates
- **Plugins**: Plugin system integration for extensibility

## 🚀 Key Features

- **Wizard Framework**: Build complex multi-step forms with validation
- **Modal Dialogs**: Display information, confirmations, and custom content
- **Component Library**: Pre-built controls for common UI patterns
- **MVVM Pattern**: Separation of UI and business logic
- **Event System**: Component communication through events
- **Service Injection**: Dependency injection for UI services
- **Plugin Support**: Extend with custom components and features
- **Responsive Design**: Mobile-friendly layouts

## 🔧 Usage

Use the wizard builder to create multi-step workflows:

```csharp
var wizard = new DefaultWizardBuilder<NewWalletModel>()
    .NewWizard("Create New Wallet")
    .WithModel(new NewWalletModel())
    .AddStep<WalletNameStep>()
    .AddStep<WalletTypeStep>()
    .AddStep<SummaryStep>()
    .OnFinished(async model => {
        // Handle wizard completion
        return Result.Success;
    })
    .Build();
```

Display a modal dialog:

```csharp
// In a component
await ViewService.DialogAsync(content, "Dialog Title");
```

## 📦 Dependencies

- **Hydrogen**: Core framework
- **Microsoft.AspNetCore.Components**: Blazor component framework
- **Microsoft.JSInterop**: JavaScript interop for interactive features

## � Related Projects

- [Hydrogen.DApp.Presentation.Loader](./Hydrogen.DApp.Presentation.Loader) - WebAssembly host for this library
- [Hydrogen.DApp.Presentation2](./Hydrogen.DApp.Presentation2) - Alternative presentation implementation
- [Hydrogen.DApp.Presentation.WidgetGallery](./Hydrogen.DApp.Presentation.WidgetGallery) - Component showcase
- [Hydrogen](../Hydrogen) - Core framework
