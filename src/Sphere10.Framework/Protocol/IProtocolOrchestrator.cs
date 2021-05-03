using System;
using System.Collections.Generic;
using System.Threading;

namespace Sphere10.Framework.Protocol {
	public interface IProtocolOrchestrator<out TEndpoint, out TMessageID, TMessageType, out TNonce, out TPayload, TMessage, TCommand, TRequest, TResponse, in THandshake, out TProtocolChannel, in TProtocolHub>
		where THandshake : IProtocolHandshake<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TMessage : IProtocolMessage<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TCommand : IProtocolCommand<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TRequest : IProtocolRequest<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TResponse : IProtocolResponse<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TProtocolChannel : IProtocolChannel<TEndpoint, TMessageID, TMessageType, TNonce, TPayload, TMessage, TCommand, TRequest, TResponse>
		where TProtocolHub : IProtocolHub<TEndpoint, TMessageID, TMessageType, TNonce, TPayload, TMessage, TCommand, TRequest, TResponse, THandshake, TProtocolChannel> {

		IObjectSerializer<TMessage> Serializer { get; init; }

		IList<TMessage> MessageHandlers { get; init; }

		IDictionary<TMessageType, TCommand> CommandHandlers { get; init; }

		IDictionary<TMessageType, Func<TRequest, TResponse>> RequestHandlers { get; init; }

		void Run(TProtocolHub hub, CancellationToken cancellationToken);

	}
}
