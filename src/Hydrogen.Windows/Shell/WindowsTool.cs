// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;
using System.IO;
using Hydrogen.Windows;


namespace Tools;

public static partial class WinShell {

	//public static void CreateShortcutForRunningApplication(string shortcutPath, bool overwrite = true) {
	//    CreateShortcutForApplication(Application.ExecutablePath, shortcutPath, overwrite);
	//}

	public static void CreateShortcutForApplication(string executablePath, string shortcutPath, string arguments = null, bool overwrite = true, ShellLink.LinkDisplayMode displayMode = ShellLink.LinkDisplayMode.Normal) {
		using (ShellLink shortcut = new ShellLink()) {
			shortcut.Target = executablePath;
			shortcut.WorkingDirectory = Path.GetDirectoryName(executablePath);
			shortcut.Description = "My Shorcut Name Here";
			shortcut.DisplayMode = displayMode;
			shortcut.Arguments = arguments;
			if (File.Exists(shortcutPath) && overwrite) {
				File.Delete(shortcutPath);
			}
			shortcut.Save(shortcutPath);
		}
	}
	public static string DetermineStartupShortcutFilename(string applicationName) {
		return
			string.Format(
				"{1}{0}{2}.LNK",
				Path.DirectorySeparatorChar,
				Environment.GetFolderPath(Environment.SpecialFolder.Startup),
				Tools.FileSystem.ToWellFormedPath(applicationName)
			);
	}
	public static void AddProgramAsStartup(string startPath) {


		//string startupDir =
		//            string.Format("{0}\\Start Menu\\Programs\\Startup",
		//            Environment.GetEnvironmentVariable("ALLUSERSPROFILE"));
		//string appDir = Path.GetDirectoryName(
		//    Application.ExecutablePath);

		//if (Directory.Exists(startupDir) &&
		//    Directory.Exists(appDir)) {

		//    string startupIconFile = startupDir + "\\" + "RSI Warrior.lnk";
		//    string appIconFile = appDir + "\\" + "RSI Warrior.lnk";
		//    bool iconExistsInStartup = File.Exists(startupIconFile);
		//    bool iconExistsInAppDir = File.Exists(appIconFile); ;

		//    // copy startup icon to startup directory
		//    if (Configuration.Instance.LoadOnStartup) {
		//        // if exists in startup folder and not in appDir copy from startup to appdir
		//        if (iconExistsInStartup && !iconExistsInAppDir) {
		//            File.Copy(startupIconFile, appIconFile, true);
		//        } else if (iconExistsInAppDir && !iconExistsInStartup) {
		//            // if exists in appdir and not in startup copy to startup
		//            File.Copy(appIconFile, startupIconFile, true);
		//        }
		//    } else {
		//        // if exists in startup folder and not in appDir move from startup to appdir
		//        if (iconExistsInStartup && !iconExistsInAppDir) {
		//            File.Move(startupIconFile, appIconFile);
		//        } else {
		//            // if exists in startup delete it from startup
		//            File.Delete(startupIconFile);
		//        }
		//    }
		//}
	}

	/// <summary>
	/// Opens a file in explorer. 
	/// </summary>
	/// <param name="filePath">The fully qualified path and filename of the target to open as highlighted by explorer</param>
	public static void HighlightFileInExplorer(string filePath) {
		if (File.Exists(filePath)) {
			Process.Start("explorer.exe", @"/select, " + filePath);
		}
	}
}
