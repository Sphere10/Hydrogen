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

		public static CommandLineArgs Arguments = new CommandLineArgs(
			new[] {
				"Hydrogen Node v1.0",
				"Copyright (c) Sphere 10 Software 2021 - {CurrentYear}"
			},
			new string[0],
			new CommandLineArg[] {
				new("service", "Run the node in the background"),
				new("host", "The handle provided by the host process for anonymous pipe IPC"),
			},
			CommandLineArgOptions.CaseSensitive | CommandLineArgOptions.DoubleDash | CommandLineArgOptions.PrintHelpOnH |
			CommandLineArgOptions.PrintHelpOnHelp);


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

			bool parsed = Arguments.TryParse(args, out var results, out var messages);
			
			var hasHost = args.Length > 0;
			var hostHandle = hasHost ? args[0] : null;
			try {
				Sphere10Framework.Instance.StartFramework();
				var stopNodeTokenSource = new CancellationTokenSource();
				var stopListeningToHostTokenSource = new CancellationTokenSource();
				if (hasHost) {
					var hostListenTask = new Task(() => ListenToHostCommands(args[0], stopNodeTokenSource),
						stopListeningToHostTokenSource.Token);
					hostListenTask.Start();
				}
				Navigator.Start(stopNodeTokenSource.Token);
				if (hasHost)
					stopListeningToHostTokenSource.Cancel();
			} catch (Exception error) {
				SystemLog.Info("Hydrogen Node terminated abnormally");
				SystemLog.Exception(error);
			}
		}
	}
}
