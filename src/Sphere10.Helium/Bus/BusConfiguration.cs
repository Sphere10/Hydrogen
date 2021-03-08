﻿using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Bus
{
    public class BusConfiguration : IBusConfiguration
    {
        public string SourceEndpointName { get; set; }

        public EnumEndpointType EndpointType { get; set; }

        public bool IsPersisted { get; set; } = true;

        public string FilePathForLocalQueuePersistence { get; set; }
        
        public string FileName { get; set; }

        public QueueConfigDto QueueConfigDto { get; set; }

        public int PageSize { get; set; }
    }
}
