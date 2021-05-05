namespace Sphere10.Framework.Protocol {
	public class ProtocolRequestBase<TEndpoint, TMessageID, TMessageType> : ProtocolMessageBase<TEndpoint, TMessageID, TMessageType>, IProtocolRequest<TEndpoint, TMessageID, TMessageType> {
	}
}
