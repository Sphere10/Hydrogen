using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sphere10.Framework.Collections.StreamMapped;

namespace Sphere10.Framework.Collections {

	// this is like a StreamMappedList except it maps the object over non-contiguous sectors instead of a contiguous stream.
	// It uses a StreamMappedList of sectors under the hood.
	public class ClusteredStreamMappedList<T> : RangedListBase<T> {
		private const int HeaderSize = 256;

		private readonly int _clusterSize;
		private readonly int _storageClusterCount;
		private readonly int _maxItems;
		private readonly IObjectSerializer<T> _serializer;
		private readonly Stream _stream;

		private StreamMappedList<Cluster> _clusters;
		private IExtendedList<bool> _clusterStatus;
		private IExtendedList<ItemListing> _listings;

		public ClusteredStreamMappedList(int clusterSize,
			int maxItems,
			int storageClusterCount,
			IObjectSerializer<T> serializer,
			Stream stream) {
			_clusterSize = clusterSize;
			_storageClusterCount = storageClusterCount;
			_maxItems = maxItems;

			_serializer = serializer;
			_stream = stream;

			if (!_serializer.IsFixedSize) {
				throw new ArgumentException("Non fixed sized items not supported");
			}

			Initialize();
		}

		public override int Count => _listings.Count;

		public int Capacity => _maxItems;

		public override void AddRange(IEnumerable<T> items) {
			var itemsArray = items as T[] ?? items.ToArray();

			if (!itemsArray.Any()) {
				return;
			}

			foreach (T item in itemsArray) {
				int clusterStartIndex = AddItemToClusters(item);

				_listings.Add(new ItemListing {
					Size = _serializer.CalculateSize(item),
					ClusterStartIndex = clusterStartIndex
				});
			}

			UpdateCountHeader();
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<T> items) {
			T[] itemsArray = items as T[] ?? items.ToArray();
			int[] results = new int[itemsArray.Length];

			foreach ((ItemListing listing, int i) in _listings.WithIndex().Where(x => x.Item1.Size != 0)) {
				T item = ReadItemFromClusters(listing.ClusterStartIndex, listing.Size);

				foreach (var (t, index) in itemsArray.WithIndex()) {
					if (EqualityComparer<T>.Default.Equals(t, item)) {
						results[index] = i;
					}
				}
			}

			return results;
		}

		public override IEnumerable<T> ReadRange(int index, int count) {
			for (int i = 0; i < count; i++) {
				var listing = _listings[index + i];
				yield return ReadItemFromClusters(listing.ClusterStartIndex, listing.Size);
			}
		}

		public override void UpdateRange(int index, IEnumerable<T> items) {
			T[] itemsArray = items.ToArray();

			List<ItemListing> itemListings = new List<ItemListing>();
			for (int i = 0; i < itemsArray.Length; i++) {
				ItemListing listing = _listings[index + i];
				RemoveItemFromClusters(listing.ClusterStartIndex);
				int startIndex = AddItemToClusters(itemsArray[i]);

				itemListings.Add(new ItemListing {
					Size = _serializer.CalculateSize(itemsArray[i]),
					ClusterStartIndex = startIndex
				});
			}

			_listings.UpdateRange(index, itemListings);
		}

		public override void InsertRange(int index, IEnumerable<T> items) {
			var itemsArray = items as T[] ?? items.ToArray();

			if (_listings.Count + itemsArray.Length > _maxItems)
				throw new ArgumentException("Insufficient space");

			if (!itemsArray.Any()) {
				return;
			}

			List<ItemListing> listings = new List<ItemListing>();

			foreach (T item in itemsArray) {
				int clusterIndex = AddItemToClusters(item);

				listings.Add(new ItemListing {
					Size = _serializer.CalculateSize(item),
					ClusterStartIndex = clusterIndex
				});
			}

			_listings.InsertRange(index, listings);

			UpdateCountHeader();
		}

		public override void RemoveRange(int index, int count) {
			for (int i = 0; i < count; i++) {
				ItemListing listing = _listings[index + i];
				RemoveItemFromClusters(listing.ClusterStartIndex);
			}

			_listings.RemoveRange(index, count);

			UpdateCountHeader();
		}

		private T ReadItemFromClusters(int startCluster, int size) {
			int? next = startCluster;
			int remaining = size;

			ByteArrayBuilder builder = new ByteArrayBuilder();

			while (next != -1) {
				Cluster cluster = _clusters[next.Value];
				next = cluster.Next;

				if (cluster.Next < 0) {
					builder.Append(cluster.Data.Take(remaining).ToArray());
				} else {
					builder.Append(cluster.Data);
					remaining -= cluster.Data.Length;
				}
			}

			return _serializer.Deserialize(size,
				new EndianBinaryReader(EndianBitConverter.Little, new MemoryStream(builder.ToArray())));
		}

