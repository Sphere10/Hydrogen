namespace Sphere10.Framework.Protocol {
	public interface IProtocolChannel<out TEndpoint, out TMessageID, out TMessageType, out TNonce, out TPayload, in TMessage, in TCommand, in TRequest, TResponse>
		where TMessage : IProtocolMessage<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TCommand : IProtocolCommand<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TRequest : IProtocolRequest<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>
		where TResponse : IProtocolResponse<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> {

		void SendMessage(TMessage message);

		void SendCommand(TCommand command);

		void SendRequest(TRequest request);

		void ReceiveCommand(TCommand command);

		TResponse ReceiveRequest(TRequest request);

		void ReceiveResponse(TRequest sentRequest, TResponse response);

	}
}
