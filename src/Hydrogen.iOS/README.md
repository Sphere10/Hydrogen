<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 📱 Hydrogen.iOS

**iOS platform abstraction** layer providing native iOS APIs, Xamarin.iOS integration, and Swift interoperability for building native and hybrid mobile applications on Apple's iOS platform.

Hydrogen.iOS enables **cross-platform mobile development** with seamless integration to iOS native capabilities including camera, location, notifications, background tasks, and HomeKit support while maintaining compatibility with Hydrogen framework abstractions.

## ⚡ 10-Second Example

```csharp
using Hydrogen.iOS;
using UIKit;

// Access device capabilities
var deviceInfo = iOSDeviceInfo.Current;
Console.WriteLine($"Device: {deviceInfo.Model}");
Console.WriteLine($"OS Version: {deviceInfo.SystemVersion}");

// Request camera permission
var cameraAccess = await iOSPermissions.RequestCameraAccessAsync();

// Send local notification
var notification = new iOSLocalNotification {
    Title = "Hello iOS",
    Body = "From Hydrogen Framework",
    DelaySeconds = 5
};
await notification.ScheduleAsync();
```

## 🏗️ Core Concepts

**Native API Wrappers**: Safe wrappers around iOS native APIs for UIKit, CoreLocation, AVFoundation, etc.

**Permission Management**: Declarative permission handling with user prompts and graceful fallbacks.

**Device Capabilities**: Detection and access to device features (camera, GPS, microphone, etc.).

**Notification Support**: Local and remote push notifications with scheduling and categorization.

**Background Task Management**: iOS background modes, app refresh, and silent notifications.

## 🔧 Core Examples

### Device Information & Capabilities

```csharp
using Hydrogen.iOS;
using UIKit;

// Get device information
var deviceInfo = iOSDeviceInfo.Current;
Console.WriteLine($"Model: {deviceInfo.Model}");                    // "iPhone14Pro"
Console.WriteLine($"OS Version: {deviceInfo.SystemVersion}");       // "16.0"
Console.WriteLine($"Device ID: {deviceInfo.UniqueIdentifier}");     // UDID
Console.WriteLine($"Uptime: {deviceInfo.Uptime}");                 // TimeSpan

// Check device capabilities
bool hasFaceID = iOSDeviceCapabilities.HasFaceID;
bool hasGPS = iOSDeviceCapabilities.HasGPS;
bool hasCamera = iOSDeviceCapabilities.HasCamera;
bool has5G = iOSDeviceCapabilities.Has5GConnectivity;

// Get storage information
var storage = await iOSStorageInfo.GetStorageStatsAsync();
Console.WriteLine($"Total: {storage.TotalBytes} bytes");
Console.WriteLine($"Available: {storage.AvailableBytes} bytes");
Console.WriteLine($"Used: {storage.UsedBytes} bytes");
```

### Permission Request Patterns

```csharp
using Hydrogen.iOS;
using CoreLocation;
using AVFoundation;

// Camera permission
var cameraStatus = await iOSPermissions.RequestCameraAccessAsync();
switch (cameraStatus) {
    case PermissionStatus.Granted:
        Console.WriteLine("Camera access granted");
        // Use camera
        break;
    case PermissionStatus.Denied:
        Console.WriteLine("User denied camera access");
        break;
    case PermissionStatus.Restricted:
        Console.WriteLine("Camera access restricted by policy");
        break;
}

// Location permission (precise)
var locationStatus = await iOSPermissions.RequestLocationAccessAsync(
    LocationAccuracy.Full);

// Location permission (approximate)
var approxLocationStatus = await iOSPermissions.RequestLocationAccessAsync(
    LocationAccuracy.Approximate);

// Microphone permission
var micStatus = await iOSPermissions.RequestMicrophoneAccessAsync();

// Calendar/Contacts permission
var calendarStatus = await iOSPermissions.RequestCalendarAccessAsync();
var contactsStatus = await iOSPermissions.RequestContactsAccessAsync();

// Health/HealthKit permission
var healthStatus = await iOSPermissions.RequestHealthKitAccessAsync();

// Check current permission status without requesting
var photoLibStatus = iOSPermissions.GetPhotoLibraryPermissionStatus();
Console.WriteLine($"Photo Library Permission: {photoLibStatus}");
```

