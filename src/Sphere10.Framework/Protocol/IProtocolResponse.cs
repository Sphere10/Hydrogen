using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Framework.Protocol {

	public interface IProtocolResponse<TEndpoint, TMessageID, TMessageType> : IProtocolMessage<TEndpoint, TMessageID, TMessageType> {
		TMessageID RequestID { get; init; }
	}

}
