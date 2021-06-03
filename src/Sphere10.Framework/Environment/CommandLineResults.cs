using System.Linq;

namespace Sphere10.Framework {
	public class CommandLineResults {
		
		public ILookup<string, string> Parameters { get; set; }

		public string CommandName { get; set; } // This is empty for the root CommandLineResults

		public CommandLineResults SubCommand { get; set; } // Should be 1 command here

		public bool HelpRequested { get; set; }
	}
}
