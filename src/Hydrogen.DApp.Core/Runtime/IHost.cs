using System.Threading.Tasks;
using Sphere10.Framework;
using Sphere10.Framework.Communications;

namespace Sphere10.Hydrogen.Core.Runtime {
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
