namespace Sphere10.Framework.Protocol {
	public class ProtocolRequest<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> : ProtocolMessage<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>, IProtocolRequest<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> {
	}
}
