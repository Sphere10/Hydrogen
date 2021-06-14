using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {

    public interface IRequestHandler {
		object Execute(ProtocolChannel channel, object request);
	}

}
