﻿namespace Sphere10.Helium.Bus
{
    public class BusConfiguration : IBusConfiguration
    {
        public string SourceEndpointName { get; set; }

        public EnumEndpointType EndpointType { get; set; }

        public bool IsPersisted { get; set; } = true;

        public string FilePathForLocalQueuePersistence { get; set; }

        public int PageSize { get; set; }

        public int InMemoryPages { get; set; }

        public int ClusterSize { get; set; }

        public int ListingClusterCount { get; set; }

        public int StorageClusterCount { get; set; }

        public string FileNameForLocalQueue { get; set; }
    }
}
