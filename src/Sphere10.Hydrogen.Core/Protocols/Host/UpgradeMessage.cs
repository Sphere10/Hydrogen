using System;

namespace Sphere10.Hydrogen.Core.Protocols.Host {
	[Serializable]
	public class UpgradeMessage {
		public string HydrogenApplicationPackagePath { get; set; }
	}
}
