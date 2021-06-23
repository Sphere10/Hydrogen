using Sphere10.Framework;
using Sphere10.Framework.Communications;
using Sphere10.Hydrogen.Core.HAP;
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
using Sphere10.Hydrogen.Core.Protocols.Host;

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
				new("development", "Used for development and internal use only", CommandLineParameterOptions.Optional),
			},

			Options = CommandLineArgumentOptions.DoubleDash | CommandLineArgumentOptions.PrintHelpOnH
		};

		private static ILogger Logger { get; set; }

		private static Protocol NodeProtocol { get; set; }

		private static AnonymousPipe NodePipe { get; set; }

		private static HydrogenApplicationPaths Folders { get; set; }

		private static bool DevelopmentMode { get; set; }

		private static bool Upgrading { get; set; }

		private static Protocol BuildProtocol(ILogger logger)
			=> new ProtocolBuilder<AnonymousPipe>()
				.Requests
					.ForRequest<PingMessage>().RespondWith(() => new PongMessage())
				.Responses
					.ForResponse<PongMessage>().ToRequest<PingMessage>().HandleWith(() => Logger.Info("Received Pong"))
				.Commands
					.ForCommand<UpgradeMessage>().Execute(async upgradeMessage => await UpgradeNode(upgradeMessage.HydrogenApplicationPackagePath))
					.ForCommand<ShutdownMessage>().Execute(async () => await NodePipe.Close())
				.Messages
					.Use(HostProtocolHelper.BuildMessageSerializer())
				.Build();

		private static async Task RunHost() {
			var tcs = new TaskCompletionSource<bool>();
			NodePipe.Closed += () => {
				if (!Upgrading)
					tcs.SetResult(true);
			};
			await StartNode();
			var protocolOrchestrator = new ProtocolOrchestrator(NodePipe, NodeProtocol);
			var protocolRunner = protocolOrchestrator.Run();
			await Task.WhenAll(tcs.Task, protocolRunner);
		}

		private static async Task StartNode() {
			Logger.Info("Starting node");
			NodePipe?.Dispose();
			NodePipe = AnonymousPipe.ToChildProcess(Folders.NodeExecutable, NodeProtocol.MessageSerializer, "-hostread {0} -hostwrite {1}", string.Format);
			await NodePipe.Open();
		}

		private static async Task StopNode() {
			Logger.Info("Requesting node shutdown");
			await NodePipe.SendMessage(ProtocolMessageType.Command, new ShutdownMessage());
			if (!await NodePipe.TryWaitClose(TimeSpan.FromMinutes(1)))
				throw new HostException("Node failed to shutdown");
		}

		private static async Task UpgradeNode(string hapPath) {
			Logger.Info($"Upgrading application with: {hapPath}");
			try {
				Upgrading = true;
				await StopNode();
				await DeployHAP(hapPath);
				await StartNode();
			} finally {
				Upgrading = false;
			}
		}

		private static async Task DeployHAP(string newHapPath) {
			Guard.Ensure(!DevelopmentMode, "Cannot deploy HAP in development mode");
			Guard.FileExists(newHapPath);
			await Tools.FileSystem.DeleteAllFilesAsync(Folders.HapFolder, true);
			var zipPackage = new ZipPackage(newHapPath);
			zipPackage.ExtractTo(Folders.HapFolder);
		}


		private static string GetDevelopmentNodeExecutable() {
			const string nodeProject = "Sphere10.Hydrogen.Node";
			const string nodeExecutable = "Sphere10.Hydrogen.Node.exe";
#if DEBUG
			const string buildConfiguration = "Debug";
#elif RELEASE
			const string buildConfiguration = "Release";
#else
#error Unrecognized build configuration
#endif
			var hostExecutable = Assembly.GetEntryAssembly()?.Location;

			if (string.IsNullOrEmpty(hostExecutable) || !Path.IsPathFullyQualified(hostExecutable))
				throw new SoftwareException("Development mode can only be executed from a file-system");

			var srcDir = Tools.FileSystem.GetParentDirectoryPath(hostExecutable, 5);
			return Path.Combine(srcDir, nodeProject, "bin", buildConfiguration, "net5.0", nodeExecutable);
		}

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

				// Get user override deployment path (if applicable)
				if (userArgs.Parameters.Contains("path"))
					defaultDeployPath = userArgs.Parameters["path"].Single();

				// setup folders
				var appRoot = userArgs.Parameters.Contains("path") ? userArgs.Parameters["path"].Single() : Tools.Text.FormatEx(ConfigurationManager.AppSettings["AppRoot"]);

				// setup logger
				Logger = new MulticastLogger(new FileAppendLogger(Folders.HostLog));

				// Build HostProtocol from the hosts perspective
				NodeProtocol = BuildProtocol(Logger);

				// Deploy user specified HAP (if applicable)
				if (userArgs.Parameters.Contains("deploy")) {
					await Task.Run(() => DeployHAP(defaultDeployPath));
				}

				await RunHost();
			} catch (Exception error) {
				Console.WriteLine($"Hydrogen host terminated abnormally.");
				Console.Write(error.ToDiagnosticString());
			}
		}
	}
}
