namespace Sphere10.Framework.Protocol {
	public abstract class ProtocolChannel<TEndpoint, TMessageID, TMessageType, TNonce, TPayload, TMessage, TCommand, TRequest, TResponse> : IProtocolChannel<TEndpoint, TMessageID, TMessageType, TNonce, TPayload, TMessage, TCommand, TRequest, TResponse>
		where TMessage : IProtocolMessage<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TCommand : IProtocolCommand<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TRequest : IProtocolRequest<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TResponse : IProtocolResponse<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> {

		public abstract void SendMessage(TMessage message);

		public abstract void SendCommand(TCommand command);

		public abstract void SendRequest(TRequest request);

		public abstract void ReceiveCommand(TCommand command);

		public abstract TResponse ReceiveRequest(TRequest request);

		public abstract void ReceiveResponse(TRequest sentRequest, TResponse response);
	}
}
