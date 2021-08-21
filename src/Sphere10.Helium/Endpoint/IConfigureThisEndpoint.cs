using Sphere10.Helium.Bus;

namespace Sphere10.Helium.Endpoint {
	public interface IConfigureThisEndpoint {

		public void SetupEndpoint(BusConfigurationDto busConfigurationDto);

	}
}