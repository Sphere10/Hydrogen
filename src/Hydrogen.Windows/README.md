# Hydrogen.Windows

Windows-specific utilities and platform integration layer for Hydrogen applications, providing access to Windows APIs and features.

## 📋 Overview

`Hydrogen.Windows` provides Windows-specific implementations and utilities for Hydrogen applications, including registry access, Windows services, event logging, security management, and other platform-specific features.

## 🏗️ Architecture

The library includes several key modules:

- **Security**: NT objects, user/group management, SID resolution
- **Registry Management**: Safe registry access and modification
- **Windows Services**: Windows service installation and management
- **Event Logging**: Windows Event Log integration
- **Access Control**: Windows security descriptors and ACLs
- **System Integration**: Integration with Windows system features

## 🚀 Key Features

- **NT Object Model**: Working with Windows NT security objects (users, groups, domains)
- **SID Management**: Security Identifier resolution and manipulation
- **User & Group Operations**: Create, update, and manage local users and groups
- **Privilege Management**: Grant and revoke user privileges
- **Registry API**: Type-safe registry access
- **Service Installation**: Programmatic Windows service setup
- **Event Logging**: Write to Windows Event Log
- **Security Management**: Access control and permissions
- **System Integration**: Integration with Windows system features

## 🔧 Usage

### Working with NT Objects

Access Windows NT security information:

```csharp
using Hydrogen.Windows;
using Hydrogen.Windows.Security;

// Get current machine/host information
NTHost host = NTHost.CurrentMachine;
var hostName = host.Name;      // Machine name
var hostSID = host.SID;        // Machine SID

// Create NT remote object with resolved SID
var remoteObj = new NTRemoteObject(
	host.Name,
	"Domain",
	"Name",
	host.SID,
	WinAPI.ADVAPI32.SidNameUse.Domain
);
```

### Dangling Objects (Unresolved References)

Work with security objects that don't yet have a resolved SID (e.g., deleted users):

```csharp
using Hydrogen.Windows.Security;

// Create dangling object by name only (no SID resolution)
var danglingUser = new NTDanglingObject(
	NTHost.CurrentMachine.Name,
	"DeletedUser"
);

// Or create by SID with unknown name
var danglingBySID = new NTDanglingObject(
	NTHost.CurrentMachine.Name,
	NTHost.CurrentMachine.SID,
	WinAPI.ADVAPI32.SidNameUse.Invalid
);
```

### Local User Management

Create, read, and update local Windows users:

```csharp
using Hydrogen.Windows.Security;

// Get local host
NTHost host = NTHost.CurrentMachine;

// Get built-in users
var adminUser = host.GetUserByName("Administrator");
var guestUser = host.GetUserByName("Guest");

// Get local groups
var adminsGroup = host.GetGroupByName("Administrators");
var guestsGroup = host.GetGroupByName("Guests");

// Get user SID
var adminSID = adminUser.SID;

// Check user properties
var homeDir = adminUser.HomeDirectory;
var scriptPath = adminUser.ScriptPath;
var logonServer = adminUser.LogonServer;

// Update user properties
adminUser.HomeDirectory = "C:\\Users\\NewPath";
adminUser.ScriptPath = "C:\\Scripts\\Login.bat";

// Update user password
adminUser.SetPassword("NewPassword123!");

// Manage user privileges
adminUser.Privileges.Add(UserPrivilege.SeNetworkLogonRight);

// Check group membership
var groups = adminUser.Groups;  // Returns list of group memberships
var isMember = adminsGroup.Members.Contains(adminUser);
```

### Working with Groups

Manage group membership and properties:

```csharp
using Hydrogen.Windows.Security;

var host = NTHost.CurrentMachine;

// Get a group
var adminsGroup = host.GetGroupByName("Administrators");

// Get group members (returns list of users)
var members = adminsGroup.Members;

// Check if specific user is member
var adminUser = host.GetUserByName("Administrator");
var isAdmin = members.Contains(adminUser);

// Get group properties
var groupSID = adminsGroup.SID;
var comment = adminsGroup.Comment;
```

## 📦 Dependencies

- **Hydrogen**: Core framework
- **Hydrogen.NET**: .NET-specific utilities
- **Microsoft.Win32.Registry**: Registry access
- **System.ServiceProcess.ServiceController**: Service management
- **System.Diagnostics.EventLog**: Event logging
- **System.Security.AccessControl**: Security descriptors

## 💡 Key Concepts

- **SID (Security Identifier)**: Windows unique identifier for users, groups, and computers
- **NT Object**: Named security principal with resolved SID
- **Dangling Object**: Security reference that couldn't be resolved to a SID
- **Local Users/Groups**: Machine-specific security principals (vs. domain principals)
- **Privileges**: Specific rights granted to users (SeNetworkLogonRight, SeLockMemoryPrivilege, etc.)

## ⚠️ Permissions Required

- Most operations require **Administrator privileges**
- Reading current machine information doesn't require special permissions
- Creating/modifying users requires elevation
- Writing to Event Log may require elevation depending on event source

## 📄 Related Projects

- [Hydrogen.Windows.Forms](../Hydrogen.Windows.Forms) - Windows Forms UI framework
- [Hydrogen.Windows.LevelDB](../Hydrogen.Windows.LevelDB) - Windows LevelDB integration
- [Hydrogen.NET](../Hydrogen.NET) - .NET framework utilities

