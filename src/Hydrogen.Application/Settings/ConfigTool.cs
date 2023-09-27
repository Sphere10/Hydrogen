// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Configuration;

// ReSharper disable CheckNamespace
namespace Tools;

/// <summary>
/// Configuration helper class. 
/// </summary>
/// <remarks></remarks>
public static class Config {

	/// <summary>
	/// Gets an app setting.
	/// </summary>
	/// <param name="key">The app setting key.</param>
	/// <param name="defaultValue">The default value.</param>
	/// <returns>String appSetting from config (or default value if missing)</returns>
	public static string GetAppSetting(string key, string defaultValue = "") {
		var returnValue = defaultValue;
		var settingValue = ConfigurationManager.AppSettings[key];
		if (settingValue != null) {
			returnValue = settingValue;
		}
		return returnValue;
	}

	/// <summary>
	/// Gets a strongly-typed app setting.
	/// </summary>
	/// <typeparam name="T">Type</typeparam>
	/// <param name="key">The name.</param>
	/// <param name="defaultValue">The default value.</param>
	/// <returns>Strongly-typed appSetting from config (or default value if missing)</returns>
	public static T GetAppSetting<T>(string key, T defaultValue = default(T)) where T : struct {
		var returnValue = defaultValue;
		var stringValue = GetAppSetting(key, null);
		if (stringValue != null) {
			returnValue = Tools.Parser.Parse<T>(stringValue);
		}
		return returnValue;
	}
}
