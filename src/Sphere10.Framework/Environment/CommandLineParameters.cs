using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sphere10.Framework {

	public class CommandLineParameters {

		private const int ArgumentLineLengthPadded = 15;

		private readonly string[] _parameterPrefixes = new[] { "-", "--", "/" };

		public CommandLineParameters() : this(null, null, null, null) {
		}

		public CommandLineParameters(string[] header, string[] footer, CommandLineParameter[] parameters, CommandLineArgumentOptions options = CommandLineArgumentOptions.Default)
			: this(header, footer, parameters, null, options) {
		}

		public CommandLineParameters(string[] header, string[] footer, CommandLineParameter[] parameters, CommandLineCommand[] commands, CommandLineArgumentOptions options = CommandLineArgumentOptions.Default) {
			Guard.Argument(options > 0, nameof(options), "Argument options must allow at least one argument format option.");
			header ??= new string[0];
			footer ??= new string[0];
			commands ??= new CommandLineCommand[0];
			parameters ??= new CommandLineParameter[0];
			Options = options;
			Header = header;
			Footer = footer;
			Parameters = parameters;
			Commands = commands;
		}

		public string[] Header { get; init; } = Array.Empty<string>();

		public string[] Footer { get; init; } = Array.Empty<string>();

		public CommandLineParameter[] Parameters { get; init; } = Array.Empty<CommandLineParameter>();

		public CommandLineCommand[] Commands { get; init; } = Array.Empty<CommandLineCommand>();

		public CommandLineArgumentOptions Options { get; init; } =
			CommandLineArgumentOptions.DoubleDash | CommandLineArgumentOptions.SingleDash | CommandLineArgumentOptions.PrintHelpOnHelp |
			CommandLineArgumentOptions.ForwardSlash | CommandLineArgumentOptions.PrintHelpOnH;

		public Result<CommandLineResults> TryParseArguments(string[] args) {
			Guard.ArgumentNotNull(args, nameof(args));

			var parseResults = Result<CommandLineResults>.Default;
			var argResults = new LookupEx<string, string>();
			var lastResult = new CommandLineResults(new Dictionary<string, CommandLineResults>(), argResults);
			parseResults.Value = lastResult;

			var parsedCommands = ParseCommands(args);
			var parsedArgs = ParseArgsToLookup(args);

			if (Options.HasFlag(CommandLineArgumentOptions.PrintHelpOnH)) {
				if (parsedArgs.Contains("H") || parsedArgs.Contains("h")) {
					argResults.Add("Help", string.Empty);
				}
			}

			if (Options.HasFlag(CommandLineArgumentOptions.PrintHelpOnHelp)) {
				if (parsedArgs.Contains("Help") || parsedArgs.Contains("help")) {
					argResults.Add("Help", string.Empty);
				}
			}

			foreach (var argument in Parameters) {
				if (argument.Dependencies.Any()) {
					foreach (var dependency in argument.Dependencies) {
						if (!parsedArgs.Contains(dependency)) {
							parseResults.AddError($"Argument {argument.Name} has unmet dependency {dependency}.");
						}
					}
				}

				if (argument.Traits.HasFlag(CommandLineParameterOptions.Mandatory))
					if (!parsedArgs.Contains(argument.Name)) {
						parseResults.AddError($"Argument {argument.Name} is required.");
					}

				if (!argument.Traits.HasFlag(CommandLineParameterOptions.Multiple))
					if (parsedArgs.CountForKey(argument.Name) > 1) {
						parseResults.AddError($"Argument {argument.Name} supplied more than once but does not support multiple values.");
					}

				if (parseResults.Success && parsedArgs.Contains(argument.Name))
					argResults.AddRange(argument.Name, parsedArgs[argument.Name]);
			}

			CommandLineCommand command = null;

			foreach (var commandName in parsedCommands) {
				string name = commandName;
				command = command?.SubCommands
							  .FirstOrDefault(x => x.Name == name)
						  ?? Commands.FirstOrDefault(x => x.Name == commandName);

				if (command is null) {
					parseResults.AddError($"Unknown command {commandName}.");
					break;
				}

				var commandArgResults = new LookupEx<string, string>();
				var commandResult = new Dictionary<string, CommandLineResults>();

				foreach (var argument in command.Parameters) {
					if (argument.Dependencies.Any()) {
						foreach (var dependency in argument.Dependencies) {
							if (!parsedArgs.Contains(dependency)) {
								parseResults.AddError($"Argument {argument.Name} has unmet dependency {dependency}.");
							}
						}
					}

					if (argument.Traits.HasFlag(CommandLineParameterOptions.Mandatory))
						if (!parsedArgs.Contains(argument.Name)) {
							parseResults.AddError($"Argument {argument.Name} is required.");
						}

					if (!argument.Traits.HasFlag(CommandLineParameterOptions.Multiple))
						if (parsedArgs.CountForKey(argument.Name) > 1) {
							parseResults.AddError($"Argument {argument.Name} supplied more than once but does not support multiple values.");
						}

					if (parseResults.Success && parsedArgs.Contains(argument.Name))
						commandArgResults.AddRange(argument.Name, parsedArgs[argument.Name]);
				}

				if (parseResults.Failure)
					break;

				var result = new CommandLineResults(commandResult, commandArgResults);
				lastResult.Commands.Add(commandName, result);
				lastResult = result;
			}

			return parseResults;
		}

		public void PrintHelp() {

			List<string> GetNameOptions(CommandLineParameter arg) {
				var nameOptions = new List<string>();
				var hasValue = arg.Traits.HasFlag(CommandLineParameterOptions.RequiresValue);
				if (Options.HasFlag(CommandLineArgumentOptions.SingleDash))
					nameOptions.Add($"-{arg.Name}{(hasValue ? " <value>" : "")}");

				if (Options.HasFlag(CommandLineArgumentOptions.DoubleDash))
					nameOptions.Add($"--{arg.Name}{(hasValue ? " <value>" : "")}");

				if (Options.HasFlag(CommandLineArgumentOptions.ForwardSlash))
					nameOptions.Add($"/{arg.Name}{(hasValue ? " <value>" : "")}");

				return nameOptions;
			}

			void PrintCommands(IEnumerable<CommandLineCommand> commands, int level = 1) {
				string itemIndentation = string.Empty.PadRight(level * 2);

				foreach (var command in commands) {
					string line = (itemIndentation + command.Name).PadRight(ArgumentLineLengthPadded) + "\t\t" + command.Description;
					Console.WriteLine(line);

					foreach (var arg in command.Parameters) {
						var nameOptions = GetNameOptions(arg);
						for (int i = 0; i < nameOptions.Count; i++) {
							if (i < nameOptions.Count - 1)
								Console.WriteLine(itemIndentation + nameOptions[i]);
							else
								Console.WriteLine((itemIndentation + nameOptions[i]).PadRight(ArgumentLineLengthPadded) + "\t\t" + arg.Description);
						}
					}

					if (command.SubCommands.Any()) {
						level++;
						PrintCommands(command.SubCommands, level);
					}
				}
			}

			PrintHeader();

			Console.WriteLine(string.Empty);

			if (Parameters.Any()) {
				Console.WriteLine("Arguments:");
				foreach (var arg in Parameters) {
					var nameOptions = GetNameOptions(arg);
					for (int i = 0; i < nameOptions.Count; i++) {
						if (i < nameOptions.Count - 1)
							Console.WriteLine("  " + nameOptions[i]);
						else
							Console.WriteLine(("  " + nameOptions[i]).PadRight(ArgumentLineLengthPadded) + "\t\t" + arg.Description);
					}
				}
			}

			Console.WriteLine("Commands:");
			PrintCommands(Commands);

			foreach (var line in Footer) {
				Console.WriteLine(line);
			}
		}

		public void PrintHeader() {
			foreach (var line in Header)
				Console.WriteLine(line);
		}

		private string BuildArgNameMatchPattern() {
			var builder = new StringBuilder();
			builder.Append("(");

			var hasSingle = Options.HasFlag(CommandLineArgumentOptions.SingleDash);
			var hasSlash = Options.HasFlag(CommandLineArgumentOptions.ForwardSlash);
			var hasDouble = Options.HasFlag(CommandLineArgumentOptions.DoubleDash);

			if (hasSingle || hasSlash) {
				builder.Append("[");

				if (Options.HasFlag(CommandLineArgumentOptions.ForwardSlash))
					builder.Append("/");

				if (Options.HasFlag(CommandLineArgumentOptions.SingleDash))
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
			var parameterPrefixes = new List<string>();

			if (Options.HasFlag(CommandLineArgumentOptions.DoubleDash))
				parameterPrefixes.Add("--");

			if (Options.HasFlag(CommandLineArgumentOptions.SingleDash))
				parameterPrefixes.Add("-");

			if (Options.HasFlag(CommandLineArgumentOptions.ForwardSlash))
				parameterPrefixes.Add("/");

			bool TryExtractParameter(string arg, out string name) {
				name = null;
				foreach (var prefix in parameterPrefixes) {
					if (arg.StartsWith(prefix)) {
						name = arg.TrimStart(prefix);

						return true;
					}
				}

				return false;
			}

			var lookupEx = new LookupEx<string, string>(Options.HasFlag(CommandLineArgumentOptions.CaseSensitive)
				? StringComparer.Ordinal
				: StringComparer.OrdinalIgnoreCase);

			string parameter = null;

			foreach (var arg in args) {
				if (IsParameter(arg)) {

					if (parameter is not null && lookupEx.CountForKey(parameter) == 0)
						lookupEx.Add(parameter, null);

					parameter = null;
					if (TryExtractParameter(arg, out var parsedParameter)) {
						if (TryParseValueWithParameter(parsedParameter, out var parameterWithoutValue, out var value))
							lookupEx.Add(parameterWithoutValue, value);

						parameter = parsedParameter;
					}
				} else {
					if (parameter is not null)
						lookupEx.Add(parameter, arg);
				}
			}

			if (parameter is not null && lookupEx.CountForKey(parameter) == 0)
				lookupEx.Add(parameter, null);

			return lookupEx;
		}

		private string[] ParseCommands(string[] args) {
			var results = new List<string>();

			for (int i = 0; i < args.Length; i++) {
				string arg = args[i];

				if (!IsParameter(arg)) {
					results.Add(arg);
				} else
					break;
			}

			return results.ToArray();
		}

		private bool TryParseValueWithParameter(string arg, out string parameter, out string value) {
			var regex = new Regex(@"^(?<name>\w+)([:=])?(?<value>.+)?$");
			var match = regex.Match(arg);

			value = null;
			parameter = null;

			if (match.Success) {
				char[] trimChars = { '"', '\'', ' ' };
				parameter = match.Groups["name"].Value;
				var valueGroup = match.Groups["value"].Value;
				value = string.IsNullOrEmpty(valueGroup) ? null : valueGroup.Trim(trimChars);
			}

			return parameter is not null && value is not null;
		}

		private bool IsParameter(string arg) => _parameterPrefixes.Any(arg.StartsWith);
	}
}
