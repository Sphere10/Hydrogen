using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {
	public class StreamMappedDynamicClusteredList<T> : RangedListBase<T> {

		private const int HeaderSize = 256;

		private readonly int _clusterDataSize;
		private readonly IObjectSerializer<T> _serializer;
		private readonly StreamMappedPagedList<Cluster> _clusters;
		private readonly EndianBinaryWriter _headerWriter;

		private int _listingGenesisIndex;
		private int _storageGenesisIndex;
		
		private readonly BoundedStream _headerStream;
		private readonly Stream _innerStream;

		private List<int> _freeClusters;
		private PreAllocatedList<ItemListing> _listings;

		public StreamMappedDynamicClusteredList(int clusterDataSize, IObjectSerializer<T> serializer, Stream stream) {
			_clusterDataSize = clusterDataSize;
			_serializer = serializer;

			_innerStream = stream;
			_headerStream = new BoundedStream(stream, 0, HeaderSize - 1);
			_headerWriter = new EndianBinaryWriter(EndianBitConverter.Little, _headerStream);
			_clusters = new StreamMappedPagedList<Cluster>(new ClusterSerializer(clusterDataSize), new NonClosingStream(new BoundedStream(stream, _headerStream.MaxAbsolutePosition +1, long.MaxValue) { UseRelativeOffset = true}));
			_freeClusters = new List<int>();

			var listingStore = new StreamMappedPagedList<ItemListing>(new ItemListingSerializer(), new MemoryStream());
			listingStore.AddRange(Tools.Array.Gen(1000, default(ItemListing)));
			_listings = new PreAllocatedList<ItemListing>(listingStore);
			
			if (!RequiresLoad) {
				WriteHeader();
				Loaded = true;
			}
		}

		public override int Count => _listings.Count;

		public bool RequiresLoad => _innerStream.Length > 0 && !Loaded;

		public bool Loaded { get; private set; }

		public override void AddRange(IEnumerable<T> items) {
			CheckState();
			var listings = new List<ItemListing>();
			
			foreach (T item in items) {
				int index = AddItemToClusters(item);
				
				listings.Add(new ItemListing {
					ClusterStartIndex = index,
					Size = _serializer.CalculateSize(item)
				});
			}
			
			_listings.AddRange(listings);
			UpdateCountHeader();
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<T> items) {
			var itemsArray = items as T[] ?? items.ToArray();
			var results = new int[itemsArray.Length];

			foreach (var (listing, i) in _listings.WithIndex().Where(x => x.Item1.Size != 0)) {
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

		public void Load() {
			ReadHeader();
			
			for (int i = 0; i < _clusters.Count; i++) {
				_clusters.ReadItemRaw(i, 0, sizeof(int), out var bytes);
				ClusterTraits traits = (ClusterTraits)BitConverter.ToInt32(bytes);

				if (traits is ClusterTraits.Free) {
					_freeClusters.Add(i);
				}
			}
			
			Loaded = true;
		}

		private int AddItemToClusters(T item) {
			var clusters = new List<Cluster>();

			using var stream = new MemoryStream();
			_serializer.Serialize(item, new EndianBinaryWriter(EndianBitConverter.Little, stream));
			var data = stream.ToArray();
			var segments = data.Partition(_clusterDataSize)
				.ToList();
			int[] clusterNumbers = GetFreeClusters(segments.Count).ToArray();

			for (var i = 0; i < segments.Count; i++) {
				var segment = segments[i].ToArray();
				var clusterData = new byte[_clusterDataSize];
				segment.CopyTo(clusterData, 0);

				clusters.Add(new Cluster {
					Traits = ClusterTraits.Used,
					Number = clusterNumbers[i],
					Data = clusterData,
					Next = segments.Count - 1 == i ? -1 : clusterNumbers[i + 1]
				});
			}

			foreach (var cluster in clusters)
				if (!_clusters.Any())
					_clusters.Add(cluster);
				else if (cluster.Number >= _clusters.Count)
					_clusters.Add(cluster);
				else
					_clusters[cluster.Number] = cluster;

			WriteHeader();
			
			return clusters.First().Number;
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
				_freeClusters.Add(cluster.Number);
				next = cluster.Next;
			}
		}

		private IEnumerable<int> GetFreeClusters(int clustersRequired) {
			List<int> clusterNumbers = _freeClusters.Take(clustersRequired)
				.ToList();

			_freeClusters.RemoveRangeSequentially(0, clusterNumbers.Count);

			int newClustersRequired = clustersRequired - clusterNumbers.Count;
			int nextCluster = 0;
			if (_clusters.Any()) {
				_clusters.ReadItemRaw(_clusters.Count - 1, 4, 4, out var numberBytes);
				nextCluster = BitConverter.ToInt32(numberBytes) + 1;
			}

			for (int i = 0; i < newClustersRequired; i++) {
				clusterNumbers.Add(nextCluster);
				nextCluster++;
			}

			return clusterNumbers;
		}

		private void WriteHeader() 
		{
			byte[] headerBytes = Tools.Array.Gen(HeaderSize, default(byte));

			new ByteArrayBuilder()
				.Append(EndianBitConverter.Little.GetBytes(_listings.Count))
				.Append(EndianBitConverter.Little.GetBytes(_listingGenesisIndex))
				.Append(EndianBitConverter.Little.GetBytes(_storageGenesisIndex))
				.ToArray()
				.CopyTo(headerBytes, 0);
			
			_headerStream.Seek(0, SeekOrigin.Begin);
			_headerWriter.Write(headerBytes);
		}

		private void UpdateCountHeader() {
			_headerStream.Seek(0, SeekOrigin.Begin);
			_headerWriter.Write(_listings.Count);
		}

		private void ReadHeader() {
			var reader = new EndianBinaryReader(EndianBitConverter.Little, _headerStream);
			_headerStream.Seek(0, SeekOrigin.Begin);
			
			int count = reader.ReadInt32();
			_listingGenesisIndex = reader.ReadInt32();
			_storageGenesisIndex = reader.ReadInt32();
		}

		private void CheckState() {
			if (RequiresLoad) {
				throw new InvalidOperationException("List requires loading as stream contains existing data.");
			}
		}
	}
}
