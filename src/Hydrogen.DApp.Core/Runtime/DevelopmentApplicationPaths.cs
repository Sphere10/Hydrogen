// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Reflection;

namespace Hydrogen.DApp.Core.Runtime;

/// <summary>
/// Used for running within IDE.
/// </summary>
public class DevelopmentApplicationPaths : IApplicationPaths {
	internal const string NodeProjectDir = "Hydrogen.DApp.Node";
	internal const string NodeExecutableFileName = "Hydrogen.DApp.Node.exe";
	internal const string GuiProjectDir = "Hydrogen.DApp.Presentation2.Loader";
	internal const string GuiExecutableFileName = "Hydrogen.DApp.Presentation2.Loader.exe";

	private readonly string _solutionFolderPath;

	public DevelopmentApplicationPaths()
		: this(GetHydrogenSolutionFolder()) {
	}

	public DevelopmentApplicationPaths(string hydrogenSolutionFolder) {
		Guard.DirectoryExists(hydrogenSolutionFolder);
		_solutionFolderPath = hydrogenSolutionFolder;
		Root = Tools.FileSystem.GetTempEmptyDirectory(true);
		CreateDirectoriesIfMissing();
	}

	public string Platform {
		get {
#if NET5_0
				return "net5.0";
#elif NET6_0
				return "net6.0";
#else
			throw new NotImplementedException("This needs to be moved out");

#endif
		}
	}

	public string BuildConfiguration {
		get {
#if DEBUG
			return "Debug";
#elif RELEASE
			return "Release";
#else
#error Unrecognized build configuration
#endif
		}
	}

	public string Root { get; }

	public string HapFolder => throw new NotSupportedException("Development mode does not use HAP folder");

	public string NodeFolder => Path.Combine(_solutionFolderPath, NodeProjectDir, "bin", BuildConfiguration, Platform);

	public string GuiFolder => Path.Combine(_solutionFolderPath, GuiProjectDir, "bin", BuildConfiguration, Platform, GuiExecutableFileName);

	public string ChainFolder => Path.Combine(Root, ApplicationPaths.ChainDirectoryName);

	public string LogsFolder => Path.Combine(Root, ApplicationPaths.LogsDirectoryName);

	public string TempFolder => Path.Combine(Root, ApplicationPaths.TempDirectoryName);

	public string ArchivesFolder => Path.Combine(Root, ApplicationPaths.ArchiveDirectoryName);

	public string NodeExecutable => Path.Combine(NodeFolder, NodeExecutableFileName);

	public string GuiExecutable => Path.Combine(GuiFolder, GuiExecutableFileName);

	public string HostLog => Path.Combine(LogsFolder, ApplicationPaths.HostLogFileName);

	public string NodeLog => Path.Combine(LogsFolder, ApplicationPaths.NodeLogFileName);

	public string GUILog => Path.Combine(LogsFolder, ApplicationPaths.GUILogFileName);

	private void CreateDirectoriesIfMissing() {
		if (!Directory.Exists(Root))
			Directory.CreateDirectory(Root);

		if (!Directory.Exists(LogsFolder))
			Directory.CreateDirectory(LogsFolder);

		if (!Directory.Exists(TempFolder))
			Directory.CreateDirectory(TempFolder);

		if (!Directory.Exists(ArchivesFolder))
			Directory.CreateDirectory(ArchivesFolder);
	}

	private static string GetHydrogenSolutionFolder() {
		var runningExecutablePath = Assembly.GetEntryAssembly()?.Location;
		if (string.IsNullOrEmpty(runningExecutablePath) || !Path.IsPathFullyQualified(runningExecutablePath))
			throw new SoftwareException("Development mode can only be executed from a file-system");
		return Tools.FileSystem.GetParentDirectoryPath(runningExecutablePath, 5); // 5 levels of parent dir's to get solution folder
	}
}
