using Hydrogen;
using Hydrogen.Communications.RPC;

namespace Hydrogen.DApp.Node.RPC {

	public class RpcServerConfig {

		public bool IsLocal { get; set; }
		public int Port { get; set; }
		public int MaxListeners { get; set; }
	}
}

