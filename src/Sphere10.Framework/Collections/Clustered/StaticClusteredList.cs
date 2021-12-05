using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {


	/// <summary>
	/// A list whose items are stored in a linked-list of clusters serialized over a stream. It is "static" since the capacity of the list needs to be known on activation.
	/// Items can added/removed arbitrarily and are stored in a non-contiguous manner over a linked-list of clusters (similar in principle to how file systems work). The limitation here is that the
	/// the list's capacity is inherantly bound at construction and cannot grow. This limitation results in a notable performance over <see cref="DynamicClusteredList{T, TListing}"/> which does not suffer this limitation.
	/// The performance enhancement arises due to efficient cluster lookup.
	/// </summary>
	/// <typeparam name="T">The type of item being stored in the list</typeparam>
	/// <typeparam name="TListing">The type of listing which tracks the stored items. This can be specified for advanced use-cases such as a clustered dictionary implementation.</typeparam>
	/// <remarks>
	/// The underlying implementation is similar to <see cref="DynamicClusteredList{T,TListing}"/> except the "listing sector" is efficiently serialized as a contiguous block of memory before the "data sector" containing the clusters, thus is more efficient.
	/// </remarks>
	/// <remarks>
	/// This uses <see cref="StreamPagedList{TItem}"/> under the hood.
	/// </remarks>
	internal sealed class StaticClusteredList<T, TListing> : ClusteredListImplBase<T, TListing> where TListing : IClusteredItemListing {
		private const int HeaderSize = 256;

		private readonly long _maxStorageBytes;
		private IExtendedList<bool> _clusterStatus;  // mapped to a BitVector on the stream which denotes which clusters are available or not
		private IExtendedList<TListing> _listings;   // mapped to a paged stream (single page for all fixed-size items)
		private readonly Endianness _endianness;

		public StaticClusteredList(
			int clusterDataSize,
			int maxItems,
			long maxStorageBytes,
			Stream stream,
			IItemSerializer<T> itemSerializer,
			IItemSerializer<TListing> listingSerializer,
			IEqualityComparer<T> itemComparer = null,
			Endianness endianness = Endianness.LittleEndian)
			: base(clusterDataSize, stream, itemSerializer, listingSerializer, itemComparer) {
			
			Guard.ArgumentInRange(clusterDataSize, 0, int.MaxValue, nameof(clusterDataSize));
			Guard.ArgumentInRange(maxItems, 0, int.MaxValue, nameof(maxItems));
			Guard.ArgumentInRange(maxStorageBytes, HeaderSize, int.MaxValue, nameof(maxStorageBytes));
			Guard.ArgumentNotNull(stream, nameof(stream));
			Guard.ArgumentNotNull(itemSerializer, nameof(itemSerializer));
			Guard.ArgumentNotNull(listingSerializer, nameof(listingSerializer));
			Guard.Argument(listingSerializer.IsStaticSize, nameof(listingSerializer), "Listings must be static sized and cannot be dynamic");

			Capacity = maxItems;
			_maxStorageBytes = maxStorageBytes;
			_endianness = endianness;
			_listings = new ExtendedList<TListing>(); // this is a temporary assignment which is overwritten on Initialize

		}

		public override int Count => _listings?.Count ?? 0;

		public int Capacity { get; }

		public override IReadOnlyList<TListing> Listings => _listings;

		public override void AddRange(IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));

			CheckLoaded();

			var itemsArray = items as T[] ?? items.ToArray();

			if (!itemsArray.Any())
				return;

			foreach (var item in itemsArray) 
				_listings.Add(AddItemToClusters(Count, item));

			UpdateCountHeader();
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			CheckLoaded();

			var itemsArray = items as T[] ?? items.ToArray();
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
				yield return ReadItemFromClusters(index + i, listing);
			}
		}

		public override void UpdateRange(int index, IEnumerable<T> items) {
			CheckLoaded();
			var itemsArray = items.ToArray();
			CheckRange(index, itemsArray.Length);

			var itemListings = new List<TListing>();
			for (var i = 0; i < itemsArray.Length; i++) {
				var listing = _listings[index + i];
				itemListings.Add(UpdateItemInClusters(index + i, listing, itemsArray[i]));
			}

			_listings.UpdateRange(index, itemListings);
		}

		public override void InsertRange(int index, IEnumerable<T> items) {
			CheckLoaded();
			Guard.ArgumentNotNull(items, nameof(items));
			Guard.ArgumentInRange(index, 0, Count, nameof(index));

			var itemsArray = items as T[] ?? items.ToArray();

			if (_listings.Count + itemsArray.Length > Capacity)
				throw new ArgumentException("Insufficient space");

			if (!itemsArray.Any())
				return;

			var listings = new List<TListing>();

			foreach (var (item, i) in itemsArray.WithIndex()) {
				listings.Add(AddItemToClusters(index + i, item));
			}

			_listings.InsertRange(index, listings);

			UpdateCountHeader();
		}

		public override void RemoveRange(int index, int count) {
			CheckLoaded();
			CheckRange(index, count);

			for (var i = 0; i < count; i++) {
				var listing = _listings[index + i];
				RemoveItemFromClusters(index + i, listing);
			}

			_listings.RemoveRange(index, count);

			UpdateCountHeader();
		}

		public override void Load() {
			base.Load();
			var clusterSerializer = new ClusterSerializer(ClusterDataSize);

			var listingTotalSize = ListingSerializer.StaticSize * Capacity;
			var availableClusterStorageBytes = _maxStorageBytes - HeaderSize - listingTotalSize;
			var bytesPerCluster = clusterSerializer.StaticSize + sizeof(bool);
			if (availableClusterStorageBytes < bytesPerCluster) 
				throw new InvalidOperationException("Max storage bytes is insufficient for list.");
			
			int storageClusterCount = (int)Math.Floor(availableClusterStorageBytes / (clusterSerializer.StaticSize + 0.125));
			int statusTotalSize = (int)Math.Ceiling((decimal)storageClusterCount / 8);
			
			var listingsStream = new BoundedStream(InnerStream, HeaderSize, HeaderSize + listingTotalSize - 1) { UseRelativeOffset = true };
			var statusStream = new BoundedStream(InnerStream, listingsStream.MaxAbsolutePosition + 1, listingsStream.MaxAbsolutePosition + statusTotalSize) { UseRelativeOffset = true };
			var clusterStream = new BoundedStream(InnerStream, statusStream.MaxAbsolutePosition + 1, long.MaxValue) { UseRelativeOffset = true };

			int itemCount = 0;
			if (InnerStream.Length > 0)
				itemCount = ReadHeader();
			
			var preAllocatedListingStore = new StreamPagedList<TListing>(ListingSerializer, listingsStream, _endianness) { IncludeListHeader = false };
			preAllocatedListingStore.Load();
			_listings = new PreAllocatedList<TListing>(preAllocatedListingStore, itemCount);
			
			var status = new BitVector(statusStream);
			_clusterStatus = new PreAllocatedList<bool>(status, status.Count);
			
			var pageSize = ItemSerializer.IsStaticSize ? clusterSerializer.StaticSize * storageClusterCount : int.MaxValue;
			
			Clusters = new StreamPagedList<Cluster>(StreamPagedListType.Static, clusterSerializer, clusterStream, pageSize) {
				IncludeListHeader = false
			};
			
			Clusters.Load();
		}

		protected override void Initialize() {
			base.Initialize();

			if (InnerStream.Length == 0)
				WriteHeader();
			
			var clusterSerializer = new ClusterSerializer(ClusterDataSize);
			var listingTotalSize = ListingSerializer.StaticSize * Capacity;
			var availableClusterStorageBytes = _maxStorageBytes - HeaderSize - listingTotalSize;
			var bytesPerCluster = clusterSerializer.StaticSize + sizeof(bool);
			if (availableClusterStorageBytes < bytesPerCluster)
				throw new InvalidOperationException("Max storage bytes is insufficient for list.");

			var storageClusterCount = (int)Math.Floor(availableClusterStorageBytes / (clusterSerializer.StaticSize + 0.125));
			var statusTotalSize = (int)Math.Ceiling((decimal)storageClusterCount / 8);

			var listingsStream = new BoundedStream(InnerStream, HeaderSize, HeaderSize + listingTotalSize - 1) { UseRelativeOffset = true };
			var statusStream = new BoundedStream(InnerStream, listingsStream.MaxAbsolutePosition + 1, listingsStream.MaxAbsolutePosition + statusTotalSize) { UseRelativeOffset = true };
			var clusterStream = new BoundedStream(InnerStream, statusStream.MaxAbsolutePosition + 1, long.MaxValue) { UseRelativeOffset = true };


			var preAllocatedListingStore = new StreamPagedList<TListing>(ListingSerializer, listingsStream, _endianness) { IncludeListHeader = false };
			preAllocatedListingStore.AddRange(Tools.Array.Gen(Capacity, default(TListing)));
			_listings = new PreAllocatedList<TListing>(preAllocatedListingStore, 0);

			var status = new BitVector(statusStream);
			status.AddRange(Tools.Array.Gen(storageClusterCount, false));
			_clusterStatus = new PreAllocatedList<bool>(status, status.Count);

			var pageSize = ItemSerializer.IsStaticSize ? clusterSerializer.StaticSize * storageClusterCount : int.MaxValue;

			Clusters = new StreamPagedList<Cluster>(StreamPagedListType.Static, clusterSerializer, clusterStream, pageSize, _endianness) {
				IncludeListHeader = false
			};
		}

		protected override IEnumerable<int> GetFreeClusters(int numberRequired) {
			var clusterNumbers = _clusterStatus
				.WithIndex()
				.Where(x => !x.Item1)
				.Take(numberRequired)
				.Select(x => x.Item2)
				.ToArray();			
			
			Guard.Ensure(clusterNumbers.Length == numberRequired, "Insufficient free storage clusters to store item");
			foreach (var clusterNumber in clusterNumbers) 
				_clusterStatus[clusterNumber] = true;

			return clusterNumbers;
		}
		
		protected override void MarkClusterFree(int clusterNumber) => _clusterStatus[clusterNumber] = false;

		internal override void UpdateListing(int index, TListing listing) {
			_listings[index] = listing;
		}

		private void WriteHeader() {
			var writer = new EndianBinaryWriter(EndianBitConverter.For(_endianness), InnerStream);

			InnerStream.Seek(0, SeekOrigin.Begin);

			writer.Write(0);
			writer.Write(Tools.Array.Gen(HeaderSize - (int)InnerStream.Position, (byte)0)); // padding
		}

		private int ReadHeader() {
			var reader = new EndianBinaryReader(EndianBitConverter.Little, InnerStream);
			InnerStream.Seek(0, SeekOrigin.Begin);

			var count = reader.ReadInt32();
			return count;
		}

		private void UpdateCountHeader() {
			var writer = new EndianBinaryWriter(EndianBitConverter.Little, InnerStream);
			InnerStream.Seek(0, SeekOrigin.Begin);

			writer.Write(_listings.Count);
		}
	}

}
