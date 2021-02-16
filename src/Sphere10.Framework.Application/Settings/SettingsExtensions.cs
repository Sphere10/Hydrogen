using System;

namespace Sphere10.Framework.Application {
	public static class SettingsExtensions {

		public static bool Has<T>(this ISettingsProvider settingsProvider, object id = null) where T : SettingsObject {
			SettingsObject settings = null;
			return settingsProvider.ContainsSetting(typeof(T), id);
		}


		public static T Get<T>(this ISettingsProvider settingsProvider, object id = null) where T : SettingsObject {
			SettingsObject settings = null;
			if (!settingsProvider.ContainsSetting(typeof(T), id)) {
				settings = settingsProvider.NewSetting(typeof(T), id) as T;
				if (settings == null)
					throw new Exception(string.Format("Settings provider did not create a settings object of requested type '{0}'", typeof(T).FullName));

				if (settingsProvider.AutoSaveNewSettings)
					settingsProvider.SaveSetting(settings);

				return (T)settings;
			}
			settings = settingsProvider.LoadSetting(typeof(T), id) as T;
			if (settings == null)
				throw new Exception(string.Format("Settings provider did not load a settings object of requested type '{0}'", typeof(T).FullName));
			return (T)settings;
		}

		public static void ReloadSetting(this ISettingsProvider settingsProvider, SettingsObject setting) {
			var reloaded = settingsProvider.LoadSetting(setting.GetType(), setting.ID);
			Tools.Object.CopyMembers(setting, reloaded);
		}

		/*public static bool Contains<T>(this ISettingsProvider settingsProvider, object id = null) where T : SettingsObject {
			return settingsProvider.ContainsSetting(typeof(T), id);
		}

		public static bool ContainsSetting<T>(this ISettingsProvider settingsProvider, SettingsObject settings) {
			return settingsProvider.ContainsSetting(settings.GetType(), settings.ID);
		}*/

	}
}