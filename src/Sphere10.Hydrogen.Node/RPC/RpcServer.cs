using Sphere10.Framework;
using Sphere10.Framework.Communications.RPC;

namespace Sphere10.Hydrogen.Node.RPC {
	//Generic json rpc server singleton for spot calls (Pulse mode)
	public class RpcServer {
		static private JsonRpcServer _instance = null;

		static public void SetLogger(ILogger l) => _instance.SetLogger(l);
		static public void Start(bool isLocal, int port, int maxListeners) {
			//Start server()
			_instance = new JsonRpcServer(new TcpEndPointListener(isLocal, port, maxListeners), JsonRpcConfig.Default);
			_instance.Start();
		}

		static public void Stop() {
			_instance.Stop();
			_instance = null;
		}
	}

}

