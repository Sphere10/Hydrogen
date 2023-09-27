// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.DApp.Core.Runtime;

/// <summary>
/// Class that provides the folders for the Hydrogen Application. It can also create them if they don't exist.
/// </summary>
public interface IApplicationPaths {
	string HapFolder { get; }
	string NodeFolder { get; }
	string GuiFolder { get; }
	string ChainFolder { get; }
	string LogsFolder { get; }
	string TempFolder { get; }
	string ArchivesFolder { get; }
	string NodeExecutable { get; }
	string GuiExecutable { get; }
	string HostLog { get; }
	string NodeLog { get; }
	string GUILog { get; }

}
