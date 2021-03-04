using System;
using Sphere10.Helium.MessageType;

namespace Sphere10.Helium.Bus
{
    public class Bus
    {
        public IBus Create(BusConfiguration busConfiguration)
        {
            if (busConfiguration.EndpointType == EnumEndpointType.SendAndForget ||
                busConfiguration.EndpointType == EnumEndpointType.SendAndResponse)
            {
                throw new NotImplementedException();
            }

            return null;
        }
    }
}
