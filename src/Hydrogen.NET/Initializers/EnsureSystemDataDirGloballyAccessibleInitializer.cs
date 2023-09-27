// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Hydrogen.Application;

public class EnsureSystemDataDirGloballyAccessibleInitializer : ApplicationInitializerBase {

	public override int Priority => -1;

	public override void Initialize() {

		var dir = Tools.Text.FormatEx("{SystemDataDir}");
		if (!Directory.Exists(dir)) {
			Directory.CreateDirectory(dir);
			var fileInfo = new DirectoryInfo(dir);
			// Get a FileSecurity object that represents the current security settings.
			var directorySecurity = fileInfo.GetAccessControl();

			// Add the FileSystemAccessRule to the security settings. 
			directorySecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
				FileSystemRights.Modify,
				InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
				PropagationFlags.None,
				AccessControlType.Allow));

			// Set the new access settings.
			fileInfo.SetAccessControl(directorySecurity);
		}

	}

}
