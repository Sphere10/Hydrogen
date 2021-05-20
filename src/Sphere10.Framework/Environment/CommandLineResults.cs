namespace Sphere10.Framework {
	public class CommandLineResults {
		public CommandLineResults(LookupEx<string, CommandLineResults> commands, LookupEx<string, string> arguments) {
			Guard.ArgumentNotNull(commands, nameof(commands));
			Guard.ArgumentNotNull(arguments, nameof(arguments));

			Commands = commands;
			Arguments = arguments;
		}
		public LookupEx<string, string> Arguments { get; }

		public LookupEx<string, CommandLineResults> Commands { get; }

		public bool HelpRequested => Arguments.Contains("Help");
	}
}
