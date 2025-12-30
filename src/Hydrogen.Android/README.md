<!-- Copyright (c) 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved. Author: Herman Schoenfeld (sphere10.com) -->

# 🤖 Hydrogen.Android

**Android platform abstraction** layer providing native Android APIs, Xamarin.Android integration, and Java interoperability for building native and hybrid mobile applications on Android platform.

Hydrogen.Android enables **cross-platform mobile development** with seamless integration to Android native capabilities including camera, sensors, location, notifications, background services, and hardware features while maintaining compatibility with Hydrogen framework abstractions.

## ⚡ 10-Second Example

```csharp
using Hydrogen.Android;
using Android.App;
using Android.Content;

// Access device information
var deviceInfo = AndroidDeviceInfo.Current;
Console.WriteLine($"Device: {deviceInfo.Model}");
Console.WriteLine($"Android Version: {deviceInfo.AndroidVersion}");

// Request camera permission
var cameraAccess = await AndroidPermissions.RequestCameraAccessAsync();

// Send local notification
var notification = new AndroidNotification {
    Title = "Hello Android",
    Message = "From Hydrogen Framework",
    DelaySeconds = 5
};
await notification.ShowAsync();
```

## 🏗️ Core Concepts

**Native API Wrappers**: Safe wrappers around Android Framework APIs for Activities, Services, Intents, etc.

**Permission Management**: Declarative permission handling with runtime permission requests and manifests.

**Sensor Integration**: Access to accelerometer, gyroscope, magnetometer, proximity, light sensors.

**Background Services**: Long-running background tasks, foreground services, job scheduling.

**Hardware Features**: Vibration, LED, camera, microphone, GPS access with proper lifecycle management.

## 🔧 Core Examples

### Device Information & Sensors

```csharp
using Hydrogen.Android;
using Android.Hardware;

// Get device information
var deviceInfo = AndroidDeviceInfo.Current;
Console.WriteLine($"Model: {deviceInfo.Model}");                    // "Pixel 6"
Console.WriteLine($"Manufacturer: {deviceInfo.Manufacturer}");      // "Google"
Console.WriteLine($"Android Version: {deviceInfo.AndroidVersion}"); // "12"
Console.WriteLine($"Device ID: {deviceInfo.UniqueDeviceId}");      // Unique identifier

// Check device capabilities
bool hasGPS = AndroidDeviceCapabilities.HasGPS;
bool hasCamera = AndroidDeviceCapabilities.HasCamera;
bool hasAccelerometer = AndroidDeviceCapabilities.HasAccelerometer;
bool hasNFC = AndroidDeviceCapabilities.HasNFC;
bool hasBluetooth = AndroidDeviceCapabilities.HasBluetooth;

// Get battery information
var batteryInfo = await AndroidBattery.GetBatteryInfoAsync();
Console.WriteLine($"Battery Level: {batteryInfo.Level}%");
Console.WriteLine($"Is Charging: {batteryInfo.IsCharging}");
Console.WriteLine($"Charge Status: {batteryInfo.ChargingStatus}");

// Get storage information
var storageInfo = AndroidStorage.GetStorageInfo();
Console.WriteLine($"Total Storage: {storageInfo.TotalBytes} bytes");
Console.WriteLine($"Available: {storageInfo.AvailableBytes} bytes");

// Access accelerometer
var accelerometer = new AndroidSensor(SensorType.Accelerometer);
accelerometer.SensorValueChanged += (x, y, z, accuracy) => {
    Console.WriteLine($"Acceleration: X={x}, Y={y}, Z={z}");
};
accelerometer.Start(SensorDelay.Normal);
```

### Runtime Permission Handling

```csharp
using Hydrogen.Android;
using Android.Manifest;

// Camera permission
var cameraStatus = await AndroidPermissions.RequestPermissionAsync(
    Permission.Camera);

switch (cameraStatus) {
    case PermissionStatus.Granted:
        Console.WriteLine("Camera permission granted");
        break;
    case PermissionStatus.Denied:
        Console.WriteLine("User denied camera permission");
        break;
    case PermissionStatus.Restricted:
        Console.WriteLine("Permission restricted by policy");
        break;
}

// Multiple permissions at once
var statuses = await AndroidPermissions.RequestPermissionsAsync(new[] {
    Permission.Camera,
    Permission.RecordAudio,
    Permission.AccessFineLocation
});

foreach (var status in statuses) {
    Console.WriteLine($"{status.Key}: {status.Value}");
}

// Location permission with optional justification
var locationRationale = await AndroidPermissions.RequestPermissionAsync(
    Permission.AccessFineLocation,
    rationale: "Location is needed to show nearby services");

// Check permission without requesting
var photoStatus = AndroidPermissions.GetPermissionStatus(Permission.ReadExternalStorage);
if (photoStatus != PermissionStatus.Granted) {
    Console.WriteLine("Photo access not granted");
}

// Storage permission (varies by Android version)
var storageStatus = await AndroidPermissions.RequestStoragePermissionAsync();
```

