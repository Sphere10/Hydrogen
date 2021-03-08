using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework.Collections
{
    public class ListingSector
    {
        private readonly int _clusterSize;
        private readonly int _storageClusterStart;

        public ListingSector(int clusterSize, int listingClusterCount, int storageClusterCount)
            : this(clusterSize,
                new BitArray(storageClusterCount, true),
                new List<StorageItemListing>(Enumerable.Repeat(default(StorageItemListing), CalculateMaximumItems(
                    clusterSize, listingClusterCount,
                    storageClusterCount))))
        {
            _storageClusterStart = listingClusterCount;
        }

        public ListingSector(int clusterSize, BitArray storageClusterStatus, List<StorageItemListing> storageListings)
        {
            _clusterSize = clusterSize;
            StorageClusterStatus = storageClusterStatus;
            StorageListings = storageListings;
        }

        public BitArray StorageClusterStatus { get; }

        public List<StorageItemListing> StorageListings { get; }

        public int Capacity => StorageListings.Count;

        public int Count => StorageListings.OrderByDescending(x => x.Size)
            .Count(x => x.Size != 0);

        /// <summary>
        /// Determines how many item listings may be stored in the available listing sector clusters. 
        /// </summary>
        /// <param name="clusterSize"></param>
        /// <param name="listingClusterCount"></param>
        /// <param name="storageClusterCount"></param>
        /// <returns></returns>
        private static int CalculateMaximumItems(int clusterSize, int listingClusterCount, int storageClusterCount)
        {
            int totalSpace = listingClusterCount * clusterSize;
            int statusSize = (int)Math.Ceiling((decimal)storageClusterCount / 8);
            int listingCount = sizeof(int);

            return (int)Math.Floor((decimal)(totalSpace - statusSize - listingCount) / (sizeof(int) + sizeof(int) /*sizeof(StorageItemListing)*/)); 
        }

        public IEnumerable<Cluster> AddItems(List<byte[]> data)
        {
            if (data.Count + Count > Capacity)
            {
                throw new InvalidOperationException("Item limit reached, no available listing cluster space.");
            }
            
            var free = StorageListings
                    .WithIndex()
                    .FirstOrDefault(x => x.Item1.Size == 0)
                ?? throw new InvalidOperationException("Item limit reached, no available listing cluster space.");
            
                
            return InsertItemRange(free.Item2, data);
        }

        public IEnumerable<Cluster> InsertItemRange(int index, IEnumerable<byte[]> items)
        {
            if (StorageListings.Count(x => x.Size == 0) < items.Count())
            {
                throw new InvalidOperationException("Item limit reached, no available listing cluster space.");
            }
            
            List<StorageItemListing> listings = new List<StorageItemListing>();
            List<Cluster> clusters = new List<Cluster>();
            
            foreach (byte[] data in items)
            {
                List<IEnumerable<byte>> segments = data.PartitionBySize(x => 1, _clusterSize)
                    .ToList();

                int[] numbers = StorageClusterStatus.Cast<bool>()
                    .WithIndex()
                    .Where(x => x.Item1)
                    .Take(segments.Count)
                    .Select(x => x.Item2 + _storageClusterStart)
                    .ToArray();

                if (numbers.Length < segments.Count)
                {
                    throw new InvalidOperationException("Item limit reached, no available storage cluster space.");
                }
                
                for (var i = 0; i < segments.Count; i++)
                {
                    byte[] segment = segments[i].ToArray();
                    byte[] clusterData = new byte[_clusterSize];
                    segment.CopyTo(clusterData, 0);

                    clusters.Add(new Cluster
                    {
                        Number = numbers[i],
                        Data = clusterData,
                        Next = segments.Count - 1 == i ? -1 : numbers[i + 1]
                    });
                }
            
                foreach (var cluster in clusters)
                {
                    StorageClusterStatus[cluster.Number - _storageClusterStart] = false;
                } 
                
                listings.Add(new StorageItemListing
                {
                    Size = data.Length,
                    StartIndex = numbers[0]
                });
            }

            var end = StorageListings.WithIndex().First(x => x.Item1.Size == 0);
            StorageListings.RemoveRange(end.Item2, listings.Count);
            StorageListings.InsertRange(index, listings);
            
            return clusters;
        }

        public IEnumerable<Cluster> UpdateItemRange(int index, IEnumerable<byte[]> items, IEnumerable<int> currentClusters)
        {
            List<StorageItemListing> listings = new List<StorageItemListing>();
            List<Cluster> clusters = new List<Cluster>();
            
            foreach (int item in currentClusters.Select(x => x - _storageClusterStart))
            {
                StorageClusterStatus[item] = true;
            }
            
            foreach (byte[] data in items)
                
            {
                List<IEnumerable<byte>> segments = data.PartitionBySize(x => 1, _clusterSize)
                    .ToList();

                int[] numbers = StorageClusterStatus.Cast<bool>()
                    .WithIndex()
                    .Where(x => x.Item1)
                    .Take(segments.Count)
                    .Select(x => x.Item2 + _storageClusterStart)
                    .ToArray();

                if (numbers.Length < segments.Count)
                {
                    throw new InvalidOperationException("Item limit reached, no available storage cluster space.");
                }
                
                for (var i = 0; i < segments.Count; i++)
                {
                    byte[] segment = segments[i].ToArray();
                    byte[] clusterData = new byte[_clusterSize];
                    segment.CopyTo(clusterData, 0);

                    clusters.Add(new Cluster
                    {
                        Number = numbers[i],
                        Data = clusterData,
                        Next = segments.Count - 1 == i ? -1 : numbers[i + 1]
                    });
                }
            
                foreach (var cluster in clusters)
                {
                    StorageClusterStatus[cluster.Number - _storageClusterStart] = false;
                } 
                
                listings.Add(new StorageItemListing
                {
                    Size = data.Length,
                    StartIndex = numbers[0]
                });
            }

            for (int i = 0; i < listings.Count; i++)
            {
                StorageListings[index + i] = listings[i];
            }
            
            return clusters;
        }

        public void RemoveItemRange(int index, int count, IEnumerable<int> clusterNumbers)
        {
            int[] itemArray = clusterNumbers as int[] ?? clusterNumbers.ToArray();
            itemArray = itemArray.Select(x => x - _storageClusterStart).ToArray();
            
            foreach (int item in itemArray)
            {
              StorageClusterStatus[item] = true;
            }

            StorageListings.RemoveRange(index, count);
            StorageListings.AddRange(Enumerable.Repeat<StorageItemListing>(default, count));
        }

        public StorageItemListing GetItem(int index)
        {
            Guard.ArgumentInRange(index, 0, StorageListings.Count - 1, nameof(index));

            if (StorageListings[index].Size != 0)
            {
                return StorageListings[index];
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public IEnumerable<(int clusterStartIndex, int size)> GetAllItems()
        {
            return StorageListings.OrderByDescending(x => x.Size)
                .Where(x => x.Size != 0)
                .Select(x => (x.StartIndex, x.Size));
        }
    }

}