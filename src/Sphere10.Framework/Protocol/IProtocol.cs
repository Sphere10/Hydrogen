using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;

namespace Sphere10.Framework.Protocol {
	public interface IProtocol<TEndpoint, TMessageType, TMessage, TCommandHandler, TRequestHandler>
		where TCommandHandler : ICommandHandler<TEndpoint, TMessage>
		where TRequestHandler : IRequestHandler<TEndpoint, TMessage> {
		
		IItemSerializer<TMessage> Serializer { get; init; }

		IDictionary<TMessageType, TCommandHandler> CommandHandlers { get; init; }

		IDictionary<TMessageType, TRequestHandler> RequestHandlers { get; init; }

	}

}
