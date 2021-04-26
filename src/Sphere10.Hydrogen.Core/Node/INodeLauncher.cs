using System.Threading;
using Sphere10.Framework;

namespace Sphere10.Hydrogen.Node {

	public interface INodeLauncher {

		Result<NodeExitCode> Run(CancellationToken stopRunningToken);

	}
}
