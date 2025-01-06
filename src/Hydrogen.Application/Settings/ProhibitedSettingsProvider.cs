// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Application;

public class ProhibitedSettingsProvider : ISettingsProvider {
	private const string ExceptionMessage = "Settings are prohibited in this application";
	
	public event EventHandlerEx Changed { add => throw new NotSupportedException(ExceptionMessage); remove => throw new NotSupportedException(ExceptionMessage); }
	public bool AutoSaveNewSettings => throw new NotSupportedException(ExceptionMessage);

	public bool EncryptSettings => throw new NotSupportedException(ExceptionMessage);

	public void ClearSettings() => throw new NotSupportedException(ExceptionMessage);


	public bool ContainsSetting(Type settingsObjectType, object id = null) => throw new NotSupportedException(ExceptionMessage);


	public void DeleteSetting(SettingsObject settings) => throw new NotSupportedException(ExceptionMessage);


	public SettingsObject LoadSetting(Type settingsObjectType, object id = null) => throw new NotSupportedException(ExceptionMessage);


	public SettingsObject NewSetting(Type settingsObjectType, object id = null) => throw new NotSupportedException(ExceptionMessage);


	public void SaveSetting(SettingsObject settings) => throw new NotSupportedException(ExceptionMessage);


}
