//-----------------------------------------------------------------------
// <copyright file="ProhibitedSettingsProvider.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2021</date>
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Sphere10.Framework.Application {
	public class ProhibitedSettingsProvider : ISettingsProvider {
		private const string ExceptionMessage = "Settings are prohibited in this application";
		public bool AutoSaveNewSettings => throw new NotSupportedException(ExceptionMessage);

		public bool EncryptSettings => throw new NotSupportedException(ExceptionMessage);

		public void ClearSettings() => throw new NotSupportedException(ExceptionMessage);


		public bool ContainsSetting(Type settingsObjectType, object id = null) => throw new NotSupportedException(ExceptionMessage);


		public void DeleteSetting(SettingsObject settings) => throw new NotSupportedException(ExceptionMessage);


		public SettingsObject LoadSetting(Type settingsObjectType, object id = null) => throw new NotSupportedException(ExceptionMessage);


		public SettingsObject NewSetting(Type settingsObjectType, object id = null) => throw new NotSupportedException(ExceptionMessage);


		public void SaveSetting(SettingsObject settings) =>	throw new NotSupportedException(ExceptionMessage);
		

	}
}