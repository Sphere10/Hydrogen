using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {
	public class StreamMappedDynamicClusteredList<T> : StreamMappedClusteredListBase<T> {

		private const int HeaderSize = 256;

		private readonly EndianBinaryWriter _headerWriter;

		private readonly BoundedStream _headerStream;

		private readonly List<int> _freeClusters;
		private StreamMappedPagedList<ItemListing> _listingStore;
		private PreAllocatedList<ItemListing> _listings;

		public StreamMappedDynamicClusteredList(int clusterDataSize, IObjectSerializer<T> serializer, Stream stream) : base(clusterDataSize, serializer, stream) {
			_headerStream = new BoundedStream(stream, 0, HeaderSize - 1);
			_headerWriter = new EndianBinaryWriter(EndianBitConverter.Little, _headerStream);
			_freeClusters = new List<int>();

			Clusters = new StreamMappedPagedList<Cluster>(new ClusterSerializer(clusterDataSize),
				new NonClosingStream(new BoundedStream(stream, _headerStream.MaxAbsolutePosition + 1, long.MaxValue) { UseRelativeOffset = true }));

			if (!RequiresLoad) {
				_listingStore = new StreamMappedPagedList<ItemListing>(new ItemListingSerializer(), new FragmentedStream(new FragmentProvider(this))) { IncludeListHeader = false };
				_listings = new PreAllocatedList<ItemListing>(_listingStore);
				_listingStore.Add(default);

				WriteHeader();
				Loaded = true;
			}
		}

		public override int Count => _listings?.Count ?? 0;

		internal override StreamMappedPagedList<Cluster> Clusters { get; set; }

		public override void AddRange(IEnumerable<T> items) {
			CheckLoaded();

			var listings = new List<ItemListing>();

			foreach (T item in items) {
				listings.Add(AddItemToClusters(item));
			}

			if (_listings.Capacity < _listings.Count + listings.Count) {
				int required = listings.Count - _listings.Capacity - _listings.Count;
				_listingStore.AddRange(Tools.Array.Gen(required, default(ItemListing)));
			}

			_listings.AddRange(listings);
			UpdateCountHeader();
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<T> items) {
			CheckLoaded();
			
			var itemsArray = items as T[] ?? items.ToArray();
			
			if (!itemsArray.Any()) {
				return new List<int>();
			}
			
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
			CheckLoaded();
			CheckRange(index, count);
			
			for (var i = 0; i < count; i++) {
				var listing = _listings[index + i];
				yield return ReadItemFromClusters(listing.ClusterStartIndex, listing.Size);
			}
		}

		public override void UpdateRange(int index, IEnumerable<T> items) {
			var itemsArray = items.ToArray();
			
			CheckLoaded();
			CheckRange(index, itemsArray.Length);
			
			var itemListings = new List<ItemListing>();
			for (var i = 0; i < itemsArray.Length; i++) {
				var listing = _listings[index + i];
				RemoveItemFromClusters(listing.ClusterStartIndex);
				itemListings.Add( AddItemToClusters(itemsArray[i]));
			}

			_listings.UpdateRange(index, itemListings);
		}

		public override void InsertRange(int index, IEnumerable<T> items) {
			var itemsArray = items as T[] ?? items.ToArray();
			
			if (!itemsArray.Any())
				return;
			
			var listings = new List<ItemListing>();

			foreach (var item in itemsArray) {
				listings.Add(AddItemToClusters(item));
			}

			if (_listings.Capacity < _listings.Count + listings.Count) {
				int required = _listings.Capacity - _listings.Count + listings.Count;
				_listingStore.AddRange(Tools.Array.Gen(required, default(ItemListing)));
			}

			_listings.InsertRange(index, listings);
			UpdateCountHeader();
		}

		public override void RemoveRange(int index, int count) {
			CheckLoaded();
			CheckRange(index, count);
			
			for (var i = 0; i < count; i++) {
				var listing = _listings[index + i];
				RemoveItemFromClusters(listing.ClusterStartIndex);
			}

			_listings.RemoveRange(index, count);
			UpdateCountHeader();
		}

		public override void Load() {
			int count = ReadHeader();
			ItemListingSerializer serializer = new ItemListingSerializer();
			int listingsBytesLength = count * serializer.FixedSize;

			Clusters.Load();

			_listingStore = new StreamMappedPagedList<ItemListing>(serializer, new FragmentedStream(new FragmentProvider(listingsBytesLength, this))) { IncludeListHeader = false };
			_listingStore.Load();

			_listings = new PreAllocatedList<ItemListing>(_listingStore);
			_listings.AddRange(_listingStore);

			for (int i = 0; i < Clusters.Count; i++) {
				Clusters.ReadItemRaw(i, 0, sizeof(int), out var bytes);
				ClusterTraits traits = (ClusterTraits)BitConverter.ToInt32(bytes);

				if (traits is ClusterTraits.Free) {
					_freeClusters.Add(i);
				}
			}

			Loaded = true;
		}

		protected override IEnumerable<int> GetFreeClusterNumbers(int numberRequired) {
			List<int> clusterNumbers = _freeClusters.Take(numberRequired)
				.ToList();

			_freeClusters.RemoveRangeSequentially(0, clusterNumbers.Count);

			int newClustersRequired = numberRequired - clusterNumbers.Count;
			int nextCluster = 0;
			if (Clusters.Any()) {
				Clusters.ReadItemRaw(Clusters.Count - 1, 4, 4, out var numberBytes);
				nextCluster = BitConverter.ToInt32(numberBytes) + 1;
			}

			for (int i = 0; i < newClustersRequired; i++) {
				clusterNumbers.Add(nextCluster);
				nextCluster++;
			}

			return clusterNumbers;
		}

		private void WriteHeader() {
			byte[] headerBytes = Tools.Array.Gen(HeaderSize, default(byte));

			new ByteArrayBuilder()
				.Append(EndianBitConverter.Little.GetBytes(_listings.Count))
				.ToArray()
				.CopyTo(headerBytes, 0);

			_headerStream.Seek(0, SeekOrigin.Begin);
			_headerWriter.Write(headerBytes);
		}

		private void RemoveItemFromClusters(int startCluster) {
			var next = startCluster;

			while (next != -1) {
				var cluster = Clusters[next];
				_freeClusters.Add(cluster.Number);
				next = cluster.Next;
			}
		}

		private void UpdateCountHeader() {
			_headerStream.Seek(0, SeekOrigin.Begin);
			_headerWriter.Write(_listings.Count);
		}

		private int ReadHeader() {
			var reader = new EndianBinaryReader(EndianBitConverter.Little, _headerStream);
			_headerStream.Seek(0, SeekOrigin.Begin);
			return reader.ReadInt32();
		}

		private class FragmentProvider : IFragmentProvider {

			private readonly StreamMappedDynamicClusteredList<T> _parent;

			private readonly int _nextClusterOffset;

			public FragmentProvider(StreamMappedDynamicClusteredList<T> parent) : this(0, parent) {
			}

			public FragmentProvider(long length, StreamMappedDynamicClusteredList<T> parent) {
				_parent = parent;
				_nextClusterOffset = sizeof(int) + sizeof(int) + _parent.ClusterDataSize;
				Length = length;
				Count = (int)Math.Ceiling((decimal)length / ClusterDataSize);
			}

			private StreamMappedPagedList<Cluster> Clusters => _parent.Clusters;

			private int ClusterDataSize => _parent.ClusterDataSize;

			public long Length { get; private set; }

			public int Count { get; private set; }

			public Span<byte> GetFragment(int index) {
				Guard.ArgumentInRange(index, 0, Count - 1, nameof(index));

				long fragmentLength;
				if (Length > ClusterDataSize) {
					fragmentLength = index * ClusterDataSize <= Length
						? ClusterDataSize
						: index * ClusterDataSize - Length;
				} else
					fragmentLength = Length;

				int clusterNumber = GetClusterNumberOfFragment(index);

				return Clusters[clusterNumber]
					.Data[..(int)fragmentLength];
			}

			public (int fragmentIndex, int fragmentPosition) GetFragment(long position, out Span<byte> fragment) {
				int fragmentIndex = (int)Math.Floor((decimal)position / ClusterDataSize);
				int lastCluster = GetClusterNumberOfFragment(fragmentIndex);

				var fragmentLength = Length > ClusterDataSize
					? Math.Min(Length - fragmentIndex * ClusterDataSize, ClusterDataSize)
					: Length;

				fragment = Clusters[lastCluster].Data[..(int)fragmentLength];

				return (fragmentIndex, (int)position % ClusterDataSize);
			}

			public bool TryRequestSpace(int bytes, out int[] newFragmentIndexes) {
				long remainingAvailableSpace;

				if (Clusters.Any()) {
					if (Length > ClusterDataSize)
						remainingAvailableSpace = Count * ClusterDataSize - Length;
					else
						remainingAvailableSpace = ClusterDataSize - Length;
				} else
					remainingAvailableSpace = 0;

				if (remainingAvailableSpace >= bytes) {
					Length += bytes;
					newFragmentIndexes = new int[0];
					return true;
				}

				var numberRequired = (int)Math.Ceiling(((decimal)bytes - remainingAvailableSpace) / ClusterDataSize);
				int[] clusterNumbers = _parent.GetFreeClusterNumbers(numberRequired).ToArray();
				newFragmentIndexes = Enumerable.Range(Count, numberRequired).ToArray();

				for (var i = 0; i < clusterNumbers.Length; i++) {
					var cluster = new Cluster {
						Next = i == clusterNumbers.Length - 1 ? -1 : clusterNumbers[i + 1],
						Data = new byte[ClusterDataSize],
						Number = clusterNumbers[i],
						Traits = ClusterTraits.Used
					};

					if (!Clusters.Any())
						Clusters.Add(cluster);
					else if (cluster.Number >= Clusters.Count)
						Clusters.Add(cluster);
					else
						Clusters[cluster.Number] = cluster;

					Count++;
				}

				int lastCluster = GetClusterNumberOfFragment(Count - 1);

				if (Count != 0) {
					var last = Clusters[lastCluster];
					last.Next = clusterNumbers.First();
					Clusters.Update(lastCluster, last);
				}

				Length += bytes;

				return true;
			}

			public int ReleaseSpace(int bytes, out int[] releasedFragmentIndexes) {
				int numberOfClustersToBeReleased = (int)Math.Floor((decimal)bytes / ClusterDataSize);
				int fragmentIndex = Count - numberOfClustersToBeReleased - 1;

				if (numberOfClustersToBeReleased > 0) {
					int newTailCluster = GetClusterNumberOfFragment(fragmentIndex);
					Cluster cluster = Clusters[newTailCluster];
					int next = cluster.Next;
					cluster.Next = -1;
					Clusters.Update(newTailCluster, cluster);

					_parent.RemoveItemFromClusters(next);
					Count -= numberOfClustersToBeReleased;
				}

				releasedFragmentIndexes = Enumerable.Range(fragmentIndex, numberOfClustersToBeReleased).ToArray();
				Length -= bytes;

				return bytes;
			}

			public void UpdateFragment(int fragmentIndex, int fragmentPosition, Span<byte> updateSpan) {
				Guard.ArgumentInRange(fragmentIndex, 0, Count - 1, nameof(fragmentIndex));

				int clusterNumber = GetClusterNumberOfFragment(fragmentIndex);

				var cluster = Clusters[clusterNumber];
				updateSpan.CopyTo(cluster.Data.AsSpan(fragmentPosition));
				Clusters.Update(clusterNumber, cluster);
			}

			private int GetClusterNumberOfFragment(int fragmentIndex) {
				int clusterNumber = 0;
				while (fragmentIndex > 0) {

					int nextCluster = ReadClusterNext(clusterNumber);

					if (nextCluster != -1) {
						clusterNumber = nextCluster;
					}

					fragmentIndex--;
				}

				return clusterNumber;
			}

			private int ReadClusterNext(int clusterNumber) {
				Clusters.ReadItemRaw(clusterNumber, _nextClusterOffset, sizeof(int), out Span<byte> next);
				return BitConverter.ToInt32(next);
			}
		}
	}
}
