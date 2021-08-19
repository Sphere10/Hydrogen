# Settings

Settings library allow a simple way for application to make use of user and system configurable settings which can be modified. 

## Declare your settings

Settings objects inherit `SettingsObject` and have properties. Default values of properties can be specified by `DefaultValueAttribute` or directly with assignment. To use an "AppSetting" from your `app.config` or `web.config` as a default value for a property, declare an `AppSettingAttribute` over that property with the name of the app setting.

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

## Load your settings

On first time load, the default values are used. Every other time, they are loaded from saved value. `UserSettings` are visible only to the (roaming) user whereas `GlobalSettings` are visible to all system users. Multiple instances of a settings can be differentiated by a string `ID`.

```csharp
	var mainScreenSettings = UserSettings.Get<ScreenSizeSettings>("MainScreen");  
	var aboutScreenSettings = UserSettings.Get<ScreenSizeSettings>("AboutScreen");  
	var database = GlobalSettings.Get<DatabaseSettings>();
```

## Persist your settings 

After loading your settings, you can modify them and save them.

```csharp
    screenSettings.Width = 150;
	screenSettings.Save();
```

Next time you load them (via the same provider you retrieved them from), their updated values will be returned.

## Encryption

In your top-level assembly-info, declare the password your product can use

```csharp
[assembly: AssemblyProductSecret("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
```

This is used to encrypt settings. Only properties marked with "EncryptedAttribute"" will be encrypted.


## Implementing your own settings provider

```csharp
	public class MyCustomSettingsProvider : ISettingsProvider {

	 ...
    }
```

## Changing default `UserSettings` and `GlobalSettings` providers

Add a ModuleConfiguration to your top-level project and register your provider.

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