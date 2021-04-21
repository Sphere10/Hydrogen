using Sphere10.Framework.Application;
using Sphere10.Hydrogen.Node.UI;

namespace Sphere10.Hydrogen.Node {
	public class StartNodeTask : IApplicationStartTask {
		// add priority and synchronous mode

		public void Start() {
			Navigator.Start();
		}
	}
}
