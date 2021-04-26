using Sphere10.Framework;
using System;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// <see cref="CommandLineArgs_OLD"/> for how to extract strings. Note, it needs to capture arguments in double quotes (lookup github) for exampeples
	/// </summary>
	/// <example>
	///	class Program {
	///
	///		public static CommandLineArgs Arguments = new CommandLineArgs {
	///			Header = new[] {
	///				"Some App v1.0",
	///				"Copyright (c) Company Name 1984"
	///			},
    ///
	///			Arguments = new CommandLineArg[] {
	///				new CommandLineArg("filename", "The full path to the filename", traits: CommandLineArgTraits.Optional | CommandLineArgTraits.Multiple),
	///				new CommandLineArg("age",      "The age of the person", traits: CommandLineArgTraits.Mandatory, dependencies: "filename"),
	///				new CommandLineArg("gender",   "The gender of the person. Allowable values are Male and Female.", dependencies:  "age, filename" )
	///			},
    ///
	///			Options = CommandLineArgOptions.CaseSensitive | CommandLineArgOptions.DoubleDash | CommandLineArgOptions.PrintHelpOnH | CommandLineArgOptions.PrintHelpOnHelp
	///
	///		};
	///
    ///		// --filename "c:\my folder with spaces\my file.txt" --age 28 --gender male 
	///		static void Main(string[] args) {
	///			if (args.Length == 1) {
	///				Arguments.Print();
	///				Environment.Exit(0);
    ///			}
	///
	///			if (!Arguments.TryParse(args, out var results))      // TryParse will print error
	///				Environment.Exit(-1);
	///
	///			var allFiles = results["filename"].ToArray();  // was optional/multiple, so could be multiple values
	///			var age = results["age"].Single();  // was mandatory, so Single()
	///			var gender = results["gender"].SingleOrDefault();   // was optional but not multiple, so SingleOrDefault();
	///
	///
	///		}
	///	}
	/// </example>
	public class CommandLineArgs {

		public string[] Header { get; init; }
		public CommandLineArgOptions Options { get; init; }
		public CommandLineArg[] Arguments { get; init; }
		public string[] Footer { get; init; }



		public bool TryParse(string[] args, out ILookup<string, string> results) {
			// Implementation note: you can use a LookupEx for arguments
			results = new LookupEx<string, string>();
			throw new NotImplementedException();
		}

		public void Print() {
			foreach(var headerLine in Header)
				System.Console.WriteLine(Header);

			foreach (var arg in Arguments)
				System.Console.WriteLine(arg.Description);
		}

	}
}
