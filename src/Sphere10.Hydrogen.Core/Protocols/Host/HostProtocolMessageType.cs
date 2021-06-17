namespace Sphere10.Hydrogen.Core.Protocols.Host {
	public enum HostProtocolMessageType {
		Ping = 1,
		Pong = 2,
		Rollback = 3,
		Shutdown = 4,
		Upgrade = 5
	}
}
