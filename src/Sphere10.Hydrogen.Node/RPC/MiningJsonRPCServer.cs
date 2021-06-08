using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sphere10.Framework;
using Sphere10.Framework.Communications.RPC;
using Sphere10.Hydrogen.Core.Configuration;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Core.Maths;

namespace Sphere10.Hydrogen.Core.Mining {
	public class MiningRPCServer {
		static JsonRpcServer _instance = null;
		static public void Start(bool isLocal, int port, int maxListeners) {
			//Start server()
			_instance = new JsonRpcServer(new TcpEndPointListener(isLocal, port, maxListeners));
			_instance.Start();
			Thread.Sleep(250);
		}

		static public void Stop() {
			_instance.Stop();
			_instance = null;
		}
	}
}
