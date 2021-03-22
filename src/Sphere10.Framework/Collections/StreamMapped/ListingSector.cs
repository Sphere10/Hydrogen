using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework.Collections
{
    public class ListingSector
    {
        private readonly int _clusterSize;
        private readonly int _maxItems;
        private readonly StreamMappedList<ItemListing> _listings;
        private readonly StreamMappedList<bool> _clusterStatus;

        public ListingSector(
            int clusterSize, 
            int maxItems, 
            int storageClusterCount,
            StreamMappedList<ItemListing> listings, 
            StreamMappedList<bool> clusterStatus)
        {
            _clusterSize = clusterSize;
            _maxItems = maxItems;
            _listings = listings;
            _clusterStatus = clusterStatus;
            
            if (!clusterStatus.Any())
            {
                clusterStatus.AddRange(Enumerable.Repeat(true, storageClusterCount));
            }

            if (!listings.Any())
            {
                listings.AddRange(Enumerable.Repeat(default(ItemListing), maxItems));
            }
        }

        public int Capacity => _maxItems;

        public int Count => _listings.Count;

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
            int end = _listings.Count == 0 ? 0 : _listings.Count - 1;

            return InsertItemRange(end, data);
        }

        public IEnumerable<Cluster> InsertItemRange(int index, IEnumerable<byte[]> items)
        {
            if (_listings.Count + items.Count() > _maxItems)
            {
                throw new InvalidOperationException("Item limit reached, no available listing cluster space.");
            }
            
            List<ItemListing> listings = new List<ItemListing>();
            List<Cluster> clusters = new List<Cluster>();
            
            foreach (byte[] data in items)
            {
                List<IEnumerable<byte>> segments = data.PartitionBySize(x => 1, _clusterSize)
                    .ToList();

                int[] numbers = _clusterStatus
                    .WithIndex()
                    .Where(x => x.Item1)
                    .Take(segments.Count)
                    .Select(x => x.Item2)
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
                    _clusterStatus[cluster.Number] = false;
                } 
                
                listings.Add(new ItemListing
                {
                    Size = data.Length,
                    ClusterStartIndex = numbers[0]
                });
            }

            for (int i = 0; i < listings.Count; i++)
            {
                _listings[index + i] = listings[i];
            }

            return clusters;
        }

        public IEnumerable<Cluster> UpdateItemRange(int index, IEnumerable<byte[]> items, IEnumerable<int> currentClusters)
        {
            List<ItemListing> listings = new List<ItemListing>();
            List<Cluster> clusters = new List<Cluster>();
            
            foreach (int item in currentClusters)
            {
                _clusterStatus[item] = true;
            }
            
            foreach (byte[] data in items)
                
            {
                List<IEnumerable<byte>> segments = data.PartitionBySize(x => 1, _clusterSize)
                    .ToList();

                int[] numbers = _clusterStatus
                    .WithIndex()
                    .Where(x => x.Item1)
                    .Take(segments.Count)
                    .Select(x => x.Item2)
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
                    _clusterStatus[cluster.Number] = false;
                } 
                
                listings.Add(new ItemListing
                {
                    Size = data.Length,
                    ClusterStartIndex = numbers[0]
                });
            }

            _listings.UpdateRange(index, listings);
            
            return clusters;
        }

        public void RemoveItemRange(int index, int count, IEnumerable<int> clusterNumbers)
        {
            int[] itemArray = clusterNumbers as int[] ?? clusterNumbers.ToArray();
            itemArray = itemArray.ToArray();
            
            foreach (int item in itemArray)
            {
              _clusterStatus[item] = true;
            }

            _listings.RemoveRange(index, count);
            _listings.AddRange(Enumerable.Repeat<ItemListing>(default, count));
        }

        public ItemListing GetItem(int index)
        {
            if (_listings[index].Size != 0)
            {
                return _listings[index];
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public IEnumerable<(int clusterStartIndex, int size)> GetAllItems()
        {
            return _listings.OrderByDescending(x => x.Size)
                .Where(x => x.Size != 0)
                .Select(x => (StartIndex: x.ClusterStartIndex, x.Size));
        }
    }

}