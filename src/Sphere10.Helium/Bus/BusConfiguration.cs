using Sphere10.Helium.Endpoint;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Bus {
	public record BusConfiguration 
	{
		public string SourceEndpointName { get; set; }

		public EnumEndpointType EndpointType { get; set; }

		public bool IsPersisted { get; set; } = true;

		public string FilePathForLocalQueuePersistence { get; set; }

		public string FileName { get; set; }

		public int RouteQueueReadRatePerMinute { get; set; } = 60;

		public QueueConfigDto QueueConfigDto { get; set; }
	}
}
