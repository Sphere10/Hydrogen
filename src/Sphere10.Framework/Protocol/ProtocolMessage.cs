namespace Sphere10.Framework.Protocol {
	public class ProtocolMessage<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> : IProtocolMessage<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> {
		public TMessageID ID { get; init; }
		public TEndpoint Sender { get; init; }
		public TMessageType MessageType { get; init; }
		public TNonce Nonce { get; init; }
		public TPayload Payload { get; init; }
	}
}
