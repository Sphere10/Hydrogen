//-----------------------------------------------------------------------
// <copyright file="ISettingsProvider.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Hydrogen.Application {
    public class SettingsProviderDecorator : ISettingsProvider {

        public SettingsProviderDecorator(ISettingsProvider internalSettingsProvider) {
			InternalSettingsProvider = internalSettingsProvider;
        }

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
}