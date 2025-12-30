<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 🪟 Hydrogen.Windows

**Windows platform integration layer** providing secure, type-safe access to Windows-specific APIs, security features, registry management, and system integration.

Hydrogen.Windows enables **secure Windows system programming** through abstractions over Windows APIs, handling NT security objects, Windows services, event logging, and registry access while maintaining safety and compatibility.

## ⚡ 10-Second Example

```csharp
using Hydrogen.Windows.Security;

// Get current machine information
var host = NTHost.CurrentMachine;
Console.WriteLine($"Machine: {host.Name}");
Console.WriteLine($"SID: {host.SID}");

// Get local administrator user
var admin = host.GetUserByName("Administrator");
Console.WriteLine($"Admin: {admin.Name}");
Console.WriteLine($"Admin SID: {admin.SID}");

// Get group membership
var adminGroup = host.GetGroupByName("Administrators");
var isMember = adminGroup.Members.Contains(admin);
Console.WriteLine($"Is member of Administrators: {isMember}");
```

## 🏗️ Core Concepts

**NT Security Objects**: Windows NT domain and local security principals (users, groups, computers).

**Security Identifiers (SIDs)**: Unique Windows identifiers for security objects.

**Local vs Remote**: Support for local machine and remote machine security operations.

**Dangling Objects**: Handle security references that couldn't be resolved to SIDs (deleted accounts).

**Privilege Management**: Grant and revoke user privileges at the system level.

**Access Control**: Windows security descriptors and ACLs.

## 🔧 Core Examples

### Accessing Windows NT Objects

```csharp
using Hydrogen.Windows.Security;
using Hydrogen.Windows;

// Get current machine (local host)
var currentHost = NTHost.CurrentMachine;
Console.WriteLine($"Current Machine: {currentHost.Name}");
Console.WriteLine($"Machine SID: {currentHost.SID}");

// Connect to remote machine
var remoteHost = NTHost.OpenRemote("REMOTE-COMPUTER-NAME");
Console.WriteLine($"Remote Machine: {remoteHost.Name}");
Console.WriteLine($"Remote SID: {remoteHost.SID}");

// Create NT object reference (resolved)
var ntObject = new NTRemoteObject(
    currentHost.Name,           // Host name
    "DOMAIN",                   // Domain name
    "UserName",                 // Object name
    currentHost.SID,            // SID
    WinAPI.ADVAPI32.SidNameUse.User  // Object type
);

Console.WriteLine($"NT Object: {ntObject.Domain}\\{ntObject.Name}");
```

### Local User Management

```csharp
using Hydrogen.Windows.Security;

var host = NTHost.CurrentMachine;

// Get built-in users
var adminUser = host.GetUserByName("Administrator");
var guestUser = host.GetUserByName("Guest");

Console.WriteLine($"Admin: {adminUser.Name}");
Console.WriteLine($"Guest: {guestUser.Name}");

// Get user properties
var homeDir = adminUser.HomeDirectory;
var scriptPath = adminUser.ScriptPath;
var logonServer = adminUser.LogonServer;
var sid = adminUser.SID;

Console.WriteLine($"Home Directory: {homeDir}");
Console.WriteLine($"Script Path: {scriptPath}");
Console.WriteLine($"SID: {sid}");

// Update user properties
adminUser.HomeDirectory = "C:\\Users\\NewPath";
adminUser.ScriptPath = "C:\\Scripts\\Login.bat";

// Set password
adminUser.SetPassword("NewPassword123!");

// Get all local users
var allUsers = host.GetAllUsers();
foreach (var user in allUsers) {
    Console.WriteLine($"User: {user.Name} (SID: {user.SID})");
}
```

### Group Management & Membership

```csharp
using Hydrogen.Windows.Security;

var host = NTHost.CurrentMachine;

// Get groups
var adminsGroup = host.GetGroupByName("Administrators");
var usersGroup = host.GetGroupByName("Users");
var guestsGroup = host.GetGroupByName("Guests");

// Get group members
var adminMembers = adminsGroup.Members;
Console.WriteLine($"Administrators group has {adminMembers.Count} members:");
foreach (var member in adminMembers) {
    Console.WriteLine($"  - {member.Name}");
}

// Check group membership
var adminUser = host.GetUserByName("Administrator");
bool isAdmin = adminsGroup.Members.Contains(adminUser);
Console.WriteLine($"Administrator is admin: {isAdmin}");

// Get groups that user belongs to
var userGroups = adminUser.Groups;
Console.WriteLine($"Administrator belongs to {userGroups.Count} groups:");
foreach (var group in userGroups) {
    Console.WriteLine($"  - {group.Name}");
}

// Get group properties
var comment = adminsGroup.Comment;
var groupSID = adminsGroup.SID;

Console.WriteLine($"Group SID: {groupSID}");
Console.WriteLine($"Comment: {comment}");

// Get all groups
var allGroups = host.GetAllGroups();
Console.WriteLine($"Total groups: {allGroups.Count}");
```

