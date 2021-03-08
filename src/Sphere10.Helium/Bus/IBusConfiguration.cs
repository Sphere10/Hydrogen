namespace Sphere10.Helium.Bus
{
    public interface IBusConfiguration
    {
        public string SourceEndpointName { get; set; }

        public EnumEndpointType EndpointType { get; set; }

        public bool IsPersisted { get; set; }

        public string FilePathForLocalQueuePersistence { get; set; }

        public int PageSize { get; set; }
        
        public int InMemoryPages { get; set; }

        public int ClusterSize { get; set; }

        public int ListingClusterCount { get; set; }

        public int StorageClusterCount { get; set; }
    }
}