using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sphere10.Framework.Collections.StreamMapped;

namespace Sphere10.Framework.Collections
{

    // this is like a StreamMappedList except it maps the object over non-contiguous sectors instead of a contiguous stream.
    // It uses a StreamMappedList of sectors under the hood.
    public class ClusteredStreamMappedList<T> : RangedListBase<T>
    {
        private readonly int _clusterSize;
        private readonly int _storageClusterCount;

        private readonly IObjectSerializer<T> _serializer;
        private readonly Stream _stream;

        private StreamMappedList<Cluster> _clusters;
        private IExtendedList<bool> _clusterStatus;
        private IExtendedList<ItemListing> _listings;

        private int _maxItems;

        public ClusteredStreamMappedList(int clusterSize,
            int maxItems,
            int storageClusterCount,
            IObjectSerializer<T> serializer,
            Stream stream)
        {
            _clusterSize = clusterSize;
            _storageClusterCount = storageClusterCount;
            _maxItems = maxItems;

            _serializer = serializer;
            _stream = stream;

            if (!_serializer.IsFixedSize)
            {
                throw new ArgumentException("Non fixed sized items not supported");
            }

            Initialize();
        }

        public override int Count => _listings.Count(x => x.Size > 0);

        public int Capacity => _maxItems;

        public override void AddRange(IEnumerable<T> items)
        {
            if (!items.Any())
            {
                return;
            }

            List<ItemListing> itemListings = new List<ItemListing>();
            foreach (T item in items)
            {
                int index = AddItemToClusters(item);
                itemListings.Add(new ItemListing
                {
                    Size = _serializer.CalculateSize(item),
                    StartIndex = index
                });
            }
            
            _listings.AddRange(itemListings);
        }

        public override IEnumerable<int> IndexOfRange(IEnumerable<T> items)
        {
            T[] itemsArray = items as T[] ?? items.ToArray();
            int[] results = new int[itemsArray.Length];

            foreach ((ItemListing listing, int i) in _listings.WithIndex())
            {
                T item = ReadItemFromClusters(listing.StartIndex, listing.Size);
                
                foreach (var (t, index) in itemsArray.WithIndex())
                {
                    if (EqualityComparer<T>.Default.Equals(t, item))
                    {
                        results[index] = i;
                    }
                }
            }

            return results;
        }

        public override IEnumerable<T> ReadRange(int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var listing = _listings[index];

                yield return ReadItemFromClusters(listing.StartIndex, listing.Size);
            }
        }

        public override void UpdateRange(int index, IEnumerable<T> items)
        {
            T[] itemsArray = items.ToArray();

            List<ItemListing> itemListings = new List<ItemListing>();
            for (int i = 0; i < itemsArray.Length; i++)
            {
                ItemListing listing = _listings[index + i];
                RemoveItemFromClusters(listing.StartIndex);
                int startIndex = AddItemToClusters(itemsArray[i]);
                
                itemListings.Add(new ItemListing { Size = _serializer.CalculateSize(itemsArray[i]), StartIndex = startIndex});
            }

            _listings.UpdateRange(index, itemListings);
        }

        private int AddItemToClusters(T item)
        {
            List<Cluster> clusters = new List<Cluster>();

            byte[] data = _serializer.SerializeLE(item);

            List<IEnumerable<byte>> segments = data.PartitionBySize(x => 1, _clusterSize)
                .ToList();

            int[] numbers = _clusterStatus
                .WithIndex()
                .Where(x => x.Item1)
                .Take(segments.Count)
                .Select(x => x.Item2)
                .ToArray();

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
                _clusterStatus[cluster.Number] = true;
            }

            int first = _listings.WithIndex().First(x => x.Item1.Size == 0).Item2;
            _listings[first] = new ItemListing
            {
                Size = data.Length,
                StartIndex = clusters[0].Number
            };

            return _listings[first].StartIndex;
        }

        public override void InsertRange(int index, IEnumerable<T> items)
        {
            if (!items.Any())
            {
                return;
            }

            List<ItemListing> listings = new List<ItemListing>();

            foreach (T item in items)
            {
                int clusterIndex = AddItemToClusters(item);
                
                listings.Add(new ItemListing
                {
                    Size = _serializer.CalculateSize(item),
                    StartIndex = clusterIndex
                });
            }
            
            _listings.InsertRange(index, listings);
        }

        public override void RemoveRange(int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                ItemListing listing = _listings[index + i];
                RemoveItemFromClusters(listing.StartIndex);
            }

            _listings.RemoveRange(index, count);
        }

        private T ReadItemFromClusters(int startCluster, int size)
        {
            int? next = startCluster;
            int remaining = size;
            MemoryStream stream = new MemoryStream();

            while (next != -1)
            {
                Cluster cluster = _clusters[next.Value];
                next = cluster.Next;

                if (cluster.Next < 0)
                {
                    stream.Write(cluster.Data, 0, remaining);
                }
                else
                {
                    stream.Write(cluster.Data);
                    remaining -= cluster.Data.Length;
                }
            }

            return _serializer.Deserialize(size,
                new EndianBinaryReader(EndianBitConverter.Little, new MemoryStream(stream.ToArray())));
        }

        private void RemoveItemFromClusters(int startCluster)
        {
            int next = startCluster;

            while (next != -1)
            {
                Cluster cluster = _clusters[next];

                next = cluster.Next;
                cluster.Next = -1;
                _clusters.Update(cluster.Number, cluster);
                _clusterStatus[cluster.Number] = false;
            }
        }

        private void Initialize()
        {
            int listingSize = sizeof(int) + sizeof(int);
            int listingTotalSize = listingSize * _maxItems;
            int statusTotalSize = sizeof(bool) * _storageClusterCount;
            int clusterTotalSize = _clusterSize * _storageClusterCount;

            BoundedStream listingsStream = new BoundedStream(_stream, 0, listingTotalSize - 1)
                {UseRelativeOffset = true};
            BoundedStream statusStream = new BoundedStream(_stream, listingsStream.MaxAbsolutePosition + 1,
                listingsStream.MaxAbsolutePosition + statusTotalSize) {UseRelativeOffset = true};
            BoundedStream clusterStream = new BoundedStream(_stream, statusStream.MaxAbsolutePosition + 1,
                statusStream.MaxAbsolutePosition + clusterTotalSize) {UseRelativeOffset = true};

            var listings = new StreamMappedList<ItemListing>(new ItemListingSerializer(), listingsStream)
                {IncludeListHeader = false};
            listings.AddRange(Enumerable.Repeat(default(ItemListing), _maxItems));
            _listings = new AllocatedList<ItemListing>(listings);

            var status = new StreamMappedList<bool>(new BoolSerializer(), statusStream)
                {IncludeListHeader = false};
            status.AddRange(Enumerable.Repeat(false, _storageClusterCount));
            
            _clusterStatus = new AllocatedList<bool>(status);

            _clusters = new StreamMappedList<Cluster>(new ClusterSerializer(_clusterSize), clusterStream, _clusterSize);
        }
    }

}