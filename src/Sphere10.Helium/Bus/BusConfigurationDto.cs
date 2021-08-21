using Sphere10.Helium.Endpoint;

namespace Sphere10.Helium.Bus {
	public record BusConfigurationDto {
		public string SourceEndpointName { get; set; }

		public EnumEndpointType EndpointType { get; set; }

		public bool IsPersisted { get; set; } = true;

		public string FilePathForLocalQueuePersistence { get; set; }

		public string FileName { get; set; }

		public int RouteQueueReadRatePerMinute { get; set; } = 60;
	}
}
