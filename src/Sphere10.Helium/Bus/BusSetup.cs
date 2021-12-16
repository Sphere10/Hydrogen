using System;
using Sphere10.Helium.HeliumNode;

namespace Sphere10.Helium.Bus {
	public class BusSetup {
		public IBus Create(BusConfigurationSettings endpointConfigurationDto) {
			if (endpointConfigurationDto.EndpointType == EnumEndpointType.SendAndForget) {

				return null;
			}

			if (endpointConfigurationDto.EndpointType == EnumEndpointType.SendAndResponse) {
				if (string.IsNullOrEmpty(endpointConfigurationDto.SourceEndpointName))
					throw new ArgumentNullException(endpointConfigurationDto.SourceEndpointName.GetType().FullName,
						"Cannot proceed! Need a return address for the Response.");

				return null;
			}

			if (endpointConfigurationDto.EndpointType == EnumEndpointType.PublishAndSubscribe) {

				return null;
			}

			throw new ArgumentOutOfRangeException(endpointConfigurationDto.SourceEndpointName.GetType().FullName,
				"Cannot proceed! CRITICAL error no bus specified.");
		}
	}
}
