using Sphere10.Framework;
using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using Sphere10.Framework.Application;
using Sphere10.Hydrogen.Node;
using Sphere10.Hydrogen.Node.UI;


namespace Sphere10.Hydrogen.Node {

	class Program {

		public static CommandLineParameters Parameters = new(
			new[] {
				"Hydrogen Node v1.0",
				"Copyright (c) Sphere 10 Software 2021 - {CurrentYear}"
			},
			new string[0],
			new[] { new CommandLineParameter("quiet", "No console output") },
			new CommandLineCommand[] {
				new("service", "Run the node in the background",
					new CommandLineParameter[] { new("loglevel", "Service log level - info, warn, error") },
					new CommandLineCommand[] { new("install", "install as service") }),
				new("host", "The handle provided by the host process for anonymous pipe IPC"),
			});

		private static void ListenToHostCommands(string pipeHandleAsString, CancellationTokenSource cts) {
			using var pipeClient = new AnonymousPipeClientStream(PipeDirection.In, pipeHandleAsString);
			using var streamReader = new StreamReader(pipeClient);
			while (true) {
				switch (streamReader.ReadLine()) {
					case "ABORT":
						cts.Cancel();
						break;
				}
			}
		}

		static void Main(string[] args) {
			Result<CommandLineResults> parsed = Parameters.TryParseArguments(args);

			if (parsed.Success) {
				parsed.Value.

			} else if (parsed.Failure) {
				Parameters.PrintHeader();
				foreach (string errorMessage in parsed.ErrorMessages) {
					Console.WriteLine(errorMessage);
				}
			}
			
			if (parsed.Value.HelpRequested) {
				Parameters.PrintHelp();
			}

			//
			// var hasHost = args.Length > 0;
			// var hostHandle = hasHost ? args[0] : null;
			// try {
			// 	Sphere10Framework.Instance.StartFramework();
			// 	var stopNodeTokenSource = new CancellationTokenSource();
			// 	var stopListeningToHostTokenSource = new CancellationTokenSource();
			// 	if (hasHost) {
			// 		var hostListenTask = new Task(() => ListenToHostCommands(args[0], stopNodeTokenSource),
			// 			stopListeningToHostTokenSource.Token);
			// 		hostListenTask.Start();
			// 	}
			// 	Navigator.Start(stopNodeTokenSource.Token);
			// 	if (hasHost)
			// 		stopListeningToHostTokenSource.Cancel();
			// } catch (Exception error) {
			// 	SystemLog.Info("Hydrogen Node terminated abnormally");
			// 	SystemLog.Exception(error);
			// }
		}
	}
}
