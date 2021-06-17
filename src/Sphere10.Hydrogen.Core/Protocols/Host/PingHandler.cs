using Sphere10.Framework;
using Sphere10.Framework.Communications;

namespace Sphere10.Hydrogen.Core.Protocols.Host {
	public class PingHandler<TChannel> : RequestHandlerBase<TChannel, Ping, Pong> where TChannel : ProtocolChannel {
		public PingHandler(ILogger logger) {
			Logger = logger;
		}

		public override Pong Execute(TChannel channel, Ping command) {
			Logger.Info($"[Host Protocol] Received Ping");
			return new Pong();
		}

		public ILogger Logger { get; init; }
	}
}
