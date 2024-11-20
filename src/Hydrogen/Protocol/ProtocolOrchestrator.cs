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

/// <summary>
/// Responsible for orchestrating a protocol over a channel. The responsibilities here include message-based communications,
/// handshaking, request-response workflow, command workflow.
/// </summary>
public class ProtocolOrchestrator {
	public const int DefaultTimeoutMS = 5000;
	private readonly SynchronizedDictionary<int, object> _unfulfilledRequests; // TODO add expiration 
	private ProtocolOrchestratorState _state;
	private int _activeMode;
	private volatile int _messageID;
	private HandshakeState _handshakeState;
	private object _handshakeSync, _handshakeAck, _handshakeVerack;
	private readonly TaskCompletionSource<bool> _handshakeFinishedTrigger;
	private readonly ProcessingQueue<ProtocolMessageEnvelope> _outgoingMessages;
	private readonly ProcessingQueue<ProtocolMessageEnvelope> _incomingMessages;


	public event EventHandlerEx<ProtocolMessageEnvelope> ReceivedMessage;
	public event EventHandlerEx<ProtocolMessageEnvelope> SentMessage;
	public event EventHandlerEx<ProtocolOrchestratorState> StateChanged;
	public event EventHandlerEx<ProtocolOrchestratorQueue, ProtocolMessageEnvelope, Exception> MessageError;

	public ProtocolOrchestrator(ProtocolChannel channel, Protocol protocol) {
		Guard.ArgumentNotNull(channel, nameof(channel));
		Guard.ArgumentNotNull(protocol, nameof(protocol));
		var result = protocol.Validate();
		Guard.Argument(result.IsSuccess, nameof(protocol), result.ErrorMessages.ToParagraphCase());
		_unfulfilledRequests = new SynchronizedDictionary<int, object>();
		Channel = channel;
		Protocol = protocol;
		ActiveMode = 0;
		_messageID = 0;
		_state = ProtocolOrchestratorState.NotStarted;
		_handshakeState = HandshakeState.NotStarted;
		_handshakeSync = _handshakeAck = _handshakeVerack = null;
		_handshakeFinishedTrigger = new TaskCompletionSource<bool>();
		_outgoingMessages = new ProcessingQueue<ProtocolMessageEnvelope>(ProcessSendMessage);
		_incomingMessages = new ProcessingQueue<ProtocolMessageEnvelope>(ProcessReceivedMessage);
		_incomingMessages.Errors += (env, error) => MessageError?.InvokeAsync(ProtocolOrchestratorQueue.Inbound, env, error);
		_outgoingMessages.Errors += (env, error) => MessageError?.InvokeAsync(ProtocolOrchestratorQueue.Outbound, env, error);
		Channel.ReceivedBytes += ProcessReceivedBytes;
		Channel.Closed += async () => await Finish();
		Channel.Closed += () => _handshakeFinishedTrigger.TrySetCanceled(CancellationToken.None);

	}

	public ProtocolChannel Channel { get; }

	public Protocol Protocol { get; }

	public ProtocolOrchestratorState State {
		get => _state;
		set {
			if (_state == value)
				return;
			_state = value;
			NotifyStateChanged(_state);
		}
	}

	public int ActiveMode {
		get => _activeMode;
		set {
			Guard.Ensure(Protocol is not null, "Protocol not set");
			Guard.ArgumentInRange(value, 0, Protocol.Modes.Length, nameof(value), "Protocol has no such mode");
			_activeMode = value;
			//_messageID = 0;  // do not reset message id on mode-change
			EnvelopeSerializer = new ProtocolMessageEnvelopeSerializer(Protocol.Factory.GetSerializer<object>());
		}
	}

	public ILogger Logger { get; init; } = new NoOpLogger();

	protected ProtocolMessageEnvelopeSerializer EnvelopeSerializer { get; private set; }

	public static async Task RunToEnd(ProtocolChannel channel, Protocol protocol, CancellationToken cancellationToken, ILogger logger = null) {
		var orchestrator = new ProtocolOrchestrator(channel, protocol) { Logger = logger ?? new NoOpLogger() };
		await orchestrator.Start(cancellationToken);
		await orchestrator.RunToEnd(cancellationToken);
	}

	public Task Start() => Start(Channel.DefaultTimeout);

	public Task Start(TimeSpan timeout) => Start(new CancellationTokenSource(timeout).Token);

