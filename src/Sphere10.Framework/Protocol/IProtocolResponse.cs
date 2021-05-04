using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Framework.Protocol {

	public interface IProtocolResponse<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> : IProtocolMessage<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> {
		TMessageID RequestID { get; init; }
	}

}
