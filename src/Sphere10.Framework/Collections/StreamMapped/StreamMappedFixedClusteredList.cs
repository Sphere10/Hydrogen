using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {


	/// <summary>
	/// A list implementation which is mapped onto a stream via clusters. Items can added/removed anywhere, and they are stored
	/// in a non-contiguous manner via clusters of data (similar in principle to a file system format). The limitation here is that the
	/// the list is inherantly bounded from construction and cannot grow past the pre-determined maximum number of items. This uses
	/// <see cref="StreamMappedPagedList{TItem}"/> under the hood.
	/// </summary>
	public class StreamMappedFixedClusteredList<T> : RangedListBase<T> {
		private const int HeaderSize = 256;

		private readonly int _clusterDataSize;
		private readonly IObjectSerializer<T> _serializer;
		private readonly int _storageClusterCount;
		private readonly Stream _stream;

		private StreamMappedPagedList<Cluster> _clusters;
		private IExtendedList<bool> _clusterStatus;
		private IExtendedList<ItemListing> _listings;

		public StreamMappedFixedClusteredList(int clusterDataSize, int maxItems, IObjectSerializer<T> serializer, Stream stream) {
			_clusterDataSize = clusterDataSize;
			Capacity = maxItems;

			_serializer = serializer;
			_stream = stream;
			
			if (!_serializer.IsFixedSize)
				throw new ArgumentException("Non fixed sized items not supported");

			_storageClusterCount = (int)Math.Ceiling(_serializer.FixedSize / (double)clusterDataSize) * maxItems;
			
			Initialize();
		}

		public override int Count => _listings.Count;

		public int Capacity { get; }

		public override void AddRange(IEnumerable<T> items) {
			var itemsArray = items as T[] ?? items.ToArray();

			if (!itemsArray.Any())
				return;

			foreach (var item in itemsArray) {
				var clusterStartIndex = AddItemToClusters(item);

				_listings.Add(new ItemListing {
					Size = _serializer.CalculateSize(item),
					ClusterStartIndex = clusterStartIndex
				});
			}

			UpdateCountHeader();
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<T> items) {
			var itemsArray = items as T[] ?? items.ToArray();
			var results = new int[itemsArray.Length];

			foreach ((var listing, var i) in _listings.WithIndex().Where(x => x.Item1.Size != 0)) {
				var item = ReadItemFromClusters(listing.ClusterStartIndex, listing.Size);

				foreach (var (t, index) in itemsArray.WithIndex())
					if (EqualityComparer<T>.Default.Equals(t, item))
						results[index] = i;
			}

			return results;
		}

		public override IEnumerable<T> ReadRange(int index, int count) {
			for (var i = 0; i < count; i++) {
				var listing = _listings[index + i];
				yield return ReadItemFromClusters(listing.ClusterStartIndex, listing.Size);
			}
		}

		public override void UpdateRange(int index, IEnumerable<T> items) {
			var itemsArray = items.ToArray();

			var itemListings = new List<ItemListing>();
			for (var i = 0; i < itemsArray.Length; i++) {
				var listing = _listings[index + i];
				RemoveItemFromClusters(listing.ClusterStartIndex);
				var startIndex = AddItemToClusters(itemsArray[i]);

				itemListings.Add(new ItemListing {
					Size = _serializer.CalculateSize(itemsArray[i]),
					ClusterStartIndex = startIndex
				});
			}

			_listings.UpdateRange(index, itemListings);
		}

		public override void InsertRange(int index, IEnumerable<T> items) {
			var itemsArray = items as T[] ?? items.ToArray();

			if (_listings.Count + itemsArray.Length > Capacity)
				throw new ArgumentException("Insufficient space");

			if (!itemsArray.Any())
				return;

			var listings = new List<ItemListing>();

			foreach (var item in itemsArray) {
				var clusterIndex = AddItemToClusters(item);

				listings.Add(new ItemListing {
					Size = _serializer.CalculateSize(item),
					ClusterStartIndex = clusterIndex
				});
			}

			_listings.InsertRange(index, listings);

			UpdateCountHeader();
		}

		public override void RemoveRange(int index, int count) {
			for (var i = 0; i < count; i++) {
				var listing = _listings[index + i];
				RemoveItemFromClusters(listing.ClusterStartIndex);
			}

			_listings.RemoveRange(index, count);

			UpdateCountHeader();
		}

		private T ReadItemFromClusters(int startCluster, int size) {
			int? next = startCluster;
			var remaining = size;

			var builder = new ByteArrayBuilder();

			while (next != -1) {
				var cluster = _clusters[next.Value];
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
			var next = startCluster;

			while (next != -1) {
				var cluster = _clusters[next];
				_clusterStatus[cluster.Number] = false;
				next = cluster.Next;
			}
		}

		private int AddItemToClusters(T item) {
			var clusters = new List<Cluster>();

			using var stream = new MemoryStream();
			_serializer.Serialize(item, new EndianBinaryWriter(EndianBitConverter.Little, stream));
			var data = stream.ToArray();

			var segments = data.PartitionBySize(x => 1, _clusterDataSize)
				.ToList();

			var numbers = _clusterStatus
				.WithIndex()
				.Where(x => !x.Item1)
				.Take(segments.Count)
				.Select(x => x.Item2)
				.ToArray();

			for (var i = 0; i < segments.Count; i++) {
				var segment = segments[i].ToArray();
				var clusterData = new byte[_clusterDataSize];
				segment.CopyTo(clusterData, 0);

				clusters.Add(new Cluster {
					Number = numbers[i],
					Data = clusterData,
					Next = segments.Count - 1 == i ? -1 : numbers[i + 1]
				});
			}

			foreach (var cluster in clusters)
				_clusterStatus[cluster.Number] = true;

			foreach (var cluster in clusters)
				if (!_clusters.Any())
					_clusters.Add(cluster);
				else if (cluster.Number >= _clusters.Count)
					_clusters.Add(cluster);
				else
					_clusters[cluster.Number] = cluster;

			return clusters.First().Number;
		}

		private void Initialize() {

			var clusterSerializer = new ClusterSerializer(_clusterDataSize);
			var listingSize = sizeof(int) + sizeof(int);
			var listingTotalSize = listingSize * Capacity;
			var statusTotalSize = sizeof(bool) * _storageClusterCount;
			var clusterTotalSize = clusterSerializer.FixedSize * _storageClusterCount;

			var listingsStream = new BoundedStream(_stream, HeaderSize, HeaderSize + listingTotalSize - 1) { UseRelativeOffset = true };
			var statusStream = new BoundedStream(_stream, listingsStream.MaxAbsolutePosition + 1, listingsStream.MaxAbsolutePosition + statusTotalSize) { UseRelativeOffset = true };
			var clusterStream = new BoundedStream(_stream, statusStream.MaxAbsolutePosition + 1, statusStream.MaxAbsolutePosition + clusterTotalSize) { UseRelativeOffset = true };

			if (_stream.Length == 0)
				WriteHeader();
			else
				ReadHeader();

			var preAllocatedListingStore = new StreamMappedPagedList<ItemListing>(new ItemListingSerializer(), listingsStream) { IncludeListHeader = false };

			if (preAllocatedListingStore.RequiresLoad)
				preAllocatedListingStore.Load();
			else
				preAllocatedListingStore.AddRange(Tools.Array.Gen(Capacity, default(ItemListing)));

			_listings = new PreAllocatedList<ItemListing>(preAllocatedListingStore);
			var status = new StreamMappedPagedList<bool>(new BoolSerializer(), statusStream) { IncludeListHeader = false };

			if (status.RequiresLoad)
				status.Load();
			else
				status.AddRange(Tools.Array.Gen(_storageClusterCount, false));
			_clusterStatus = new PreAllocatedList<bool>(status);

			if (!_clusterStatus.Any())
				_clusterStatus.AddRange(status);

			_clusters = new StreamMappedPagedList<Cluster>(StreamMappedPagedListType.FixedSize, clusterSerializer , clusterStream, clusterSerializer.FixedSize * _storageClusterCount) {
				IncludeListHeader = false
			};

			if (_clusters.RequiresLoad)
				_clusters.Load();

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
