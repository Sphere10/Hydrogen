using Sphere10.Framework;
using System;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// <see cref="CommandLineArgs_OLD"/> for how to extract strings. Note, it needs to capture arguments in double quotes (lookup github) for exampeples
	/// </summary>
	public class CommandLineArgs {

		public string[] Header { get; init; }
		public CommandLineArgOptions Options { get; init; }
		public CommandLineArg[] Arguments { get; init; }



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
