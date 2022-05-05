using System.Threading.Tasks;

namespace Hydrogen.Communications {

    public interface IRequestHandler {
		object Execute(ProtocolOrchestrator orchestrator, object request);
	}

}
