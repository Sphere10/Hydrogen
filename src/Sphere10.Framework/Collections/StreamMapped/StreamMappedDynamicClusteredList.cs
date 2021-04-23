using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {
	public abstract class StreamMappedDynamicClusteredList<T, TListing> : StreamMappedClusteredListBase<T, TListing> where TListing : IItemListing {
		private const int HeaderSize = 256;

		private readonly EndianBinaryWriter _headerWriter;
		private readonly BoundedStream _headerStream;
		private readonly List<int> _freeClusters;
		private readonly LittleEndianBitConverter _bitConverter;
		private StreamMappedPagedList<TListing> _listingStore;
		private PreAllocatedList<TListing> _listings;

		protected StreamMappedDynamicClusteredList(int clusterDataSize, Stream stream, IObjectSerializer<T> itemSerializer, IObjectSerializer<TListing> listingSerializer, IEqualityComparer<T> itemComparer = null)
			: base(clusterDataSize, stream, itemSerializer, listingSerializer, itemComparer) {
			_headerStream = new BoundedStream(stream, 0, HeaderSize - 1);
			_headerWriter = new EndianBinaryWriter(EndianBitConverter.Little, _headerStream);
			_freeClusters = new List<int>();
			_bitConverter = EndianBitConverter.Little;

			Clusters = new StreamMappedPagedList<Cluster>(
				new ClusterSerializer(clusterDataSize),
				new NonClosingStream(new BoundedStream(stream, _headerStream.MaxAbsolutePosition + 1, long.MaxValue) { UseRelativeOffset = true })
			);
		}

		public override int Count => _listings?.Count ?? 0;

		public override void AddRange(IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			CheckLoaded();

			var listings = new List<TListing>();

			foreach (var item in items) {
				listings.Add(AddItemToClusters(Count, item));
			}

			if (_listings.Capacity < _listings.Count + listings.Count) {
				var required = _listings.Capacity - _listings.Count + listings.Count;
				_listingStore.AddRange(Tools.Array.Gen(required, default(TListing)));
			}

			_listings.AddRange(listings);
			UpdateCountHeader();
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			CheckLoaded();
			
			var itemsArray = items as T[] ?? items.ToArray();

			if (!itemsArray.Any()) {
				return new List<int>();
			}

			var results = new int[itemsArray.Length];

			foreach (var (listing, i) in _listings.WithIndex()) {
				var item = ReadItemFromClusters(i, listing);
				foreach (var (t, index) in itemsArray.WithIndex())
					if (ItemComparer.Equals(t, item))
						results[index] = i;
			}

			return results;
		}

		public override IEnumerable<T> ReadRange(int index, int count) {
			CheckRange(index, count);
			CheckLoaded();

			for (var i = 0; i < count; i++) {
				var listing = _listings[index + i];
				var item = ReadItemFromClusters(index + i, listing);
				yield return item;
			}
		}

		public override void UpdateRange(int index, IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			var itemsArray = items as T[] ?? items.ToArray();

			CheckLoaded();
			CheckRange(index, itemsArray.Length);

			var itemListings = new List<TListing>();
			for (var i = 0; i < itemsArray.Length; i++) {
				var listing = _listings[i + index];
				itemListings.Add(UpdateItemInClusters(index + i, listing, itemsArray[i]));
			}

			_listings.UpdateRange(index, itemListings);
		}

		public override void InsertRange(int index, IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			Guard.ArgumentInRange(index, 0, Math.Max(0, Count), nameof(index));

			var itemsArray = items as T[] ?? items.ToArray();

			if (!itemsArray.Any())
				return;

			var listings = new List<TListing>();
			// Add item data 
			foreach (var (item, i) in itemsArray.WithIndex()) {
				listings.Add(AddItemToClusters(index + i, item));
			}

			// Add listings
			if (_listings.Capacity < _listings.Count + listings.Count) {
				var required = _listings.Capacity - _listings.Count + listings.Count;
				_listingStore.AddRange(Tools.Array.Gen(required, default(TListing)));
			}

			_listings.InsertRange(index, listings);
			UpdateCountHeader();
		}

		public override void RemoveRange(int index, int count) {
			CheckRange(index, count);
			CheckLoaded();
			

			for (var i = 0; i < count; i++) {
				var listing = _listings[index + i];
				RemoveItemFromClusters(index + i, listing);
			}

			_listings.RemoveRange(index, count);
			UpdateCountHeader();
		}

		public override void Load() {
			base.Load();
			var count = ReadHeader();
			var listingsBytesLength = count * ListingSerializer.FixedSize;

			Clusters.Load();

			_listingStore = new StreamMappedPagedList<TListing>(ListingSerializer, new FragmentedStream(new FragmentProvider(listingsBytesLength, this))) {
				IncludeListHeader = false
			};
			_listingStore.Load();

			_listings = new PreAllocatedList<TListing>(_listingStore);
			_listings.AddRange(_listingStore);

			for (var i = 0; i < Clusters.Count; i++) {
				Clusters.ReadItemRaw(i, 0, sizeof(int), out var bytes);
				var traits = (ClusterTraits)_bitConverter.ToInt32(bytes);

				if (traits is ClusterTraits.Free) {
					_freeClusters.Add(i);
				}
			}
		}

		protected override void MarkClusterFree(int clusterNumber) => _freeClusters.Add(clusterNumber);

		protected override IEnumerable<int> GetFreeClusterNumbers(int numberRequired) {
			var clusterNumbers = _freeClusters.Take(numberRequired)
				.ToList();

			_freeClusters.RemoveRangeSequentially(0, clusterNumbers.Count);

			var newClustersRequired = numberRequired - clusterNumbers.Count;
			var nextCluster = 0;
			if (Clusters.Any()) {
				Clusters.ReadItemRaw(Clusters.Count - 1, 4, 4, out var numberBytes);
				nextCluster = _bitConverter.ToInt32(numberBytes) + 1;
			}

			for (var i = 0; i < newClustersRequired; i++) {
				clusterNumbers.Add(nextCluster);
				nextCluster++;
			}

			return clusterNumbers;
		}

		protected override void Initialize() {
			base.Initialize();
			_listingStore = new StreamMappedPagedList<TListing>(ListingSerializer, new FragmentedStream(new FragmentProvider(this))) {
				IncludeListHeader = false
			};
			_listings = new PreAllocatedList<TListing>(_listingStore);
			WriteHeader();

			// listing store capacity set to 1, reserving cluster 0 as item listing. 
			_listingStore.Add(default);
		}

		private void WriteHeader() {
			var headerBytes = Tools.Array.Gen(HeaderSize, default(byte));

			new ByteArrayBuilder()
				.Append(_bitConverter.GetBytes(_listings.Count))
				.ToArray()
				.CopyTo(headerBytes, 0);

			_headerStream.Seek(0, SeekOrigin.Begin);
			_headerWriter.Write(headerBytes);
		}

		private void UpdateCountHeader() {
			_headerStream.Seek(0, SeekOrigin.Begin);
			_headerWriter.Write(_listings.Count);
		}

		private int ReadHeader() {
			var reader = new EndianBinaryReader(_bitConverter, _headerStream);
			_headerStream.Seek(0, SeekOrigin.Begin);
			return reader.ReadInt32();
		}

		private class FragmentProvider : IStreamFragmentProvider {

			private readonly StreamMappedDynamicClusteredList<T, TListing> _parent;

			private readonly int _nextClusterOffset;

			private readonly IDictionary<int, int> _fragmentClusterMap;

			public FragmentProvider(StreamMappedDynamicClusteredList<T, TListing> parent) : this(0, parent) {
			}

			public FragmentProvider(long byteCount, StreamMappedDynamicClusteredList<T, TListing> parent) {
				_parent = parent;
				_nextClusterOffset = sizeof(int) + sizeof(int) + _parent.ClusterDataSize;
				ByteCount = byteCount;
				_fragmentClusterMap = new Dictionary<int, int>();

				//Fragment provider assumes cluster 0 has been reserved for item listings. fill
				// existing cluster map for quick listing lookup. 
				if (ByteCount > 0) {
					var next = 0;
					var fragmentIndex = 0;

					while (next >= 0) {
						_fragmentClusterMap.Add(fragmentIndex, next);
						next = ReadClusterNext(next);
						fragmentIndex++;
					}
				}
			}

			private StreamMappedPagedList<Cluster> Clusters => _parent.Clusters;

			private int ClusterDataSize => _parent.ClusterDataSize;

			public long ByteCount { get; private set; }

			public int FragmentCount => _fragmentClusterMap.Count;

			public Span<byte> GetFragment(int index) {
				Guard.ArgumentInRange(index, 0, FragmentCount - 1, nameof(index));

				long fragmentLength;
				if (ByteCount > ClusterDataSize) {
					fragmentLength = index * ClusterDataSize <= ByteCount
						? ClusterDataSize
						: index * ClusterDataSize - ByteCount;
				} else
					fragmentLength = ByteCount;

				var clusterNumber = _fragmentClusterMap[index];

				return Clusters[clusterNumber]
					.Data[..(int)fragmentLength];
			}

			public (int fragmentIndex, int fragmentPosition) GetFragment(long position, out Span<byte> fragment) {
				var fragmentIndex = (int)Math.Floor((decimal)position / ClusterDataSize);
				var lastCluster = _fragmentClusterMap[fragmentIndex];

				var fragmentLength = ByteCount > ClusterDataSize
					? Math.Min(ByteCount - fragmentIndex * ClusterDataSize, ClusterDataSize)
					: ByteCount;

				fragment = Clusters[lastCluster].Data[..(int)fragmentLength];

				return (fragmentIndex, (int)position % ClusterDataSize);
			}

			public bool TryRequestSpace(int bytes, out int[] newFragmentIndexes) {
				long remainingAvailableSpace;

				if (Clusters.Any()) {
					if (ByteCount > ClusterDataSize)
						remainingAvailableSpace = FragmentCount * ClusterDataSize - ByteCount;
					else
						remainingAvailableSpace = ClusterDataSize - ByteCount;
				} else
					remainingAvailableSpace = 0;

				if (remainingAvailableSpace >= bytes) {
					ByteCount += bytes;
					newFragmentIndexes = new int[0];
					return true;
				}

				var numberRequired = (int)Math.Ceiling(((decimal)bytes - remainingAvailableSpace) / ClusterDataSize);
				var clusterNumbers = _parent.GetFreeClusterNumbers(numberRequired).ToArray();
				newFragmentIndexes = Enumerable.Range(FragmentCount, numberRequired).ToArray();
				var lastCluster = _fragmentClusterMap.Any() ? _fragmentClusterMap.Last().Value : -1;

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
					
					_fragmentClusterMap.Add(newFragmentIndexes[i], clusterNumbers[i]);
				}

				if (lastCluster >= 0) {
					var last = Clusters[lastCluster];
					last.Next = clusterNumbers.First();
					Clusters.Update(lastCluster, last);
				}

				ByteCount += bytes;

				return true;
			}

			public int ReleaseSpace(int bytes, out int[] releasedFragmentIndexes) {
				var numberOfClustersToBeReleased = (int)Math.Floor((decimal)bytes / ClusterDataSize);
				var fragmentIndex = FragmentCount - numberOfClustersToBeReleased - 1;

				if (numberOfClustersToBeReleased > 0) {
					var newTailCluster = _fragmentClusterMap[fragmentIndex];
					var cluster = Clusters[newTailCluster];
					var next = cluster.Next;
					cluster.Next = -1;
					Clusters.Update(newTailCluster, cluster);

					while (next != -1) {
						var nextCluster = Clusters[next];
						_parent.MarkClusterFree(nextCluster.Number);
						next = nextCluster.Next;
					}
				}

				releasedFragmentIndexes = Enumerable.Range(fragmentIndex, numberOfClustersToBeReleased).ToArray();

				foreach (var releasedFragmentIndex in releasedFragmentIndexes) {
					_fragmentClusterMap.Remove(releasedFragmentIndex);
				}

				ByteCount -= bytes;

				return bytes;
			}

			public void UpdateFragment(int fragmentIndex, int fragmentPosition, Span<byte> updateSpan) {
				Guard.ArgumentInRange(fragmentIndex, 0, FragmentCount - 1, nameof(fragmentIndex));

				var clusterNumber = _fragmentClusterMap[fragmentIndex];

				var cluster = Clusters[clusterNumber];
				updateSpan.CopyTo(cluster.Data.AsSpan(fragmentPosition));
				Clusters.Update(clusterNumber, cluster);
			}

			private int ReadClusterNext(int clusterNumber) {
				Clusters.ReadItemRaw(clusterNumber, _nextClusterOffset, sizeof(int), out var next);
				return EndianBitConverter.Little.ToInt32(next);
			}
		}
	}


	public class StreamMappedDynamicClusteredList<T> : StreamMappedDynamicClusteredList<T, ItemListing> {

		public StreamMappedDynamicClusteredList(int clusterDataSize, Stream stream, IObjectSerializer<T> itemSerializer, IEqualityComparer<T> itemComparer = null)
			: base(clusterDataSize, stream, itemSerializer, new ItemListingSerializer(), itemComparer) {
		}

		protected override ItemListing NewListingInstance(int itemSizeBytes, int clusterStartIndex) {
			return new ItemListing {
				Size = itemSizeBytes,
				ClusterStartIndex = clusterStartIndex
			};
		}
	}
}
