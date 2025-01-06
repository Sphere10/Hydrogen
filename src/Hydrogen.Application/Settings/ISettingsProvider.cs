// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Application;

public interface ISettingsProvider {

	event EventHandlerEx Changed;

	bool AutoSaveNewSettings { get; }

	bool EncryptSettings { get; }

	SettingsObject NewSetting(Type settingsObjectType, object id = null);

	bool ContainsSetting(Type settingsObjectType, object id = null);

	SettingsObject LoadSetting(Type settingsObjectType, object id = null);

	void SaveSetting(SettingsObject settings);

	void DeleteSetting(SettingsObject settings);

	void ClearSettings();

	public bool Has<T>(object id = null) where T : SettingsObject
		=> ContainsSetting(typeof(T), id);

	public T Get<T>(object id = null) where T : SettingsObject {
		SettingsObject settings;
		if (!ContainsSetting(typeof(T), id)) {
			settings = NewSetting(typeof(T), id) as T;
			if (settings == null)
				throw new Exception(string.Format("Settings provider did not create a settings object of requested type '{0}'", typeof(T).FullName));

			if (AutoSaveNewSettings)
				SaveSetting(settings);

			return (T)settings;
		}
		settings = LoadSetting(typeof(T), id) as T;
		if (settings == null)
			throw new Exception(string.Format("Settings provider did not load a settings object of requested type '{0}'", typeof(T).FullName));
		return (T)settings;
	}

	public void ReloadSetting(SettingsObject setting) {
		var reloaded = LoadSetting(setting.GetType(), setting.ID);
		Tools.Object.CopyMembers(setting, reloaded);
	}

}