### Local Notifications

```csharp
using Hydrogen.Android;
using Android.App;

// Simple notification
var notification = new AndroidNotification {
    Title = "Reminder",
    Message = "You have a meeting in 15 minutes",
    ChannelId = "reminders",
    DelaySeconds = 60,
    Priority = NotificationPriority.High
};

var notificationId = await notification.ShowAsync();
Console.WriteLine($"Notification shown: {notificationId}");

// Notification with action buttons
var actionNotification = new AndroidNotification {
    Title = "Message",
    Message = "New message received",
    ChannelId = "messages",
    Priority = NotificationPriority.High,
    Actions = new[] {
        new NotificationAction { Id = "reply", Title = "Reply" },
        new NotificationAction { Id = "mark_read", Title = "Mark as Read" }
    },
    OnActionClicked = async (actionId) => {
        Console.WriteLine($"User clicked: {actionId}");
        await HandleNotificationActionAsync(actionId);
    }
};

await actionNotification.ShowAsync();

// Scheduled notification (repeating)
var scheduledNotification = new AndroidNotification {
    Title = "Daily Task",
    Message = "Complete your daily task",
    ChannelId = "daily_tasks",
    ScheduledTime = DateTime.Today.AddHours(9),  // 9 AM
    Repeats = true,
    RepeatInterval = NotificationRepeatInterval.Daily
};

await scheduledNotification.ScheduleAsync();

// Create notification channel (Android 8.0+)
var channel = new NotificationChannel {
    Id = "important",
    Name = "Important Notifications",
    Description = "Notifications that require immediate attention",
    Importance = NotificationImportance.High,
    EnableVibration = true,
    EnableLights = true
};

AndroidNotification.CreateChannel(channel);

// Cancel notification
await AndroidNotification.CancelNotificationAsync(notificationId);
```

### Camera & Media Capture

```csharp
using Hydrogen.Android;
using Android.Provider;

// Initialize camera
var camera = new AndroidCamera();

// Check camera availability
int cameraCount = camera.CameraCount;
Console.WriteLine($"Available cameras: {cameraCount}");

// Capture photo
var photo = await camera.TakePictureAsync(new PhotoCaptureOptions {
    Quality = PhotoQuality.High,
    SaveToGallery = true,
    FlashMode = FlashMode.Auto
});

if (photo != null) {
    Console.WriteLine($"Photo saved to: {photo.FilePath}");
    Console.WriteLine($"Size: {photo.Width}x{photo.Height}");
}

// Record video
var recording = camera.StartVideoRecording(new VideoRecordingOptions {
    Quality = VideoQuality.High,
    MaxDuration = TimeSpan.FromMinutes(5),
    EnableAudio = true,
    SaveToGallery = true
});

await Task.Delay(TimeSpan.FromSeconds(10));  // Record 10 seconds
var videoPath = await camera.StopVideoRecordingAsync();
Console.WriteLine($"Video saved to: {videoPath}");

// Pick media from gallery
var mediaPicker = new AndroidMediaPicker();
var media = await mediaPicker.PickMediaAsync(new MediaPickerOptions {
    AllowMultiple = true,
    MediaTypes = new[] { MediaType.Photo, MediaType.Video }
});

foreach (var item in media) {
    Console.WriteLine($"Selected: {item.Filename}");
}
```

### Location Services

```csharp
using Hydrogen.Android;
using Android.Locations;

// Request location permission first
var status = await AndroidPermissions.RequestPermissionAsync(
    Permission.AccessFineLocation);

if (status == PermissionStatus.Granted) {
    var locationManager = new AndroidLocationManager();
    
    // Get current location
    var location = await locationManager.GetCurrentLocationAsync();
    Console.WriteLine($"Latitude: {location.Latitude}");
    Console.WriteLine($"Longitude: {location.Longitude}");
    Console.WriteLine($"Accuracy: {location.Accuracy}");
    
    // Start location updates
    locationManager.LocationUpdated += (sender, e) => {
        Console.WriteLine($"Location: {e.Latitude}, {e.Longitude}");
    };
    
    locationManager.StartLocationUpdates(
        provider: LocationManager.GpsProvider,
        minTimeMs: 5000,           // Update every 5 seconds
        minDistanceM: 10);         // Or every 10 meters
    
    // Reverse geocode
    var address = await locationManager.GetAddressAsync(
        location.Latitude, location.Longitude);
    
    Console.WriteLine($"Address: {address.GetAddressLine(0)}");
    Console.WriteLine($"City: {address.Locality}");
    
    // Cleanup
    locationManager.StopLocationUpdates();
}
```