### Dangling Objects (Unresolved References)

```csharp
using Hydrogen.Windows.Security;

var host = NTHost.CurrentMachine;

// Create dangling object by name (SID not resolved)
// Used when referenced user/group no longer exists
var danglingUser = new NTDanglingObject(
    host.Name,
    "DeletedUserName"
);

Console.WriteLine($"Dangling Object Name: {danglingUser.Name}");
Console.WriteLine($"Dangling Object SID: {danglingUser.SID}");  // null

// Create dangling object by SID (name unknown)
// Used when you have a SID but the account is deleted
var deletedUserSID = new SecurityIdentifier("S-1-5-21-2623811981-2622556916-1865790152-1001");
var danglingBySID = new NTDanglingObject(
    host.Name,
    deletedUserSID,
    WinAPI.ADVAPI32.SidNameUse.User
);

Console.WriteLine($"Dangling (by SID) SID: {danglingBySID.SID}");
Console.WriteLine($"Dangling (by SID) Name: {danglingBySID.Name}");  // Unknown

// Example: Cleaning up ACLs with dangling references
var acl = // ... get ACL from file/registry
var aceToRemove = acl.FindDanglingEntries().FirstOrDefault();
if (aceToRemove != null) {
    acl.RemoveAccessRule(aceToRemove);
    Console.WriteLine("Removed dangling ACE");
}
```

### Privilege Management

```csharp
using Hydrogen.Windows.Security;

var host = NTHost.CurrentMachine;
var adminUser = host.GetUserByName("Administrator");

// Grant privilege
adminUser.Privileges.Add(UserPrivilege.SeNetworkLogonRight);
adminUser.Privileges.Add(UserPrivilege.SeLockMemoryPrivilege);
adminUser.Privileges.Add(UserPrivilege.SeDebugPrivilege);

Console.WriteLine("Privileges granted");

// Check if user has privilege
bool hasNetworkLogon = adminUser.Privileges.Contains(UserPrivilege.SeNetworkLogonRight);
Console.WriteLine($"Has network logon right: {hasNetworkLogon}");

// Revoke privilege
adminUser.Privileges.Remove(UserPrivilege.SeLockMemoryPrivilege);
Console.WriteLine("Privilege revoked");

// List all user privileges
var privileges = adminUser.Privileges;
Console.WriteLine($"Total privileges: {privileges.Count}");
foreach (var priv in privileges) {
    Console.WriteLine($"  - {priv}");
}

// Common privileges
var commonPrivileges = new[] {
    UserPrivilege.SeNetworkLogonRight,           // Network access
    UserPrivilege.SeInteractiveLogonRight,       // Interactive logon
    UserPrivilege.SeBackupPrivilege,             // Backup privilege
    UserPrivilege.SeRestorePrivilege,            // Restore privilege
    UserPrivilege.SeSecurityPrivilege,           // Manage audit logs
    UserPrivilege.SeDebugPrivilege,              // Debug programs
    UserPrivilege.SeTakeOwnershipPrivilege,      // Take ownership
    UserPrivilege.SeLockMemoryPrivilege          // Lock memory pages
};
```

### Access Control & Permissions

```csharp
using Hydrogen.Windows.Security;
using System.Security.AccessControl;
using System.IO;

// Get security descriptor for file
var fileInfo = new FileInfo("C:\\Important\\Data.txt");
var fileSecurity = fileInfo.GetAccessControl();
var rules = fileSecurity.GetAccessRules(true, true, typeof(NTAccount));

// Display access rules
Console.WriteLine("Access Rules:");
foreach (AccessRule rule in rules) {
    var rights = rule is FileSystemAccessRule fsRule ? fsRule.FileSystemRights : AccessControlType.Allow;
    Console.WriteLine($"  Identity: {rule.IdentityReference}");
    Console.WriteLine($"  Type: {rule.AccessControlType}");
    Console.WriteLine($"  Rights: {rights}");
}

// Set permissions
var adminNTAccount = new NTAccount(NTHost.CurrentMachine.Name, "Administrator");
var fileAccessRule = new FileSystemAccessRule(
    adminNTAccount,
    FileSystemRights.FullControl,
    AccessControlType.Allow);

fileSecurity.AddAccessRule(fileAccessRule);
fileInfo.SetAccessControl(fileSecurity);
Console.WriteLine("Permissions updated");

// Remove user permissions
fileSecurity.RemoveAccessRule(fileAccessRule);
fileInfo.SetAccessControl(fileSecurity);
Console.WriteLine("Permissions removed");
```

### Windows Services

