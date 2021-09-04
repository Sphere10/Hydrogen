using Sphere10.Framework.Application;

namespace Sphere10.Helium.HeliumNode {
	public class HeliumNodeSettings : SettingsObject {
		public bool FlushLocalQueueOnStartup { get; set; }
		public bool FlushPrivateQueueOnStartup { get; set; }
	}
}