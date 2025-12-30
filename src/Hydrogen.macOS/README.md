<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 🍎 Hydrogen.macOS

**macOS platform abstraction** layer providing native macOS APIs, Xamarin.Mac integration, and Swift interoperability for building native desktop applications on Apple's macOS platform.

Hydrogen.macOS enables **cross-platform desktop development** with seamless integration to macOS native capabilities including file system, notifications, clipboard, menubar, dock, accessibility features, and system services while maintaining compatibility with Hydrogen framework abstractions.

## ⚡ 10-Second Example

```csharp
using Hydrogen.macOS;
using AppKit;

// Access system information
var systemInfo = macOSSystemInfo.Current;
Console.WriteLine($"OS Version: {systemInfo.OSVersion}");
Console.WriteLine($"Processor: {systemInfo.ProcessorCount} cores");

// Show notification
var notification = new macOSNotification {
    Title = "Hello macOS",
    Subtitle = "From Hydrogen Framework",
    Message = "This is a notification"
};
await notification.ShowAsync();

// Work with pasteboard
var pasteboard = macOSPasteboard.General;
await pasteboard.SetStringAsync("Hello from Hydrogen");
```

## 🏗️ Core Concepts

**Native API Wrappers**: Safe wrappers around AppKit, Foundation, and Cocoa frameworks.

**Window Management**: Titlebar customization, full-screen modes, and window positioning.

**Notification Support**: NSUserNotifications with actions and scheduling.

**File Operations**: File dialogs, recent files, file type associations.

**System Integration**: Dock icon management, menu bar apps, accessibility features.

## 🔧 Core Examples

### System Information & Capabilities

```csharp
using Hydrogen.macOS;
using Foundation;
using AppKit;

// Get system information
var systemInfo = macOSSystemInfo.Current;
Console.WriteLine($"OS Version: {systemInfo.OSVersion}");              // "12.6"
Console.WriteLine($"Architecture: {systemInfo.Architecture}");        // "arm64"
Console.WriteLine($"Processor Cores: {systemInfo.ProcessorCount}");   // 8
Console.WriteLine($"RAM: {systemInfo.TotalMemory} GB");               // "16"
Console.WriteLine($"Dark Mode: {systemInfo.IsDarkModeEnabled}");      // true

// Check system capabilities
bool supportsActivityKit = macOSCapabilities.SupportsActivityKit;
bool supportsWeatherKit = macOSCapabilities.SupportsWeatherKit;
bool supportsLiveActivities = macOSCapabilities.SupportsLiveActivities;

// Get user information
var userDefaults = macOSUserDefaults.Standard;
string userName = userDefaults.GetString("AppleSetupComplete");

// Monitor system theme changes
macOSAppearance.AppearanceChanged += (sender, isDarkMode) => {
    Console.WriteLine($"Theme changed to: {(isDarkMode ? "Dark" : "Light")}");
};
```

### File Dialogs & Operations

```csharp
using Hydrogen.macOS;
using AppKit;

// Open file dialog
var filePicker = new macOSFilePicker();
var selectedFile = await filePicker.PickFileAsync(new FilePickerOptions {
    Title = "Select a File",
    AllowedFileTypes = new[] { "pdf", "doc", "docx" },
    AllowMultiple = false
});

if (selectedFile != null) {
    Console.WriteLine($"Selected: {selectedFile.Path}");
}

// Save file dialog
var saveDialog = new macOSFilePicker();
var savePath = await saveDialog.PickSaveFileAsync(new SaveFileOptions {
    Title = "Save Document",
    FileName = "document",
    AllowedFileTypes = new[] { "txt", "pdf" }
});

if (savePath != null) {
    Console.WriteLine($"Will save to: {savePath}");
}

// Select folder
var folderPicker = new macOSFolderPicker();
var selectedFolder = await folderPicker.PickFolderAsync(new FolderPickerOptions {
    Title = "Select a Folder"
});

if (selectedFolder != null) {
    Console.WriteLine($"Selected folder: {selectedFolder.Path}");
}

// File operations
var fileManager = macOSFileManager.Default;

// Check if file exists
bool exists = fileManager.FileExists("~/Documents/document.txt");

// Get file size
long fileSize = fileManager.GetFileSize("~/Documents/image.png");
Console.WriteLine($"File size: {fileSize} bytes");

// Get creation date
var creationDate = fileManager.GetCreationDate("~/Documents/file.txt");
Console.WriteLine($"Created: {creationDate}");

// Get modification date
var modDate = fileManager.GetModificationDate("~/Documents/file.txt");
Console.WriteLine($"Modified: {modDate}");
```

