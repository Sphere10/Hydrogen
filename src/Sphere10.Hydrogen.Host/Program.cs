using Sphere10.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace Sphere10.Hydrogen.Host {

	class Program {
		public static CommandLineArgs Arguments = new CommandLineArgs {
			Header = new[] {
				"Hydrogen Host v1.0",
				"Copyright (c) Sphere 10 Software 2021 - {CurrentYear}"
			},

			Arguments = new CommandLineArg[] {
				new("development", "Used during development only"),
				new("node", "Path to the node assembly which is started by the host"),
			},

			Options = CommandLineArgOptions.CaseSensitive | CommandLineArgOptions.DoubleDash | CommandLineArgOptions.PrintHelpOnH | CommandLineArgOptions.PrintHelpOnHelp,

			Footer = new[] {
				"NOTE: The Hydrogen Host will forward all arguments marked [N] above to the Hydrogen Node which is launched as a child-process."
			}
		};

		private static void RunNode(string nodeExecutable, CancellationToken stopNodeToken) {
			var nodeProcess = new Process();
			using var pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
			nodeProcess.StartInfo.FileName = nodeExecutable;
			nodeProcess.StartInfo.Arguments = pipeServer.GetClientHandleAsString();
			nodeProcess.StartInfo.UseShellExecute = false;
			nodeProcess.Start();
			pipeServer.DisposeLocalCopyOfClientHandle();
			using var streamWriter = new StreamWriter(pipeServer) {
				AutoFlush = true
			};
			stopNodeToken.Register(() => streamWriter.WriteLine("ABORT"));
			nodeProcess.WaitForExit();
			nodeProcess.Close();
		}

		private static void MonitorHydrogenApplicationUpdate(CancellationTokenSource stopNodeCancellationTokenSource) {

		}

		private static string GetDevelopmentNodeExecutable() {
			const string nodeProject = "Sphere10.Hydrogen.Node";
			const string nodeExecutable = "Sphere10.Hydrogen.Node.exe";
#if DEBUG
			const string buildConfiguration = "Debug";
#elif RELEASE
			const string BuildConfiguration = "Release";
#else
#error Unrecognized build configuration
#endif
			var hostExecutable = Assembly.GetEntryAssembly()?.Location;

			if (string.IsNullOrEmpty(hostExecutable) || !Path.IsPathFullyQualified(hostExecutable))
				throw new SoftwareException("Development mode can only be executed from a file-system");

			var srcDir = Tools.FileSystem.GetParentDirectoryPath(hostExecutable, 5);
			return Path.Combine(srcDir, nodeProject, "bin", buildConfiguration, "net5.0", nodeExecutable);
		}


		static void Main(string[] args) {
			var stopNodeCancellationTokenSource = new CancellationTokenSource();
			try {
				// Note: use CommandLineArgs
				var nodeExecutable = GetDevelopmentNodeExecutable();
				RunNode(nodeExecutable, stopNodeCancellationTokenSource.Token);
			}
			catch (Exception error) {
				Console.WriteLine($"Hydrogen host terminated abnormally.");
				Console.Write(error.ToDiagnosticString());
			}

		}
	}
}
