using System;
using Sphere10.Helium.Endpoint;

namespace Sphere10.Helium.Bus {
	public class BusSetup {
		public IBus Create(BusConfiguration endpointConfiguration) {
			if (endpointConfiguration.EndpointType == EnumEndpointType.SendAndForget) {
				ISendOnlyBus sendOnlyBus = new SendOnlyBus(null, null);

				return sendOnlyBus as IBus;
			}

			if (endpointConfiguration.EndpointType == EnumEndpointType.SendAndResponse) {
				if (string.IsNullOrEmpty(endpointConfiguration.SourceEndpointName))
					throw new ArgumentNullException(endpointConfiguration.SourceEndpointName.GetType().FullName,
						"Cannot proceed! Need a return address for the Response.");

				ISendOnlyBus sendOnlyBus = new SendOnlyBus(null, null);

				return sendOnlyBus as IBus;
			}

			if (endpointConfiguration.EndpointType == EnumEndpointType.PublishAndSubscribe) {
				IBus bus = new Bus(null, null, null);

				return bus;
			}

			throw new ArgumentOutOfRangeException(endpointConfiguration.SourceEndpointName.GetType().FullName,
				"Cannot proceed! CRITICAL error no bus specified.");
		}
	}
}
