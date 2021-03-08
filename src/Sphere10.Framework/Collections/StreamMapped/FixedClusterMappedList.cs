using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        private readonly StreamMappedList<Cluster> _clusters;

        private ListingSector _listingSector;

        public FixedClusterMappedList(int clusterSize,
            int listingClusterCount,
            int storageClusterCount,
            IObjectSerializer<T> serializer,
            Stream stream)
        {
            _clusters = new StreamMappedList<Cluster>(1, new ClusterSerializer(clusterSize), stream);
            if (_clusters.RequiresLoad)
            {
                _clusters.Load();
            }

            _clusterSize = clusterSize;
            _listingClusterCount = listingClusterCount;
            _storageClusterCount = storageClusterCount;
            _listingSectorSize = (int) Math.Ceiling((decimal) _listingClusterCount / _clusterSize);

            _serializer = serializer;

            InitializeListingSector();
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

            UpdateListingSector();
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
                StorageItemListing listing = _listingSector.GetItem(index + i);
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

            UpdateListingSector();
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

            UpdateListingSector();
        }

        public override void RemoveRange(int index, int count)
        {
            List<int> removedItems = new List<int>();

            for (int i = 0; i < count; i++)
            {
                StorageItemListing listing = _listingSector.GetItem(index + i);
                IEnumerable<int> numbers = RemoveDataFromClusters(listing.StartIndex);
                removedItems.AddRange(numbers);
            }

            _listingSector.RemoveItemRange(index, count, removedItems);

            UpdateListingSector();
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

        private void UpdateListingSector()
        {
            RemoveDataFromClusters(0);

            byte[] data = SerializeListingSector(_listingSector);
            IEnumerable<byte>[] partitions = data.PartitionBySize(x => 1, _clusterSize)
                .ToArray();

            List<Cluster> sectorClusters = new List<Cluster>();

            for (int i = 0; i < partitions.Length; i++)
            {
                byte[] clusterData = new byte[_clusterSize];
                partitions[i].ToArray().CopyTo(clusterData, 0);

                sectorClusters.Add(new Cluster
                {
                    Data = clusterData,
                    Number = i,
                    Next = partitions.Length - 1 == i ? -1 : i + 1
                });
            }

            foreach (Cluster sectorCluster in sectorClusters)
            {
                _clusters[sectorCluster.Number] = sectorCluster;
            }
        }

        private void InitializeListingSector()
        {
            if (!_clusters.Any())
            {
                ListingSector sector = new ListingSector(_clusterSize, _listingClusterCount, _storageClusterCount);
                byte[] sectorBytes = SerializeListingSector(sector);
                IEnumerable<byte>[] partitions = sectorBytes.PartitionBySize(x => 1, _clusterSize)
                    .ToArray();

                List<Cluster> sectorClusters = new List<Cluster>();

                for (int i = 0; i < partitions.Length; i++)
                {
                    byte[] clusterData = new byte[_clusterSize];
                    partitions[i].ToArray().CopyTo(clusterData, 0);

                    sectorClusters.Add(new Cluster
                    {
                        Data = clusterData,
                        Number = i,
                        Next = partitions.Length - 1 == i ? -1 : i + 1
                    });
                }

                _clusters.AddRange(sectorClusters);
                _listingSector = sector;
            }
            else
            {
                byte[] bytes = ReadDataFromClusters(0, _listingSectorSize);
                ListingSector sector = DeserializeListingSector(bytes);
                _listingSector = sector;
            }
        }

        private byte[] SerializeListingSector(ListingSector sector)
        {
            MemoryBuffer sectorBytes = new MemoryBuffer();

            byte[] BitArrayToByteArray(BitArray bits)
            {
                byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
                bits.CopyTo(ret, 0);
                return ret;
            }

            sectorBytes.AddRange(BitArrayToByteArray(sector.StorageClusterStatus));

            foreach (StorageItemListing sectorListing in sector.StorageListings)
            {
                sectorBytes.AddRange(EndianBitConverter.Little.GetBytes(sectorListing.StartIndex));
                sectorBytes.AddRange(EndianBitConverter.Little.GetBytes(sectorListing.Size));
            }

            return sectorBytes.ToArray();
        }

        private ListingSector DeserializeListingSector(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            EndianBinaryReader reader = new EndianBinaryReader(EndianBitConverter.Little, stream);

            byte[] statusBytes = reader.ReadBytes((int) Math.Ceiling((decimal) _storageClusterCount / 8));
            BitArray status = new BitArray(statusBytes);
            List<StorageItemListing> listings = new List<StorageItemListing>();

            while (true)
            {
                try
                {
                    int index = reader.ReadInt32();
                    int size = reader.ReadInt32();
                    listings.Add(new StorageItemListing {Size = size, StartIndex = index});
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }

            return new ListingSector(_clusterSize, status, listings);
        }
    }

}