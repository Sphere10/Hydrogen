//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.VisualBasic.CompilerServices;
//using Sphere10.Framework.Stateless;

//namespace Sphere10.Framework.Communications {

//	/// <summary>
//	/// Responsible for orchestrating a protocol over a channel. The responsibilities here include message-based communications,
//	/// handshaking, request-response workflow, command workflow.
//	/// </summary>
//	public class ProtocolOrchestrator {
//		public const int DefaultTimeoutMS = 5000;
//		private readonly SynchronizedDictionary<int, object> _unfulfilledRequests; // TODO add expiration 
//		private int _activeMode;
//		private volatile int _messageID;

//		private enum Trigger { Start, ReceivedMessage, SendMessage, ChannelClosed };
//		private readonly StateMachine<ProtocolOrchestratorState, Trigger> _machine;
//		private readonly StateMachine<ProtocolOrchestratorState, Trigger>.TriggerWithParameters<ProtocolMessageEnvelope> _receivedMessageTrigger;
//		private readonly StateMachine<ProtocolOrchestratorState, Trigger>.TriggerWithParameters<ProtocolMessageEnvelope, CancellationToken, Result> _sendMessageTrigger;
//		private readonly StateMachine<ProtocolOrchestratorState, Trigger>.TriggerWithParameters<CancellationToken> _startTrigger;

//		public event EventHandlerEx<ProtocolMessageEnvelope> ReceivedMessage;
//		public event EventHandlerEx<ProtocolMessageEnvelope> SentMessage;
//		public event EventHandlerEx<ProtocolOrchestratorState> StateChanged;

//		public ProtocolOrchestrator(ProtocolChannel channel, Protocol protocol) {
//			Guard.ArgumentNotNull(channel, nameof(channel));
//			Guard.ArgumentNotNull(protocol, nameof(protocol));
//			var result = protocol.Validate();
//			Guard.Argument(result.Success, nameof(protocol), result.ErrorMessages.ToParagraphCase());

//			_unfulfilledRequests = new SynchronizedDictionary<int, object>();
//			Channel = channel;
//			Protocol = protocol;
//			Channel.ReceivedBytes += ProcessReceivedBytes;
//			Channel.Closed += () => _machine.Fire(Trigger.ChannelClosed);

//			ActiveMode = 0;
//			_messageID = 0;

//			// Setup state-machine
//			_machine = new StateMachine<ProtocolOrchestratorState, Trigger>(ProtocolOrchestratorState.NotStarted, FiringMode.Immediate);
//			_machine.OnUnhandledTrigger((state, trigger) => new ProtocolException($"Orchestrator unable to handle '{trigger}' since it is in state '{state}'"));
//			_machine.OnTransitioned(x => SystemLog.Debug($"Transitioned from: {x.Source} to {x.Destination}"));
//			_machine.OnTransitioned(x => NotifyStateChanged(x.Destination));
//			_receivedMessageTrigger = _machine.SetTriggerParameters<ProtocolMessageEnvelope>(Trigger.ReceivedMessage);
//			_sendMessageTrigger = _machine.SetTriggerParameters<ProtocolMessageEnvelope, CancellationToken, Result>(Trigger.SendMessage);
//			_startTrigger = _machine.SetTriggerParameters<CancellationToken>(Trigger.Start);

//			// declare variables which capture sync, ack and verack messages (both client and server workflows)
//			object syncMessage = null, ackMessage = null, verackMessage = null;

//			// Configure handshake state-machine common transitions
//			_machine
//				.Configure(ProtocolOrchestratorState.NotStarted)
//				.Permit(Trigger.ChannelClosed, ProtocolOrchestratorState.Finished);

//			_machine
//				.Configure(ProtocolOrchestratorState.Finished)
//				.Ignore(Trigger.ChannelClosed)
//				.Ignore(Trigger.ReceivedMessage);

//			if (Protocol.Handshake.Type == ProtocolHandshakeType.None) {
//				ConfigureNoHandshake();
//			} else {
//				_machine
//					.Configure(ProtocolOrchestratorState.NotStarted)
//					.Permit(Trigger.Start, ProtocolOrchestratorState.Handshaking);

