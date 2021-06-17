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
	public class HydrogenApplicationFolders {
		private const string AppDirName = "hap";
		private const string NodeDirName = "node";
		private const string GuiDirName = "gui";
		private const string ChainDirName = "chain";
		private const string LogsDirName = "logs";
		private const string TempDirName = "temp";
		private const string ArchiveDirName = "archive";

		private readonly string _root;

		public HydrogenApplicationFolders(string root, bool createIfNotExists = false) {
			_root = root;
			if (createIfNotExists)
				CreateDirectoriesIfMissing();
			else
				EnsureDirectoriesExist();
		}

		public string Root => _root;
		public string HapPath => Path.Combine(Root, AppDirName);
		public string NodePath => Path.Combine(HapPath, NodeDirName);
		public string GuiPath => Path.Combine(HapPath, NodeDirName);
		public string ChainPath => Path.Combine(Root, ChainDirName);
		public string LogsPath => Path.Combine(Root, LogsDirName);
		public string TempPath => Path.Combine(Root, TempDirName);
		public string ArchivePath => Path.Combine(Root, ArchiveDirName);

		private void CreateDirectoriesIfMissing() {
			if (!Directory.Exists(_root))
				Directory.CreateDirectory(_root);

			if (!Directory.Exists(HapPath))
				Directory.CreateDirectory(HapPath);

			if (!Directory.Exists(NodePath))
				Directory.CreateDirectory(NodePath);

			if (!Directory.Exists(GuiPath))
				Directory.CreateDirectory(GuiPath);

			if (!Directory.Exists(LogsPath))
				Directory.CreateDirectory(LogsPath);

			if (!Directory.Exists(TempPath))
				Directory.CreateDirectory(TempPath);

			if (!Directory.Exists(ArchivePath))
				Directory.CreateDirectory(ArchivePath);
		}

		private void EnsureDirectoriesExist() {
			Guard.DirectoryExists(HapPath);
			Guard.DirectoryExists(NodePath);
			Guard.DirectoryExists(GuiPath);
			Guard.DirectoryExists(ChainPath);
			Guard.DirectoryExists(LogsPath);
			Guard.DirectoryExists(TempPath);
			Guard.DirectoryExists(ArchivePath);
		}
	}
}
