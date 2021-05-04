namespace Sphere10.Framework.Protocol {
	public class ProtocolHandshake<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> : ProtocolCommand<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>, IProtocolHandshake<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> {
	}
}
