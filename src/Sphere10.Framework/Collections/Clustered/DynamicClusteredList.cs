using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// A list whose items are stored in a linked-list of clusters serialized over a stream. It is dynamic since the capacity of the list does not need to be known on activation.
	/// Items can added/removed arbitrarily and are stored in a non-contiguous manner over a linked-list of clusters (similar in principle to how file systems work). Unlike <see cref="StaticClusteredList{T,TListing}"/> there is
	/// no limitation in the list's capacity imposed at construction since the listing's themselves are stored in clusters. However this benefit is paid for by a notable performance penalty since item listings are more costly to retrieve.
	/// </summary>
	/// <typeparam name="T">The type of item being stored in the list</typeparam>
	/// <typeparam name="TListing">The type of listing which tracks the stored items</typeparam>
	/// <remarks>
	/// The underlying implementation is similar to <see cref="StaticClusteredList{T}"/> except the "listing sector" is serialized as a fragmented stream over a linked-list of clusters whereas in <see cref="StaticClusteredList{T}"/> the listing sector is stored in a contiguous BLOB before the content sector.
	/// </remarks>
	internal sealed class DynamicClusteredList<T, TListing> : ClusteredListImplBase<T, TListing> where TListing : IClusteredItemListing {
		private const int HeaderSize = 256;
		private const int KnownFreeClustersLimit = 100;

		private readonly EndianBinaryWriter _headerWriter;
		private readonly BoundedStream _headerStream; // header portion
		private readonly List<int> _knownFreeClusters; // list of free clusters
		private readonly IItemSerializer<TListing> _listingSerializer; // listing serializer
		private PreAllocatedList<TListing> _listings; // pre-allocated list (which can be grown) to contain listing headers of items

		private StreamPagedList<TListing>
			_listingSector; // dynamic listing sector that is mapped over the clusters through a fragment provider

		private readonly EndianBitConverter _bitConverter;
		private readonly Endianness _endianness;

		public DynamicClusteredList(int clusterDataSize, Stream stream, IItemSerializer<T> itemSerializer,
		                            IItemSerializer<TListing> listingSerializer, IEqualityComparer<T> itemComparer = null,
		                            Endianness endianness = Endianness.LittleEndian)
			: base(clusterDataSize, stream, itemSerializer, listingSerializer, itemComparer) {
			Guard.Argument(clusterDataSize > 0, nameof(clusterDataSize), "Must be greater than 0");
			Guard.ArgumentNotNull(stream, nameof(stream));
			Guard.ArgumentNotNull(itemSerializer, nameof(itemSerializer));
			Guard.ArgumentNotNull(listingSerializer, nameof(listingSerializer));
			Guard.Argument(listingSerializer.IsStaticSize,
				nameof(listingSerializer),
				"Listings must be static sized and cannot be dynamic");

			_listingSerializer = listingSerializer;
			_headerStream = new BoundedStream(stream, 0, HeaderSize - 1);
			_headerWriter = new EndianBinaryWriter(EndianBitConverter.Little, _headerStream);
			_knownFreeClusters = new List<int>();
			_bitConverter = EndianBitConverter.For(endianness);
			_endianness = endianness;

			Clusters = new StreamPagedList<Cluster>(
				new ClusterSerializer(clusterDataSize),
				new NonClosingStream(new BoundedStream(stream, _headerStream.MaxAbsolutePosition + 1, long.MaxValue)
					{ UseRelativeOffset = true })
			);

			_listingSector =
				new StreamPagedList<TListing>(ListingSerializer, new FragmentedStream(new FragmentProvider(this)), _endianness) {
					IncludeListHeader = false
				};
			_listings = new PreAllocatedList<TListing>(_listingSector, 0);
		}

		public override int Count => _listings?.Count ?? 0;

		public override IReadOnlyList<TListing> Listings => (IExtendedList<TListing>)_listings ?? new ExtendedList<TListing>();

		public override void AddRange(IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			CheckLoaded();

			var newListings = new List<TListing>();

			foreach (var item in items) {
				newListings.Add(AddItemToClusters(Count, item));
			}

			if (_listings.Capacity < _listings.Count + newListings.Count) {
				var required = _listings.Capacity - _listings.Count + newListings.Count;
				_listingSector.AddRange(Tools.Array.Gen(required, default(TListing)));
			}

			_listings.AddRange(newListings);
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
				_listingSector.AddRange(Tools.Array.Gen(required, default(TListing)));
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
			var listingsBytesLength = count * ListingSerializer.StaticSize;

			Clusters.Load();

			_listingSector = new StreamPagedList<TListing>(ListingSerializer,
				new FragmentedStream(new FragmentProvider(listingsBytesLength, this)),
				_endianness) {
				IncludeListHeader = false
			};
			_listingSector.Load();

			_listings = new PreAllocatedList<TListing>(_listingSector, _listingSector.Count);

			_knownFreeClusters.AddRange(FindFreeClusters(KnownFreeClustersLimit));
		}

		protected override void Initialize() {
			base.Initialize();
			WriteHeader();

			// listing store capacity set to 1, reserving cluster 0 as item listing. 
			_listingSector.Add(default);
		}

		protected override void MarkClusterFree(int clusterNumber) {
			Clusters[clusterNumber].Traits = ClusterTraits.Free;

			if (_knownFreeClusters.Count < KnownFreeClustersLimit)
				_knownFreeClusters.Add(clusterNumber);
		}

		protected override IEnumerable<int> ConsumeClusters(int numberRequired) {
			var clusterNumbers = _knownFreeClusters.Take(numberRequired)
			                                       .ToList();

			_knownFreeClusters.RemoveRangeSequentially(0, clusterNumbers.Count);

			if (clusterNumbers.Count < numberRequired) {
				var foundClusters = FindFreeClusters(numberRequired - clusterNumbers.Count);
				clusterNumbers.AddRange(foundClusters);
			}

			if (clusterNumbers.Count < numberRequired) {
				clusterNumbers.AddRange(Enumerable.Range(Clusters.Count, numberRequired - clusterNumbers.Count));
			}
			
			Debug.Assert(clusterNumbers.Count == numberRequired, "Number of required clusters is returned.");

			return clusterNumbers;
		}

		internal override void UpdateListing(int index, TListing listing) {
			_listings[index] = listing;
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

		private ClusterTraits ReadClusterTraits(int clusterIndex) {
			Clusters.ReadItemRaw(clusterIndex, 0, sizeof(int), out var bytes);
			return (ClusterTraits)_bitConverter.ToInt32(bytes);
		}

		private int ReadNextClusterNumber(int clusterIndex) {
			Clusters.ReadItemRaw(clusterIndex, 4, sizeof(int), out var numberBytes);
			return _bitConverter.ToInt32(numberBytes) + 1;
		}

		private List<int> FindFreeClusters(int numberRequired) {

			var knownTaken = new SortedSet<int>();
			var foundClusters = new List<int>();

			var nextCluster = 0;
			var remaining = numberRequired;

			if (Clusters.Any()) {
				while (remaining > 0 && nextCluster < Clusters.Count) {
					if (!knownTaken.Contains(nextCluster)) {

						// Read first 8 bytes, the traits and next cluster number.
						Clusters.ReadItemRaw(nextCluster, 0, 8, out var bytes);
						var traits = (ClusterTraits)_bitConverter.ToInt32(bytes.AsSpan()[..4]);

						if (traits.HasFlag(ClusterTraits.Free)) {
							foundClusters.Add(nextCluster);
							remaining--;
						} else {
							knownTaken.Add(nextCluster);

							//We have next cluster, add it to known taken.
							var next = _bitConverter.ToInt32(bytes.AsSpan()[4..]);
							if (next > 0)
								knownTaken.Add(next);
						}
					}

					nextCluster++;
				}
			}

			return foundClusters;
		}

		// NOTE: this is a buggy implementation (not written by Herman) that needs to be unit tested
		//  - Releasing small amount of space consecutively may be problematic
		//  - No need for a _fragmentClusterMap, the fragment indexes should be the cluster indexes
		//  - _fragmentClusterMap can explode in size when large numbers of clusters


		private class FragmentProvider : IStreamFragmentProvider {

			private readonly DynamicClusteredList<T, TListing> _parent;

			private readonly int _nextClusterOffset;

			private int _clusterTailIndex;

			private int _fragmentCount = 0;

			public FragmentProvider(DynamicClusteredList<T, TListing> parent) : this(0, parent) {
			}

			public FragmentProvider(long byteCount, DynamicClusteredList<T, TListing> parent) {
				_parent = parent;
				_nextClusterOffset =
					sizeof(int) + sizeof(int) +
					_parent.ClusterDataSize; // TODO: should sizeof(int)*2 be parent._listingSerializer.StaticSize ?
				TotalBytes = byteCount;

				//Fragment provider assumes cluster 0 has been reserved for item listings. fill
				// existing cluster map for quick listing lookup. 
				if (TotalBytes > 0) {
					var next = 0;

					while (next >= 0) {
						_clusterTailIndex = next;
						_fragmentCount++;
						next = ReadClusterNext(next);
					}
				}
			}

			private StreamPagedList<Cluster> Clusters => _parent.Clusters;

			private int ClusterDataSize => _parent.ClusterDataSize;

			public long TotalBytes { get; private set; }

			public int FragmentCount => _fragmentCount;

			public Span<byte> GetFragment(int index) {
				Guard.ArgumentInRange(index, 0, FragmentCount - 1, nameof(index));

				long fragmentLength;
				if (TotalBytes > ClusterDataSize) {
					fragmentLength = index * ClusterDataSize <= TotalBytes
						? ClusterDataSize
						: index * ClusterDataSize - TotalBytes;
				} else
					fragmentLength = TotalBytes;

				int clusterNumber = GetClusterIndexByFragmentIndex(index);
				return Clusters[clusterNumber].Data[..(int)fragmentLength];
			}

			public (int fragmentIndex, int fragmentPosition) MapLogicalPositionToFragment(long position, out Span<byte> fragment) {
				var fragmentIndex = (int)Math.Floor((decimal)position / ClusterDataSize);

				var lastCluster = GetClusterIndexByFragmentIndex(fragmentIndex);

				var fragmentLength = TotalBytes > ClusterDataSize
					? Math.Min(TotalBytes - fragmentIndex * ClusterDataSize, ClusterDataSize)
					: TotalBytes;

				fragment = Clusters[lastCluster].Data[..(int)fragmentLength];

				return (fragmentIndex, (int)position % ClusterDataSize);
			}

			public bool TryRequestSpace(int bytes, out int[] newFragmentIndexes) {
				long remainingAvailableSpace;

				if (Clusters.Any()) {
					if (TotalBytes > ClusterDataSize)
						remainingAvailableSpace = FragmentCount * ClusterDataSize - TotalBytes;
					else
						remainingAvailableSpace = ClusterDataSize - TotalBytes;
				} else
					remainingAvailableSpace = 0;

				if (remainingAvailableSpace >= bytes) {
					TotalBytes += bytes;
					newFragmentIndexes = Array.Empty<int>();
					return true;
				}

				var numberRequired = (int)Math.Ceiling(((decimal)bytes - remainingAvailableSpace) / ClusterDataSize);
				var clusterNumbers = _parent.ConsumeClusters(numberRequired).ToArray();
				newFragmentIndexes = Enumerable.Range(FragmentCount, numberRequired).ToArray();

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
				}

				var firstNew = clusterNumbers.First();

				if (firstNew != _clusterTailIndex) {
					var last = Clusters[_clusterTailIndex];
					last.Next = firstNew;
					Clusters.Update(_clusterTailIndex, last);
				}
				
				_clusterTailIndex = clusterNumbers.Last();

				TotalBytes += bytes;
				_fragmentCount += clusterNumbers.Length;

				return true;
			}

			public int ReleaseSpace(int bytes, out int[] releasedFragmentIndexes) {
				var numberOfClustersToBeReleased = (int)Math.Floor((decimal)bytes / ClusterDataSize);
				var fragmentIndex = FragmentCount - numberOfClustersToBeReleased - 1;

				if (numberOfClustersToBeReleased > 0) {
					var newTailClusterIndex = GetClusterIndexByFragmentIndex(fragmentIndex);
					var cluster = Clusters[newTailClusterIndex];
					var next = cluster.Next;
					cluster.Next = -1;
					Clusters.Update(newTailClusterIndex, cluster);

					while (next != -1) {
						var nextCluster = Clusters[next];
						_parent.MarkClusterFree(nextCluster.Number);
						next = nextCluster.Next;
					}

					_clusterTailIndex = newTailClusterIndex;
				}

				releasedFragmentIndexes = Enumerable.Range(fragmentIndex, numberOfClustersToBeReleased).ToArray();

				_fragmentCount -= numberOfClustersToBeReleased;
				TotalBytes -= bytes;

				return bytes;
			}

			public void UpdateFragment(int fragmentIndex, int fragmentPosition, Span<byte> updateSpan) {
				Guard.ArgumentInRange(fragmentIndex, 0, FragmentCount - 1, nameof(fragmentIndex));

				var clusterNumber = GetClusterIndexByFragmentIndex(fragmentIndex);

				var cluster = Clusters[clusterNumber];
				updateSpan.CopyTo(cluster.Data.AsSpan(fragmentPosition));
				Clusters.Update(clusterNumber, cluster);
			}

			private int ReadClusterNext(int clusterNumber) {
				Clusters.ReadItemRaw(clusterNumber, _nextClusterOffset, sizeof(int), out var next);
				return EndianBitConverter.Little.ToInt32(next);
			}

			private int GetClusterIndexByFragmentIndex(int fragmentIndex) {
				var clusterNumber = 0;
				
				for (int i = 0; i < fragmentIndex; i++) {
					clusterNumber = ReadClusterNext(clusterNumber);
				}

				return clusterNumber;
			}
		}
	}

}
