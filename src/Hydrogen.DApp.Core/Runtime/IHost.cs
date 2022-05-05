using System.Threading.Tasks;
using Hydrogen;
using Hydrogen.Communications;

namespace Hydrogen.DApp.Core.Runtime {
	public interface IHost {
		event EventHandlerEx<AnonymousPipe> NodeStarted;
		event EventHandlerEx NodeEnded;
		HostStatus Status { get; }
		IApplicationPaths Paths { get; }
		Task DeployHAP(string newHapPath);
		Task Run();
		Task RequestShutdown();
	}
}