	public async Task Start(CancellationToken cancellationToken) {
		if (!await TryStart(cancellationToken))
			throw new InvalidOperationException("Protocol orchestrator failed to start");
	}

	public async Task<bool> TryStart(CancellationToken cancellationToken) {
		CheckState(ProtocolOrchestratorState.NotStarted);

		// Open channel if closed
		if (Channel.State != ProtocolChannelState.Open)
			if (!await Channel.TryOpen())
				return false;

		// Handshake needs to complete before it is started
		if (Protocol.Handshake.Type != ProtocolHandshakeType.None) {
			BeginHandshake();
		} else {
			State = ProtocolOrchestratorState.Started;
		}

		// Enable in/out message processing now (never loses a message)
		_incomingMessages.Enabled = true;
		_outgoingMessages.Enabled = true;

		if ( /* startWaitsForHandshake && */ Protocol.Handshake.Type != ProtocolHandshakeType.None) {
			await _handshakeFinishedTrigger.Task;
		}

		return State == ProtocolOrchestratorState.Started;
	}

	public async Task Finish() {
		if (Channel.State.IsIn(ProtocolChannelState.Open, ProtocolChannelState.Opening)) {
			await Channel.Close();
		}
		State = ProtocolOrchestratorState.Finished;
	}

	public Task RunToEnd() => RunToEnd(default);

	public async Task RunToEnd(CancellationToken cancellationToken) {
		CheckState(ProtocolOrchestratorState.Started);
		// Run until channel closed by other end (or cancelled by token)
		var tcs = new TaskCompletionSource();
		cancellationToken.Register(async () => {
			// cancellation token wants to cancel, so call Finish here (will trigger Finish state changed event)
			if (State != ProtocolOrchestratorState.Finished)
				await Finish();
		});
		StateChanged += state => {
			// Method blocks until finished state is triggered
			if (state == ProtocolOrchestratorState.Finished)
				tcs.SetResult();
		};
		await tcs.Task;
	}

	public void SendMessage(ProtocolDispatchType dispatchType, object message) {
		CheckState(ProtocolOrchestratorState.Started, ProtocolOrchestratorState.Handshaking);
		var envelope = new ProtocolMessageEnvelope {
			DispatchType = dispatchType,
			RequestID = Interlocked.Increment(ref _messageID),
			Message = message
		};
		SendMessage(envelope);
	}

	public virtual void SendMessage(ProtocolMessageEnvelope envelope) {
		_outgoingMessages.Enqueue(envelope);
	}

	protected bool ProcessSendMessage(ProtocolMessageEnvelope envelope) {
		if (!Tools.Lambda.Try( () => EnvelopeSerializer.SerializeBytesLE(envelope), out var data, out var error)) {
			Logger.Exception(error, "Failed to serialize message envelope");
			return false;
		}

		if (!Channel.TrySendBytes(data, Channel.DefaultTimeout).Result) {
			Logger.Error("Failed to transmit message envelope");
			return false;
		}
		NotifySentMessage(envelope);
		ProcessSentMessage(envelope);
		return true;
	}

	protected void ProcessReceivedBytes(ReadOnlyMemory<byte> bytes) {
		if (!Tools.Lambda.Try( () => EnvelopeSerializer.DeserializeBytesLE(bytes.Span), out var envelope, out var error)) {
			// Deal with problematic channel here (blacklist)
			Logger.Exception(error, $"Failed to deserialize message envelope (byte length: {bytes.Length})");
			// TODO: behaviour on error?
			return;
		}

		// In Started mode, the orchestrator pushes received messages down the usual pipeline
		NotifyReceivedMessage(envelope);
		_incomingMessages.Enqueue(envelope); // calls ProcessReceivedMessage one-at-a-time
	}

	protected virtual void ProcessReceivedMessage(ProtocolMessageEnvelope envelope) {
		CheckState(ProtocolOrchestratorState.Started, ProtocolOrchestratorState.Handshaking);
		if (State == ProtocolOrchestratorState.Handshaking) {
			AdvanceHandshakeStep(envelope);
		} else {
			switch (envelope.DispatchType) {
				case ProtocolDispatchType.Command:
					ProcessReceivedCommand(envelope.Message);
					break;
				case ProtocolDispatchType.Request:
					ProcessReceivedRequest(envelope.RequestID, envelope.Message);
					break;
				case ProtocolDispatchType.Response:
					ProcessReceivedResponse(envelope.RequestID, envelope.Message);
					break;
			}
		}
	}

