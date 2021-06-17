//using Sphere10.Framework;
//using Sphere10.Framework.Communications;
//using Sphere10.Hydrogen.Core.HAP;
//using Sphere10.Hydrogen.Core.Storage;
//using System;
//using System.Configuration;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Net.Sockets;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using Sphere10.Hydrogen.Core.Protocols.Host;

//namespace Sphere10.Hydrogen.Host {



//	class Program {
//		private static CommandLineParameters Arguments = new CommandLineParameters() {
//			Header = new[] {
//				"HydrogenP2P Host {CurrentVersion}",
//				"Copyright (c) Sphere 10 Software 2021 - {CurrentYear}"
//			},

//			Footer = new[] {
//				"NOTE: The Hydrogen Host will forward all arguments marked [N] above to the Hydrogen Node which is launched as a child-process."
//			},

//			Parameters = new CommandLineParameter[] {
//				new("path", "Root path for Hydrogen Application and location of where deployments occur", CommandLineParameterOptions.Optional | CommandLineParameterOptions.RequiresValue),
//				new("deploy", "Path to an Hydrogen Application Package (HAP) to deploy ", CommandLineParameterOptions.Optional | CommandLineParameterOptions.RequiresValue),
//				new("development", "Used for development and internal use only", CommandLineParameterOptions.Optional),
//			},

//			Options = CommandLineArgumentOptions.DoubleDash | CommandLineArgumentOptions.PrintHelpOnH
//		};

//		private static HydrogenApplicationFolders Folders { get; set; }

//		private static bool DevelopmentMode { get; set; }

//		public static Protocol BuildForHost<TChannel>(ILogger logger) where TChannel : ProtocolChannel
//			=> new ProtocolBuilder<TChannel>()
//				.Requests
//					.ForRequest<Ping>().RespondWith((_, _) => new Pong())
//				.Responses
//					.ForResponse<Pong>().ToRequest<Ping>().HandleWith((_, _, _) => { })
//				.Commands
//					.ForCommand<Upgrade>().Execute(async (_, upgrade) => await DeployHAP(upgrade.HydrogenApplicationPackagePath))
//				.Messages
//					.Use(HostProtocolHelper.BuildMessageSerializer())
//				.Build();

//		private static void RunNode(string nodeExecutable, CancellationToken stopNodeToken) {
//			var nodeProcess = new Process();
//			using var pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
//			nodeProcess.StartInfo.FileName = nodeExecutable;
//			nodeProcess.StartInfo.Arguments = pipeServer.GetClientHandleAsString();
//			nodeProcess.StartInfo.UseShellExecute = false;
//			nodeProcess.Start();
//			pipeServer.DisposeLocalCopyOfClientHandle();
//			using var streamWriter = new StreamWriter(pipeServer) {
//				AutoFlush = true
//			};
//			stopNodeToken.Register(() => streamWriter.WriteLine("ABORT"));
//			//Thread.Sleep(5000);
//			//streamWriter.WriteLine("ABORT");
//			nodeProcess.WaitForExit();
//			nodeProcess.Close();
//		}

//		private static async Task DeployHAP(string newHapPath) {
//			Guard.Ensure(!DevelopmentMode, "Cannot deploy HAP in development mode");
//			Guard.FileExists(newHapPath);
//			await Tools.FileSystem.DeleteAllFilesAsync(Folders.HapPath, true);
//			var zipPackage = new ZipPackage(newHapPath);
//			zipPackage.ExtractTo(Folders.HapPath);
//		}


//		private static string GetDevelopmentNodeExecutable() {
//			const string nodeProject = "Sphere10.Hydrogen.Node";
//			const string nodeExecutable = "Sphere10.Hydrogen.Node.exe";
//#if DEBUG
//			const string buildConfiguration = "Debug";
//#elif RELEASE
//			const string buildConfiguration = "Release";
//#else
//#error Unrecognized build configuration
//#endif
//			var hostExecutable = Assembly.GetEntryAssembly()?.Location;

//			if (string.IsNullOrEmpty(hostExecutable) || !Path.IsPathFullyQualified(hostExecutable))
//				throw new SoftwareException("Development mode can only be executed from a file-system");

//			var srcDir = Tools.FileSystem.GetParentDirectoryPath(hostExecutable, 5);
//			return Path.Combine(srcDir, nodeProject, "bin", buildConfiguration, "net5.0", nodeExecutable);
//		}

//		static async Task Main(string[] args) {
//			try {
//				var defaultDeployPath = ConfigurationManager.AppSettings["DefaultDeployPath"];
//				var userArgsResult = Arguments.TryParseArguments(args);
//				if (userArgsResult.Failure) {
//					userArgsResult.ErrorMessages.ForEach(Console.WriteLine);
//					return;
//				}
//				var userArgs = userArgsResult.Value;
//				if (userArgs.HelpRequested) {
//					Arguments.PrintHelp();
//					return;
//				}

//				// Get user override deployment path (if applicable)
//				if (userArgs.Parameters.Contains("path"))
//					defaultDeployPath = userArgs.Parameters["path"].Single();

//				// setup folders
//				var appRoot = userArgs.Parameters.Contains("path") ? userArgs.Parameters["path"].Single() : Tools.Text.FormatEx(ConfigurationManager.AppSettings["AppRoot"]);


//				// setup logger


//				// Deploy user specified HAP (if applicable)
//				if (userArgs.Parameters.Contains("deploy")) {
//					await DeployHAP(defaultDeployPath);
//				}





//				RunNode(nodeExecutable, stopNodeCancellationTokenSource.Token);
//			} catch (Exception error) {
//				Console.WriteLine($"Hydrogen host terminated abnormally.");
//				Console.Write(error.ToDiagnosticString());
//			}
//		}
//	}
//}
