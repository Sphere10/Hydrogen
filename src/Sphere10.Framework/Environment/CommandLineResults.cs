using System.Linq;

namespace Sphere10.Framework {
	public class CommandLineResults {
		public CommandLineResults(LookupEx<string, CommandLineResults> subCommands, LookupEx<string, string> arguments) {
			Guard.ArgumentNotNull(subCommands, nameof(subCommands));
			Guard.ArgumentNotNull(arguments, nameof(arguments));

			SubCommands = subCommands;
			Arguments = arguments;
		}
		public LookupEx<string, string> Arguments { get; }

		public LookupEx<string, CommandLineResults> SubCommands { get; }
	}
}