### Notifications

```csharp
using Hydrogen.macOS;
using UserNotifications;

// Request notification permission
var permission = await macOSNotifications.RequestPermissionAsync();

if (permission == NotificationPermissionStatus.Granted) {
    // Simple notification
    var notification = new macOSNotification {
        Title = "Alert",
        Subtitle = "Notification Subtitle",
        Message = "This is the notification body text",
        SoundName = NSSound.NameGlass
    };
    
    await notification.ShowAsync();
    
    // Notification with action buttons
    var interactiveNotification = new macOSNotification {
        Title = "Action Required",
        Message = "Please review this item",
        Actions = new[] {
            new NotificationAction { Id = "accept", Title = "Accept" },
            new NotificationAction { Id = "decline", Title = "Decline" }
        },
        OnActionClicked = async (actionId) => {
            Console.WriteLine($"User selected: {actionId}");
            await HandleActionAsync(actionId);
        },
        CustomData = new Dictionary<string, string> {
            { "itemId", "item123" }
        }
    };
    
    await interactiveNotification.ShowAsync();
    
    // Scheduled notification
    var scheduledNotification = new macOSNotification {
        Title = "Reminder",
        Message = "Time for your break",
        ScheduleTime = DateTime.Now.AddMinutes(5),
        SoundName = NSSound.NameBubble
    };
    
    await scheduledNotification.ScheduleAsync();
}
```

### Pasteboard (Clipboard) Management

```csharp
using Hydrogen.macOS;
using AppKit;

// General pasteboard (system clipboard)
var pasteboard = macOSPasteboard.General;

// Set text to clipboard
await pasteboard.SetStringAsync("Hello macOS");

// Get text from clipboard
string clipboardText = await pasteboard.GetStringAsync();
Console.WriteLine($"Clipboard: {clipboardText}");

// Set multiple types to clipboard
var pasteboardItems = new Dictionary<string, object> {
    { NSPasteboard.StringType, "Plain text" },
    { NSPasteboard.RtfType, "<rtf>Formatted text</rtf>" }
};
await pasteboard.SetMultipleAsync(pasteboardItems);

// Check if clipboard has specific type
bool hasString = pasteboard.HasString;
bool hasColor = pasteboard.HasColor;
bool hasImage = pasteboard.HasImage;

Console.WriteLine($"Has text: {hasString}");
Console.WriteLine($"Has image: {hasImage}");

// Clear clipboard
await pasteboard.ClearAsync();
```

### Window Management

```csharp
using Hydrogen.macOS;
using AppKit;

// Get main window
var mainWindow = macOSWindowManager.MainWindow;

if (mainWindow != null) {
    Console.WriteLine($"Window Title: {mainWindow.Title}");
    Console.WriteLine($"Is Visible: {mainWindow.IsVisible}");
    Console.WriteLine($"Size: {mainWindow.Width}x{mainWindow.Height}");
}

// Create and manage windows
var windowManager = new macOSWindowManager();

// Create new window
var window = new macOSWindow {
    Title = "My Application",
    Width = 800,
    Height = 600,
    AllowsFullScreen = true
};

// Customize title bar
window.TitlebarAppearsTransparent = true;
window.ToolbarStyle = NSWindowToolbarStyle.Expanded;

// Add window to application
await windowManager.AddWindowAsync(window);

// Set window position
windowManager.PositionWindow(window, WindowPosition.Center);

// Show in full screen
await windowManager.EnterFullScreenAsync(window);

// Cycle through all windows
var allWindows = windowManager.GetAllWindows();
foreach (var w in allWindows) {
    Console.WriteLine($"Window: {w.Title}");
}
```

### Dock Integration

```csharp
using Hydrogen.macOS;
using AppKit;

// Get dock menu
var dockMenu = macOSDock.GetDockMenu();

// Add custom menu items to dock
var menuItem = new NSMenuItem {
    Title = "Custom Action",
    Action = new ObjCRuntime.Selector("customAction:")
};

dockMenu.AddItem(menuItem);

// Set dock icon
var image = NSImage.FromFile("app-icon.png");
await macOSDock.SetIconAsync(image);

// Update dock badge
await macOSDock.SetBadgeAsync("99+");
await macOSDock.ClearBadgeAsync();

// Add window preview to dock
await macOSDock.RegisterWindowPreviewAsync(mainWindow);

// Open files with app (for dragged items)
macOSDock.RegisterFileTypesHandler((files) => {
    foreach (var file in files) {
        Console.WriteLine($"Opened: {file}");
        HandleOpenedFile(file);
    }
});
```

