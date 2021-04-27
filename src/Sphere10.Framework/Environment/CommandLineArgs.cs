using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sphere10.Framework {

	public class CommandLineArgs {
		public string[] Header { get; }
		
		public string[] Footer { get; }

		public CommandLineArgOptions Options { get; } =
			CommandLineArgOptions.DoubleDash | CommandLineArgOptions.SingleDash | CommandLineArgOptions.PrintHelpOnHelp |
			CommandLineArgOptions.ForwardSlash | CommandLineArgOptions.PrintHelpOnH;

		public CommandLineArg[] Arguments { get; }

		public CommandLineArgs(string[] header, string[] footer, CommandLineArg[] arguments, CommandLineArgOptions? options = null) {
			Guard.ArgumentNotNull(header, nameof(header));
			Guard.ArgumentNotNull(arguments, nameof(arguments));
			Guard.ArgumentNotNull(footer, nameof(footer));

			Guard.Argument(header.Length > 0, nameof(header), "Header message is required.");
			Guard.Argument(arguments.Length > 0, nameof(arguments), "At least one argument is required.");

			if (options is not null) {
				Options = options.Value;
			}

			Header = header;
			Footer = footer;
			Arguments = arguments;
		}

		public bool TryParse(string[] args, out ILookup<string, string> results, out string[] messages) {
			var parsedArgs = ParseArgsToLookup(args);
			var validArgs = new LookupEx<string, string>();
			var messagesList = new List<string>();

			if (Options.HasFlag(CommandLineArgOptions.PrintHelpOnH)) {
				if (parsedArgs.Contains("H")) {
					PrintHelp();
					messages = new string[0];
					results = validArgs;
					return false;
				}
			}

			if (Options.HasFlag(CommandLineArgOptions.PrintHelpOnHelp)) {
				if (parsedArgs.Contains("Help")) {
					PrintHelp();
					messages = new string[0];
					results = validArgs;
					return false;
				}
			}

			foreach (var argument in Arguments) {
				var isValid = true;

				if (argument.Dependencies.Any()) {
					foreach (var dependency in argument.Dependencies) {
						if (!parsedArgs.Contains(dependency)) {
							messagesList.Add($"Argument {argument.Name} has unmet dependency {dependency}.");
							isValid = false;
						}
					}
				}

				if (argument.Traits.HasFlag(CommandLineArgTraits.Mandatory))
					if (!parsedArgs.Contains(argument.Name)) {
						messagesList.Add($"Argument {argument.Name} is required.");
						isValid = false;
					}

				if (!argument.Traits.HasFlag(CommandLineArgTraits.Multiple))
					if (parsedArgs.CountForKey(argument.Name) > 1) {
						messagesList.Add($"Argument {argument.Name} supplied more than once but does not support multiple values.");
						isValid = false;
					}

				if (isValid && parsedArgs.Contains(argument.Name))
					validArgs.AddRange(argument.Name, parsedArgs[argument.Name]);
			}

			messages = messagesList.ToArray();
			results = validArgs;
			return !messages.Any();
		}

		public void PrintHelp() {
			foreach (var line in Header)
				Console.WriteLine(line);

			foreach (var arg in Arguments)
				Console.WriteLine(arg.Description);

			foreach (var line in Footer) {
				Console.WriteLine(line);
			}
		}

		private string BuildArgNameMatchPattern() {
			var builder = new StringBuilder();
			builder.Append("(");

			var hasSingle = Options.HasFlag(CommandLineArgOptions.SingleDash);
			var hasSlash = Options.HasFlag(CommandLineArgOptions.ForwardSlash);
			var hasDouble = Options.HasFlag(CommandLineArgOptions.DoubleDash);

			if (hasSingle || hasSlash) {
				builder.Append("[");

				if (Options.HasFlag(CommandLineArgOptions.ForwardSlash))
					builder.Append("/");

				if (Options.HasFlag(CommandLineArgOptions.SingleDash))
					builder.Append("-");

				builder.Append("]");

				if (hasDouble)
					builder.Append("|");
			}

			if (hasDouble)
				builder.Append("--");

			builder.Append(")");
			return builder.ToString();
		}

		private LookupEx<string, string> ParseArgsToLookup(string[] args) {
			var lookupEx = new LookupEx<string, string>(Options.HasFlag(CommandLineArgOptions.CaseSensitive)
				? StringComparer.Ordinal
				: StringComparer.OrdinalIgnoreCase);
			var parameterMatchPattern = BuildArgNameMatchPattern();

			var regex = new Regex("^" + parameterMatchPattern + @"{1}(?<name>\w+)([:=])?(?<value>.+)?$",
				RegexOptions.IgnoreCase | RegexOptions.Compiled);
			char[] trimChars = { '"', '\'', ' ' };

			foreach (var arg in args) {
				var part = regex.Match(arg);

				if (part.Success) {
					var parameter = part.Groups["name"].Value;
					lookupEx.Add(parameter, part.Groups["value"].Value.Trim(trimChars));
				}
			}

			return lookupEx;
		}
	}
}
