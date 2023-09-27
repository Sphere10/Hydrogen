// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Linq;
using Microsoft.Win32;
using System.IO;
using Hydrogen.Application;

namespace Hydrogen.Windows.Forms;

public class RegistryAutoRunServicesProvider : IAutoRunServices {

	public RegistryAutoRunServicesProvider() {
	}

	public bool DoesAutoRun(AutoRunType type, string applicationName, string executable) {
		var key = GetRegistryKey(type);
		try {
			return
				key
					.GetValueNames()
					.Select(name => key.GetValue(name))
					.Any(value => value is string && ((string)value).ToUpperInvariant() == executable.ToUpperInvariant());
		} finally {
			key.Close();
		}
	}

	public void SetAutoRun(AutoRunType type, string applicationName, string executable, string arguments) {
		var key = GetRegistryKey(type);
		try {
			key.SetValue(applicationName ?? CalculateKeyFromExecutable(executable, arguments), executable);
		} finally {
			key.Close();
		}
	}

	public void RemoveAutoRun(AutoRunType type, string applicationName, string executable) {
		var key = GetRegistryKey(type);
		try {
			(
					from name in key.GetValueNames()
					let value = key.GetValue(name)
					where (value is string) && ((string)value).ToUpperInvariant() == executable.ToUpperInvariant()
					select name
				)
				.ForEach(key.DeleteValue);
		} finally {
			key.Close();
		}
	}

	private RegistryKey GetRegistryKey(AutoRunType type) {
		RegistryKey key;
		switch (type) {
			case AutoRunType.AllUsers:
				key = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\RunAsync");
				break;
			case AutoRunType.AllUsersRunOnce:
				key = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce");
				break;
			case AutoRunType.CurrentUser:
				key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\RunAsync");
				break;
			case AutoRunType.CurrentUserRunOnce:
				key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\RunRunOnce");
				break;
			default:
				throw new SoftwareException("Unsupported AutoStartType '{0}'", type);
		}
		return key;
	}

	protected virtual string CalculateKeyFromExecutable(string executable, string arguments) {
		return Path.GetFileNameWithoutExtension(executable).RemoveCamelCase() + (arguments ?? string.Empty);
	}


}
