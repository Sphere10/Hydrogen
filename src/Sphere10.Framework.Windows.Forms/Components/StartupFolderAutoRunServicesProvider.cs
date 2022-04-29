//-----------------------------------------------------------------------
// <copyright file="StartupFolderAutoRunServicesProvider.cs" company="Sphere 10 Software">
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
using Sphere10.Framework;
using Sphere10.Framework.Windows;

namespace Sphere10.Framework.Application {


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

		public void SetAutoRun(AutoRunType type, string applicationName, string executable) {
			switch (type) {
				case AutoRunType.CurrentUser:
                    Tools.WinShell.CreateShortcutForApplication(executable, Tools.WinShell.DetermineStartupShortcutFilename(applicationName));
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
	}
}
