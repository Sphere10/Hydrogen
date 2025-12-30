# ⚙️ Settings

**Settings management library** providing simple, type-safe, encrypted configuration storage for application and user settings.

## 💫 Declare your settings

Settings objects inherit `SettingsObject` and have properties. Default values can be specified by `DefaultValueAttribute` or directly with assignment. Use `AppSettingAttribute` to link to `app.config` or `web.config`:

```csharp

	public class DatabaseSettings : SettingsObject {

		[AppSetting("DatabaseServer")]   // this looks up DatabaseServer from app.config or web.config to use as default value 
		public string Server { get; set; } = "localhost"

		[AppSetting("DatabaseServer")]
		[DefaultValue("google.com")]						
		public string OtherServer { get; set; }

		[Encrypted]
		public string Password { get; set; }

		[AppSetting("DatabaseName")]
		public string DatabsaeName { get; set; }

	}

	public class ScreenSizeSettings : SettingsObject {

		[DefaultValue(100)]
		public int Width { get; set; }

		public int Height { get; set; } = 100;

	}
```

## 📄 Load your settings

On first load, default values are used. On subsequent loads, persisted values are retrieved. `UserSettings` are per-user; `GlobalSettings` are system-wide. Multiple instances can be differentiated by string `ID`:

```csharp
	var mainScreenSettings = UserSettings.Get<ScreenSizeSettings>("MainScreen");  
	var aboutScreenSettings = UserSettings.Get<ScreenSizeSettings>("AboutScreen");  
	var database = GlobalSettings.Get<DatabaseSettings>();
```

## 📑 Persist your settings

Modify settings and save them back to persistent storage:

```csharp
    screenSettings.Width = 150;
	screenSettings.Save();
```

Next time you load them (via the same provider you retrieved them from), their updated values will be returned.

## 🔐 Encryption

Declare the product secret in your top-level assembly-info to enable encryption:

```csharp
[assembly: AssemblyProductSecret("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
```

This is used to encrypt settings. Only properties marked with "EncryptedAttribute"" will be encrypted.


## 🔧 Implementing your own settings provider

```csharp
	public class MyCustomSettingsProvider : ISettingsProvider {

	 ...
    }
```

## ⚡ Changing default `UserSettings` and `GlobalSettings` providers

Add a `ModuleConfiguration` to register custom providers:

```csharp

public class ModuleConfiguration : ModuleConfigurationBase {
		public override void RegisterComponents(ComponentRegistry registry) {
			registry.RegisterComponentInstance<ISettingsProvider>(new CachedSettingsProvider( new MyCustomSettingsProvider())), "UserSettings") ;
			registry.RegisterComponentInstance<ISettingsProvider>(new CachedSettingsProvider( new MyCustomSettingsProvider())), "SystemSettings") ;

			// note: use CachedSettingsProvider optimizes frequent settings access, omit if you want your custom provider to control every access
			// note: ensure that User/System settings are treated differently and never persited in same place
		}
    }
}

```