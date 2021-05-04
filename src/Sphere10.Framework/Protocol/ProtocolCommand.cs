namespace Sphere10.Framework.Protocol {
	public class ProtocolCommand<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> : ProtocolMessage<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>,  IProtocolCommand<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> {

	}
}
