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
    public class FixedClusterMappedList<T> : RangedListBase<T>
    {
        private readonly int _clusterSize;
        private readonly int _listingClusterCount;
        private readonly int _storageClusterCount;
        private readonly int _listingSectorSize;

        private readonly IObjectSerializer<T> _serializer;
        private readonly Stream _stream;
        private StreamMappedList<Cluster> _clusters;

        private ListingSector _listingSector;
        private int _maxItems;

        public FixedClusterMappedList(int clusterSize,
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

        public override int Count => _listingSector.Count;

        public int Capacity => _listingSector.Capacity;

        public override void AddRange(IEnumerable<T> items)
        {
            if (!items.Any())
            {
                return;
            }
            
            List<byte[]> newData = new List<byte[]>();

            foreach (T item in items)
            {
                newData.Add(_serializer.SerializeLE(item));
            }

            var clusters = _listingSector.AddItems(newData);

            foreach (Cluster cluster in clusters)
            {
                if (_clusters.Count == cluster.Number)
                {
                    _clusters.Add(cluster);
                }
                else
                {
                    if (cluster.Number < _clusters.Count)
                    {
                        _clusters.Update(cluster.Number, cluster);
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Unable to add cluster number {cluster.Number}, append next only, current cluster number {_clusters.Count - 1}");
                    }
                }
            }
        }

        public override IEnumerable<int> IndexOfRange(IEnumerable<T> items)
        {
            T[] itemsArray = items as T[] ?? items.ToArray();
            int[] results = new int[itemsArray.Length];

            foreach (((int clusterStartIndex, int size), int i) in _listingSector.GetAllItems().WithIndex())
            {
                byte[] data = ReadDataFromClusters(clusterStartIndex, size);

                T item = _serializer.Deserialize(data.Length,
                    new EndianBinaryReader(EndianBitConverter.Little, new MemoryStream(data)));

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
                var listing = _listingSector.GetItem(index + i);

                byte[] data = ReadDataFromClusters(listing.StartIndex, listing.Size);
                T item = _serializer.Deserialize(listing.Size,
                    new EndianBinaryReader(EndianBitConverter.Little, new MemoryStream(data)));

                yield return item;
            }
        }

        public override void UpdateRange(int index, IEnumerable<T> items)
        {
            T[] itemsArray = items.ToArray();

            List<int> removedClusters = new List<int>();
            List<byte[]> updatedData = new List<byte[]>();

            for (int i = 0; i < itemsArray.Length; i++)
            {
                ItemListing listing = _listingSector.GetItem(index + i);
                IEnumerable<int> numbers = RemoveDataFromClusters(listing.StartIndex);
                removedClusters.AddRange(numbers);
                byte[] data = _serializer.SerializeLE(itemsArray[i]);

                updatedData.Add(data);
            }

            var updatedClusters = _listingSector.UpdateItemRange(index, updatedData, removedClusters);

            foreach (Cluster cluster in updatedClusters)
            {
                _clusters[cluster.Number] = cluster;
            }
        }

        public override void InsertRange(int index, IEnumerable<T> items)
        {
            if (!items.Any())
            {
                return;
            }

            List<byte[]> newData = new List<byte[]>();

            foreach (T item in items)
            {
                newData.Add(_serializer.SerializeLE(item));
            }

            var clusters = _listingSector.InsertItemRange(index, newData);

            foreach (Cluster sectorCluster in clusters)
            {
                if (_clusters.Count == sectorCluster.Number)
                {
                    _clusters.Add(sectorCluster);
                }
                else
                {
                    if (sectorCluster.Number < _clusters.Count)
                    {
                        _clusters.Update(sectorCluster.Number, sectorCluster);
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Unable to add cluster number {sectorCluster.Number}, append next only, current cluster number {_clusters.Count - 1}");
                    }
                }
            }
        }

        public override void RemoveRange(int index, int count)
        {
            List<int> removedItems = new List<int>();

            for (int i = 0; i < count; i++)
            {
                ItemListing listing = _listingSector.GetItem(index + i);
                IEnumerable<int> numbers = RemoveDataFromClusters(listing.StartIndex);
                removedItems.AddRange(numbers);
            }

            _listingSector.RemoveItemRange(index, count, removedItems);
        }

        private byte[] ReadDataFromClusters(int startCluster, int size)
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

            return stream.ToArray();
        }

        private IEnumerable<int> RemoveDataFromClusters(int startCluster)
        {
            int next = startCluster;

            List<int> numbers = new List<int>();
            while (next != -1)
            {
                Cluster cluster = _clusters[next];

                next = cluster.Next;
                cluster.Next = -1;
                _clusters.Update(cluster.Number, cluster);

                numbers.Add(cluster.Number);
            }

            return numbers;
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
                statusStream.MaxAbsolutePosition + clusterTotalSize) { UseRelativeOffset = true};

            _listingSector = new ListingSector(_clusterSize,
                _maxItems,
                _storageClusterCount,
                new StreamMappedList<ItemListing>( new ItemListingSerializer(), listingsStream) { IncludeListHeader = false},
                new StreamMappedList<bool>( new BoolSerializer(), statusStream)
                    {IncludeListHeader = false});
            
            _clusters = new StreamMappedList<Cluster>(new ClusterSerializer(_clusterSize), clusterStream, _clusterSize);
        }
    }

}