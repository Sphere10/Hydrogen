using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {
	
	public interface IRequestHandler<in TChannel, TMessage> where TChannel : ProtocolChannel {
		TMessage Execute(TChannel channel, TMessage request);
	}
}
