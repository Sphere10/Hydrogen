namespace Sphere10.Framework {
	public class CommandLineArgCommand : CommandLineArg {

		public CommandLineArgCommand(string name, string description, CommandLineArg[] args = null, CommandLineArgCommand[] commands = null, params string[] dependencies) : base(name, description, default, dependencies) {
			Args = args ?? new CommandLineArg[0];
			Commands = commands ?? new CommandLineArgCommand[0];
		}

		public CommandLineArg[] Args { get; }
		
		public CommandLineArgCommand[] Commands { get; }
	}
}
