﻿using System;
using System.Threading.Tasks;
using Hydrogen;
using Hydrogen.Communications;
using Hydrogen.DApp.Core.Storage;
using Void = Hydrogen.Void;

namespace Hydrogen.DApp.Core.Runtime {

	public class Host : IHost {
		public event EventHandlerEx<AnonymousPipe> NodeStarted;
		public event EventHandlerEx NodeEnded;

		private ProtocolOrchestrator _hostProtocolOrchestrator;
		private TaskCompletionSourceEx _hostTask;
		private Task _protocolRunner;

		public Host(ILogger logger, IApplicationPaths paths) {
			Logger = logger;
			Paths = paths;
			Status = HostStatus.Stopped;
		}

		public HostStatus Status { get; private set; }
		
		public IApplicationPaths Paths { get; }
		
		protected AnonymousPipe NodePipe { get; set; }

		protected ILogger Logger { get; }
		
		private Protocol BuildProtocol()
			=> new ProtocolBuilder()
				.Requests
					.ForRequest<PingMessage>().RespondWith(() => new PongMessage())
				.Responses
					.ForResponse<PongMessage>().ToRequest<PingMessage>().HandleWith(() => Logger.Info("Received Pong"))
				.Commands
					.ForCommand<UpgradeMessage>().Execute(async upgradeMessage => await UpgradeApplication(upgradeMessage.HydrogenApplicationPackagePath))
					.ForCommand<ShutdownMessage>().Execute(async () => await RequestShutdown())
				.Messages
					.UseOnly(HostProtocolHelper.BuildMessageSerializer())
				.Build();

		public virtual async Task DeployHAP(string hapPath) {
			CheckStatus(HostStatus.Stopped, HostStatus.Upgrading);
			Guard.FileExists(hapPath);
			await Tools.FileSystem.DeleteAllFilesAsync(Paths.HapFolder, true);
			var zipPackage = new ZipPackage(hapPath);
			zipPackage.ExtractTo(Paths.HapFolder);
		}

		public virtual async Task Run() {
			CheckStatus(HostStatus.Stopped);
			_hostTask = new TaskCompletionSourceEx();
			await StartNode();  // starts the node child-process
			await _hostTask.Task;  // wait until host task is finished (host finishes when node channel closed and protocol finishes
			await _protocolRunner; // wait for protocol to finish
		}

		public virtual async Task RequestShutdown() 
			=> await NodePipe.Close();

		protected virtual async Task StartNode() {
			CheckStatus(HostStatus.Stopped, HostStatus.Upgrading);
			Logger.Info("Starting node");
			if (NodePipe != null)
				await NodePipe.DisposeAsync();
			NodePipe = AnonymousPipe.ToChildProcess(Paths.NodeExecutable, "-host {0}:{1}", string.Format);
			NodePipe.Closed += NotifyNodeEnded;
			await NodePipe.Open();
			if (Status != HostStatus.Upgrading)
				Status = HostStatus.Running;
			NotifyNodeStarted(NodePipe);
		}

		protected virtual async Task StopNode() {
			CheckStatus(HostStatus.Running, HostStatus.Upgrading);
			Logger.Info("Requesting node shutdown");
			_hostProtocolOrchestrator.SendMessage(ProtocolDispatchType.Command, new ShutdownMessage());
			if (!await NodePipe.TryWaitClose(TimeSpan.FromMinutes(1)))
				throw new HostException("Node failed to shutdown");
			if (Status != HostStatus.Upgrading)
				Status = HostStatus.Stopped;
			// Note: NotifyNodeEnded is called by the NodePipe.Closed event handler from StartNode
		}

		protected async Task UpgradeApplication(string hapPath) {
			Logger.Info($"Upgrading application with: {hapPath}");
			Status = HostStatus.Upgrading;
			try {
				await StopNode();
				await DeployHAP(hapPath);
				await StartNode();
			} finally {
				Status = HostStatus.Running;
			}
		}

		protected virtual void OnNodeStarted() {
			if (_protocolRunner != null && !_protocolRunner.IsCompleted)
				throw new InvalidOperationException("Previous protocol is still running");
			// build and run the protocol between the host and node
			var protocol = BuildProtocol();
			_hostProtocolOrchestrator = new ProtocolOrchestrator(NodePipe, protocol);
			_protocolRunner = _hostProtocolOrchestrator.RunToEnd();  // protocol runner is left running in background, awaited in Run method
		}

		protected virtual void OnNodeEnded() {
			// If child-process terminates and we're not upgrading, it means the host has completed it's function
			if (Status != HostStatus.Upgrading)
				_hostTask.SetResult();
		}

		private void NotifyNodeStarted(AnonymousPipe anonymousPipe) {
			OnNodeStarted();
			NodeStarted?.Invoke(anonymousPipe);
		}

		private void NotifyNodeEnded() {
			OnNodeEnded();
			NodeEnded?.Invoke();
		}

		private void CheckStatus(params HostStatus[] hostStatuses) {
			if (!Status.IsIn(hostStatuses))
				throw new InvalidOperationException($"Host was not in status(s): {hostStatuses.ToDelimittedString(", ")}");
		}

	}
}