### Background Services & Job Scheduling

```csharp
using Hydrogen.Android;
using Android.App;
using AndroidX.Work;

// Create background service
public class SyncService : AndroidForegroundService {
    public override void OnStartCommand(Intent intent, int flags, int startId) {
        Console.WriteLine("Sync service started");
        
        // Create foreground notification
        var notification = new Notification.Builder(this)
            .SetContentTitle("Syncing Data")
            .SetContentText("Background sync in progress")
            .SetSmallIcon(Resource.Drawable.ic_launcher)
            .Build();
        
        StartForeground(1, notification);
        
        // Do background work
        Task.Run(() => PerformSync());
    }
    
    private async Task PerformSync() {
        try {
            Console.WriteLine("Syncing user data...");
            await Task.Delay(5000);  // Simulate sync
            Console.WriteLine("Sync complete");
        } catch (Exception ex) {
            Console.WriteLine($"Sync error: {ex}");
        }
    }
}

// Schedule background job
var workRequest = new PeriodicWorkRequest.Builder(
    typeof(SyncWorker),
    interval: TimeSpan.FromHours(24))
    .AddTag("data-sync")
    .SetBackoffCriteria(
        policy: BackoffPolicy.Exponential,
        initialDelay: TimeSpan.FromMinutes(1))
    .Build();

WorkManager.GetInstance(context).EnqueueUniquePeriodicWork(
    "daily-sync",
    ExistingPeriodicWorkPolicy.KeepExistingWork,
    workRequest);

// Define the worker
public class SyncWorker : Worker {
    public SyncWorker(Context context, WorkerParameters parameters)
        : base(context, parameters) { }
    
    public override Result DoWork() {
        try {
            Console.WriteLine("Worker: Starting sync");
            // Perform sync
            return Result.Success();
        } catch {
            return Result.Retry();
        }
    }
}
```

## 🏗️ Architecture

**Native API Wrappers**: Thin wrappers around Android Framework and AndroidX components.

**Permission Abstraction**: Unified permission request interface across Android versions (API 21+).

**Sensor Integration**: Standardized access to device sensors with lifecycle management.

**Service Management**: Background services, foreground services, and job scheduling.

**Notification Framework**: Channels, actions, and delivery across Android versions.

## 📋 Best Practices

- **Runtime permissions**: Request permissions when needed, not at startup
- **Manifest requirements**: Declare all required permissions in AndroidManifest.xml
- **Sensor lifecycle**: Stop sensors when app pauses to conserve battery
- **Background limits**: Respect Doze mode and Background Execution Limits (Android 6.0+)
- **Notification channels**: Create appropriate channels for different notification types (Android 8.0+)
- **Memory management**: Clean up resources in OnDestroy and OnStop
- **API levels**: Use version checks for features available in newer Android versions
- **Battery awareness**: Use battery optimization APIs to minimize impact

## 📊 Status & Compatibility

- **Version**: 2.0+
- **Framework**: Xamarin.Android / .NET for Android
- **Android Versions**: API 21 (Android 5.0) - API 34 (Android 14)
- **Target Devices**: Phones, tablets, wearables
- **Performance**: Native performance via Xamarin.Android bridge

## 📦 Dependencies

- **Xamarin.Android**: Android platform binding
- **AndroidX**: Modern Android support libraries
- **.NET 6.0+** or **Mono** runtime
- **Android SDK 21+**

## 📚 Related Projects

- [Hydrogen.iOS](../Hydrogen.iOS) - iOS platform implementation
- [Hydrogen.macOS](../Hydrogen.macOS) - macOS platform implementation
- [Hydrogen.Windows](../Hydrogen.Windows) - Windows desktop implementation
- [Hydrogen.Application](../Hydrogen.Application) - Cross-platform application abstraction
- [Hydrogen.Tests](../../tests/Hydrogen.Tests) - Test patterns and examples

## 📄 License & Author

**License**: [Refer to repository LICENSE](../../LICENSE)  
**Author**: Herman Schoenfeld, Sphere 10 Software (sphere10.com)  
**Copyright**: © 2018-Present Herman Schoenfeld & Sphere 10 Software. All rights reserved.
