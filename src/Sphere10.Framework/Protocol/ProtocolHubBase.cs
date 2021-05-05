﻿using System.Threading;

namespace Sphere10.Framework.Protocol {
	public abstract class ProtocolHubBase<TEndpoint, TMessageID, TMessageType, TMessage, THandshake, TCommand, TRequest, TResponse, TChannel, TProtocol>
		: IProtocolHub<TEndpoint, TMessageID, TMessageType, TMessage, THandshake, TCommand, TRequest, TResponse, TChannel, TProtocol>
		where TMessage : IProtocolMessage<TEndpoint, TMessageID, TMessageType>
		where THandshake : TMessage, IProtocolHandshake<TEndpoint, TMessageID, TMessageType>
		where TCommand : TMessage, IProtocolCommand<TEndpoint, TMessageID, TMessageType>
		where TRequest : TMessage, IProtocolRequest<TEndpoint, TMessageID, TMessageType>
		where TResponse : TMessage, IProtocolResponse<TEndpoint, TMessageID, TMessageType>
		where TChannel : IProtocolChannel<TEndpoint, TMessageID, TMessageType, TMessage, THandshake, TCommand, TRequest, TResponse>, new()
		where TProtocol : IProtocol<TEndpoint, TMessageID, TMessageType, TMessage, THandshake, TCommand, TRequest, TResponse, TChannel> {
		
		private SynchronizedExtendedList<TChannel> _connections;

		protected ProtocolHubBase() {
			_connections = new SynchronizedExtendedList<TChannel>();
		}

		public int MaxChannels { get; init; }

		public ISynchronizedReadOnlyList<TChannel> Channels => _connections;

		public abstract TChannel ReceiveConnection(THandshake handshake);

		public abstract TChannel InitiateConnection(THandshake handshake);

		public void Run(TProtocol hub, CancellationToken cancellationToken) {
			throw new System.NotImplementedException();
		}
	}
}
