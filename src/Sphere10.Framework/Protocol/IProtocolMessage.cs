namespace Sphere10.Framework.Protocol {
	public interface IProtocolMessage<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> {

		TMessageID ID { get; init; }

		TEndpoint Sender { get; init; }

		TMessageType MessageType { get; init; }

		TNonce Nonce { get; init; }

		TPayload Payload { get; init; }
	}


}
