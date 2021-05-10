using System.Linq;

namespace Sphere10.Framework {
	public class CommandLineResults {
		public CommandLineResults(ILookup<string, string> arguments, ILookup<string, CommandLineResults> subCommands) {
			Guard.ArgumentNotNull(arguments, nameof(arguments));
			Guard.ArgumentNotNull(subCommands, nameof(subCommands));
			
			Arguments = arguments;
			SubCommands = subCommands;
		}

		public ILookup<string, string> Arguments { get; }

		public ILookup<string, CommandLineResults> SubCommands { get; }

	}
}
