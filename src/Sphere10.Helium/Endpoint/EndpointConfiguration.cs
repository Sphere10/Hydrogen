﻿using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Endpoint
{
    public class EndpointConfiguration : IEndpointConfiguration
    {
        public string SourceEndpointName { get; set; }

        public EnumEndpointType EndpointType { get; set; }

        public bool IsPersisted { get; set; } = true;

        public string FilePathForLocalQueuePersistence { get; set; }
        
        public string FileName { get; set; }

        public QueueConfigDto QueueConfigDto { get; set; }

        public int RouteQueueReadRatePerMinute { get; set; } = 60;

        public string ErrorQueueName { get; set; } = "ErrorQueue";

        public string AuditLogQueueName { get; set; } = "AuditLogQueue";
    }
}
