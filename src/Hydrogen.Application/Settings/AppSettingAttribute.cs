// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.ComponentModel;
using System.Configuration;

namespace Hydrogen.Application;

public class AppSettingAttribute : DefaultValueAttribute {

	public AppSettingAttribute(string key)
		: base(GetAppSetting(key)) {
		Key = key;
	}

	public string Key { get; }

	public override object Value => GetAppSetting(Key);

	private static string GetAppSetting(string key)
		=> ConfigurationManager.AppSettings[key];
}
