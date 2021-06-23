using Sphere10.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Hydrogen.Core.HAP {

	/// <summary>
	/// Class that provides the folders for the Hydrogen Application. It can also create them if they don't exist.
	/// </summary>
	/// <remarks>
	///  The folder structure is currently:
	///  
	///  %root%/hap/node
	///  %root%/hap/gui
	///  %root%/content
	///  %root%/chain
	///  %root%/logs
	///  %root%/temp
	///  %root%/archive
	/// </remarks>
	public class HydrogenApplicationPaths {
		private const string AppDirName = "hap";
		private const string NodeExecutableFile = "Sphere10.Hydrogen.Node.exe";
		private const string NodeDirName = "node";
		private const string GuiDirName = "gui";
		private const string ChainDirName = "chain";
		private const string LogsDirName = "logs";
		private const string TempDirName = "temp";
		private const string ArchiveDirName = "archive";
		private const string HostLogFileName = "host.log";
		private const string NodeLogFileName = "node.log";
		private const string GUILogFileName = "gui.log";

		private readonly string _root;

		public HydrogenApplicationPaths(string root, bool createIfNotExists = false) {
			_root = root;
			if (createIfNotExists)
				CreateDirectoriesIfMissing();
			else
				EnsureDirectoriesExist();
		}

		public string Root => _root;
		public string HapFolder => Path.Combine(Root, AppDirName);
		public string NodeFolder => Path.Combine(HapFolder, NodeDirName);
		public string GuiFolder => Path.Combine(HapFolder, NodeDirName);
		public string ChainFolder => Path.Combine(Root, ChainDirName);
		public string LogsFolder => Path.Combine(Root, LogsDirName);
		public string TempFolder => Path.Combine(Root, TempDirName);
		public string ArchivesFolder => Path.Combine(Root, ArchiveDirName);

		public string NodeExecutable => Path.Combine(NodeFolder, NodeExecutableFile);
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
}
