using System;

namespace Sphere10.Helium.Queue
{
    public record QueueConfigDto
    {
		public Guid ID { get; init; }

		public string FilePath { get; init; }

		public string TempDirectoryPath { get; init; }

		public int MaxItems { get; init; }       

		public int MaxSizeBytes { get; init; }

		public int AllocatedMemory { get; init; }

		public int TransactionalPageSize { get; init; }

        public int ClusterSize { get; init; }

        public int ListingClusterCount { get; init; }

        public int StorageClusterCount { get; init; }

        public int InputQueueReadRatePerMinute { get; init; }

        //public QueueConfigDto(int pageSize, int inMemoryPages, int clusterSize, int listingClusterCount, int storageClusterCount, int inputQueueReadRatePerMinute)
        //{
		//    //TODO UPDATE WITH NEW PROPS
        //    PageSize = pageSize;
        //    InMemoryPages = inMemoryPages;
        //    ClusterSize = clusterSize;
        //    ListingClusterCount = listingClusterCount;
        //    StorageClusterCount = storageClusterCount;
        //    InputQueueReadRatePerMinute = inputQueueReadRatePerMinute;
        //}
    }
}
