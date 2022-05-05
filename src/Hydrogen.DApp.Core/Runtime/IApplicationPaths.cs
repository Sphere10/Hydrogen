using Hydrogen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.DApp.Core.Runtime {

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
}
