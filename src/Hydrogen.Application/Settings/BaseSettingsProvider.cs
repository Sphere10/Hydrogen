//-----------------------------------------------------------------------
// <copyright file="BaseSettingsProvider.cs" company="Sphere 10 Software">
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
	public abstract class BaseSettingsProvider : ISettingsProvider {

		public virtual bool AutoSaveNewSettings => false;

		public virtual bool EncryptSettings => true;

		public virtual SettingsObject LoadSetting(Type settingsObjectType, object id = null) {
			var settings = LoadInternal(settingsObjectType, id);
			if (EncryptSettings)
				Tools.Object.DecryptMembers(settings);
			settings.Provider = this;
			return settings;
		}

		public virtual SettingsObject NewSetting(Type settingsObjectType, object id = null) {
			var settings = Tools.Object.Create(settingsObjectType) as SettingsObject;
			settings.ID = id;
			settings.Provider = this;
			if (AutoSaveNewSettings)
				SaveSetting(settings);
			return settings;
		}

		public virtual void SaveSetting(SettingsObject settings) {
			//var clone = Tools.Object.Clone(settings);
			//Tools.Object.EncryptMembers(clone);
			var undoEncryption = false;
			try {
				if (EncryptSettings) {
					Tools.Object.EncryptMembers(settings);
					undoEncryption = true;
				}
				SaveInternal(settings);
			} finally {
				if (undoEncryption)
					Tools.Object.DecryptMembers(settings);
			}
		}

		public abstract bool ContainsSetting(Type settingsObjectType, object id = null);

		public abstract void DeleteSetting(SettingsObject settings);

		public abstract void ClearSettings();

		protected abstract SettingsObject LoadInternal(Type settingsObjectType, object id = null);

		protected abstract void SaveInternal(SettingsObject settings);

	}

}
