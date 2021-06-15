using Sphere10.Framework.Communications.RPC;

namespace Sphere10.Hydrogen.Node.RPC {

	//json rpc server singleton
	public class RpcMiningJsonServer {
		static private JsonRpcServer _instance = null;
		static public void Start(bool isLocal, int port, int maxListeners) {
			//Start server()
			_instance = new JsonRpcServer(new TcpEndPointListener(isLocal, port, maxListeners));
			_instance.Start();
		}

		static public void Stop() {
			_instance.Stop();
			_instance = null;
		}
	}
}
