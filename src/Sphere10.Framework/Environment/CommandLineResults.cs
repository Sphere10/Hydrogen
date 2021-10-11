using System;
using System.Linq;

namespace Sphere10.Framework {
	public class CommandLineResults {

		public bool HasCommand(string name) => this.SubCommand != null && this.SubCommand.CommandName == name;

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

		public bool TryGetSingleArgumentValue<T>(string argName, out T value) {
			if (TryGetSingleArgumentValue(argName, out var stringValue) && GenericParser.TryParse(stringValue, out value)) {
				return true;
			}
			value = default;
			return false;
		}

		public string GetSingleArgumentValue(string argName) {
			if (!TryGetSingleArgumentValue(argName, out var value)) {
				throw new InvalidOperationException($"Command-line argument did not specify a single value: '{argName}'");
			}
			return value;
		}
		
		public T GetSingleArgumentValue<T>(string argName) {
			if (!TryGetSingleArgumentValue<T>(argName, out var value)) {
				throw new InvalidOperationException($"Command-line argument did not specify a single value: '{argName}' of type '{typeof(T).Name}'");
			}
			return value;
		}

		public string GetSingleArgumentValueOrDefault(string argName, string defaultValue = null) 
			=> TryGetSingleArgumentValue(argName, out var value) ? value : defaultValue;

		public T GetSingleArgumentValueOrDefault<T>(string argName, T defaultValue = default) 
			=> TryGetSingleArgumentValue<T>(argName, out var value) ? value : defaultValue;

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
