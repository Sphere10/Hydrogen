namespace Sphere10.Framework {
	public class CommandLineCommand : CommandLineParameter {

		public CommandLineCommand(string name, string description, CommandLineParameter[] args = null, CommandLineCommand[] commands = null, params string[] dependencies) : base(name, description, default, dependencies) {
			Args = args ?? new CommandLineParameter[0];
			Commands = commands ?? new CommandLineCommand[0];
		}

		public CommandLineParameter[] Args { get; }
		
		public CommandLineCommand[] Commands { get; }
	}
}