```csharp
using Hydrogen.Windows;
using System.ServiceProcess;

// Get service controller
var serviceController = new ServiceController("ServiceName");

// Check service status
var status = serviceController.Status;
Console.WriteLine($"Service Status: {status}");

// Start service
if (status == ServiceControllerStatus.Stopped) {
    serviceController.Start();
    serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
    Console.WriteLine("Service started");
}

// Stop service
if (status == ServiceControllerStatus.Running) {
    serviceController.Stop();
    serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
    Console.WriteLine("Service stopped");
}

// Get service info
Console.WriteLine($"Display Name: {serviceController.DisplayName}");
Console.WriteLine($"Service Name: {serviceController.ServiceName}");

// List all services
var services = ServiceController.GetServices();
foreach (var svc in services) {
    if (svc.Status == ServiceControllerStatus.Running) {
        Console.WriteLine($"Running: {svc.DisplayName}");
    }
}
```

### Event Log Integration

```csharp
using System.Diagnostics;

// Write to Windows Event Log
var eventLog = new EventLog("Application");
eventLog.Source = "MyApp";

eventLog.WriteEntry("Application started", EventLogEntryType.Information);
eventLog.WriteEntry("Critical error occurred", EventLogEntryType.Error);
eventLog.WriteEntry("Warning message", EventLogEntryType.Warning);

// Read from Event Log
var entries = eventLog.Entries;
foreach (EventLogEntry entry in entries) {
    if (entry.Source == "MyApp") {
        Console.WriteLine($"Type: {entry.Type}");
        Console.WriteLine($"Message: {entry.Message}");
        Console.WriteLine($"Time: {entry.TimeGenerated}");
        Console.WriteLine();
    }
}

// Filter recent entries
var recentEntries = entries
    .Cast<EventLogEntry>()
    .Where(e => e.Source == "MyApp")
    .Where(e => e.TimeGenerated > DateTime.Now.AddDays(-1))
    .OrderByDescending(e => e.TimeGenerated)
    .Take(100);

foreach (var entry in recentEntries) {
    Console.WriteLine($"{entry.TimeGenerated} [{entry.Type}] {entry.Message}");
}
```

## 🏗️ Architecture & Modules

**Security Module**: Windows NT security abstraction
- NT object model (users, groups, computers)
- SID resolution and management
- Local and remote operations
- User and group enumeration

**Privilege Module**: Windows privilege management
- Grant and revoke system privileges
- Privilege enumeration
- Privilege name mapping

**Access Control Module**: Security descriptors and ACLs
- ACL reading and modification
- Permission verification
- DACL and SACL management

**Services Module**: Windows service management
- Service enumeration and control
- Service status monitoring
- Start/stop operations

**Event Log Module**: Windows Event Log integration
- Event writing and reading
- Log filtering and searching
- Event type classification

**Registry Module**: Windows registry access (where applicable)
- Secure registry operations
- Registry value management
- Access control

## 📦 Dependencies

- **Hydrogen**: Core framework
- **Hydrogen.NET**: .NET framework utilities
- **System.ServiceProcess.ServiceController**: Service management (.NET built-in)
- **System.Diagnostics.EventLog**: Event logging (.NET built-in)
- **System.Security.AccessControl**: Security and ACL (.NET built-in)

## ⚠️ Permissions Required

- **Most operations**: Require **Administrator privileges**
- **Reading information**: Accessing local machine info doesn't require elevation
- **Creating/modifying users**: Requires elevation
- **Changing privileges**: Requires elevation
- **Event Log writing**: May require elevation depending on event source
- **Service management**: Requires elevation

## ⚠️ Best Practices

- **Run with minimum required privileges**: Only use admin when necessary
- **Handle dangling objects gracefully**: Expect deleted users/groups
- **Clean up ACLs**: Remove dangling references from access control lists
- **Use remote carefully**: Test thoroughly on remote machines first
- **Cache lookups**: Cache user/group lookups to avoid repeated queries
- **Error handling**: NT operations can fail if users/groups don't exist
- **Thread safety**: Create separate instances for concurrent access
- **Security**: Never log or expose sensitive security information

## ✅ Status & Compatibility

- **Maturity**: Production-tested for Windows system integration
- **.NET Target**: .NET 8.0+ (Windows), .NET Framework 4.7+ (legacy)
- **Platform**: Windows only (uses Windows-specific APIs)
- **Privileges**: Most operations require administrative elevation

## 📖 Related Projects

- [Hydrogen.NET](../Hydrogen.NET) - .NET framework utilities
- [Hydrogen.Windows.Forms](../Hydrogen.Windows.Forms) - Windows Forms UI framework
- [Hydrogen.Windows.LevelDB](../Hydrogen.Windows.LevelDB) - Windows LevelDB integration
- [Hydrogen.Application](../Hydrogen.Application) - Application framework

## ⚖️ License

Distributed under the **MIT NON-AI License**.

See the LICENSE file for full details. More information: [Sphere10 NON-AI-MIT License](https://sphere10.com/legal/NON-AI-MIT)

## 👤 Author

**Herman Schoenfeld** - Software Engineer

---

**Version**: 2.0+
