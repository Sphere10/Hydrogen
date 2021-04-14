using Sphere10.Helium.Endpoint;

namespace Sphere10.Helium.Bus {
	public interface INeedInitialization {
		void Customize(IConfigureThisEndpoint configuration);
	}
}
