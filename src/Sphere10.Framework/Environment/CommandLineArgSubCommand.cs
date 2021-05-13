namespace Sphere10.Framework {
	public class CommandLineArgSubCommand {
		public CommandLineArgSubCommand(string name, CommandLineArg[] arguments) {
			Name = name;
			Arguments = arguments;
		}

		public string Name { get; }

		public CommandLineArg[] Arguments { get; }
	}
}
