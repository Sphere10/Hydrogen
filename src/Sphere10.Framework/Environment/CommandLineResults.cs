using System;
using System.Linq;

namespace Sphere10.Framework {
	public class CommandLineResults {

		public string GetSingleArgumentValue(string argName) {
			if (!Arguments.Contains(argName))
				throw new InvalidOperationException($"Command-line argument not specified: '{argName}'");

			var values = Arguments[argName];
			switch (values.Count()) {
				case 0:
					throw new InvalidOperationException($"Command-line argument did not specify a value: '{argName}'");
				case 1:
					return values.Single();
				default:
					throw new InvalidOperationException($"Command-line argument specified more than one value: '{argName}'");
			}
		}

		public string GetSingleArgumentValueOrDefault(string argName, string defaultValue = null) {
			if (!Arguments.Contains(argName))
				return defaultValue;

			var values = Arguments[argName];
			switch (values.Count()) {
				case 0:
					throw new InvalidOperationException($"Command-line argument did not specify a value: '{argName}'");
				case 1:
					return values.Single();
				default:
					throw new InvalidOperationException($"Command-line argument specified more than one value: '{argName}'");
			}

		}


		public ILookup<string, string> Arguments { get; set; }

		public string CommandName { get; set; } // This is empty for the root CommandLineResults

		public CommandLineResults SubCommand { get; set; } // Should be 1 command here

		public bool HelpRequested { get; set; }
	}
}
