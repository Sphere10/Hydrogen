using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Protocol {
	public interface IProtocolChannel<TEndpoint, TMessageType, TMessage> : IDisposable {
		public event EventHandlerEx<object> Opening;
		public event EventHandlerEx<object> Opened;
		public event EventHandlerEx<object> Closing;
		public event EventHandlerEx<object> Closed;
		public event EventHandlerEx<object, TMessageType, TMessage> ReceivedMessage;

		TEndpoint Local { get; init; }

		TEndpoint Remote { get; init; }

		ProtocolChannelInitiator Initiator { get; init; }

		void Open(TMessage handshake);

		void Close();

		void SendMessage(TMessageType messageType, TMessage message);
		
	}


}
