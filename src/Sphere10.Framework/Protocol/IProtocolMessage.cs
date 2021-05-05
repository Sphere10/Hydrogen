namespace Sphere10.Framework.Protocol {
	public interface IProtocolMessage<TEndpoint, TMessageID, TMessageType> {

		TMessageID ID { get; init; }

		TEndpoint Sender { get; init; }

		TMessageType MessageType { get; init; }

	}


}
