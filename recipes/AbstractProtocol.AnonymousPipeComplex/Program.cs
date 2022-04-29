using Sphere10.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sphere10.Framework.Communications;
using System.Threading;
using System.Text;

namespace AbstractProtocol.AnonymousPipeComplex {

	/// <summary>
	/// A simple abstract protocol that's communicated over AnonymousPipe channels.
	/// </summary>
	public class Program {

		/// <summary>
		/// These are the command line paramters pass by the Parent process (server) to the client process (server).
		/// </summary>
		public static CommandLineParameters Parameters = new() {
			Commands = new CommandLineCommand[] {
				new("anonymouspipeclient", "AnonymousPipeClient child process tester") {
					Parameters = new CommandLineParameter[] {
						new("read", "AnonymousPipeClient child process tester", CommandLineParameterOptions.Mandatory | CommandLineParameterOptions.RequiresValue),
						new("write", "AnonymousPipeClient child process tester", CommandLineParameterOptions.Mandatory | CommandLineParameterOptions.RequiresValue),
					}
				}
			},
		};

		public static async Task Main(string[] args) {
			bool isClient = false;
			// Parse command line arguments
			var commandLineResults = Parameters.TryParseArguments(args);
			if (commandLineResults.Failure) {
				Environment.Exit(-1);
			}

			// Build the protocol channel
			ProtocolChannel channel = null;
			try {
				if (commandLineResults.Value.SubCommand?.CommandName != "anonymouspipeclient") {
					// This process is running as the server

					// Setup logging for server 
					//ILogger logger = null;
					ILogger logger = new FileAppendLogger("c:/temp/server.log"); // uncomment for diagnostics
					SystemLog.RegisterLogger(new PrefixLogger(new TimestampLogger(new MulticastLogger(new ConsoleLogger(), logger ?? new NoOpLogger())), "[SERVER]: "));

					// The protocol channel here is the (parent process) -> (child process) anonymous pipe. The below will launch
					// a child process passing in the arguments so the child process can run as the client and connect to the server.
					channel = AnonymousPipe.ToChildProcess(
						Tools.Runtime.GetExecutablePath(),
						"anonymouspipeclient --read {0} --write {1}",
						(args, readHandle, writeHandle) => string.Format(args, readHandle, writeHandle)
					);
				} else {
					// This process is running as the client
					isClient = true;

					// Setup logging for client (uncomment for diagnostic of individual log)
					//ILogger logger = null;
					ILogger logger = new FileAppendLogger("c:/temp/client.log"); // uncomment for diagnostics
					SystemLog.RegisterLogger(new PrefixLogger(new TimestampLogger(new MulticastLogger(new ConsoleLogger(), logger ?? new NoOpLogger())), "[CLIENT]: "));

					// Get the server's endpoint details from command line (passed in by the parent process when launches child process)
					var endpoint = new AnonymousPipeEndpoint {
						ReaderHandle = commandLineResults.Value.SubCommand.GetSingleArgumentValue<string>("read"),
						WriterHandle = commandLineResults.Value.SubCommand.GetSingleArgumentValue<string>("write"),
					};

					// The protocol channel here is the (child process) -> (parent process) anonymous pipe
					channel = AnonymousPipe.FromChildProcess(endpoint);
				}

				// NOTE: the code below this point runs identically for client/server process. 

				// subscribe to events for logging
				channel.Opening += () => SystemLog.Info("Opening");
				channel.Opened += () => SystemLog.Info("Opened");
				//channel.Handshake += () => SystemLog.Info("Handshake");
				channel.Closing += () => SystemLog.Info("Closing");
				channel.Closed += () => SystemLog.Info("Closed");
				//channel.ReceivedBytes += (byteCount) => SystemLog.Info($"Received Bytes: {byteCount.Length} total");
				//((AnonymousPipe)channel).ReceivedString += (@string) => SystemLog.Info($"Received String: {@string}");

				//channel.SentBytes += (byteCount) => SystemLog.Info($"Sent Bytes: {byteCount.Length} total");
				//((AnonymousPipe)channel).SentString += (@string) => SystemLog.Info($"Sent String: {@string}");


				// Open the channel
				// Note:
				//   - if channel is the server (parent process -> child process), this will launch the child process and initiate handshake
				//   - if channel is the client (child process -> parent process), this will connect to parent process and finalize handshake
				//await channel.Open();

				// channel handshake has completed by this point

				// Now, we orchestrate the protocol. Note, both server/client orchestrate the protocol 
				// exactly the same way. 
				var protocol = AppProtocol.Build();
				var orchestrator = new ProtocolOrchestrator(channel, protocol);
				orchestrator.ReceivedMessage += (message) => SystemLog.Info($"Received Message: {ToString(message)}");
				orchestrator.SentMessage += (message) => SystemLog.Info($"Sent Message: {ToString(message)}");
				orchestrator.StateChanged += (state) => SystemLog.Info($"Orchestration State: {state}");


				// Both client and server start
				await orchestrator.Start();
			
				
				// Both client and server will ping each other (each will pong to the other)
				orchestrator.SendMessage(ProtocolDispatchType.Request, new Ping());


				//// Make server and client send 10 random messages to each other with intermittent delays
				//for (var i = 0; i < 10; i++) {
				//	var message = GenerateRandomSendableMessage(out var dispatchType);
				//	orchestrator.SendMessage(dispatchType, message);
				//	await Task.Delay(TimeSpan.FromMilliseconds(Tools.Maths.RNG.Next(1000)));
				//}
				
				// Client: Wait 2 seconds before closing
				if (isClient) {
					await Task.Delay(TimeSpan.FromSeconds(5));
					await channel.Close();
				} else {
					// Server: waits for end of orchestrator
					await orchestrator.RunToEnd();
				}
			} catch (Exception error) {
				SystemLog.Exception(error);
			} finally {
				// Ensure channel is disposed in all cases
				if (channel != null)
					await channel.DisposeAsync();
			}
		}

