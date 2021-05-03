namespace Sphere10.Framework.Protocol {
	public interface IProtocolMessage<out TEndpoint, out TMessageID, out TMessageType, out TNonce, out TPayload> {

		TMessageID ID { get; }

		TEndpoint Sender { get; }

		TMessageType MessageType { get; }

		TNonce Nonce { get; }

		TPayload Payload { get; }
	}
}
