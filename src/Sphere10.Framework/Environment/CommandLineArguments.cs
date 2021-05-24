using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {
	public class CommandLineArguments {
		public CommandLineArguments(IDictionary<string, CommandLineArguments> commands, ILookup<string, string> arguments) {
			Guard.ArgumentNotNull(commands, nameof(commands));
			Guard.ArgumentNotNull(arguments, nameof(arguments));
			Commands = commands;
			Arguments = arguments;
		}
		public ILookup<string, string> Arguments { get; }

		public IDictionary<string, CommandLineArguments> Commands { get; }

		public bool HelpRequested => Arguments.Contains("Help") || Arguments.Contains("help");
	}
}
