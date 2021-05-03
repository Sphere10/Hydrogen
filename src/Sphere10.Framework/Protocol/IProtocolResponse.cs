using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Framework.Protocol {

	public interface IProtocolResponse<out TEndpoint, out TMessageID, out TMessageType, out TNonce, out TPayload> : IProtocolMessage<TEndpoint, TMessageID, TMessageType, TNonce, TPayload> {

		TMessageID RequestID { get; }
	}

}