	protected virtual void ProcessSentMessage(ProtocolMessageEnvelope envelope) {
		if (envelope.DispatchType == ProtocolDispatchType.Request) {
			_unfulfilledRequests[envelope.RequestID] = envelope.Message;
		}
	}

	protected virtual void ProcessReceivedCommand(object command) {
		var commandType = command.GetType();
		var commandHandler = ResolveCommandHandler(commandType);
		ThreadPool.QueueUserWorkItem(_ => {
			try {
				commandHandler.Execute(this, command);
			} catch (Exception error) {
				Logger.Exception(error, $"Command handler for '{commandType.Name}' failed (mode: {ActiveMode})");
			}
		});
	}

	protected virtual void ProcessReceivedRequest(int requestID, object request) {
		var requestType = request.GetType();
		var requestHandler = ResolveRequestHandler(requestType);
		ThreadPool.QueueUserWorkItem(_ => {
			try {
				var response = requestHandler.Execute(this, request);
				var envelope = new ProtocolMessageEnvelope {
					DispatchType = ProtocolDispatchType.Response,
					RequestID = requestID,
					Message = response
				};
				SendMessage(envelope);
			} catch (Exception error) {
				Logger.Exception(error, $"Request handler for '{requestType.Name}' failed (mode: {ActiveMode})");
			}
		});
	}

	protected virtual void ProcessReceivedResponse(int requestID, object response) {
		var responseType = response.GetType();
		object request;
		IResponseHandler responseHandler;
		using (_unfulfilledRequests.EnterWriteScope()) {
			if (!_unfulfilledRequests.TryGetValue(requestID, out request))
				throw new ProtocolException($"No unfulfilled Request with ID {requestID} was found (mode: {ActiveMode})");
			var requestType = request.GetType();
			responseHandler = ResolveResponseHandler(requestType, responseType);
		}
		ThreadPool.QueueUserWorkItem(_ => {
			try {
				responseHandler.Execute(this, request, response);
			} catch (Exception error) {
				Logger.Exception(error, $"Response handler for '{responseType.Name}' failed");
			}
		});
	}

	protected virtual void OnReceivedMessage(ProtocolMessageEnvelope messageEnvelope) {
	}

	protected virtual void OnSentMessage(ProtocolMessageEnvelope messageEnvelope) {
	}

	protected virtual void OnStateChanged(ProtocolOrchestratorState state) {
	}

	private void BeginHandshake() {
		CheckState(ProtocolOrchestratorState.NotStarted);
		Guard.Ensure(Protocol.Handshake.Type != ProtocolHandshakeType.None);
		Guard.Ensure(_handshakeState == HandshakeState.NotStarted, "Handshake already started");
		State = ProtocolOrchestratorState.Handshaking;
		if (Channel.LocalRole == Protocol.Handshake.Initiator) {
			_handshakeSync = Protocol.Handshake.Handler.GenerateHandshake(this);
			SendMessage(ProtocolDispatchType.Command, _handshakeSync);
			_handshakeState = HandshakeState.AwaitingAck;
		} else {
			_handshakeState = HandshakeState.AwaitingSync;
		}
	}

