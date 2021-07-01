using Sphere10.Framework;
using Sphere10.Framework.Communications;
using Sphere10.Hydrogen.Core.Runtime;
using Sphere10.Hydrogen.Core.Storage;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Sphere10.Hydrogen.Host {


	class Program {
		private static CommandLineParameters Arguments = new CommandLineParameters() {
			Header = new[] {
				"HydrogenP2P Host {CurrentVersion}",
				"Copyright (c) Sphere 10 Software 2021 - {CurrentYear}"
			},

			Footer = new[] {
				"NOTE: The Hydrogen Host will forward all arguments marked [N] above to the Hydrogen Node which is launched as a child-process."
			},

			Parameters = new CommandLineParameter[] {
				new("path", "Root path for Hydrogen Application and location of where deployments occur", CommandLineParameterOptions.Optional | CommandLineParameterOptions.RequiresValue),
				new("deploy", "Path to an Hydrogen Application Package (HAP) to deploy ", CommandLineParameterOptions.Optional | CommandLineParameterOptions.RequiresValue),
				new("verbose", "Logs DEBUG and INFO messages which is useful for diagnostic", CommandLineParameterOptions.Optional),
				new("development", "Used for development and internal use only", CommandLineParameterOptions.Optional),
			},

			Options = CommandLineArgumentOptions.DoubleDash | CommandLineArgumentOptions.PrintHelpOnH
		};


		static async Task Main(string[] args) {
			try {
				var defaultDeployPath = ConfigurationManager.AppSettings["DefaultDeployPath"];
				var userArgsResult = Arguments.TryParseArguments(args);
				if (userArgsResult.Failure) {
					userArgsResult.ErrorMessages.ForEach(Console.WriteLine);
					return;
				}
				var userArgs = userArgsResult.Value;
				if (userArgs.HelpRequested) {
					Arguments.PrintHelp();
					return;
				}

				// Check no custom path or deployable HAP in development mode
				if (userArgs.Parameters.Contains("development")) {
					if (userArgs.Parameters.Contains("path")) {
						Console.WriteLine("Error: Cannot specify an override 'path' in 'development' mode");
						return;
					}
					if (userArgs.Parameters.Contains("deploy")) {
						Console.WriteLine("Error: Cannot deploy a HAP in 'development' mode");
						return;
					}
				}

				// Determine Hydrogen Application Paths
				IApplicationPaths applicationPaths;
				if (userArgs.Parameters.Contains("development")) {
					applicationPaths = new DevelopmentApplicationPaths();
				} else if (userArgs.Parameters.Contains("path")) {
					applicationPaths = new ApplicationPaths(userArgs.Parameters["path"].Single(), true);
				} else {
					applicationPaths = new ApplicationPaths(Tools.Text.FormatEx(ConfigurationManager.AppSettings["AppRoot"]));
				}

				// Setup the logger
				ILogger logger = new MulticastLogger(new FileAppendLogger(applicationPaths.HostLog)) {
					Options = userArgs.Parameters.Contains("verbose")
						? LogOptions.DebugEnabled | LogOptions.InfoEnabled | LogOptions.WarningEnabled | LogOptions.ErrorEnabled
						: LogOptions.WarningEnabled | LogOptions.ErrorEnabled
				};


				// Setup the host
				IHost host = userArgs.Parameters.Contains("development")
					? new DevelopmentHost(logger, applicationPaths)
					: new Sphere10.Hydrogen.Core.Runtime.Host(logger, applicationPaths);


				// Deploy user specified HAP (if applicable)
				if (userArgs.Parameters.Contains("deploy")) {
					await Task.Run(() => host.DeployHAP(defaultDeployPath));
				}

				// Run the host
				await host.Run();
				Console.WriteLine("Hydrogen host terminated succesfully");
			} catch (Exception error) {
				Console.WriteLine($"Hydrogen host terminated abnormally");
				Console.Write(error.ToDiagnosticString());
			}
		}
	}
}
