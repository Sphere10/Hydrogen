namespace Sphere10.Framework.Protocol {
	public class ProtocolMessageBase<TEndpoint, TMessageID, TMessageType> : IProtocolMessage<TEndpoint, TMessageID, TMessageType> {
		public TMessageID ID { get; init; }
		public TEndpoint Sender { get; init; }
		public TMessageType MessageType { get; init; }
	}
}
