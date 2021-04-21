using Sphere10.Framework;
using Sphere10.Framework.Application;

namespace Sphere10.Hydrogen.Node {
	public class HydrogenInitializer : IApplicationInitializeTask {
		public int Priority => 0;

		public void Initialize() {
			SystemLog.RegisterLogger(new TimestampLogger(new DebugLogger()));
		}
	}


}
