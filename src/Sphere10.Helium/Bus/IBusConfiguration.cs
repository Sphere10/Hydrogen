namespace Sphere10.Helium.Bus
{
    public interface IBusConfiguration
    {
        public string SourceEndpointName { get; set; }

        public EnumEndpointType EndpointType { get; set; }

        public bool IsPersisted { get; set; }

        public string FilePathForLocalQueuePersistence { get; set; }
    }
}