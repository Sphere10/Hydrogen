﻿using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Endpoint
{
    public interface IEndpointConfiguration
    {
        public string SourceEndpointName { get; set; }

        public EnumEndpointType EndpointType { get; set; }

        public bool IsPersisted { get; set; }

        public string FilePathForLocalQueuePersistence { get; set; }

        public string FileName { get; set; }

        public QueueConfigDto QueueConfigDto { get; set; }

        public int RouteQueueReadRatePerMinute { get; set; }

        public string ErrorQueueName { get; set; }

        public string AuditLogQueueName { get; set; }
    }
}