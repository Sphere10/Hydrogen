namespace Sphere10.Framework {
	public class CommandLineCommand : CommandLineParameter {

		public CommandLineCommand(string name, CommandLineParameter[] parameters = null, CommandLineCommand[] subCommands = null, params string[] dependencies)
			: this(name, string.Empty, parameters, subCommands, dependencies) {
		}

		public CommandLineCommand(string name, string description, CommandLineParameter[] parameters = null, CommandLineCommand[] subCommands = null, params string[] dependencies) 
			: base(name, description, default, dependencies) {
			Parameters = parameters ?? new CommandLineParameter[0];
			SubCommands = subCommands ?? new CommandLineCommand[0];
		}

		public CommandLineParameter[] Parameters { get; init; }
		
		public CommandLineCommand[] SubCommands { get; init; }
	}
}
