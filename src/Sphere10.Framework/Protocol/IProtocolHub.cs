using System.Collections.Generic;
using System.Threading;

namespace Sphere10.Framework.Protocol {
	public interface IProtocolHub<TEndpoint, TMessageID, TMessageType, TMessage, THandshake, TCommand, TRequest, TResponse, TChannel, TProtocol>
		where TMessage : IProtocolMessage<TEndpoint, TMessageID, TMessageType>
		where THandshake : TMessage, IProtocolHandshake<TEndpoint, TMessageID, TMessageType>
		where TCommand : TMessage, IProtocolCommand<TEndpoint, TMessageID, TMessageType>
		where TRequest : TMessage, IProtocolRequest<TEndpoint, TMessageID, TMessageType>
		where TResponse : TMessage, IProtocolResponse<TEndpoint, TMessageID, TMessageType>
		where TChannel : IProtocolChannel<TEndpoint, TMessageID, TMessageType, TMessage, THandshake, TCommand, TRequest, TResponse>, new()
		where TProtocol : IProtocol<TEndpoint, TMessageID, TMessageType, TMessage, THandshake, TCommand, TRequest, TResponse, TChannel> {
		
		int MaxChannels { get; init; }

		ISynchronizedReadOnlyList<TChannel> Channels { get; }

		TChannel ReceiveConnection(THandshake handshake);

		TChannel InitiateConnection(THandshake handshake);

		void Run(TProtocol hub, CancellationToken cancellationToken);
	}


}