	// Advanced the handshake state-machine. Not a task since called by ProcessingQueue
	private void AdvanceHandshakeStep(ProtocolMessageEnvelope envelope) {
		CheckState(ProtocolOrchestratorState.Handshaking);
		Guard.Ensure(Protocol.Handshake.Type != ProtocolHandshakeType.None);
		Guard.Ensure(_handshakeState != HandshakeState.NotStarted, "Handshake has not been started");
		var handshakePassed = false;
		var handshakeFailed = false;
		if (Channel.LocalRole == Protocol.Handshake.Initiator) {

			#region Initiator workflow

			switch (_handshakeState) {
				case HandshakeState.AwaitingSync:
					throw new InternalErrorException("Handshake initiator should never be awaiting sync");
				case HandshakeState.AwaitingAck:
					if (envelope.Message.GetType() != Protocol.Handshake.AckMessageType) {
						handshakeFailed = true;
						break;
					}
					_handshakeAck = envelope.Message;
					var outcome = Protocol.Handshake.Handler.VerifyHandshake(this, _handshakeSync, _handshakeAck, out _handshakeVerack);
					if (outcome != HandshakeOutcome.Accepted) {
						handshakeFailed = true;
						// TODO: add channel blacklisting/reporting behaviours here
						break;
					}
					handshakePassed = true;
					if (Protocol.Handshake.Type == ProtocolHandshakeType.ThreeWay) {
						SendMessage(ProtocolDispatchType.Command, _handshakeVerack);
					}
					break;
				case HandshakeState.AwaitingVerack:
					throw new InternalErrorException("Handshake initiator should never be awaiting verack");
			}

			#endregion

		} else {

			#region Receiver workflow

			switch (_handshakeState) {
				case HandshakeState.NotStarted:
				case HandshakeState.AwaitingSync:
					if (envelope.Message.GetType() != Protocol.Handshake.SyncMessageType) {
						handshakeFailed = true;
						break;
					}
					_handshakeSync = envelope.Message;
					var outcome = Protocol.Handshake.Handler.ReceiveHandshake(this, _handshakeSync, out _handshakeAck);
					if (outcome != HandshakeOutcome.Accepted) {
						handshakeFailed = true;
						// TODO: add channel blacklisting/reporting behaviours here
						break;
					}
					SendMessage(ProtocolDispatchType.Command, _handshakeAck);
					if (Protocol.Handshake.Type == ProtocolHandshakeType.TwoWay)
						handshakePassed = true;
					else
						_handshakeState = HandshakeState.AwaitingVerack;
					break;
				case HandshakeState.AwaitingAck:
					throw new InternalErrorException("Handshake receiver should never be awaiting ack");
				case HandshakeState.AwaitingVerack:
					if (envelope.Message.GetType() != Protocol.Handshake.VerackMessageType) {
						handshakeFailed = true;
						break;
					}
					if (Protocol.Handshake.Handler.AcknowledgeHandshake(this, _handshakeSync, _handshakeAck, _handshakeVerack)) {
						handshakePassed = true;
					} else {
						handshakeFailed = true;
					}
					break;
			}

			#endregion

		}

		if (handshakeFailed) {
			State = ProtocolOrchestratorState.Finished;
			_handshakeFinishedTrigger.SetResult(false);
		} else if (handshakePassed) {
			State = ProtocolOrchestratorState.Started;
			_handshakeFinishedTrigger.SetResult(true);
		}
	}

	private IMessageGenerator ResolveMessageGenerator(Type messageType) {
		if (!Protocol.Modes[ActiveMode].MessageGenerators.TryGetValue(messageType, out var messageGenerator))
			throw new ProtocolException($"Message Generator for '{messageType.Name}' not found (mode: {ActiveMode})");
		return messageGenerator;
	}

	private ICommandHandler ResolveCommandHandler(Type commandType) {
		if (!Protocol.Modes[ActiveMode].CommandHandlers.TryGetValue(commandType, out var commandHandler))
			throw new ProtocolException($"Command handler for '{commandType}' not found (mode: {ActiveMode})");
		return commandHandler;
	}

	private IRequestHandler ResolveRequestHandler(Type requestType) {
		if (!Protocol.Modes[ActiveMode].RequestHandlers.TryGetValue(requestType, out var requestHandler))
			throw new ProtocolException($"Request handler for '{requestType}' not found (mode: {ActiveMode})");
		return requestHandler;
	}

	private IResponseHandler ResolveResponseHandler(Type requestType, Type responseType) {
		if (!Protocol.Modes[ActiveMode].ResponseHandlers.TryGetValue(requestType, responseType, out var responseHandler))
			throw new ProtocolException($"Response handler for '{responseType}' for Request '{requestType}' not found (mode: {ActiveMode})");
		return responseHandler;
	}

	private void NotifyReceivedMessage(ProtocolMessageEnvelope messageEnvelope) {
		OnReceivedMessage(messageEnvelope);
		Tools.Threads.RaiseAsync(ReceivedMessage, messageEnvelope);
	}

	private void NotifySentMessage(ProtocolMessageEnvelope messageEnvelope) {
		OnSentMessage(messageEnvelope);
		Tools.Threads.RaiseAsync(SentMessage, messageEnvelope);
	}

	private void NotifyStateChanged(ProtocolOrchestratorState state) {
		OnStateChanged(state);
		Tools.Threads.RaiseAsync(StateChanged, state);
	}

	private void CheckState(params ProtocolOrchestratorState[] expectedStates) {
		if (!State.IsIn(expectedStates))
			throw new InvalidOperationException($"Orchestrator state was not in {expectedStates.ToDelimittedString(", ")} (was {State})");
	}


	private enum HandshakeState {
		NotStarted,
		AwaitingSync,
		AwaitingAck,
		AwaitingVerack,
	}
}