### Local Notifications

```csharp
using Hydrogen.iOS;
using UserNotifications;

// Simple notification
var notification = new iOSLocalNotification {
    Title = "Reminder",
    Body = "You have a meeting in 15 minutes",
    DelaySeconds = 60,
    SoundName = UNNotificationSoundName.Default
};

// Schedule notification
var notificationId = await notification.ScheduleAsync();
Console.WriteLine($"Scheduled notification: {notificationId}");

// Scheduled notification with repeat
var dailyNotification = new iOSLocalNotification {
    Title = "Daily Task",
    Body = "Complete your daily task",
    ScheduleTime = DateTime.Today.AddHours(9),  // 9 AM
    Repeats = true,
    RepeatInterval = NotificationRepeatInterval.Daily,
    Badge = 1
};

await dailyNotification.ScheduleAsync();

// Notification with custom sound and action buttons
var interactiveNotification = new iOSLocalNotification {
    Title = "Message Received",
    Body = "You have a new message",
    Badge = 1,
    Sound = "notification.wav",
    CustomData = new Dictionary<string, object> {
        { "conversationId", "conv123" },
        { "senderId", "user456" }
    },
    Actions = new[] {
        new NotificationAction { Id = "reply", Title = "Reply", IsDestructive = false },
        new NotificationAction { Id = "ignore", Title = "Ignore", IsDestructive = true }
    }
};

var actionId = await interactiveNotification.ScheduleAsync();

// Get pending notifications
var pending = await iOSLocalNotification.GetPendingNotificationsAsync();
Console.WriteLine($"Pending notifications: {pending.Count}");

// Cancel notification
await iOSLocalNotification.CancelNotificationAsync(notificationId);
```

### Camera & Photo Access

```csharp
using Hydrogen.iOS;
using AVFoundation;
using Photos;

// Initialize camera
var camera = new iOSCamera();

// Check camera availability
if (!camera.IsFrontCameraAvailable) {
    Console.WriteLine("Front camera not available");
    return;
}

// Capture photo
var photo = await camera.CapturePhotoAsync();
if (photo != null) {
    var imageData = photo.ToBytesArray();
    Console.WriteLine($"Photo captured: {imageData.Length} bytes");
}

// Record video
var recording = camera.StartVideoRecording(new iOSVideoRecordingOptions {
    Quality = AVCaptureSessionPreset.High,
    MaxDuration = TimeSpan.FromMinutes(5),
    SoundEnabled = true
});

await Task.Delay(TimeSpan.FromSeconds(10));  // Record for 10 seconds
var videoUrl = await camera.StopVideoRecordingAsync();
Console.WriteLine($"Video saved to: {videoUrl}");

// Access photo library
var photoPicker = new iOSPhotoPicker();
var selectedPhoto = await photoPicker.PickPhotoAsync();

if (selectedPhoto != null) {
    Console.WriteLine($"Selected photo: {selectedPhoto.Filename}");
    var imageData = await selectedPhoto.LoadImageDataAsync();
}

// Save photo to gallery
var image = new UIImage("photo.jpg");
await iOSPhotos.SavePhotoAsync(image);
```

### Location Services

```csharp
using Hydrogen.iOS;
using CoreLocation;

// Initialize location manager
var locationManager = new iOSLocationManager();

// Request permission first
var status = await iOSPermissions.RequestLocationAccessAsync(
    LocationAccuracy.Full);

if (status == PermissionStatus.Granted) {
    // Get current location
    var location = await locationManager.GetCurrentLocationAsync();
    Console.WriteLine($"Latitude: {location.Latitude}");
    Console.WriteLine($"Longitude: {location.Longitude}");
    Console.WriteLine($"Accuracy: {location.HorizontalAccuracy}");
    
    // Start continuous location tracking
    locationManager.LocationUpdated += (sender, e) => {
        Console.WriteLine($"New location: {e.Location.Latitude}, {e.Location.Longitude}");
    };
    
    locationManager.StartUpdatingLocation(
        desiredAccuracy: LocationAccuracy.Best,
        distanceFilter: 10);  // Update every 10 meters
    
    // Reverse geocode (get address from coordinates)
    var address = await locationManager.ReverseGeocodeAsync(
        location.Latitude, location.Longitude);
    Console.WriteLine($"Address: {address}");
}
```