//				switch (Protocol.Handshake.Type, Channel.LocalRole) {
//					case (ProtocolHandshakeType.None, CommunicationRole.Client):
//					case (ProtocolHandshakeType.None, CommunicationRole.Server):
//						ConfigureNoHandshake();
//						break;
//					case (ProtocolHandshakeType.TwoWay, CommunicationRole.Client):
//						ConfigureHandshake2WayClient();
//						break;
//					case (ProtocolHandshakeType.TwoWay, CommunicationRole.Server):
//						ConfigureHandshake2WayServer();
//						break;
//					case (ProtocolHandshakeType.ThreeWay, CommunicationRole.Client):
//						ConfigureHandshake3WayClient();
//						break;
//					case (ProtocolHandshakeType.ThreeWay, CommunicationRole.Server):
//						ConfigureHandshake3WayServer();
//						break;
//					default:
//						throw new ArgumentOutOfRangeException();
//				}
//			}

//			#region Configure the state-machine

//			// Configure: Started
//			_machine
//				.Configure(ProtocolOrchestratorState.Started)
//				.OnEntryAsync(ProcessStart)
//				.InternalTransitionAsync(_sendMessageTrigger, (env, c, r, _) => TrySendEnvelopeInternal(env, c, r))
//				.Ignore(Trigger.ReceivedMessage)
//				.Permit(Trigger.ChannelClosed, ProtocolOrchestratorState.Finished);

//			void ConfigureNoHandshake() {
//				// Configure: NotStarted for handshakeless connections
//				_machine
//					.Configure(ProtocolOrchestratorState.NotStarted)
//					.Permit(Trigger.Start, ProtocolOrchestratorState.Started);
//			}

//			void ConfigureHandshake2WayClient() {
//				// Configure: Handshaking for 2-way client
//				_machine
//					.Configure(ProtocolOrchestratorState.Handshaking)
//					.InitialTransition(ProtocolOrchestratorState.AwaitingAck)
//					.Permit(Trigger.ChannelClosed, ProtocolOrchestratorState.Finished);

//				_machine
//					.Configure(ProtocolOrchestratorState.AwaitingAck)
//					.SubstateOf(ProtocolOrchestratorState.Handshaking)
//					.OnEntryAsync(SendHandshake)
//					.IgnoreIf(_sendMessageTrigger, (env, _, _) => IsHandshakeMessage(env)) // ignore SYNC we just sent
//					.PermitIf(_receivedMessageTrigger, ProtocolOrchestratorState.Started, VerifyHandshake); // receive only ACK
//			}

//			void ConfigureHandshake2WayServer() {
//				// Configure: Handshaking for 2-way server
//				_machine
//					.Configure(ProtocolOrchestratorState.Handshaking)
//					.InitialTransition(ProtocolOrchestratorState.AwaitingHandshake)
//					.Permit(Trigger.ChannelClosed, ProtocolOrchestratorState.Finished);

//				_machine
//					.Configure(ProtocolOrchestratorState.AwaitingHandshake)
//					.SubstateOf(ProtocolOrchestratorState.Handshaking)
//					.PermitIf(_receivedMessageTrigger, ProtocolOrchestratorState.Started, ReceiveHandshake);

//				_machine
//					.Configure(ProtocolOrchestratorState.Started)
//					.OnEntryAsync(SendAck);
//			}

//			void ConfigureHandshake3WayClient() {
//				// Configure: Handshaking for 3-way client
//				// note: it is identical to 2-way except sends verack at the end
//				ConfigureHandshake2WayClient();
//				_machine
//					.Configure(ProtocolOrchestratorState.Started)
//					.OnEntryAsync(SendVerack);
//			}

//			void ConfigureHandshake3WayServer() {
//				// Configure: Handshaking for 3-way server
//				_machine
//					.Configure(ProtocolOrchestratorState.Handshaking)
//					.InitialTransition(ProtocolOrchestratorState.AwaitingHandshake)
//					.Permit(Trigger.ChannelClosed, ProtocolOrchestratorState.Finished);

//				_machine
//					.Configure(ProtocolOrchestratorState.AwaitingHandshake)
//					.SubstateOf(ProtocolOrchestratorState.Handshaking)
//					.PermitIf(_receivedMessageTrigger, ProtocolOrchestratorState.AwaitingVerack, ReceiveHandshake);

//				_machine
//					.Configure(ProtocolOrchestratorState.AwaitingVerack)
//					.SubstateOf(ProtocolOrchestratorState.Handshaking)
//					.OnEntryAsync(SendAck)
//					.PermitIf(_receivedMessageTrigger, ProtocolOrchestratorState.Started, AcknowledgeHandshake);
//			}

