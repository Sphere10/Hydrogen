# Hydrogen.Windows

Windows-specific utilities and platform integration layer for Hydrogen applications, providing access to Windows APIs and features.

## 📋 Overview

`Hydrogen.Windows` provides Windows-specific implementations and utilities for Hydrogen applications, including registry access, Windows services, event logging, and other platform-specific features.

## 🏗️ Architecture

The library includes:

- **Registry Management**: Safe registry access and modification
- **Windows Services**: Windows service installation and management
- **Event Logging**: Windows Event Log integration
- **Security & Access Control**: Windows security descriptors and ACLs
- **System Integration**: Integration with Windows system features

## 🚀 Key Features

- **Registry API**: Type-safe registry access
- **Service Installation**: Programmatic Windows service setup
- **Event Logging**: Write to Windows Event Log
- **Security Management**: Access control and permissions
- **System Information**: Windows-specific system information

## 📦 Dependencies

- **Hydrogen**: Core framework
- **Hydrogen.NET**: .NET-specific utilities
- **Microsoft.Win32.Registry**: Registry access
- **System.ServiceProcess.ServiceController**: Service management
- **System.Diagnostics.EventLog**: Event logging
- **System.Security.AccessControl**: Security descriptors

## 📄 Related Projects

- [Hydrogen.Windows.Forms](../Hydrogen.Windows.Forms) - Windows Forms UI framework
- [Hydrogen.Windows.LevelDB](../Hydrogen.Windows.LevelDB) - Windows LevelDB integration
- [Hydrogen.NET](../Hydrogen.NET) - .NET framework utilities