		private static object GenerateRandomSendableMessage(out ProtocolDispatchType dispatchType) {
			
			switch (new[] { AppProtocolMessageType.Ping, AppProtocolMessageType.RequestListFolder, AppProtocolMessageType.RequestFilePart, AppProtocolMessageType.NotifyNewTransaction, AppProtocolMessageType.NotifyNewBlock, AppProtocolMessageType.NotifyLayer2Message }.Randomize().ToArray()[0]) {
				case AppProtocolMessageType.Ping:
					dispatchType = ProtocolDispatchType.Request;
					return new Ping();

				case AppProtocolMessageType.RequestListFolder:
					dispatchType = ProtocolDispatchType.Request;
					return RequestListFolder.GenRandom();

				case AppProtocolMessageType.RequestFilePart:
					dispatchType = ProtocolDispatchType.Request;
					return RequestFilePart.GenRandom();

				case AppProtocolMessageType.NotifyNewTransaction:
					dispatchType = ProtocolDispatchType.Command;
					return NotifyNewTransaction.GenRandom();

				case AppProtocolMessageType.NotifyNewBlock:
					dispatchType = ProtocolDispatchType.Command;
					return NotifyNewBlock.GenRandom();

				case AppProtocolMessageType.NotifyLayer2Message:
					dispatchType = ProtocolDispatchType.Command;
					return NotifyLayer2Message.GenRandom();

				default:
					throw new InternalErrorException();
			}
        }

		private static string ToString(object @obj) {
			switch (@obj) {
				case DirectoryDescriptor x:
					return $"<DirectoryDescriptor> Name: {x.Name} CreatedOn: {x.CreatedOn:yyyy-MM-dd HH:mm:ss}";
				case FileDescriptor x:
					return $"<FileDescriptor> Name: {x.Name} Size: ({Tools.Memory.GetBytesReadable(x.Size)}) CreatedOn: {x.CreatedOn:yyyy-MM-dd HH:mm:ss}";
				case FilePart x:
					return $"<FilePart> Data: {x.Data?.ToHexString()}";
				case FolderContents x:
					return $"<FolderContents> Items: [{x.Items?.ToDelimittedString(", ")}]";
				case NotifyLayer2Message x:
					return $"<NotifyLayer2Message> ID: {x.ID}, SubMessage1: {x.SubMessage1}, SubMessage2: {x.SubMessage2}";
				case NotifyNewBlock x:
					return $"<NotifyNewBlock> ID: {x.GlobalID}, Value: {x.Value}";
				case NotifyNewTransaction x:
					return $"<NotifyNewTransaction> Name: {x.Name}, Age: {x.Age}";
				case Ping x:
					return "<Ping>";
				case Pong x:
					return "<Pong>";
				case RequestFilePart x:
					return $"<RequestFilePart> Filename: '{x.Filename}', Offset: '{x.Offset}', Length: '{x.Length}'";
				case RequestListFolder x:
					return $"<RequestListFolder> Folder: '{x.Folder}'";
				default:
					return obj?.ToString() ?? "NULL";
			}
		}

	}
}
