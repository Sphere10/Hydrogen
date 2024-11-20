// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;
using Hydrogen.Communications;
using Hydrogen.DApp.Core.Storage;

namespace Hydrogen.DApp.Core.Runtime;

public class Host : IHost {
	public event EventHandlerEx<AnonymousPipe> NodeStarted;
	public event EventHandlerEx NodeEnded;

	private ProtocolOrchestrator _hostProtocolOrchestrator;
	private TaskCompletionSource _hostTask;
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
			.AddRequestResponse<PingMessage, PongMessage>(() => new PongMessage(), () => Logger.Info("Received Pong"))
			.AddCommand<UpgradeMessage>( async upgradeMessage => await UpgradeApplication(upgradeMessage.HydrogenApplicationPackagePath))
			.AddCommand<ShutdownMessage>(async () => await RequestShutdown())
			.AutoBuildSerializers()
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
		_hostTask = new TaskCompletionSource();
		await StartNode(); // starts the node child-process
		await _hostTask.Task; // wait until host task is finished (host finishes when node channel closed and protocol finishes
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
		_protocolRunner = _hostProtocolOrchestrator.RunToEnd(); // protocol runner is left running in background, awaited in Run method
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
