// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen;
using Hydrogen.Communications;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AbstractProtocol.UDPSimple;

public class Program {
	public static async Task Main(string[] args) {
		//// Register a console logger
		SystemLog.RegisterLogger(new TimestampLogger(new ConsoleLogger()));

		var serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 100);
		var clientEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 101);

		try {
			// Build the server/client as separate threads
			var endpoint1 = Task.Run(async () => {
				//////////
				// Server
				//////////

				// 1. Create the channel 
				var channel = new UDPChannel(serverEndpoint, clientEndpoint, CommunicationRole.Server);
				// note: you generally do not need to worry about channels when using an orchestrator as it's all handled for you. 
				channel.ReceivedBytes += (b) => SystemLog.Info($"[SERVER]: Received {b.Length} bytes");
				channel.SentBytes += (b) => SystemLog.Info($"[SERVER]: Sent {b.Length} bytes");
				channel.Opening += () => SystemLog.Info("[SERVER]: Opening");
				channel.Opened += () => SystemLog.Info("[SERVER]: Opened");
				channel.Closing += () => SystemLog.Info("[SERVER]: Closing");
				channel.Closed += () => SystemLog.Info("[SERVER]: Closed");


				// 2. Create the protocol
				var protocol = AppProtocol.Build();

				// 3. Create the orchestrator
				var orchestator = new ProtocolOrchestrator(channel, protocol);
				orchestator.ReceivedMessage += (message) => SystemLog.Info($"[SERVER] Received Message 1: {message}");
				orchestator.SentMessage += (message) => SystemLog.Info($"[SERVER] Sent Message 1: {message}");
				orchestator.StateChanged +=
					(state) => SystemLog.Info($"[SERVER] Orchestration state changed to {state}");
				orchestator.MessageError += (queue, envelope, error) =>
					SystemLog.Error(
						$"[SERVER] Queue: {queue}, Message: {envelope}, Error: {error.ToDiagnosticString()}");

				// 4. Start the orchestrator
				// NOTE: this will open the channel if not already opened and commence the receive loop. It will
				// handle requests/responses in the background until finished.

				await orchestator.Start();

				// 5. Run for 2 seconds. 
				// NOTE: we cannot use orchestator.RunToEnd in UDPChannels because channels cannot detect when remote endpoint closes.
				await Task.Delay(2000);

				// 6. Finish the client
				await orchestator.Finish();
			});

			var endpoint2 = Task.Run(async () => {
				//////////
				// Client
				//////////

				// 1. Create the channel
				// note: the role is now Client but in UDPChannel it doesn't really change anything, but it IS
				// needed for handshaking.
				var channel = new UDPChannel(clientEndpoint, serverEndpoint, CommunicationRole.Client);

				// note: you generally do not need to worry about channels when using an orchestrator as it's all handled for you. 
				channel.ReceivedBytes += (b) => SystemLog.Info($"[CLIENT]: Received {b.Length} bytes");
				channel.SentBytes += (b) => SystemLog.Info($"[CLIENT]: Sent {b.Length} bytes");
				channel.Opening += () => SystemLog.Info("[CLIENT]: Opening");
				channel.Opened += () => SystemLog.Info("[CLIENT]: Opened");
				channel.Closing += () => SystemLog.Info("[CLIENT]: Closing");
				channel.Closed += () => SystemLog.Info("[CLIENT]: Closed");

				// 2. Create the protocol
				var protocol = AppProtocol.Build();

				// 3. Create the orchestrator
				var orchestator = new ProtocolOrchestrator(channel, protocol);
				orchestator.ReceivedMessage += (message) => SystemLog.Info($"[CLIENT] Received Message 1: {message}");
				orchestator.SentMessage += (message) => SystemLog.Info($"[CLIENT] Sent Message 1: {message}");
				orchestator.StateChanged +=
					(state) => SystemLog.Info($"[CLIENT] Orchestration state changed to {state}");
				orchestator.MessageError += (queue, envelope, error) =>
					SystemLog.Error(
						$"[CLIENT] Queue: {queue}, Message: {envelope}, Error: {error.ToDiagnosticString()}");

				// 4. Start the orchestrator
				// note: this will open the channel if not already opened and commence the receive loop. It will
				// handle requests/responses in the background until finished.
				await orchestator.Start();

				// Let's sleep 200 ms to allow other endpointtask to setup
				await Task.Delay(200);
				orchestator.SendMessage(ProtocolDispatchType.Request, new RequestBytes());

				// 5. Finish the client
				await orchestator.Finish();
			});

			await Task.WhenAll(endpoint1, endpoint2);
		} catch (Exception error) {
			SystemLog.Exception(error);
		}
	}
}
