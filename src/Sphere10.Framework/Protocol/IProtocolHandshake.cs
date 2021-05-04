namespace Sphere10.Framework.Protocol {
	public interface IProtocolHandshake<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> : IProtocolCommand<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> {
	}

}
