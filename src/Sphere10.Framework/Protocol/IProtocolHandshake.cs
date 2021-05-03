namespace Sphere10.Framework.Protocol {
	public interface IProtocolHandshake<out TEndpoint, out TMessageID, out TMessageType, out TNonce, out TPayload> : IProtocolCommand<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> {
	}
}
