// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

public class Program {
	static void Main(string[] args) {
	}
}


//using Hydrogen;
//using System;
//using System.IO;
//using System.IO.Pipes;
//using System.Linq;
//using System.Net.Sockets;
//using System.Reflection;
//using System.Security.Policy;
//using System.Threading;
//using System.Threading.Tasks;
//using Hydrogen.DApp.Node;
//using Hydrogen.Application;
//using Hydrogen.DApp.Node.UI;
//using Hydrogen.DApp.Node.RPC;
//using Hydrogen.DApp.Core.Maths;
//using Hydrogen.DApp.Core.Mining;
//using Hydrogen.DApp.Core.Consensus.Serializers;
//using Hydrogen.DApp.Node.UI.Components;

//using Hydrogen.CryptoEx;
//using Hydrogen.DApp.Core.Runtime;

//namespace Hydrogen.DApp.Node {

//	class Program {


//		private static CommandLineParameters Arguments = new CommandLineParameters() {
//			Header = new[] {
//				"HydrogenP2P Node {CurrentVersion}",
//				"Copyright (c) Sphere 10 Software 2021 - {CurrentYear}"
//			},

//			Parameters = new CommandLineParameter[] {
//				new("host", "Host read/write ports ", CommandLineParameterOptions.Optional | CommandLineParameterOptions.RequiresValue),
//			},

//			Options = CommandLineArgumentOptions.DoubleDash | CommandLineArgumentOptions.PrintHelpOnH
//		};


//		static async Task Main(string[] args) {
//			var userArgsResult = Arguments.TryParseArguments(args);
//			if (userArgsResult.Failure) {
//				userArgsResult.ErrorMessages.ForEach(Console.WriteLine);
//				return;
//			}
//			var userArgs = userArgsResult.Value;
//			if (userArgs.HelpRequested) {
//				Arguments.PrintHelp();
//				return;
//			}

//			HydrogenFramework.Instance.StartFramework();

//			var stopNodeTokenSource = new CancellationTokenSource();

//			INode hostedNode = default;
//			Task hostedNodeRunner = default;
//			if (userArgs.Arguments.Contains("host")) {
//				var hostParams = userArgs.Arguments["host"].Single();
//				var splits = hostParams.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
//				if (splits.Length != 2 || !int.TryParse(splits[0], out var readPort) || !int.TryParse(splits[1], out var writePort)) {
//					Console.WriteLine("Invalid format for host read/write port");
//					return;
//				}
//				//hostedNode = new Hydrogen.DApp.Core.Runtime.Node()
//				//hostedNodeRunner = hostedNode.Run(stopNodeTokenSource.Token);
//			}

//			Navigator.Start(stopNodeTokenSource.Token);

//			if (hostedNode != default) {
//				await hostedNode.RequestShutdown();
//				await hostedNodeRunner;
//			}
//		}
//	}
//}
