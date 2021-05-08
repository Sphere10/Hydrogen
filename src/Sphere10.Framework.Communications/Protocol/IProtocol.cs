using System.Collections.Generic;

namespace Sphere10.Framework.Communications.Protocol {
	public interface IProtocol<TEndpoint, TMessageType, TMessage, TCommandHandler, TRequestHandler>
		where TCommandHandler : ICommandHandler<TEndpoint, TMessage>
		where TRequestHandler : IRequestHandler<TEndpoint, TMessage> {
		
		IItemSerializer<TMessage> Serializer { get; init; }

		IDictionary<TMessageType, TCommandHandler> CommandHandlers { get; init; }

		IDictionary<TMessageType, TRequestHandler> RequestHandlers { get; init; }

	}

}
