//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Sphere10.Framework;
//using Sphere10.Framework.Communications;
//using Sphere10.Hydrogen.Core.HAP;
//using Sphere10.Hydrogen.Core.Protocols.Host;
//using Sphere10.Hydrogen.Core.Storage;

//namespace Sphere10.Hydrogen.Host {

//	public class HydrogenHost : IHydrogenHost {
//		public HydrogenHost(ILogger logger, HydrogenApplicationPaths appPaths) {
//			HostProtocolOrchestrator = default;
//			Upgrading = false;
//			Logger = logger;
//			Folders = appPaths;
//			Protocol = BuildProtocol();
//		}
		
//		public HydrogenApplicationPaths Folders { get; }

//		private ILogger Logger { get; }

//		private bool Upgrading { get; set;  }

//		private Protocol Protocol { get; set; }

//		private ProtocolOrchestrator HostProtocolOrchestrator { get; set; }

//		private Protocol BuildProtocol()
//			=> new ProtocolBuilder<AnonymousPipe>()
//				.Requests
//					.ForRequest<PingMessage>().RespondWith(() => new PongMessage())
//				.Responses
//					.ForResponse<PongMessage>().ToRequest<PingMessage>().HandleWith(() => Logger.Info("Received Pong"))
//				.Commands
//					.ForCommand<UpgradeMessage>().Execute(async upgradeMessage => await UpgradeNode(upgradeMessage.HydrogenApplicationPackagePath))
//					.ForCommand<ShutdownMessage>().Execute(async () => await RequestStopHost())
//				.Messages
//					.Use(HostProtocolHelper.BuildMessageSerializer())
//				.Build();

//		public async Task StartHost() {
//			var HostProtocolOrchestrator = new ProtocolOrchestrator(NodePipe, NodeProtocol);
//			var protocolRunner = protocolOrchestrator.Run();

//			var tcs = new TaskCompletionSource<bool>();
//			HostProtocolOrchestrator.Channel.Closed += () => {
//				if (!Upgrading)
//					tcs.SetResult(true);
//			};
//			await StartNode();
//			var protocolRunner = HostProtocolOrchestrator.Run();
//			await Task.WhenAll(tcs.Task, protocolRunner);
//		}

//		public async Task RequestStopHost() => await HostProtocolOrchestrator.Channel.Close();

//		public async Task StartNode() {
//			Guard.Ensure(HostProtocolOrchestrator == null, "Nod");
//			Logger.Info("Starting node");
//			if (HostProtocolOrchestrator != null) {
//				await HostProtocolOrchestrator.Channel.DisposeAsync();
				
//			var pipe  = AnonymousPipe.ToChildProcess(Folders.NodeExecutable, NodeProtocol.MessageSerializer, "-hostread {0} -hostwrite {1}", string.Format);
//			await NodePipe.Open();
//		}

		

//		public async Task StopNode() {
//			Logger.Info("Requesting node shutdown");
//			await NodePipe.SendMessage(ProtocolMessageType.Command, new ShutdownMessage());
//			if (!await NodePipe.TryWaitClose(TimeSpan.FromMinutes(1)))
//				throw new HostException("Node failed to shutdown");
//		}

//		public async Task UpgradeNode(string hapPath) {
//			Logger.Info($"Upgrading application with: {hapPath}");
//			try {
//				_upgrading = true;
//				await StopNode();
//				await DeployHAP(hapPath);
//				await StartNode();
//			} finally {
//				_upgrading = false;
//			}
//		}

//		public async Task DeployHAP(string newHapPath) {
//			Guard.FileExists(newHapPath);
//			await Tools.FileSystem.DeleteAllFilesAsync(Folders.HapFolder, true);
//			var zipPackage = new ZipPackage(newHapPath);
//			zipPackage.ExtractTo(Folders.HapFolder);
//		}

//	}

//	public interface IHydrogenHost {

//		Task StartHost();

//	}
//}
