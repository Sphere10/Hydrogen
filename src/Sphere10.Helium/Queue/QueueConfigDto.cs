namespace Sphere10.Helium.Queue
{
    public record QueueConfigDto
    {
        public int PageSize { get; init; }

        public int InMemoryPages { get; init; }

        public int ClusterSize { get; init; }

        public int ListingClusterCount { get; init; }

        public int StorageClusterCount { get; init; }

        public QueueConfigDto(int pageSize, int inMemoryPages, int clusterSize, int listingClusterCount, int storageClusterCount)
        {
            PageSize = pageSize;
            InMemoryPages = inMemoryPages;
            ClusterSize = clusterSize;
            ListingClusterCount = listingClusterCount;
            StorageClusterCount = storageClusterCount;
        }
    }
}