### Menu Bar Applications

```csharp
using Hydrogen.macOS;
using AppKit;

// Create status bar item
var statusBar = NSStatusBar.System;
var statusItem = statusBar.CreateStatusItem(NSStatusItemLength.Variable);

statusItem.Button.Title = "◉";
statusItem.ToolTip = "My App";

// Create menu
var menu = new NSMenu();
menu.AddItem(new NSMenuItem("Show Main Window") {
    Action = new ObjCRuntime.Selector("showWindow:")
});
menu.AddItem(NSMenuItem.SeparatorItem);
menu.AddItem(new NSMenuItem("Settings...") {
    Action = new ObjCRuntime.Selector("openSettings:")
});
menu.AddItem(NSMenuItem.SeparatorItem);
menu.AddItem(new NSMenuItem("Quit") {
    Action = new ObjCRuntime.Selector("terminate:")
});

statusItem.Menu = menu;

// Hide main window from dock
NSApplication.SharedApplication.ActivationPolicy = 
    NSApplicationActivationPolicy.Accessory;
```

### System Services & Gestures

```csharp
using Hydrogen.macOS;
using AppKit;

// Access system services
var services = NSApplication.SharedApplication.ServicesProvider;

// Touch events (trackpad)
macOSGestures.SwipeLeft += () => {
    Console.WriteLine("Swiped left");
};

macOSGestures.SwipeRight += () => {
    Console.WriteLine("Swiped right");
};

macOSGestures.Pinch += (zoom) => {
    Console.WriteLine($"Pinched: {zoom}");
};

// Keyboard shortcuts
macOSKeyboard.RegisterGlobalHotkey(
    modifiers: NSEventModifierMask.CommandKeyMask,
    keyCode: (ushort)'M',  // Cmd+M
    handler: () => {
        Console.WriteLine("Global hotkey triggered");
    });

// Monitor screen changes
macOSScreen.ScreensChanged += () => {
    var screens = macOSScreen.GetAllScreens();
    Console.WriteLine($"Available screens: {screens.Count}");
};
```

## 🏗️ Architecture

**Native API Wrappers**: Thin wrappers around AppKit, Foundation, and Cocoa frameworks.

**Window Management**: Centralized window lifecycle and positioning management.

**Notification Framework**: NSUserNotification integration with actions and scheduling.

**File System Integration**: File dialogs, operations, and type associations.

**System Integration**: Dock, menu bar, and global hotkey support.

## 📋 Best Practices

- **Appearance support**: Handle both light and dark mode appropriately
- **Keyboard shortcuts**: Use standard macOS keyboard shortcuts (Cmd instead of Ctrl)
- **File handling**: Use file dialogs for all file access; respect sandbox restrictions
- **Menu design**: Follow macOS Human Interface Guidelines for menu organization
- **Dock icon**: Provide custom dock icon for application branding
- **Performance**: Run long operations on background threads
- **Accessibility**: Support VoiceOver and keyboard navigation
- **Permissions**: Request microphone, camera, or location permissions when needed

## 📊 Status & Compatibility

- **Version**: 2.0+
- **Framework**: Xamarin.Mac / .NET for macOS
- **macOS Versions**: 10.13 (High Sierra) - 13+ (Ventura)
- **Architectures**: Intel (x86_64), Apple Silicon (arm64)
- **Performance**: Native performance via Xamarin.Mac bridge

## 📦 Dependencies

- **Xamarin.Mac**: macOS platform binding
- **AppKit**: Apple's graphical framework
- **Foundation**: Core system framework
- **.NET 6.0+** or **Mono** runtime

## 📚 Related Projects

- [Hydrogen.iOS](../Hydrogen.iOS) - iOS platform implementation
- [Hydrogen.Android](../Hydrogen.Android) - Android platform implementation
- [Hydrogen.Windows](../Hydrogen.Windows) - Windows desktop implementation
- [Hydrogen.Application](../Hydrogen.Application) - Cross-platform application abstraction
- [Hydrogen.Tests](../../tests/Hydrogen.Tests) - Test patterns and examples

## 📄 License & Author

**License**: [Refer to repository LICENSE](../../LICENSE)  
**Author**: Herman Schoenfeld, Sphere 10 Software (sphere10.com)  
**Copyright**: © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.
