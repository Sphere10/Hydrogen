# Hydrogen.Application

A framework library that provides comprehensive application lifecycle, command-line interface, presentation, and dependency injection support for Hydrogen-based applications.

## 📋 Overview

`Hydrogen.Application` is the foundation for building applications on top of the Hydrogen framework. It provides abstractions and utilities for:

- **Application Lifecycle**: Manage application initialization, configuration, and shutdown
- **Dependency Injection (IoC)**: Integrated service collection and resolution
- **Command-Line Interface**: Parse and handle command-line arguments
- **DRM & Product Management**: License validation and product identification
- **REST API Support**: REST endpoint and HTTP handler utilities
- **Presentation Layer Foundation**: Base classes for UI components and view models
- **Settings Management**: Configuration and settings persistence
- **Logging Infrastructure**: Integrated logging utilities

## 🏗️ Architecture

The library is organized into several key modules:

- **Core**: Core framework components
- **Lifecycle**: Application initialization and shutdown hooks
- **IoC**: Dependency injection configuration and extensions
- **CommandLine**: Command-line argument parsing and handling
- **Product**: Product identification and DRM support
- **Rest**: REST API helpers and utilities
- **Settings**: Configuration and settings management
- **Presentation**: UI base classes and view model infrastructure

## 🚀 Key Features

- Extensible application lifecycle management
- Seamless integration with Microsoft.Extensions.DependencyInjection
- Modular configuration system
- REST API support utilities
- Product licensing and identification
- Presentation layer abstraction for both desktop and web UIs

## 🔧 Usage

This library is typically used as a foundation for other Hydrogen-based applications:

```csharp
// In your application startup
using Hydrogen.Application;

var framework = HydrogenFramework.Create(new HydrogenFrameworkOptions {
    // Configuration options
});

// Services are automatically configured for dependency injection
```

## 📦 Dependencies

- **Hydrogen**: Core utilities and extensions
- **Microsoft.Extensions.DependencyInjection**: Modern dependency injection
- **System.Configuration.ConfigurationManager**: Configuration support

## 📄 Related Projects

- [Hydrogen](../Hydrogen) - Core framework library
- [Hydrogen.DApp.Core](../Hydrogen.DApp.Core) - DApp-specific core functionality
- [Hydrogen.Web.AspNetCore](../Hydrogen.Web.AspNetCore) - ASP.NET Core integration
