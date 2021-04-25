using Sphere10.Framework;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using Sphere10.Framework.Application;
using Sphere10.Hydrogen.Node.UI;

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

			Options = CommandLineArgOptions.CaseSensitive | CommandLineArgOptions.DoubleDash | CommandLineArgOptions.PrintHelpOnH | CommandLineArgOptions.PrintHelpOnHelp

		};


		public static void DoDevelopmentFlow() {
			// Load up the Node in sibling folder on dev environment
			var nodeAssemblyPath = GetNodeAssemblyPath();
			var nodeAssembly = Assembly.LoadFrom(nodeAssemblyPath);

			string GetNodeAssemblyPath() {
				var hostExecutable = Assembly.GetEntryAssembly().Location;

				if (!Path.IsPathFullyQualified(hostExecutable))
					throw new SoftwareException("Development mode can only be executed from a file-system");

				
				var srcDir = Tools.FileSystem.GetParentDirectoryPath(hostExecutable, 5); 
				var nodeDir = Path.Combine(srcDir, "Sphere10.Hydrogen.Node", "bin", "Debug","net5.0", "Sphere10.Hydrogen.Node.dll");

				return nodeDir;
			}

		}

		static void Main(string[] args) {
			//if (args.Length == 1) {
			//	Arguments.Print();
			//	Environment.Exit(0);
			//}

			//if (!Arguments.TryParse(args, out var results))      // TryParse will print error
			//	Environment.Exit(-1);
			try {

				DoDevelopmentFlow();
				Sphere10Framework.Instance.StartFramework();
			}
			catch (Exception error) {
				Console.WriteLine($"Hydrogen host terminated abnormally.");
				Console.Write(error.ToDiagnosticString());
			}

		}
	}
}
