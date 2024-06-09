// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen.Communications;

public abstract class ProtocolChannel : IDisposable, IAsyncDisposable {
	public const int DefaultTimeoutMS = 5000;
	private const int DefaultMinMessageLength = 0;
	private const int DefaultMaxMessageLength = 65536;

	// Events
	public event EventHandlerEx Opening;
	public event EventHandlerEx Opened;
	public event EventHandlerEx Closing;
	public event EventHandlerEx Closed;
	public event EventHandlerEx<ReadOnlyMemory<byte>> ReceivedBytes;
	public event EventHandlerEx<Memory<byte>> SentBytes;

	// Fields
	protected CancellationTokenSource _cancelReceiveLoop;
	private Task _receiveLoop;

	// Constructors
	protected ProtocolChannel() {
		_cancelReceiveLoop = new CancellationTokenSource();
		_receiveLoop = null; // set by StartReceiveLoop
		DefaultTimeout = TimeSpan.FromMilliseconds(DefaultTimeoutMS);
		State = ProtocolChannelState.Closed;
	}

	#region Properties

	public ProtocolChannelState State { get; private set; }

	public TimeSpan DefaultTimeout { get; set; }

	public abstract CommunicationRole LocalRole { get; }

	public virtual CommunicationRole Initiator => CommunicationRole.Client;

	#endregion

	#region Methods

	public async Task Open() {
		if (!await TryOpen())
			throw new ProtocolException("Protocol channel could not be opened (endpoint and/or handshake problem)");
	}

	public virtual async Task<bool> TryOpen() {
		try {
			CheckState(ProtocolChannelState.Closed);
			SetState(ProtocolChannelState.Opening);
			NotifyOpening();
			await OpenInternal();
			StartReceiveLoop();
			SetState(ProtocolChannelState.Open);
			NotifyOpened();
			return true;
		} catch (Exception error) {
			// TODO: add Logging
			return false;
		}
	}

	public virtual async Task Close() {
		CheckState(ProtocolChannelState.Open, ProtocolChannelState.Closing, ProtocolChannelState.Closed);
		if (State == ProtocolChannelState.Closed)
			return;
		if (State != ProtocolChannelState.Closing) {
			SetState(ProtocolChannelState.Closing);
			NotifyClosing();
		}
		await BeginClose(new CancellationTokenSource(DefaultTimeout).Token);
		await StopReceiveLoop();
		await CloseInternal();
		// Note: NotifyClosed() is called by receive loop termination
	}

	public virtual Task<bool> TrySendBytes(byte[] bytes) => TrySendBytes(bytes, DefaultTimeout);

	public virtual Task<bool> TrySendBytes(byte[] bytes, TimeSpan timeout)
		=> TrySendBytes(bytes, new CancellationTokenSource(timeout).Token);

	public async Task<bool> TrySendBytes(byte[] bytes, CancellationToken cancellationToken) {
		if (!IsConnectionAlive())
			return false;

		if (!await TrySendBytesInternal(bytes, cancellationToken))
			return false;
		NotifySentBytes(bytes);
		return true;
	}

	public virtual Task<bool> TryWaitClose(TimeSpan timeout)
		=> TryWaitClose(new CancellationTokenSource(timeout).Token);

	public virtual Task<bool> TryWaitClose(CancellationToken token) {
		var tcs = new TaskCompletionSource<bool>();
		token.Register(() => tcs.SetResult(false));
		this.Closed += () => tcs.SetResult(true);
		return tcs.Task;
	}

	public void Dispose() {
		DisposeAsync().AsTask().WaitSafe();
	}

	public virtual async ValueTask DisposeAsync() {
		if (State == ProtocolChannelState.Open)
			await Close();
	}

	// Template-Pattern abstract methods
	protected abstract Task OpenInternal();

	protected virtual async Task BeginClose(CancellationToken cancellationToken) {
	}

	protected abstract Task CloseInternal();

	protected virtual void StartReceiveLoop() {
		_receiveLoop = Task.Run(() => ReceiveLoop(_cancelReceiveLoop.Token));
	}

	protected virtual async Task StopReceiveLoop() {
		const int forceCancelReceiveLoopMS = 150;
		_cancelReceiveLoop.CancelAfter(forceCancelReceiveLoopMS);
		try {
			// Receive loop SHOULD immediately react to channel State being Closed.
			// But if it does not, then after some time (forceCancelReceiveLoopMS), it is forced closed.
			await _receiveLoop;
			if (State != ProtocolChannelState.Closed) {
				SetState(ProtocolChannelState.Closed);
				NotifyClosed();
			}
		} catch (TaskCanceledException) {
		}
	}

	protected virtual async Task ReceiveLoop(CancellationToken cancellationToken) {
		// await _startReceiveLoop.Task.ConfigureAwait(false);
		CheckState(ProtocolChannelState.Opening, ProtocolChannelState.Open);
		while (State.IsIn(ProtocolChannelState.Opening, ProtocolChannelState.Open) && IsConnectionAlive() && !cancellationToken.IsCancellationRequested) {
			var bytes = await ReceiveBytesInternal(cancellationToken).IgnoringCancellationException();
			if (bytes?.Length > 0)
				NotifyReceivedBytes(bytes);
		}
		// Connection is Closed only when receive loop finishes
		Guard.Ensure(State != ProtocolChannelState.Closed, "Protocol channel was already closed");
		SetState(ProtocolChannelState.Closed);
		NotifyClosed();
	}

	protected abstract Task<byte[]> ReceiveBytesInternal(CancellationToken cancellationToken);

	protected abstract Task<bool> TrySendBytesInternal(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken);

	public abstract bool IsConnectionAlive();

	#endregion

	#region Event handler methods

	protected virtual void OnOpening() {
	}

	protected virtual void OnOpened() {
	}

	protected virtual void OnClosing() {
	}

	protected virtual void OnClosed() {
	}

	protected virtual void OnReceivedBytes(ReadOnlySpan<byte> bytes) {
	}

	protected virtual void OnSentBytes(ReadOnlySpan<byte> bytes) {
	}

	#endregion

	#region Notify methods

	protected void NotifyOpening() {
		OnOpening();
		// TODO: add a ShouldRaiseAsync property
		Tools.Threads.RaiseAsync(Opening);
	}

	protected void NotifyOpened() {
		OnOpened();
		Tools.Threads.RaiseAsync(Opened);
	}

	protected void NotifyClosing() {
		OnClosing();
		Tools.Threads.RaiseAsync(Closing);
	}

	protected void NotifyClosed() {
		OnClosed();
		Tools.Threads.RaiseAsync(Closed);
	}

	protected void NotifyReceivedBytes(byte[] bytes) {
		OnReceivedBytes(bytes);
		Tools.Threads.RaiseAsync(ReceivedBytes, bytes);
	}

	protected void NotifySentBytes(byte[] bytes) {
		OnSentBytes(bytes);
		Tools.Threads.RaiseAsync(SentBytes, bytes);
	}

	#endregion

	#region Aux

	private void CheckState(params ProtocolChannelState[] expectedStates) {
		if (!State.IsIn(expectedStates))
			throw new InvalidOperationException($"Channel state was not in {expectedStates.ToDelimittedString(", ")}");
	}

	private void SetState(ProtocolChannelState state) {
		State = state;
	}

	#endregion

}
