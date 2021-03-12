﻿using Sphere10.Helium.Bus;

namespace Sphere10.Helium.Route
{
    /// <summary>
    ///
    /// 1) The Router has an infinite loop that removes the last message from the queue (FIFO).
    /// 2) It will continue to do so until the queue is empty.
    ///  
    /// </summary>

    public class Route : IRoute
    {
        private readonly IEndpointConfiguration _endpointConfiguration;

        public Route(IEndpointConfiguration endpointConfiguration)
        {
            _endpointConfiguration = endpointConfiguration;
        }


        public void ReadLastMessageFromQueue()
        {
            throw new System.NotImplementedException();
        }
    }
}
