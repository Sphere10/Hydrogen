//-----------------------------------------------------------------------
// <copyright file="UserSettings.cs" company="Sphere 10 Software">
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
using System.IO;

namespace Hydrogen.Application {

	/// <summary>
	/// These represent settings relevant to the system user of the application, and are not shared between users.
	/// </summary>
	public static class UserSettings {

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
			=> Guard.Ensure(_provider != null, "User settings provider has not been set");
	}
}
