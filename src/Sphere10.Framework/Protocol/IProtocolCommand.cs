namespace Sphere10.Framework.Protocol {
	public interface IProtocolCommand<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> : IProtocolMessage<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> {
	}


}
