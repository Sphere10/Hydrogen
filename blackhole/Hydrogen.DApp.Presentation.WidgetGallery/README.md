<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# Hydrogen.DApp.Presentation.WidgetGallery

Component showcase and plugin demonstrating reusable Blazor widgets and UI patterns for Hydrogen DApps.

## 📋 Overview

`Hydrogen.DApp.Presentation.WidgetGallery` is both a plugin and showcase application that demonstrates all available Blazor components and widgets in the Hydrogen framework. It serves as both documentation and a working example of how to build DApp UIs.

## 🏗️ Architecture

The plugin is organized into demonstration sections:

- **Widgets**: Showcase of all UI components and controls
- **Modals**: Modal and dialog examples
- **Wizards**: Multi-step wizard examples with different step types
- **Forms**: Form validation and input examples
- **Grids**: Data grid and table examples
- **Services**: Service integration examples (viewmodels, dependency injection)

## 🚀 Key Features

- **Interactive Examples**: Live, runnable examples of each component
- **Source Visible**: Learn from working component implementations
- **Widget Creation Wizard**: Example wizard for creating custom widgets
- **Modal Examples**: Various modal dialog types and patterns
- **Responsive Layouts**: Examples of responsive grid layouts
- **Data Binding**: Examples of binding to view models
- **Service Integration**: Show dependency injection and services

## 🔧 Architecture Example

The widget gallery demonstrates a wizard for creating new widgets:

```csharp
// Step 1: Collect widget info (name, description)
// Step 2: Define dimensions (width, height, depth)
// Step 3: Set properties and configuration
// Step 4: Review and confirm

// See:  Widgets/Components/NewWidgetWizardStep.razor
// See: Widgets/ViewModels/WidgetDimensionsStepViewModel.cs
```

## 📦 Dependencies

- **Hydrogen.DApp.Presentation**: Component library
- **Microsoft.AspNetCore.Components**: Blazor framework
- **IRandomNumberService**: Example service for dependency injection

## 🎯 Usage

Access the widget gallery in the loaded application by navigating to `/widget-gallery`. This will show:

- All available widgets and components
- Working examples you can interact with
- Modal and dialog demonstrations
- Wizard and multi-step form examples

## 📖 Navigation

- `/widget-gallery` - Main component showcase
- `/widget-gallery/modals` - Modal and dialog examples
- `/widget-gallery/wizards` - Wizard implementation examples
- `/widget-gallery/forms` - Form validation examples
- `/widget-gallery/grids` - Data grid examples

## 📄 Related Projects

- [Hydrogen.DApp.Presentation](./Hydrogen.DApp.Presentation) - Component library being showcased
- [Hydrogen.DApp.Presentation.Loader](./Hydrogen.DApp.Presentation.Loader) - Host application
