using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {

    public interface IRequestHandler {
		object Execute(ProtocolOrchestrator orchestrator, object request);
	}

}
