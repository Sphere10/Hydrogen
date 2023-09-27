// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;
using Hydrogen.Application;

namespace Hydrogen.Windows.Forms;

public class StartupFolderAutoRunServicesProvider : IAutoRunServices {

	public bool DoesAutoRun(AutoRunType type, string applicationName, string executable) {
		switch (type) {
			case AutoRunType.CurrentUser:
				return File.Exists(Tools.WinShell.DetermineStartupShortcutFilename(applicationName));
				break;
			default:
				throw new SoftwareException("AutoRunType '{0}' not supported", type);
		}
	}

	public void SetAutoRun(AutoRunType type, string applicationName, string executable, string arguments) {
		switch (type) {
			case AutoRunType.CurrentUser:
				Tools.WinShell.CreateShortcutForApplication(executable, Tools.WinShell.DetermineStartupShortcutFilename(applicationName), arguments, displayMode: GetShortcutDisplayMode());
				break;
			default:
				throw new SoftwareException("AutoRunType '{0}' not supported", type);
		}
	}

	public void RemoveAutoRun(AutoRunType type, string applicationName, string executable) {
		switch (type) {
			case AutoRunType.CurrentUser:
				File.Delete(Tools.WinShell.DetermineStartupShortcutFilename(applicationName));
				break;
			default:
				throw new SoftwareException("AutoRunType '{0}' not supported", type);
		}
	}

	protected virtual ShellLink.LinkDisplayMode GetShortcutDisplayMode() => ShellLink.LinkDisplayMode.Normal;
}
