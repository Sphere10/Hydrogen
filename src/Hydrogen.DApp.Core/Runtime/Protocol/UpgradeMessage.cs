using System;

namespace Hydrogen.DApp.Core.Runtime {
	[Serializable]
	public class UpgradeMessage {
		public string HydrogenApplicationPackagePath { get; set; }
	}
}
