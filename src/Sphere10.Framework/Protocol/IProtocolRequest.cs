namespace Sphere10.Framework.Protocol {
	public interface IProtocolRequest<out TEndpoint, out TMessageID, out TMessageType, out TNonce, out TPayload> : IProtocolMessage<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> {
	}
}
