using Sphere10.Framework;
using Sphere10.Framework.Communications.RPC;

namespace Sphere10.Hydrogen.Node.RPC {

	public class RpcServerConfig {
		public bool IsLocal { get; set; }
		public int Port { get; set; }
		public int MaxListeners { get; set; }
	}
}

