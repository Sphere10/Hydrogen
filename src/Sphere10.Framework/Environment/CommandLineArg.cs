namespace Sphere10.Framework {
	public class CommandLineArg {

		public CommandLineArg(string name, string description, CommandLineArgTraits traits = CommandLineArgTraits.Optional, params string[] dependencies) {
			Name = name;
			Description = description;
			Traits = traits;
			Dependencies = dependencies ?? new string[0];
		}

		public string Name;

		public string Description;

		public CommandLineArgTraits Traits;

		public string[] Dependencies;
	}
}
