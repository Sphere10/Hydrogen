namespace Sphere10.Framework.Protocol {
	public class ProtocolResponse<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>  : ProtocolMessage<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>, IProtocolResponse<TEndpoint, TMessageID, TMessageType, TNonce, TPayload>  {

		public TMessageID RequestID { get; init; }
	}
}
