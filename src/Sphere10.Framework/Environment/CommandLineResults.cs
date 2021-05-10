using System.Linq;

namespace Sphere10.Framework {
	public class CommandLineResults {

		public ILookup<string, string> Arguments { get; }

		public ILookup<string, CommandLineResults> SubCommands { get; }

	}
}
