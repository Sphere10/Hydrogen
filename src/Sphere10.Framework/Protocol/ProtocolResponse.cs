namespace Sphere10.Framework.Protocol {
	public class ProtocolResponse<TEndpoint, TMessageID, TMessageType>  : ProtocolMessageBase<TEndpoint, TMessageID, TMessageType>, IProtocolResponse<TEndpoint, TMessageID, TMessageType>  {

		public TMessageID RequestID { get; init; }
	}
}
