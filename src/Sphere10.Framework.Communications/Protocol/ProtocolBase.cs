using System;
using System.Collections.Generic;
using System.Threading;

namespace Sphere10.Framework.Communications.Protocol {

	public abstract class ProtocolBase<TEndpoint, TMessageType, TMessage, TCommandHandler, TRequestHandler> : IProtocol<TEndpoint, TMessageType, TMessage, TCommandHandler, TRequestHandler>
		where TCommandHandler : ICommandHandler<TEndpoint, TMessage>
		where TRequestHandler : IRequestHandler<TEndpoint, TMessage> {

		public IItemSerializer<TMessage> Serializer { get; init; }
		public IDictionary<TMessageType, TCommandHandler> CommandHandlers { get; init; }
		public IDictionary<TMessageType, TRequestHandler> RequestHandlers { get; init; }


		public abstract class ProtocolHubBase<TChannel, TProtocol> : IProtocolHub<TEndpoint, TMessageType, TMessage, TCommandHandler, TRequestHandler, TChannel, TProtocol>
			where TChannel : ChannelBase, new()
			where TProtocol : ProtocolBase<TEndpoint, TMessageType, TMessage, TCommandHandler, TRequestHandler> {

			public event EventHandlerEx<object, TMessage, TChannel> ReceivedConnection;
			
			public int MaxChannels { get; init; }
		
			public ISynchronizedReadOnlyList<TChannel> Channels { get; init; }

			public abstract TChannel InitiateConnection(TEndpoint endpoint, TMessage handshake);

			public abstract void Run(TProtocol protocol, CancellationToken cancellationToken);

		}

		public abstract class ChannelBase : Disposable, IProtocolChannel<TEndpoint, TMessageType, TMessage> {
			
			public event EventHandlerEx<object> Opening;
			public event EventHandlerEx<object> Opened;
			public event EventHandlerEx<object> Closing;
			public event EventHandlerEx<object> Closed;
			public event EventHandlerEx<object, TMessageType, TMessage> ReceivedMessage;

			public TEndpoint Local { get; init; }
			public TEndpoint Remote { get; init; }
			public ProtocolChannelInitiator Initiator { get; init; }

			public abstract void Open(TMessage message);

			public abstract void Close();

			public abstract void SendMessage(TMessageType messageType, TMessage message);

			protected virtual void OnOpening() {
			}

			protected virtual void OnOpened() {
			}

			protected virtual void OnClosing() {
			}

			protected virtual void OnClosed() {
			}

			protected virtual void OnReceivedMessage(TMessageType messageType, TMessage message) {
			}

			protected void NotifyOpening() {
				OnOpening();
				Opening?.Invoke(this);
			}

			protected void NotifyOpened() {
				OnOpened();
				Opened?.Invoke(this);
			}


			protected void NotifyClosing() {
				OnClosing();
				Closing?.Invoke(this);

			}


			protected void NotifyClosed() {
				OnClosed();
				Closed?.Invoke(this);
			}


			protected void NotifyReceivedMessage(TMessageType messageType, TMessage message) {
				OnReceivedMessage(messageType, message);
				ReceivedMessage?.Invoke(this, messageType, message);
			}

		}
	}



}