//			Task SendHandshake() {
//				syncMessage = Protocol.Handshake.Handler.GenerateHandshake(this);
//				return SendMessage(ProtocolDispatchType.Command, syncMessage);
//			}

//			Task SendAck() => SendMessage(ProtocolDispatchType.Command, ackMessage);


//			Task SendVerack() => SendMessage(ProtocolDispatchType.Command, verackMessage);


//			bool IsHandshakeMessage(ProtocolMessageEnvelope envelope) {
//				if (envelope.Message.GetType() == Protocol.Handshake.SyncMessageType) {
//					syncMessage = envelope.Message;
//					return true;
//				}
//				return false;
//			}

//			bool IsAckMessage(ProtocolMessageEnvelope envelope) {
//				if (envelope.Message.GetType() == Protocol.Handshake.AckMessageType) {
//					ackMessage = envelope.Message;
//					return true;
//				}
//				return false;
//			}

//			bool IsVerackMessage(ProtocolMessageEnvelope envelope) {
//				if (envelope.Message.GetType() == Protocol.Handshake.VerackMessageType) {
//					verackMessage = envelope.Message;
//					return true;
//				}
//				return false;
//			}

//			bool ReceiveHandshake(ProtocolMessageEnvelope env) {
//				if (!IsHandshakeMessage(env))
//					return false;
//				var outcome = Protocol.Handshake.Handler.ReceiveHandshake(this, syncMessage, out ackMessage);
//				// note: ackMessage is set here
//				return outcome == HandshakeOutcome.Accepted;
//			}

//			bool VerifyHandshake(ProtocolMessageEnvelope env) {
//				if (!IsAckMessage(env))
//					return false;

//				var outcome = Protocol.Handshake.Handler.VerifyHandshake(this, syncMessage, ackMessage, out verackMessage);

//				// TODO: add some error handling here
//				return outcome == HandshakeOutcome.Accepted;
//			}

//			bool AcknowledgeHandshake(ProtocolMessageEnvelope env)
//				=> IsVerackMessage(env) && Protocol.Handshake.Handler.AcknowledgeHandshake(this, syncMessage, ackMessage, verackMessage);

//			#endregion
//		}

//		public ProtocolChannel Channel { get; }

//		public Protocol Protocol { get; }

//		public ProtocolOrchestratorState State => _machine.State;

//		public int ActiveMode {
//			get => _activeMode;
//			set {
//				Guard.Ensure(Protocol is not null, "Protocol not set");
//				Guard.ArgumentInRange(value, 0, Protocol.Modes.Count, nameof(value), "Protocol has no such mode");
//				_activeMode = value;
//				//_messageID = 0;  // do not reset message id on mode-change
//				EnvelopeSerializer = new ProtocolMessageEnvelopeSerializer(Protocol.Modes[_activeMode].MessageSerializer);
//			}
//		}

//		public ILogger Logger { get; init; } = new NoOpLogger();

//		protected ProtocolMessageEnvelopeSerializer EnvelopeSerializer { get; private set; }

//		public static async Task Run(ProtocolChannel channel, Protocol protocol, CancellationToken cancellationToken, ILogger logger = null) {
//			var orchestrator = new ProtocolOrchestrator(channel, protocol) { Logger = logger ?? new NoOpLogger() };
//			await orchestrator.RunUntilClosed(cancellationToken);
//		}

//		public Task Start() => Start(Channel.DefaultTimeout);

//		public Task Start(TimeSpan timeout) => Start(new CancellationTokenSource(timeout).Token);

//		public Task Start(CancellationToken cancellationToken) => _machine.FireAsync(_startTrigger, cancellationToken);

//		public Task RunUntilClosed() => RunUntilClosed(new CancellationTokenSource().Token);

//		public async Task RunUntilClosed(CancellationToken cancellationToken) {
//			// Run until channel closed by other end (or cancelled by token)
//			var tcs = new TaskCompletionSource<bool>();
//			cancellationToken.Register(async () => {
//				if (Channel.State == ProtocolChannelState.Open)
//					await Channel.Close();
//				tcs.SetResult(false);
//			});
//			Channel.Closed += () => tcs.SetResult(false);
			
//			// Start the state-machine if not started yet
//			if (_machine.State == ProtocolOrchestratorState.NotStarted)
//				await Start(cancellationToken);

