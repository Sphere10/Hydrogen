namespace Sphere10.Framework.Protocol {
	public interface IProtocolRequest<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> : IProtocolMessage<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> {
	}

}
