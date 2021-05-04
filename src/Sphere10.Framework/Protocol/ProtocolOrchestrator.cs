using System;
using System.Collections.Generic;
using System.Threading;

namespace Sphere10.Framework.Protocol {
	public abstract class ProtocolOrchestrator<TEndpoint, TMessageID, TMessageType, TNonce, TPayload, TMessage, TCommand, TRequest, TResponse, THandshake, TProtocolChannel, TProtocolHub> : IProtocolOrchestrator<TEndpoint, TMessageID, TMessageType, TNonce, TPayload, TMessage, TCommand, TRequest, TResponse, THandshake, TProtocolChannel, TProtocolHub>
		where THandshake : IProtocolHandshake<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TMessage : IProtocolMessage<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TCommand : IProtocolCommand<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TRequest : IProtocolRequest<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TResponse : IProtocolResponse<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TProtocolChannel : IProtocolChannel<TEndpoint, TMessageID, TMessageType, TNonce, TPayload, TMessage, TCommand, TRequest, TResponse>
		where TProtocolHub : IProtocolHub<TEndpoint, TMessageID, TMessageType, TNonce, TPayload, TMessage, TCommand, TRequest, TResponse, THandshake, TProtocolChannel> {

		public IObjectSerializer<TMessage> Serializer { get; init; }

		public IList<TMessage> MessageHandlers { get; init; }

		public IDictionary<TMessageType, TCommand> CommandHandlers { get; init; }

		public IDictionary<TMessageType, Func<TRequest, TResponse>> RequestHandlers { get; init; }

		public abstract void Run(TProtocolHub hub, CancellationToken cancellationToken);
	}
}
