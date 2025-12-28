# Settings

This library provides a simple way to define and persist user and system settings. Settings can be stored per-user or globally and can optionally be encrypted.

## Declare settings

Settings objects inherit `SettingsObject` and expose properties. Default values can be specified with `DefaultValueAttribute` or by assigning a value. To use an app setting from `app.config` or `web.config` as a default value, add `AppSettingAttribute` with the key name.

```csharp
public class DatabaseSettings : SettingsObject {

    [AppSetting("DatabaseServer")]
    public string Server { get; set; } = "localhost";

    [AppSetting("DatabaseServer")]
    [DefaultValue("google.com")]
    public string OtherServer { get; set; }

    [Encrypted]
    public string Password { get; set; }

    [AppSetting("DatabaseName")]
    public string DatabaseName { get; set; }
}

public class ScreenSizeSettings : SettingsObject {

    [DefaultValue(100)]
    public int Width { get; set; }

    public int Height { get; set; } = 100;
}
```

## Load settings

On first load, default values are used. On subsequent loads, values are read from the persisted store. `UserSettings` are visible only to the current user; `GlobalSettings` are shared across all users. Multiple instances of a settings type can be differentiated by an `ID`.

```csharp
var mainScreen = UserSettings.Get<ScreenSizeSettings>("MainScreen");
var aboutScreen = UserSettings.Get<ScreenSizeSettings>("AboutScreen");
var database = GlobalSettings.Get<DatabaseSettings>();
```

## Persist settings

```csharp
mainScreen.Width = 150;
mainScreen.Save();
```

## Encryption

Declare an assembly-level secret used to encrypt properties marked with `EncryptedAttribute`.

```csharp
[assembly: AssemblyProductSecret("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
```

## Custom settings providers

```csharp
public class MyCustomSettingsProvider : ISettingsProvider {
    // ...
}
```

## Changing default providers

Register custom providers in a module configuration.

```csharp
public class ModuleConfiguration : ModuleConfigurationBase {
    public override void RegisterComponents(ComponentRegistry registry) {
        registry.RegisterComponentInstance<ISettingsProvider>(
            new CachedSettingsProvider(new MyCustomSettingsProvider()),
            "UserSettings");

        registry.RegisterComponentInstance<ISettingsProvider>(
            new CachedSettingsProvider(new MyCustomSettingsProvider()),
            "SystemSettings");

        // Use CachedSettingsProvider to optimize frequent access.
        // Ensure User/System settings are stored separately.
    }
}
```
