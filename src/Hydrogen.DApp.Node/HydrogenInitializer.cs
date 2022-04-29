using Sphere10.Framework;
using Sphere10.Framework.Application;
using Sphere10.Hydrogen.Node.RPC;

namespace Sphere10.Hydrogen.Node {

	public interface IHydrogenA {
	}

	public class HydrogenInitializer : IApplicationInitializeTask {
		public int Priority => 0;

		public void Initialize() {
			SystemLog.RegisterLogger(new TimestampLogger(new ConsoleLogger()));

			//NOTE: Until HydrogenInitializer gets to properly reference CryptoEx module, we init it here.
			Sphere10.Framework.CryptoEx.ModuleConfiguration.Initialize();
			//SystemLog.RegisterLogger(new TimestampLogger(new DebugLogger()));


			//TODO: fetch server's init values from some global config module
			RpcServer.Start(true, 27000, 32);
		}
	}

}