//			if (Channel.State != ProtocolChannelState.Closed)
//				await tcs.Task;
//		}

//		public async Task SendMessage(ProtocolDispatchType dispatchType, object message) {
//			if (!await TrySendMessage(dispatchType, message))
//				throw new ProtocolException($"Failed to send message: '{message}'");
//		}

//		public virtual Task<bool> TrySendMessage(ProtocolDispatchType dispatchType, object message)
//			=> TrySendMessage(dispatchType, message, Channel.DefaultTimeout);

//		public virtual Task<bool> TrySendMessage(ProtocolDispatchType dispatchType, object message, TimeSpan timeout)
//			=> TrySendMessage(dispatchType, message, new CancellationTokenSource(timeout).Token);

//		public virtual async Task<bool> TrySendMessage(ProtocolDispatchType dispatchType, object message, CancellationToken cancellationToken) {
//			var envelope = new ProtocolMessageEnvelope {
//				DispatchType = dispatchType,
//				RequestID = Interlocked.Increment(ref _messageID),
//				Message = message
//			};
//			return await TrySendEnvelope(envelope, cancellationToken);
//		}

//		protected virtual async Task<bool> TrySendEnvelope(ProtocolMessageEnvelope envelope, CancellationToken cancellationToken) {
//			var result = Result<bool>.Default;
//			await _machine.FireAsync(_sendMessageTrigger, envelope, cancellationToken, result);
//			result.ErrorMessages.ForEach(Logger.Error);
//			return result.Success;
//		}

//		private async Task TrySendEnvelopeInternal(ProtocolMessageEnvelope envelope, CancellationToken cancellationToken, Result result) {
//			// Start the state-machine if not started yet
//			if (_machine.State == ProtocolOrchestratorState.NotStarted)
//				await Start(cancellationToken);

//			// TODO: return error codes
//			if (!EnvelopeSerializer.TrySerializeLE(envelope, out var data)) {
//				result.AddError("Failed to serialize message envelope");
//				return;
//			}

//			if (!await Channel.TrySendBytes(data, cancellationToken)) {
//				result.AddError("Failed to transmit message envelope");
//				return;
//			}
//			ProcessSentMessage(envelope);
//			NotifySentMessage(envelope);
//		}

//		protected virtual async Task ProcessStart() {
//			// Open channel if closed
//			if (Channel.State != ProtocolChannelState.Open)
//				await Channel.Open();
//		}

//		protected void ProcessReceivedBytes(ReadOnlyMemory<byte> bytes) {
//			if (!EnvelopeSerializer.TryDeserializeLE(bytes.Span, out var envelope)) {
//				// Deal with problematic channel here (blacklist)
//				Logger.Error($"Failed to deserialize message envelope (byte length: {bytes.Length})");
//				// TODO: behaviour on error?
//				return;
//			}

//			// transition state-machine (will throw if not allowed to receive)
//			_machine.Fire(_receivedMessageTrigger, envelope);

//			// In Started mode, the orchestrator pushes received messages down the usual pipeline
//			NotifyReceivedMessage(envelope);
//			if (_machine.State != ProtocolOrchestratorState.Started)
//				ProcessReceivedMessage(envelope);
//		}

//		protected virtual void ProcessReceivedMessage(ProtocolMessageEnvelope envelope) {
//			switch (envelope.DispatchType) {
//				case ProtocolDispatchType.Command:
//					ProcessReceivedCommand(envelope.Message);
//					break;
//				case ProtocolDispatchType.Request:
//					ProcessReceivedRequest(envelope.RequestID, envelope.Message);
//					break;
//				case ProtocolDispatchType.Response:
//					ProcessReceivedResponse(envelope.RequestID, envelope.Message);
//					break;
//			}
//		}

//		protected virtual void ProcessSentMessage(ProtocolMessageEnvelope envelope) {
//			if (envelope.DispatchType == ProtocolDispatchType.Request) {
//				_unfulfilledRequests[envelope.RequestID] = envelope.Message;
//			}
//		}

//		protected virtual void ProcessReceivedCommand(object command) {
//			var commandType = command.GetType();
//			var commandHandler = ResolveCommandHandler(commandType);
//			ThreadPool.QueueUserWorkItem(_ => {
//				try {
//					commandHandler.Execute(this, command);
//				} catch (Exception error) {
//					Logger.Error($"Command handler for '{commandType.Name}' failed (mode: {ActiveMode})");
//					Logger.LogException(error);
//				}
//			});
//		}

