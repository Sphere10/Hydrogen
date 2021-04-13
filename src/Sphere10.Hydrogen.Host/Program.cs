using Sphere10.Framework;
using System;
using System.Linq;

namespace Sphere10.Hydrogen.Host {

	public enum Gender {
		Male,
		Female
	}

	class Program {

		public static CommandLineArgs Arguments = new CommandLineArgs {
			Header = new[] {
				"Hydrogen Host v1.0",
				"Copright (c) Sphere 10 Software 2021"
			},

			Arguments = new CommandLineArg[] {
				new CommandLineArg("filename", "The full path to the filename", traits: CommandLineArgTraits.Optional | CommandLineArgTraits.Multiple),
				new CommandLineArg("age",      "The age of the person", traits: CommandLineArgTraits.Mandatory, dependencies: "filename"),
				new CommandLineArg("gender",   "The gender of the person. Allowable values are Male and Female.", dependencies:  "age, filename" )
			},

			Options = CommandLineArgOptions.CaseSensitive | CommandLineArgOptions.DoubleDash | CommandLineArgOptions.PrintHelpOnH | CommandLineArgOptions.PrintHelpOnHelp

		};


		// --filename "c:\my folder with spaces\my file.txt" --age 28 --gender male 
		static void Main(string[] args) {
			if (args.Length == 1) {
				Arguments.Print();
				Environment.Exit(0);
			}

			if (!Arguments.TryParse(args, out var results))      // TryParse will print error
				Environment.Exit(-1);

			var allFiles = results["filename"].ToArray();  // was optional/multiple, so could be multiple values
			var age = results["age"].Single();  // was mandatory, so Single()
			var gender = results["gender"].SingleOrDefault();   // was optional but not multiple, so SingleOrDefault();


		}
	}
}
