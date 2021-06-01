using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {
	public class CommandLineResults {

		public CommandLineResults(IDictionary<string, CommandLineResults> commands, ILookup<string, string> arguments) {
			Guard.ArgumentNotNull(commands, nameof(commands));
			Guard.ArgumentNotNull(arguments, nameof(arguments));
			Commands = commands;
			Arguments = arguments;
		}
		public ILookup<string, string> Arguments { get; }


		// NOTE: This should be refactored out since sibling commands is conceptually not possible

		public IDictionary<string, CommandLineResults> Commands { get; }

		// public string CommandName { get; }   // This is empty for the root CommandLineResults
		// public CommandLineResults SubCommand { get; }   // Should be 1 command here

		public (string Name, CommandLineResults Arguments) Command {
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


/// git -f -d
/// CommandLineResults
///   CommandName = ""
///   SubCommand = null
///   Arguments = -f, -d
///
///
/// git -f -d remote -g -h
/// CommandLineResults
///   CommandName = ""
///   SubCommand =
///			CommandName = remote
///         Arguments = -g -h
///         Command = null
///   Arguments = -f, -d
///
///
/// git -f -d remote
/// CommandLineResults
///   CommandName = ""
///   SubCommand =
///			CommandName = remote
///         Arguments = -g -h
///         Command = null
///   Arguments = -f, -d

