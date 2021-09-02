using Sphere10.Framework.Application;

namespace Sphere10.Helium.Endpoint {
	public class EndPointSettings : SettingsObject {
		public bool FlushLocalQueueOnStartup { get; set; }
		public bool FlushPrivateQueueOnStartup { get; set; }
	}
}