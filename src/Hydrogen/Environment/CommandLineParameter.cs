namespace Hydrogen {
	public class CommandLineParameter {

		public CommandLineParameter(string name, CommandLineParameterOptions traits = CommandLineParameterOptions.Default, params string[] dependencies) 
			: this(name, string.Empty, traits, dependencies) {
		}

		public CommandLineParameter(string name, string description, CommandLineParameterOptions traits = CommandLineParameterOptions.Default, params string[] dependencies) {
			Name = name;
			Description = description;
			Traits = traits;
			Dependencies = dependencies ?? new string[0];
		}

		public string Name { get; init; }

		public string Description { get; init; }

		public CommandLineParameterOptions Traits { get; init; }

		public string[] Dependencies { get; init; }
	}
}
