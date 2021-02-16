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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sphere10.Framework.Application {
	public interface ISettingsProvider {
		bool AutoSaveNewSettings { get; }
		bool EncryptSettings { get; }
		SettingsObject NewSetting(Type settingsObjectType, object id = null);
		bool ContainsSetting(Type settingsObjectType, object id = null);
		SettingsObject LoadSetting(Type settingsObjectType, object id = null);
		void SaveSetting(SettingsObject settings);
		void DeleteSetting(SettingsObject settings);
		void ClearSettings();
	}
}