		private void RemoveItemFromClusters(int startCluster) {
			int next = startCluster;

			while (next != -1) {
				Cluster cluster = _clusters[next];
				_clusterStatus[cluster.Number] = false;
				next = cluster.Next;
			}
		}

		private int AddItemToClusters(T item) {
			List<Cluster> clusters = new List<Cluster>();

			using var stream = new MemoryStream();
			_serializer.Serialize(item, new EndianBinaryWriter(EndianBitConverter.Little, stream));
			byte[] data = stream.ToArray();

			List<IEnumerable<byte>> segments = data.PartitionBySize(x => 1, _clusterSize)
				.ToList();

			int[] numbers = _clusterStatus
				.WithIndex()
				.Where(x => !x.Item1)
				.Take(segments.Count)
				.Select(x => x.Item2)
				.ToArray();

			for (var i = 0; i < segments.Count; i++) {
				byte[] segment = segments[i].ToArray();
				byte[] clusterData = new byte[_clusterSize];
				segment.CopyTo(clusterData, 0);

				clusters.Add(new Cluster {
					Number = numbers[i],
					Data = clusterData,
					Next = segments.Count - 1 == i ? -1 : numbers[i + 1]
				});
			}

			foreach (var cluster in clusters) {
				_clusterStatus[cluster.Number] = true;
			}

			foreach (Cluster cluster in clusters) {
				if (!_clusters.Any()) {
					_clusters.Add(cluster);
				} else if (cluster.Number >= _clusters.Count) {
					_clusters.Add(cluster);
				} else {
					_clusters[cluster.Number] = cluster;
				}
			}

			return clusters.First().Number;
		}

		private void Initialize() {
			int listingSize = sizeof(int) + sizeof(int);
			int listingTotalSize = listingSize * _maxItems;
			int statusTotalSize = sizeof(bool) * _storageClusterCount;
			int clusterTotalSize = _clusterSize * _storageClusterCount;

			BoundedStream listingsStream = new BoundedStream(_stream, HeaderSize, HeaderSize + listingTotalSize - 1)
				{ UseRelativeOffset = true };
			BoundedStream statusStream = new BoundedStream(_stream,
				listingsStream.MaxAbsolutePosition + 1,
				listingsStream.MaxAbsolutePosition + statusTotalSize) { UseRelativeOffset = true };
			BoundedStream clusterStream = new BoundedStream(_stream,
				statusStream.MaxAbsolutePosition + 1,
				statusStream.MaxAbsolutePosition + clusterTotalSize) { UseRelativeOffset = true };

			if (_stream.Length == 0) {
				WriteHeader();
			} else {
				ReadHeader();
			}

			var preAllocatedListingStore = new StreamMappedList<ItemListing>(new ItemListingSerializer(), listingsStream)
				{ IncludeListHeader = false };

			if (preAllocatedListingStore.RequiresLoad) {
				preAllocatedListingStore.Load();
			} else {
				preAllocatedListingStore.AddRange(Enumerable.Repeat(default(ItemListing), _maxItems));
			}

			_listings = new PreAllocatedList<ItemListing>(preAllocatedListingStore);

			var status = new StreamMappedList<bool>(new BoolSerializer(), statusStream)
				{ IncludeListHeader = false };

			if (status.RequiresLoad) {
				status.Load();
			} else {
				status.AddRange(Enumerable.Repeat(false, _storageClusterCount));
			}
			_clusterStatus = new PreAllocatedList<bool>(status);

			if (!_clusterStatus.Any()) {
				_clusterStatus.AddRange(status);
			}

			_clusters = new StreamMappedList<Cluster>(StreamMappedListType.FixedSize,
				new ClusterSerializer(_clusterSize),
				clusterStream,
				_clusterSize * _storageClusterCount);

			if (_clusters.RequiresLoad) {
				_clusters.Load();
			}
		}

		private void WriteHeader() {
			var writer = new EndianBinaryWriter(EndianBitConverter.Little, _stream);

			_stream.Seek(0, SeekOrigin.Begin);

			writer.Write(0);
			writer.Write(Tools.Array.Gen(HeaderSize - (int)_stream.Position, (byte)0)); // padding
		}

		private int ReadHeader() {
			var reader = new EndianBinaryReader(EndianBitConverter.Little, _stream);
			_stream.Seek(0, SeekOrigin.Begin);

			var count = reader.ReadInt32();
			return count;
		}

		private void UpdateCountHeader() {
			var writer = new EndianBinaryWriter(EndianBitConverter.Little, _stream);
			_stream.Seek(0, SeekOrigin.Begin);

			writer.Write(_listings.Count);
		}
	}

}
