namespace Sphere10.Framework {
	public class CommandLineArgCommand : CommandLineArg {

		public CommandLineArgCommand(string name, string description, CommandLineArg[] args, CommandLineArgCommand[] commands, params string[] dependencies) : base(name, description, default, dependencies) {
			Guard.ArgumentNotNull(args, nameof(args));
			Guard.ArgumentNotNull(commands, nameof(commands));
			
			Args = args;
			Commands = commands;
		}

		public CommandLineArg[] Args { get; }
		
		public CommandLineArgCommand[] Commands { get; }
	}
}
