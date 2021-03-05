using System;

namespace Sphere10.Helium.Bus
{
    public class BusSetup
    {
        public IBus Create(BusConfiguration busConfiguration)
        {
            if (busConfiguration.EndpointType == EnumEndpointType.SendAndForget)
            {
                ISendOnlyBus sendOnlyBus = new SendOnlyBus(null, null);

                return sendOnlyBus as IBus;
            }

            if (busConfiguration.EndpointType == EnumEndpointType.SendAndResponse)
            {
                if (string.IsNullOrEmpty(busConfiguration.SourceEndpointName))
                    throw new ArgumentNullException(busConfiguration.SourceEndpointName.GetType().FullName,
                        "Cannot proceed! Need a return address for the Response.");

                ISendOnlyBus sendOnlyBus = new SendOnlyBus(null, null);

                return sendOnlyBus as IBus;
            }

            if (busConfiguration.EndpointType == EnumEndpointType.PublishAndSubscribe)
            {
                IBus bus = new Bus(null, null, null);

                return bus;
            }

            throw new ArgumentOutOfRangeException(busConfiguration.SourceEndpointName.GetType().FullName,
                "Cannot proceed! CRITICAL error no bus specified.");
        }
    }
}
