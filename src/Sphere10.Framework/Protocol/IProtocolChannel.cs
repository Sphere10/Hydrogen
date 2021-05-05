using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Protocol {
	public interface IProtocolChannel<TEndpoint, TMessageID, TMessageType, TMessage, THandshake, TCommand, TRequest, TResponse> : IDisposable
		where TMessage : IProtocolMessage<TEndpoint, TMessageID, TMessageType>
		where THandshake : TMessage, IProtocolHandshake<TEndpoint, TMessageID, TMessageType>
		where TCommand : TMessage, IProtocolCommand<TEndpoint, TMessageID, TMessageType>
		where TRequest : TMessage, IProtocolRequest<TEndpoint, TMessageID, TMessageType>
		where TResponse : TMessage, IProtocolResponse<TEndpoint, TMessageID, TMessageType> {

		public event EventHandlerEx<object> Opening;
		public event EventHandlerEx<object> Opened;
		public event EventHandlerEx<object> Closing;
		public event EventHandlerEx<object> Closed;

		void Open(THandshake handshake);

		void Close();

		void SendMessage(TMessage message);

		void SendCommand(TCommand command);

		void SendRequest(TRequest request);

		void ReceiveCommand(TCommand command);

		TResponse ReceiveRequest(TRequest request);

		void ReceiveResponse(TRequest sentRequest, TResponse response);

	}

}
