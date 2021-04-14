using Sphere10.Helium.Bus;
using Sphere10.Helium.Endpoint;

namespace Sphere10.Helium.Route
{
    /// <summary>
    ///
    /// 1) The Router has an infinite loop that removes the last message from the queue (FIFO).
    /// 2) It will continue to do so until the queue is empty.
	/// 3) Even after the queue is empty it will keep on looping.
    ///  
    /// </summary>

    public class Route : IRoute
    {
        private readonly IConfigureThisEndpoint _endpointConfiguration;

        public Route(IConfigureThisEndpoint endpointConfiguration)
        {
            _endpointConfiguration = endpointConfiguration;
        }


        public void ReadLastMessageFromQueue()
        {
            throw new System.NotImplementedException();
        }
    }
}
