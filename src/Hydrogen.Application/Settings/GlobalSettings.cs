// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Application;

/// <summary>
/// Settings that are system-wide and shared by all users.
/// </summary>
public static class GlobalSettings {

	private static ISettingsProvider _provider;

	public static ISettingsProvider Provider {
		get {
			CheckProvider();
			return _provider;
		}
		set => _provider = value;
	}

	public static bool Has<T>(object id = null) where T : SettingsObject, new() {
		return Provider.Has<T>(id);
	}

	public static T Get<T>(object id = null) where T : SettingsObject, new() {
		return Provider.Get<T>(id);
	}

	public static void Clear() {
		CheckProvider();
		Provider.ClearSettings();
	}

	private static void CheckProvider()
		=> Guard.Ensure(_provider != null, "Global settings provider has not been set");
}
