using System;

namespace Sphere10.Helium.Bus
{
    public static class Bus
    {
        public static IBus Create(BusConfiguration busConfiguration)
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
