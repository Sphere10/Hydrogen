using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {

    public class ProtocolOrchestrator {
        private readonly SynchronizedDictionary<int, object> _unfulfilledRequests; // TODO add expiration 
       
        public ProtocolOrchestrator(ProtocolChannel channel, Protocol protocol){
            Guard.ArgumentNotNull(channel, nameof(channel));
            Guard.ArgumentNotNull(protocol, nameof(protocol));
            _unfulfilledRequests = new SynchronizedDictionary<int, object>();
            Channel = channel;
            Protocol = protocol;
            Channel.ReceivedMessage += (message) => ProcessReceivedMessage(message);
            Channel.SentMessage += (message) => ProcessSentMessage(message);
        }

        public ProtocolChannel Channel { get; }

        public Protocol Protocol { get; }

        public ILogger Logger { get; init; } = new NoOpLogger();

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

        public static async Task Run(ProtocolChannel channel, Protocol protocol, CancellationToken cancellationToken, ILogger logger = null) {
            var orchestrator = new ProtocolOrchestrator(channel, protocol) { Logger = logger ?? new NoOpLogger() };
            await orchestrator.Run(cancellationToken);
        }

        protected virtual void ProcessReceivedMessage(ProtocolMessageEnvelope envelope) {
            try {
                switch (envelope.MessageType) {
                    case ProtocolMessageType.Command:
                        ProcessReceivedCommand(envelope.Message);
                        break;
                    case ProtocolMessageType.Request:
                        ProcessReceivedRequest(envelope.RequestID, envelope.Message);
                        break;
                    case ProtocolMessageType.Response:
                        ProcessReceivedResponse(envelope.RequestID, envelope.Message);
                        break;
                }
            } catch (Exception error) {
                // Deal with problematic channel here (blacklist)
                throw error;
            }
        }

        protected virtual void ProcessSentMessage(ProtocolMessageEnvelope envelope) {
            if (envelope.MessageType == ProtocolMessageType.Request) {
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
                        MessageType = ProtocolMessageType.Response,
                        RequestID = requestID,
                        Message = response
                    };
                    var timeout = new CancellationTokenSource(Channel.DefaultTimeout);
                    if (!await Channel.TrySendEnvelope(envelope, timeout.Token))
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
                    throw new ProtocolException($"No unfullfilled Request with ID {requestID} was found");
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
    }
}
