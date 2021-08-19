using System;
using System.Linq;

namespace Sphere10.Framework {
	public class CommandLineResults {

		public bool HasArgument(string argName) => Arguments.Contains(argName);

		public bool TryGetArgumentValues(string argName, out string[] values) {
			if (!Arguments.Contains(argName)) {
				values = Array.Empty<string>();
				return false;
			}
			values = Arguments[argName].ToArray();
			return true;
		}

		public bool TryGetSingleArgumentValue(string argName, out string value) {
			if (TryGetArgumentValues(argName, out var values)) {
				if (values.Length == 1) {
					value = values[0];
					return true;
				}
			}
			value = null;
			return false;
		}
		
		public string GetSingleArgumentValue(string argName) {
			if (!TryGetSingleArgumentValue(argName, out var value)) {
				throw new InvalidOperationException($"Command-line argument did not specify a single value: '{argName}'");
			}
			return value;
		}

		public string GetSingleArgumentValueOrDefault(string argName, string defaultValue = null) {
			if (!TryGetSingleArgumentValue(argName, out var value)) {
				return defaultValue;
			}
			return value;
		}

		public bool TryGetEnumArgument<TEnum>(string argName, out TEnum value) where TEnum : struct {
			if (TryGetSingleArgumentValue(argName, out var strValue)) {
				return Enum.TryParse(strValue, out value);
			}
			value = default;
			return false;
		}

		public TEnum GetEnumArgument<TEnum>(string argName) where TEnum : struct {
			if (!TryGetEnumArgument<TEnum>(argName, out var value))
				throw new InvalidOperationException($"Argument '{argName}' had an unrecognized value");
			return value;
		}


		public ILookup<string, string> Arguments { get; set; }

		public string CommandName { get; set; } // This is empty for the root CommandLineResults

		public CommandLineResults SubCommand { get; set; } // Should be 1 command here

		public bool HelpRequested { get; set; }
	}
}
