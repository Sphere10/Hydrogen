using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {

    public class ProtocolOrchestrator {
	    public const int DefaultTimeoutMS = 5000;
        private readonly SynchronizedDictionary<int, object> _unfulfilledRequests; // TODO add expiration 
        private volatile int _messageID;
        public event EventHandlerEx<ProtocolMessageEnvelope> ReceivedMessage;
        public event EventHandlerEx<ProtocolMessageEnvelope> SentMessage;

        public ProtocolOrchestrator(ProtocolChannel channel, Protocol protocol){
            Guard.ArgumentNotNull(channel, nameof(channel));
            Guard.ArgumentNotNull(protocol, nameof(protocol));
            _unfulfilledRequests = new SynchronizedDictionary<int, object>();
            Channel = channel;
            Protocol = protocol;
            Channel.ReceivedBytes += ProcessReceivedBytes;
            EnvelopeSerializer = new ProtocolMessageEnvelopeSerializer(protocol.MessageSerializer);
            _messageID = 0;
        }

        public ProtocolChannel Channel { get; }

        public Protocol Protocol { get; }

        public ILogger Logger { get; init; } = new NoOpLogger();

        protected ProtocolMessageEnvelopeSerializer EnvelopeSerializer { get; }

		public static async Task Run(ProtocolChannel channel, Protocol protocol, CancellationToken cancellationToken, ILogger logger = null) {
	        var orchestrator = new ProtocolOrchestrator(channel, protocol) { Logger = logger ?? new NoOpLogger() };
	        await orchestrator.Run(cancellationToken);
        }

		public Task Run() => Run(new CancellationTokenSource().Token);

		public async Task Run(CancellationToken cancellationToken) {
			var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(async () => {
                if (Channel.State == ProtocolChannelState.Open)
                    await Channel.Close();
                tcs.SetResult(false);
            });
            Channel.Closed += () => tcs.SetResult(false);
            await tcs.Task;
        }

		public async Task SendMessage(ProtocolDispatchType dispatchType, object message) {
			if (!await TrySendMessage(dispatchType, message))
				throw new ProtocolException($"Failed to send message: '{message}'");
		}

		public virtual Task<bool> TrySendMessage(ProtocolDispatchType dispatchType, object message)
			=> TrySendMessage(dispatchType, message, Channel.DefaultTimeout);

		public virtual Task<bool> TrySendMessage(ProtocolDispatchType dispatchType, object message, TimeSpan timeout)
			=> TrySendMessage(dispatchType, message, new CancellationTokenSource(timeout).Token);

		public virtual async Task<bool> TrySendMessage(ProtocolDispatchType dispatchType, object message, CancellationToken cancellationToken) {
			var envelope = new ProtocolMessageEnvelope {
				DispatchType = dispatchType,
				RequestID = Interlocked.Increment(ref _messageID),
				Message = message
			};
			return await TrySendEnvelope(envelope, cancellationToken);
		}

		protected virtual async Task<bool> TrySendEnvelope(ProtocolMessageEnvelope envelope, CancellationToken cancellationToken) {
            // TODO: return error codes
            if (!EnvelopeSerializer.TrySerializeLE(envelope, out var data)) {
                Logger.Error("Failed to serialize message envelope");
	            return false;
            }

            if (!await Channel.TrySendBytes(data, cancellationToken)) {
	            Logger.Error("Failed to transmit message envelope");
                return false;
            }

            NotifySentMessage(envelope);
            ProcessSentMessage(envelope);
			return true;
		}

		protected void ProcessReceivedBytes(ReadOnlyMemory<byte> bytes) {
			if (!EnvelopeSerializer.TryDeserializeLE(bytes.Span, out var envelope)) {
				// Deal with problematic channel here (blacklist)
				Logger.Error("Failed to deserialize message envelope");
			}
			NotifyReceivedMessage(envelope);
            ProcessReceivedMessage(envelope);
		}

        protected virtual void ProcessReceivedMessage(ProtocolMessageEnvelope envelope) {
            try {
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
            } catch (Exception error) {
                // Deal with problematic channel here (blacklist)
                throw error;
            }
        }

        protected virtual void ProcessSentMessage(ProtocolMessageEnvelope envelope) {
            if (envelope.DispatchType == ProtocolDispatchType.Request) {
                _unfulfilledRequests[envelope.RequestID] = envelope.Message;
            }
        }

        protected virtual void ProcessReceivedCommand(object command) {
            var commandType = command.GetType();
            if (!Protocol.CommandHandlers.TryGetValue(commandType, out var commandHandler))
                throw new ProtocolException($"Command handler for '{commandType}' not found");
            ThreadPool.QueueUserWorkItem(_ => {
                try {
                    commandHandler.Execute(Channel, command);
                } catch (Exception error) {
                    Logger.Error($"Command handler for '{commandType.Name}' failed");
                    Logger.LogException(error);
                }
            });
        }

        protected virtual void ProcessReceivedRequest(int requestID, object request) {
            var requestType = request.GetType();
            if (!Protocol.RequestHandlers.TryGetValue(requestType, out var requestHandler))
                throw new ProtocolException($"Request handler for '{requestType}' not found");
            ThreadPool.QueueUserWorkItem(async _ => {
                try {
                    var response = requestHandler.Execute(Channel, request);
                    var envelope = new ProtocolMessageEnvelope {
                        DispatchType = ProtocolDispatchType.Response,
                        RequestID = requestID,
                        Message = response
                    };
                    var timeout = new CancellationTokenSource(Channel.DefaultTimeout);
                    if (!await TrySendEnvelope(envelope, timeout.Token))
                        Logger.Error($"Unable to send response '{response}' to request '{request}'");
                } catch (Exception error) {
                    Logger.Error($"Request handler for '{requestType.Name}' failed");
                    Logger.LogException(error);
                }
            });
        }

        protected virtual void ProcessReceivedResponse(int requestID, object response) {
            var responseType = response.GetType();
            object request;
            IResponseHandler responseHandler;
            using (_unfulfilledRequests.EnterWriteScope()) {
                if (!_unfulfilledRequests.TryGetValue(requestID, out request))
                    throw new ProtocolException($"No unfulfilled Request with ID {requestID} was found");
                var requestType = request.GetType();
                if (!Protocol.ResponseHandlers.TryGetValue(requestType, responseType, out responseHandler))
                    throw new ProtocolException($"Response handler for '{responseType}' for Request '{requestType}' not found");
            }
            ThreadPool.QueueUserWorkItem(_ => {
                try {
                    responseHandler.Execute(Channel, request, response);
                } catch (Exception error) {
                    Logger.Error($"Response handler for '{responseType.Name}' failed");
                    Logger.LogException(error);
                }
            });
        }

        protected virtual void OnReceivedMessage(ProtocolMessageEnvelope messageEnvelope) {
        }

        protected virtual void OnSentMessage(ProtocolMessageEnvelope messageEnvelope) {
        }


        private void NotifyReceivedMessage(ProtocolMessageEnvelope messageEnvelope) {
	        OnReceivedMessage(messageEnvelope);
	        Tools.Threads.RaiseAsync(ReceivedMessage, messageEnvelope);
        }

        private void NotifySentMessage(ProtocolMessageEnvelope messageEnvelope) {
	        OnSentMessage(messageEnvelope);
	        Tools.Threads.RaiseAsync(SentMessage, messageEnvelope);
        }



    }
}