//		protected virtual void ProcessReceivedRequest(int requestID, object request) {
//			var requestType = request.GetType();
//			var requestHandler = ResolveRequestHandler(requestType);
//			ThreadPool.QueueUserWorkItem(async _ => {
//				try {
//					var response = requestHandler.Execute(this, request);
//					var envelope = new ProtocolMessageEnvelope {
//						DispatchType = ProtocolDispatchType.Response,
//						RequestID = requestID,
//						Message = response
//					};
//					var timeout = new CancellationTokenSource(Channel.DefaultTimeout);
//					if (!await TrySendEnvelope(envelope, timeout.Token))
//						Logger.Error($"Unable to send response '{response}' to request '{request}' (mode: {ActiveMode})");
//				} catch (Exception error) {
//					Logger.Error($"Request handler for '{requestType.Name}' failed (mode: {ActiveMode})");
//					Logger.LogException(error);
//				}
//			});
//		}

//		protected virtual void ProcessReceivedResponse(int requestID, object response) {
//			var responseType = response.GetType();
//			object request;
//			IResponseHandler responseHandler;
//			using (_unfulfilledRequests.EnterWriteScope()) {
//				if (!_unfulfilledRequests.TryGetValue(requestID, out request))
//					throw new ProtocolException($"No unfulfilled Request with ID {requestID} was found (mode: {ActiveMode})");
//				var requestType = request.GetType();
//				responseHandler = ResolveResponseHandler(requestType, responseType);
//			}
//			ThreadPool.QueueUserWorkItem(_ => {
//				try {
//					responseHandler.Execute(this, request, response);
//				} catch (Exception error) {
//					Logger.Error($"Response handler for '{responseType.Name}' failed");
//					Logger.LogException(error);
//				}
//			});
//		}

//		protected virtual void OnReceivedMessage(ProtocolMessageEnvelope messageEnvelope) {
//		}

//		protected virtual void OnSentMessage(ProtocolMessageEnvelope messageEnvelope) {
//		}

//		protected virtual void OnStateChanged(ProtocolOrchestratorState state) {
//		}

//		private IMessageGenerator ResolveMessageGenerator(Type messageType) {
//			if (!Protocol.Modes[ActiveMode].MessageGenerators.TryGetValue(messageType, out var messageGenerator))
//				throw new ProtocolException($"Message Generator for '{messageType.Name}' not found (mode: {ActiveMode})");
//			return messageGenerator;
//		}

//		private ICommandHandler ResolveCommandHandler(Type commandType) {
//			if (!Protocol.Modes[ActiveMode].CommandHandlers.TryGetValue(commandType, out var commandHandler))
//				throw new ProtocolException($"Command handler for '{commandType}' not found (mode: {ActiveMode})");
//			return commandHandler;
//		}

//		private IRequestHandler ResolveRequestHandler(Type requestType) {
//			if (!Protocol.Modes[ActiveMode].RequestHandlers.TryGetValue(requestType, out var requestHandler))
//				throw new ProtocolException($"Request handler for '{requestType}' not found (mode: {ActiveMode})");
//			return requestHandler;
//		}

//		private IResponseHandler ResolveResponseHandler(Type requestType, Type responseType) {
//			if (!Protocol.Modes[ActiveMode].ResponseHandlers.TryGetValue(requestType, responseType, out var responseHandler))
//				throw new ProtocolException($"Response handler for '{responseType}' for Request '{requestType}' not found (mode: {ActiveMode})");
//			return responseHandler;
//		}

//		private void NotifyReceivedMessage(ProtocolMessageEnvelope messageEnvelope) {
//			OnReceivedMessage(messageEnvelope);
//			Tools.Threads.RaiseAsync(ReceivedMessage, messageEnvelope);
//		}

//		private void NotifySentMessage(ProtocolMessageEnvelope messageEnvelope) {
//			OnSentMessage(messageEnvelope);
//			Tools.Threads.RaiseAsync(SentMessage, messageEnvelope);
//		}

//		private void NotifyStateChanged(ProtocolOrchestratorState state) {
//			OnStateChanged(state);
//			Tools.Threads.RaiseAsync(StateChanged, state);
//		}

//	}
//}
