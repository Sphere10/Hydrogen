using System;
using Sphere10.Helium.Endpoint;

namespace Sphere10.Helium.Bus {
	public class BusSetup {
		public IBus Create(BusConfigurationDto endpointConfigurationDto) {
			if (endpointConfigurationDto.EndpointType == EnumEndpointType.SendAndForget) {
				ISendOnlyBus sendOnlyBus = new SendOnlyBus(null, null);

				return sendOnlyBus as IBus;
			}

			if (endpointConfigurationDto.EndpointType == EnumEndpointType.SendAndResponse) {
				if (string.IsNullOrEmpty(endpointConfigurationDto.SourceEndpointName))
					throw new ArgumentNullException(endpointConfigurationDto.SourceEndpointName.GetType().FullName,
						"Cannot proceed! Need a return address for the Response.");

				ISendOnlyBus sendOnlyBus = new SendOnlyBus(null, null);

				return sendOnlyBus as IBus;
			}

			if (endpointConfigurationDto.EndpointType == EnumEndpointType.PublishAndSubscribe) {
				IBus bus = new Bus(null, null, null);

				return bus;
			}

			throw new ArgumentOutOfRangeException(endpointConfigurationDto.SourceEndpointName.GetType().FullName,
				"Cannot proceed! CRITICAL error no bus specified.");
		}
	}
}
