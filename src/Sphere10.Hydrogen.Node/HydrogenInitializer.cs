using Sphere10.Framework;
using Sphere10.Framework.Application;
using Sphere10.Hydrogen.Core.Mining;

namespace Sphere10.Hydrogen.Node {
	public class HydrogenInitializer : IApplicationInitializeTask {
		public int Priority => 0;

		public void Initialize() {
			SystemLog.RegisterLogger(new TimestampLogger(new DebugLogger()));
			
			//TODO: fetch server's init values from some global config module
			MiningRPCServer.Start(true, 27000, 5);
		}
	}

}
