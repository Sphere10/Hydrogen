using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Bus
{
    public interface IBusConfiguration
    {
        public string SourceEndpointName { get; set; }

        public EnumEndpointType EndpointType { get; set; }

        public bool IsPersisted { get; set; }

        public string FilePathForLocalQueuePersistence { get; set; }

        public string FileName { get; set; }

        public QueueConfigDto QueueConfigDto { get; set; }

    }
}