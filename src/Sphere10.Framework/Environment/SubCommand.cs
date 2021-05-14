namespace Sphere10.Framework {
	public class SubCommand : CommandLineArg {

		public SubCommand(string name, string description, CommandLineArg[] args, SubCommand[] subCommands, params string[] dependencies) : base(name, description, default, dependencies) {
			Guard.ArgumentNotNull(args, nameof(args));
			Guard.ArgumentNotNull(subCommands, nameof(subCommands));
			
			Args = args;
			SubCommands = subCommands;
		}

		public CommandLineArg[] Args { get; }
		
		public SubCommand[] SubCommands { get; }
	}
}
