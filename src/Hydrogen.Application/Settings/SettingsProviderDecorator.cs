// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Application;

public class SettingsProviderDecorator : ISettingsProvider {

	public SettingsProviderDecorator(ISettingsProvider internalSettingsProvider) {
		InternalSettingsProvider = internalSettingsProvider;
	}

	public event EventHandlerEx Changed { add => InternalSettingsProvider.Changed += value; remove => InternalSettingsProvider.Changed -= value; }

	public virtual bool AutoSaveNewSettings => InternalSettingsProvider.AutoSaveNewSettings;

	public virtual bool EncryptSettings => InternalSettingsProvider.EncryptSettings;

	protected virtual ISettingsProvider InternalSettingsProvider { get; }

	public virtual void ClearSettings() => InternalSettingsProvider.ClearSettings();

	public virtual bool ContainsSetting(Type settingsObjectType, object id = null) => InternalSettingsProvider.ContainsSetting(settingsObjectType, id);

	public virtual void DeleteSetting(SettingsObject settings) => InternalSettingsProvider.DeleteSetting(settings);

	public virtual SettingsObject LoadSetting(Type settingsObjectType, object id = null) => InternalSettingsProvider.LoadSetting(settingsObjectType, id);

	public virtual SettingsObject NewSetting(Type settingsObjectType, object id = null) => InternalSettingsProvider.NewSetting(settingsObjectType, id);

	public virtual void SaveSetting(SettingsObject settings) => InternalSettingsProvider.SaveSetting(settings);
}
