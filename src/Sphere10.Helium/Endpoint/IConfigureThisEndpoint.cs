using Sphere10.Helium.Bus;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Endpoint
{
    public interface IConfigureThisEndpoint {

		public void SetupEndpoint(BusConfiguration busConfiguration);

        

    }
}