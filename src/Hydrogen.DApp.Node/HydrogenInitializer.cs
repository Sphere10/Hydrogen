using Hydrogen;
using Hydrogen.Application;
using Hydrogen.DApp.Node.RPC;

namespace Hydrogen.DApp.Node {

	public interface IHydrogenA {
	}

	public class HydrogenInitializer : BaseApplicationInitializer {
		
		public override void Initialize() {
			SystemLog.RegisterLogger(new TimestampLogger(new ConsoleLogger()));

			//NOTE: Until HydrogenInitializer gets to properly reference CryptoEx module, we init it here.
			Hydrogen.CryptoEx.ModuleConfiguration.Initialize();
			//SystemLog.RegisterLogger(new TimestampLogger(new DebugLogger()));


			//TODO: fetch server's init values from some global config module
			//RpcServer.Start(true, 27000, 32);
		}
	}

}
