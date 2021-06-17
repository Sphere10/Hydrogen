using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sphere10.Framework {

	public class CommandLineParameters {

		private const int ArgumentLineLengthPadded = 15;

		private readonly string[] _parameterPrefixes = { "-", "--", "/" };

		private StringComparison NameComparisonCasing => Options.HasFlag(CommandLineArgumentOptions.CaseSensitive) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

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

			var parameters = new LookupEx<string, string>();
			CommandLineResults currentResults = new CommandLineResults {
				Parameters = parameters
			};
			var parseResults = new Result<CommandLineResults>(currentResults);

			CommandLineCommand command = null;
			CommandLineParameter parameter = null;
			foreach (var arg in args) {
				if (IsParameter(arg)) {

					string parameterName = TrimParameterPrefix(arg);
					if (IsHelp(parameterName)) {
						currentResults.HelpRequested = true;
						break;
					}

					var paramOptions = command is null ? Parameters : command.Parameters;
					if (TryParseParameterWithValue(parameterName, out var name, out var value)) {
						//parameter with value, add to results.
						
						parameter = paramOptions.SingleOrDefault(x => string.Equals(x.Name, name, NameComparisonCasing));
						if (parameter is null) {
							parseResults.AddError($"Unknown argument {arg}");
							break;
						}

						parameters.Add(parameter.Name, value);
						parameter = null;
					} else {
						//parameter only, if requires a value set parameter var, else add to results.
						
						var paramName = parameterName;
						parameter = paramOptions.SingleOrDefault(x => string.Equals(x.Name, paramName, NameComparisonCasing));
						if (parameter is null) {
							parseResults.AddError($"Unknown argument {paramName}");
							break;
						}

						if (!parameter.Traits.HasFlag(CommandLineParameterOptions.RequiresValue)) {
							parameters.Add(parameter.Name);
							parameter = null;
						}
					}
				} else {
					// not a parameter
					if (parameter is not null) {
						//value for parameter
						
						parameters.Add(parameter.Name, arg);
						parameter = null;
					} else {
						//command
						
						command = command is null
							? Commands.SingleOrDefault(x => string.Equals(x.Name, arg, NameComparisonCasing))
							: command.SubCommands.SingleOrDefault(x => string.Equals(x.Name, arg, NameComparisonCasing));
						if (command is null) {
							parseResults.AddError($"Unknown argument {arg}");
							break;
						}

						parameters = new LookupEx<string, string>();
						SetSubCommandResult(currentResults,
							new CommandLineResults {
								Parameters = parameters,
								CommandName = command.Name
							});
					}
				}

				if (parseResults.Failure)
					break;
			}

			ValidateParameters(parseResults);

			return parseResults;
		}

		private bool IsHelp(string arg) {
			return Options.HasFlag(CommandLineArgumentOptions.PrintHelpOnHelp) && string.Equals(arg, "Help", NameComparisonCasing)
			       || Options.HasFlag(CommandLineArgumentOptions.PrintHelpOnH) && string.Equals(arg, "H", NameComparisonCasing);
		}

		private void ValidateParameters(Result<CommandLineResults> parseResults) {
			if (parseResults.Success)
				ValidateMandatoryParameters(parseResults);

			if (parseResults.Success)
				ValidateParameterOptions(parseResults);
		}

		private void ValidateParameterOptions(Result<CommandLineResults> parseResults) {
			var results = parseResults.Value;
			ValidateParameterOptionsInner(results);

			void ValidateParameterOptionsInner(CommandLineResults resultsItem) {
				foreach (var parameter in resultsItem.Parameters.Select(x => x.Key)) {

					if (IsHelp(parameter))
						continue;

					CommandLineCommand command;
					TryMatchCommandByName(resultsItem.CommandName, out command);

					CommandLineParameter paramDef = command is null
						? Parameters.Single(x => string.Equals(parameter, x.Name, NameComparisonCasing))
						: command.Parameters.Single(x => string.Equals(parameter, x.Name, NameComparisonCasing));

					foreach (var dependency in paramDef.Dependencies) {
						if (!resultsItem.Parameters.Contains(dependency))
							parseResults.AddError($"Parameter {paramDef.Name} has unmet dependency {dependency}.");
					}

					if (!paramDef.Traits.HasFlag(CommandLineParameterOptions.Multiple)) {
						LookupEx<string, string> lookupEx = results.Parameters as LookupEx<string, string>;
						if (lookupEx!.CountForKey(paramDef.Name) > 1)
							parseResults.AddError($"Parameter {paramDef.Name} supplied more than once but does not support multiple values.");
					}

					if (resultsItem.SubCommand is not null)
						ValidateParameterOptionsInner(resultsItem.SubCommand);
				}
			}
		}

		private void ValidateMandatoryParameters(Result<CommandLineResults> parseResults) {
			var results = parseResults.Value;
			var mandatoryParameters = Parameters.Where(x => x.Traits.HasFlag(CommandLineParameterOptions.Mandatory));
			foreach (var param in mandatoryParameters) {
				if (!results.Parameters.Contains(param.Name)) {
					parseResults.AddError($"Parameter {param.Name} is required.");
				}
			}

			var commandToValidate = results.SubCommand;
			CommandLineCommand commandDef = null;
			while (commandToValidate is not null) {
				commandDef = commandDef is null
					? Commands.Single(x => x.Name == commandToValidate.CommandName)
					: commandDef.SubCommands.Single(x => x.Name == commandToValidate.CommandName);

				foreach (var param in commandDef.Parameters.Where(x => x.Traits.HasFlag(CommandLineParameterOptions.Mandatory))) {
					if (!commandToValidate.Parameters.Contains(param.Name)) {
						parseResults.AddError($"Command {commandToValidate.CommandName} parameter {param.Name} is required.");
					}
				}

				commandToValidate = commandToValidate.SubCommand;
			}
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

		/// <summary>
		/// Sets SubCommand property of the first <see cref="CommandLineResults"/> with no subcommand
		/// result, descending into nested results to find the first unset.
		/// </summary>
		/// <param name="parent"> parent results item</param>
		/// <param name="newResult"> new result to be assigned</param>
		private void SetSubCommandResult(CommandLineResults parent, CommandLineResults newResult) {
			var current = parent;
			while (current.SubCommand is not null) {
				current = current.SubCommand;
			}

			current.SubCommand = newResult;
		}

		/// <summary>
		/// Removes parameter prefix from the given parameter e.g. -- or / and returns result.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		private string TrimParameterPrefix(string parameter) {
			StringBuilder builder = new StringBuilder(parameter);
			_parameterPrefixes.ForEach(x => builder.Replace(x, string.Empty));
			return builder.ToString();
		}

		/// <summary>
		/// Attempts to determine whether given arg is a combination of parameter and its value
		/// separated by space or colon e.g. "parameter=value". return value is whether it is param with value,
		/// and splits into out params.
		/// </summary>
		/// <param name="arg"></param>
		/// <param name="parameter"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private bool TryParseParameterWithValue(string arg, out string parameter, out string value) {
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

		/// <summary>
		/// Whether or not the given arg is a parameter denoted by its prefix
		/// </summary>
		/// <param name="arg"></param>
		/// <returns></returns>
		private bool IsParameter(string arg) => _parameterPrefixes.Any(arg.StartsWith);

		/// <summary>
		/// Attempts to locate a command definition by name. Starts at top most commands, and descends into
		/// nested subcommands until located or no more options.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="command"></param>
		/// <returns></returns>
		private bool TryMatchCommandByName(string name, out CommandLineCommand command) {

			CommandLineCommand[] commands = Commands;
			command = null;

			while (commands.Any() && command is null) {
				command = commands.SingleOrDefault(x => string.Equals(name, x.Name, NameComparisonCasing));

				if (command is null)
					commands = commands.SelectMany(x => x.SubCommands).ToArray();
			}

			return command is null;
		}
	}
}
