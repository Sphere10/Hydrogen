using System;
using System.Collections.Generic;
using System.Threading;

namespace Sphere10.Framework.Protocol {
	public abstract class ProtocolBase<TEndpoint, TMessageID, TMessageType, TMessage, TCommand, TRequest, TResponse, THandshake, TChannel>
		: IProtocol<TEndpoint, TMessageID, TMessageType, TMessage, THandshake, TCommand, TRequest, TResponse, TChannel>
		where TMessage : IProtocolMessage<TEndpoint, TMessageID, TMessageType>
		where THandshake : TMessage, IProtocolHandshake<TEndpoint, TMessageID, TMessageType>
		where TCommand : TMessage, IProtocolCommand<TEndpoint, TMessageID, TMessageType>
		where TRequest : TMessage, IProtocolRequest<TEndpoint, TMessageID, TMessageType>
		where TResponse : TMessage, IProtocolResponse<TEndpoint, TMessageID, TMessageType>
		where TChannel : IProtocolChannel<TEndpoint, TMessageID, TMessageType, TMessage, THandshake, TCommand, TRequest, TResponse> {

		public IItemSerializer<TMessage> Serializer { get; init; }

		public IList<TMessage> MessageHandlers { get; init; }

		public IDictionary<TMessageType, TCommand> CommandHandlers { get; init; }

		public IDictionary<TMessageType, Func<TRequest, TResponse>> RequestHandlers { get; init; }

	}
}