### Background Task Execution

```csharp
using Hydrogen.iOS;
using BackgroundTasks;

// Register background task (in AppDelegate)
BGTaskScheduler.Shared.Register(
    forTaskWithIdentifier: "com.sphere10.hydrogen.refresh",
    using: null,
    launchHandler: ProcessBackgroundTask);

// Schedule background refresh
var request = new BGAppRefreshTaskRequest("com.sphere10.hydrogen.refresh");
request.EarliestBeginDate = NSDate.FromTimeIntervalSinceNow(60);  // In 60 seconds

try {
    BGTaskScheduler.Shared.Submit(request, out NSError error);
    if (error != null) {
        Console.WriteLine($"Failed to schedule task: {error}");
    }
} catch (Exception ex) {
    Console.WriteLine($"Error scheduling background task: {ex}");
}

// Process background task
private void ProcessBackgroundTask(BGTask task) {
    Console.WriteLine("Processing background task");
    
    // Set expiration handler
    task.ExpirationHandler = () => {
        Console.WriteLine("Background task expired");
    };
    
    // Do background work
    PerformBackgroundWork();
    
    // Mark complete
    task.SetTaskCompleted(success: true);
    
    // Reschedule if needed
    ScheduleNextBackgroundTask();
}

private void PerformBackgroundWork() {
    // Sync data, update cache, etc.
    Console.WriteLine("Syncing data in background");
}

private void ScheduleNextBackgroundTask() {
    var nextRequest = new BGAppRefreshTaskRequest("com.sphere10.hydrogen.refresh");
    nextRequest.EarliestBeginDate = NSDate.FromTimeIntervalSinceNow(900);  // 15 minutes
    
    try {
        BGTaskScheduler.Shared.Submit(nextRequest, out NSError error);
    } catch { }
}
```

## 🏗️ Architecture

**Native API Wrappers**: Thin wrappers around UIKit, CoreLocation, AVFoundation, etc.

**Permission Abstraction**: Unified permission request interface across iOS versions.

**Device Capability Detection**: Safe capability checks with graceful degradation.

**Notification Management**: High-level notification scheduling and handling.

**Background Task Integration**: Simplified background execution and app refresh scheduling.

## 📋 Best Practices

- **Permission hygiene**: Request permissions only when needed, with clear user explanation
- **Capability checking**: Always check device capabilities before attempting to use them
- **Background awareness**: Respect background task time limits and set expiration handlers
- **Memory management**: Dispose of large resources when backgrounded
- **iOS version support**: Use availability attributes for iOS version-specific APIs
- **Location privacy**: Minimize location tracking scope; use approximate when possible
- **Notification scheduling**: Use repeating notifications judiciously to avoid battery drain
- **Camera resources**: Properly release camera and video recording resources

## 📊 Status & Compatibility

- **Version**: 2.0+
- **Framework**: Xamarin.iOS / .NET for iOS
- **iOS Versions**: iOS 13.0+ (13.4+ recommended)
- **Target Devices**: iPhone, iPad, iPod Touch
- **Performance**: Native performance via Xamarin.iOS bridge

## 📦 Dependencies

- **Xamarin.iOS**: iOS platform binding
- **.NET 6.0+** or **Mono** runtime
- **iOS SDK 13.0+**
- **Platform-specific frameworks**: UIKit, CoreLocation, AVFoundation, etc.

## 📚 Related Projects

- [Hydrogen.Android](../Hydrogen.Android) - Android platform implementation
- [Hydrogen.macOS](../Hydrogen.macOS) - macOS platform implementation
- [Hydrogen.Windows](../Hydrogen.Windows) - Windows desktop implementation
- [Hydrogen.Application](../Hydrogen.Application) - Cross-platform application abstraction
- [Hydrogen.Tests](../../tests/Hydrogen.Tests) - Test patterns and examples

## 📄 License & Author

**License**: [Refer to repository LICENSE](../../LICENSE)  
**Author**: Herman Schoenfeld, Sphere 10 Software (sphere10.com)  
**Copyright**: © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.
