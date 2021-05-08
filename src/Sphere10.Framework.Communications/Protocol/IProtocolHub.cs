using System.Threading;

namespace Sphere10.Framework.Communications.Protocol {
	public interface IProtocolHub<TEndpoint, TMessageType, TMessage, TCommandHandler, TRequestHandler,  TChannel, TProtocol>
		where TCommandHandler : ICommandHandler<TEndpoint, TMessage>
		where TRequestHandler : IRequestHandler<TEndpoint, TMessage>
		where TChannel : IProtocolChannel<TEndpoint, TMessageType, TMessage>, new()
		where TProtocol : IProtocol<TEndpoint, TMessageType, TMessage, TCommandHandler, TRequestHandler> {

		event EventHandlerEx<object, TMessage, TChannel> ReceivedConnection;
			
		int MaxChannels { get; init; }

		ISynchronizedReadOnlyList<TChannel> Channels { get; }

		TChannel InitiateConnection(TEndpoint endpoint, TMessage handshake);

		void Run(TProtocol protocol, CancellationToken cancellationToken);
	}


}