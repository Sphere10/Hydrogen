using Sphere10.Helium.HeliumNode;

namespace Sphere10.Helium.Bus {
	public interface INeedInitialization {
		void Customize(IConfigureHeliumEndpoint configuration);
	}
}
