// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;

namespace Hydrogen.DApp.Core.Runtime;

/// <summary>
/// Provides directories and file paths for all artefacts of a Hydrogen Application (and can create them).
/// </summary>
/// <remarks>
///  The underlying structure in the file-system is:
///  
///  %root%/hap                      ; root folder of current Hydrogen Application Package (HAP)
///  %root%/hap/node                 ; extracted node application from the currently deployed HAP
///  %root%/hap/gui                  ; extracted GUI application from the currently deployed HAP
///  %root%/hap/plugins              ; repository of all user downloaded plugins
///  %root%/content                  ; content directory that stores all files used across system, organized by content-hash
///  %root%/chain                    ; blockchain directory where blockchain is stored
///  %root%/objectspace              ; object-space directory that stores the consensus object space built by the blockchain
///  %root%/logs                     ; all logs for host, node and gui
///  %root%/temp                     ; temp files that persist between session (used primarily for transactiona file pages)
///  %root%/archive                  ; archive of previous HAP's
/// </remarks>
public class ApplicationPaths : IApplicationPaths {
	internal const string AppDirectoryName = "hap";
	internal const string NodeExecutableFilename = "Hydrogen.DApp.Node.exe";
	internal const string NodeDirectoryName = "node";
	internal const string GuiDirectoryName = "gui";
	internal const string GuiExecutableFilename = "Hydrogen.DApp.Presentation2.Loader.exe";
	internal const string ChainDirectoryName = "chain";
	internal const string LogsDirectoryName = "logs";
	internal const string TempDirectoryName = "temp";
	internal const string ArchiveDirectoryName = "archive";
	internal const string HostLogFileName = "host.log";
	internal const string NodeLogFileName = "node.log";
	internal const string GUILogFileName = "gui.log";

	private readonly string _root;

	public ApplicationPaths(string root, bool createIfNotExists = false) {
		_root = root;
		if (createIfNotExists)
			CreateDirectoriesIfMissing();
		else
			EnsureDirectoriesExist();
	}

	public string Root => _root;

	public string HapFolder => Path.Combine(Root, AppDirectoryName);

	public string NodeFolder => Path.Combine(HapFolder, NodeDirectoryName);

	public string GuiFolder => Path.Combine(HapFolder, GuiDirectoryName);

	public string ChainFolder => Path.Combine(Root, ChainDirectoryName);

	public string LogsFolder => Path.Combine(Root, LogsDirectoryName);

	public string TempFolder => Path.Combine(Root, TempDirectoryName);

	public string ArchivesFolder => Path.Combine(Root, ArchiveDirectoryName);

	public string NodeExecutable => Path.Combine(NodeFolder, NodeExecutableFilename);

	public string GuiExecutable => Path.Combine(GuiFolder, GuiExecutableFilename);

	public string HostLog => Path.Combine(LogsFolder, HostLogFileName);

	public string NodeLog => Path.Combine(LogsFolder, NodeLogFileName);

	public string GUILog => Path.Combine(LogsFolder, GUILogFileName);

	private void CreateDirectoriesIfMissing() {
		if (!Directory.Exists(_root))
			Directory.CreateDirectory(_root);

		if (!Directory.Exists(HapFolder))
			Directory.CreateDirectory(HapFolder);

		if (!Directory.Exists(NodeFolder))
			Directory.CreateDirectory(NodeFolder);

		if (!Directory.Exists(GuiFolder))
			Directory.CreateDirectory(GuiFolder);

		if (!Directory.Exists(LogsFolder))
			Directory.CreateDirectory(LogsFolder);

		if (!Directory.Exists(TempFolder))
			Directory.CreateDirectory(TempFolder);

		if (!Directory.Exists(ArchivesFolder))
			Directory.CreateDirectory(ArchivesFolder);
	}

	private void EnsureDirectoriesExist() {
		Guard.DirectoryExists(HapFolder);
		Guard.DirectoryExists(NodeFolder);
		Guard.DirectoryExists(GuiFolder);
		Guard.DirectoryExists(ChainFolder);
		Guard.DirectoryExists(LogsFolder);
		Guard.DirectoryExists(TempFolder);
		Guard.DirectoryExists(ArchivesFolder);
	}
}
