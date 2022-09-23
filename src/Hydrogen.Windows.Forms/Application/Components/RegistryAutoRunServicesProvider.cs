//-----------------------------------------------------------------------
// <copyright file="RegistryAutoRunServicesProvider.cs" company="Sphere 10 Software">
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
using System.Threading;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.IO;
using Hydrogen;
using Hydrogen.Application;

namespace Hydrogen.Windows.Forms {


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
					.Any(value => value is string && ((string) value).ToUpperInvariant() == executable.ToUpperInvariant());
			}
			finally {
				key.Close();
			}
		}

		public void SetAutoRun(AutoRunType type, string applicationName, string executable) {
			var key = GetRegistryKey(type);
			try {
				key.SetValue(applicationName ?? DetermineKeyFromExecutable(executable), executable);
			}
			finally {
				key.Close();
			}
		}

		public void RemoveAutoRun(AutoRunType type, string applicationName, string executable) {
			var key = GetRegistryKey(type);
			try {
				(
					from name in key.GetValueNames()
					let value = key.GetValue(name)
					where (value is string) && ((string) value).ToUpperInvariant() == executable.ToUpperInvariant()
					select name
				)
				.ForEach(key.DeleteValue);
			}
			finally {
				key.Close();
			}
		}

		private RegistryKey GetRegistryKey(AutoRunType type) {
			RegistryKey key;
			switch (type) {
				case AutoRunType.AllUsers:
					key = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
					break;
				case AutoRunType.AllUsersRunOnce:
					key = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce");
					break;
				case AutoRunType.CurrentUser:
					key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
					break;
				case AutoRunType.CurrentUserRunOnce:
					key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\RunRunOnce");
					break;
				default:
					throw new SoftwareException("Unsupported AutoStartType '{0}'", type);
			}
			return key;
		}

		protected virtual string DetermineKeyFromExecutable(string executable) {
			return Path.GetFileNameWithoutExtension(executable).RemoveCamelCase();
		}



	}
}
