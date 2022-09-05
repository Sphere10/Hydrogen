// TODO: Refactor command line


public class Program {
    static void Main(string[] args) { }
}

//using Hydrogen;
//using System;
//using System.IO;
//using System.Linq;
//using System.Reflection.PortableExecutable;
//using System.Runtime.CompilerServices;
//using System.Threading.Tasks;
//using Hydrogen.Communications;
//using System.Threading;
//using System.Text;

//namespace AbstractProtocol.AnonymousPipeSimple {

//	/// <summary>
//	/// This program demos an AbstractProtocol over AnonymousPipe channels.
//	/// </summary>
//	public class Program {

//		/// <summary>
//		/// These are the command line paramters pass by the Parent process (server) to the client process (server).
//		/// </summary>
//		public static CommandLineParameters Parameters = new() {
//			Commands = new CommandLineCommand[] {
//				new("anonymouspipeclient", "AnonymousPipeClient child process tester") {
//					Parameters = new CommandLineParameter[] {
//						new("read", "AnonymousPipeClient child process tester", CommandLineParameterOptions.Mandatory | CommandLineParameterOptions.RequiresValue),
//						new("write", "AnonymousPipeClient child process tester", CommandLineParameterOptions.Mandatory | CommandLineParameterOptions.RequiresValue),
//					}
//				}
//			},
//		};

//		/// <summary>
//		/// This is the protocol message 'Ping'
//		/// </summary>
//		[Serializable]
//		public class Ping {
//		}


//		/// <summary>
//		/// This is the protocol message 'Pong'
//		/// </summary>
//		[Serializable]
//		public class Pong {
//		}

//		/// <summary>
//		/// Main Program
//		/// </summary>
//		public static async Task Main(string[] args) {
//			var isClient = false;
//			// Parse command line arguments
//			var commandLineResults = Parameters.TryParseArguments(args);
//			if (commandLineResults.Failure) {
//				Environment.Exit(-1);
//			}

//			// Build the protocol channel
//			ProtocolChannel channel = null;
//			try {
//                if (commandLineResults.Value.SubCommand?.CommandName != "anonymouspipeclient") {
//                    // This process is running as the server

//                    // Setup logging for server (uncomment for diagnostic of individual log)
//                    ILogger logger = null; // new FileAppendLogger("c:/temp/server.log");
//                    SystemLog.RegisterLogger(new PrefixLogger(new TimestampLogger(new MulticastLogger(new ConsoleLogger(), logger ?? new NoOpLogger())), "[SERVER]: "));

//                    // The protocol channel here is the (parent process) -> (child process) anonymous pipe
//                    channel = AnonymousPipe.ToChildProcess(
//                        Tools.Runtime.GetExecutablePath(),
//                        "anonymouspipeclient --read {0} --write {1}",
//                        (args, readHandle, writeHandle) => string.Format(args, readHandle, writeHandle)
//                    );
//                } else {
//                    // This process is running as the client
//                    isClient = true;

//                    // Setup logging for client (uncomment for diagnostic of individual log)
//                    ILogger logger = null; // new FileAppendLogger("c:/temp/client.log");  
//                    SystemLog.RegisterLogger(new PrefixLogger(new TimestampLogger(new MulticastLogger(new ConsoleLogger(), logger ?? new NoOpLogger())), "[CLIENT]: "));

//                    // Get the server's endpoint details from command line (passed in by the parent process when launches child process)
//                    var endpoint = new AnonymousPipeEndpoint {
//                        ReaderHandle = commandLineResults.Value.SubCommand.GetSingleArgumentValue<string>("read"),
//                        WriterHandle = commandLineResults.Value.SubCommand.GetSingleArgumentValue<string>("write"),
//                    };

//                    // The protocol channel here is the (child process) -> (parent process) anonymous pipe
//                    channel = AnonymousPipe.FromChildProcess(endpoint);

//                }
//                // subscribe to events for logging
//                channel.Opening += () => SystemLog.Info("Opening");
//				channel.Opened += () => SystemLog.Info("Opened");
//				//channel.Handshake += () => SystemLog.Info("Handshake");
//				channel.Closing += () => SystemLog.Info("Closing");
//				channel.Closed += () => SystemLog.Info("Closed");
//				channel.ReceivedBytes += (byteCount) => SystemLog.Info($"Received Bytes: {byteCount.Length} total");
//				((AnonymousPipe)channel).ReceivedString += (@string) => SystemLog.Info($"Received String: {@string}");
//				channel.SentBytes += (byteCount) => SystemLog.Info($"Sent Bytes: {byteCount.Length} total");
//				((AnonymousPipe)channel).SentString += (@string) => SystemLog.Info($"Sent String: {@string}");


//				// Open the channel
//				// Note:
//				//   - if channel is the server (parent process -> child process), this will launch the child process and initiate handshake
//				//   - if channel is the client (child process -> parent process), this will connect to parent process and finalize handshake
//				await channel.Open();

//				// channel handshake has completed by this point

//				// Now, we orchestrate the protocol. Note, both server/client orchestrate the protocol 
//				// exactly the same way. 

//			var protocol =
//				new ProtocolBuilder()
//					.Requests
//						.ForRequest<Ping>().RespondWith((_, _) => new Pong())
//					.Responses
//						.ForResponse<Pong>().ToRequest<Ping>().HandleWith((ch, pingMsg, pongMsg) => SystemLog.Info("Handled Pong OK!"))
//					.Messages
//                        .For<Ping>().SerializeWith(new BinaryFormattedSerializer<Ping>())
//						.For<Pong>().SerializeWith(new BinaryFormattedSerializer<Pong>())
//					.Build();

//				var orchestrator = new ProtocolOrchestrator(channel, protocol);
//				orchestrator.ReceivedMessage += (message) => SystemLog.Info($"Received Message: {message}");
//				orchestrator.SentMessage += (message) => SystemLog.Info($"Sent Message: {message}");

//				// start the orchestrator
//				await orchestrator.Start();

//				// trigger the background run-loop (note: no await here)
//				var runProtocolTask = orchestrator.RunToEnd();

//				// Both client and server will ping each other (each will pong to the other)
//				orchestrator.SendMessage(ProtocolDispatchType.Request, new Ping());


//				// client will wait 2 seconds, then close
//				if (isClient) {
//					// Note: if client awaited protocol then it would await indefinitely, so we let protocol run in background
//					// and intervene after 1 seconds to stop it by closing the client channel directly.
//					await Task.Delay(TimeSpan.FromSeconds(1));
//					await channel.Close();
//				} else {
//					// server will run protocol (until closed by client)
//					await runProtocolTask;
//				}
//			} catch (Exception error) {
//				SystemLog.Exception(error);
//            } finally {
//				// Ensure channel is disposed in all cases
//				if (channel != null)
//					await channel.DisposeAsync();
//			} 
//		}

//	}
//}
