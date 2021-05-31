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


		// NOTE: This should be refactored out since sibling commands is conceptually not possible
		public IDictionary<string, CommandLineArguments> Commands { get; }

		public (string Name, CommandLineArguments Arguments) Command {
			get {
				if (Commands.Count == 0)
					return default;
				var singleCommand = Commands.Single();
				return (singleCommand.Key, singleCommand.Value);
			}
		}



		public bool HelpRequested => Arguments.Contains("Help") || Arguments.Contains("help");
	}
}
