namespace Sphere10.Framework.Protocol {
	public abstract class ProtocolChannelBase<TEndpoint, TMessageID, TMessageType, TMessage, THandshake, TCommand, TRequest, TResponse> 
		: Disposable, IProtocolChannel<TEndpoint, TMessageID, TMessageType, TMessage, THandshake, TCommand, TRequest, TResponse>
		where TMessage : IProtocolMessage<TEndpoint, TMessageID, TMessageType>
		where THandshake : TMessage, IProtocolHandshake<TEndpoint, TMessageID, TMessageType>
		where TCommand : TMessage, IProtocolCommand<TEndpoint, TMessageID, TMessageType>
		where TRequest : TMessage, IProtocolRequest<TEndpoint, TMessageID, TMessageType>
		where TResponse : TMessage, IProtocolResponse<TEndpoint, TMessageID, TMessageType> {

		public event EventHandlerEx<object> Opening;
		public event EventHandlerEx<object> Opened;
		public event EventHandlerEx<object> Closing;
		public event EventHandlerEx<object> Closed;

		public abstract void Open(THandshake handshake);

		public abstract void Close();

		public abstract void SendMessage(TMessage message);

		public abstract void SendCommand(TCommand command);

		public abstract void SendRequest(TRequest request);

		public abstract void ReceiveCommand(TCommand command);

		public abstract TResponse ReceiveRequest(TRequest request);

		public abstract void ReceiveResponse(TRequest sentRequest, TResponse response);
	}
}
