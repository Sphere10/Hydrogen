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
            unsafe
            {
                int totalSpace = listingClusterCount * clusterSize;
                int statusSize = (int) Math.Ceiling((decimal) storageClusterCount / 8);
                int listingCount = sizeof(int);

                return (int) Math.Floor((decimal) (totalSpace - statusSize - listingCount) /
                    sizeof(StorageItemListing));
            }
        }

        public IEnumerable<Cluster> AddItem(byte[] data)
        {
            return InsertItem(StorageListings
                    .WithIndex()
                    .First(x => x.Item1.Size == 0)
                    .Item2
                , data);
        }

        public IEnumerable<Cluster> InsertItem(int index, byte[] data)
        {
            if (Count == Capacity)
            {
                throw new InvalidOperationException("Item limit reached, no available listing cluster space.");
            }
            
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

            List<Cluster> clusters = new List<Cluster>();
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

            StorageListings.Remove(StorageListings.First(x => x.Size == 0));
            StorageListings.Insert(index, new StorageItemListing
            {
                Size = data.Length,
                StartIndex = numbers[0]
            });

            foreach (var cluster in clusters)
            {
                StorageClusterStatus[cluster.Number - _storageClusterStart] = false;
            }

            return clusters;
        }

        public void RemoveItem(int index, IEnumerable<int> clusterNumbers)
        {
            var array = clusterNumbers as int[] ??
                clusterNumbers
                    .Select(x => x - _storageClusterStart)
                    .ToArray();

            StorageListings.RemoveAt(index);
            StorageListings.Add(default);
            
            foreach (int clusterNumber in array)
            {
                StorageClusterStatus[clusterNumber] = true;
            }
        }

        public void RemoveItemRange(IEnumerable<(int index, IEnumerable<int> clusters)> items)
        {
            (int index, IEnumerable<int> clusters)[] itemArray = items as (int index, IEnumerable<int> clusters)[] ?? items.ToArray();
            
            foreach (var item in itemArray)
            {
                var array = item.clusters as int[] ??
                    item.clusters
                        .Select(x => x - _storageClusterStart)
                        .ToArray();
                
                foreach (int clusterNumber in array)
                {
                    StorageClusterStatus[clusterNumber] = true;
                }
            }

            StorageListings.RemoveRange(itemArray.First().index, itemArray.Length);
            StorageListings.AddRange(Enumerable.Repeat<StorageItemListing>(default, itemArray.Length));
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