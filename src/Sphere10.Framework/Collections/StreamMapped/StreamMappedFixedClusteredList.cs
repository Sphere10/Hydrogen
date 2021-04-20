using System;
using System.Collections;
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
	public abstract class StreamMappedFixedClusteredList<T, TListing> : StreamMappedClusteredListBase<T, TListing> where TListing : IItemListing {
		private const int HeaderSize = 256;

		private readonly int _maxStorageBytes;
		private IExtendedList<bool> _clusterStatus;
		private IExtendedList<TListing> _listings;

		public StreamMappedFixedClusteredList(
			int clusterDataSize,
			int maxItems,
			int maxStorageBytes,
			Stream stream,
			IObjectSerializer<T> itemSerializer,
			IObjectSerializer<TListing> listingSerializer,
			IEqualityComparer<T> itemComparer = null)
			: base(clusterDataSize, stream, itemSerializer, listingSerializer, itemComparer) {
			Guard.ArgumentInRange(clusterDataSize, 0, int.MaxValue, nameof(clusterDataSize));
			Guard.ArgumentInRange(maxItems, 0, int.MaxValue, nameof(maxItems));
			Guard.ArgumentInRange(maxStorageBytes, 0, int.MaxValue, nameof(maxStorageBytes));
			Guard.ArgumentNotNull(itemSerializer, nameof(itemSerializer));
			Guard.ArgumentNotNull(stream, nameof(stream));

			Capacity = maxItems;
			_maxStorageBytes = maxStorageBytes;

			if (!RequiresLoad) {
				Initialize();
			}
		}

		public override int Count => _listings?.Count ?? 0;

		public int Capacity { get; }

		public override void AddRange(IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			CheckLoaded();

			var itemsArray = items as T[] ?? items.ToArray();

			if (!itemsArray.Any())
				return;

			foreach (var item in itemsArray) {
				_listings.Add(AddItemToClusters(Count, item));
			}

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
			var clusterSerializer = new ClusterSerializer(ClusterDataSize);
			//var listingSerializer = new ItemListingSerializer();
			
			int listingTotalSize = ListingSerializer.FixedSize * Capacity;
			int availableClusterStorageBytes = _maxStorageBytes - HeaderSize - listingTotalSize;
			int bytesPerCluster = clusterSerializer.FixedSize + sizeof(bool);
			if (availableClusterStorageBytes < bytesPerCluster) 
				throw new InvalidOperationException("Max storage bytes is insufficient for list.");
			
			int storageClusterCount = availableClusterStorageBytes / (clusterSerializer.FixedSize + sizeof(bool));
			int statusTotalSize = storageClusterCount * sizeof(bool);
			
			var listingsStream = new BoundedStream(InnerStream, HeaderSize, HeaderSize + listingTotalSize - 1) { UseRelativeOffset = true };
			var statusStream = new BoundedStream(InnerStream, listingsStream.MaxAbsolutePosition + 1, listingsStream.MaxAbsolutePosition + statusTotalSize) { UseRelativeOffset = true };
			var clusterStream = new BoundedStream(InnerStream, statusStream.MaxAbsolutePosition + 1, long.MaxValue) { UseRelativeOffset = true };
			
			int itemCount = ReadHeader();
			var preAllocatedListingStore = new StreamMappedPagedList<TListing>(ListingSerializer, listingsStream) { IncludeListHeader = false };
			preAllocatedListingStore.Load();
			_listings = new PreAllocatedList<TListing>(preAllocatedListingStore);
			_listings.AddRange(preAllocatedListingStore.Take(itemCount));
			
			var status = new StreamMappedPagedList<bool>(new BoolSerializer(), statusStream) { IncludeListHeader = false };
			status.Load();
			_clusterStatus = new PreAllocatedList<bool>(status);
			_clusterStatus.AddRange(status);
			
			int pageSize = ItemSerializer.IsFixedSize ? clusterSerializer.FixedSize * storageClusterCount : int.MaxValue;
			
			Clusters = new StreamMappedPagedList<Cluster>(StreamMappedPagedListType.FixedSize, clusterSerializer, clusterStream, pageSize) {
				IncludeListHeader = false
			};
			
			Clusters.Load();
			Loaded = true;
		}
		
		protected override IEnumerable<int> GetFreeClusterNumbers(int numberRequired) {
			var numbers = _clusterStatus
				.WithIndex()
				.Where(x => !x.Item1)
				.Take(numberRequired)
				.Select(x => x.Item2)
				.ToArray();

			if (numbers.Length != numberRequired) {
				throw new InvalidOperationException("Insufficient free storage clusters to store item");
			}

			foreach (var clusterNumber in numbers) {
				_clusterStatus[clusterNumber] = true;
			}

			return numbers;
		}
		
		protected override void MarkClusterFree(int clusterNumber) => _clusterStatus[clusterNumber] = false;

		private void Initialize() {
			var clusterSerializer = new ClusterSerializer(ClusterDataSize);

			int listingTotalSize = ListingSerializer.FixedSize * Capacity;
			int availableClusterStorageBytes = _maxStorageBytes - HeaderSize - listingTotalSize;
			int bytesPerCluster = clusterSerializer.FixedSize + sizeof(bool);

			if (availableClusterStorageBytes < bytesPerCluster)
				throw new InvalidOperationException("Max storage bytes is insufficient for list.");

			int storageClusterCount = availableClusterStorageBytes / (clusterSerializer.FixedSize + sizeof(bool));
			int statusTotalSize = storageClusterCount * sizeof(bool);

			var listingsStream = new BoundedStream(InnerStream, HeaderSize, HeaderSize + listingTotalSize - 1) { UseRelativeOffset = true };
			var statusStream = new BoundedStream(InnerStream, listingsStream.MaxAbsolutePosition + 1, listingsStream.MaxAbsolutePosition + statusTotalSize) { UseRelativeOffset = true };
			var clusterStream = new BoundedStream(InnerStream, statusStream.MaxAbsolutePosition + 1, long.MaxValue) { UseRelativeOffset = true };
			
			WriteHeader();
			if (InnerStream.Length == 0)
				WriteHeader();
			
			var preAllocatedListingStore = new StreamMappedPagedList<TListing>(ListingSerializer, listingsStream) { IncludeListHeader = false };
			preAllocatedListingStore.AddRange(Tools.Array.Gen(Capacity, default(TListing)));
			_listings = new PreAllocatedList<TListing>(preAllocatedListingStore);

			//var status = new StreamMappedPagedList<bool>(new BoolSerializer(), statusStream) { IncludeListHeader = false };
			var status = new StreamMappedBitVector(statusStream);
			status.AddRange(Tools.Array.Gen(storageClusterCount, false));
			_clusterStatus = new PreAllocatedList<bool>(status);
			_clusterStatus.AddRange(status);

			int pageSize = ItemSerializer.IsFixedSize ? clusterSerializer.FixedSize * storageClusterCount : int.MaxValue;

			Clusters = new StreamMappedPagedList<Cluster>(StreamMappedPagedListType.FixedSize, clusterSerializer, clusterStream, pageSize) {
				IncludeListHeader = false
			};
			
			Loaded = true;
		}

		private void WriteHeader() {
			var writer = new EndianBinaryWriter(EndianBitConverter.Little, InnerStream);

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

	/// <summary>
	/// A list implementation which is mapped onto a stream via clusters. Items can added/removed anywhere, and they are stored
	/// in a non-contiguous manner via clusters of data (similar in principle to a file system format). The limitation here is that the
	/// the list is inherantly bounded from construction and cannot grow past the pre-determined maximum number of items. This uses
	/// <see cref="StreamMappedPagedList{TItem}"/> under the hood.
	/// </summary>
	public class StreamMappedFixedClusteredList<T> : StreamMappedFixedClusteredList<T, ItemListing> {
		public StreamMappedFixedClusteredList(
					int clusterDataSize,
					int maxItems,
					int maxStorageBytes,
					Stream stream,
					IObjectSerializer<T> itemSerializer,
					IEqualityComparer<T> itemComparer = null)
			: base(clusterDataSize, maxItems, maxStorageBytes, stream, itemSerializer, new ItemListingSerializer(), itemComparer) {
		}
		protected override ItemListing NewListingInstance(int itemSizeBytes, int clusterStartIndex) {
			return new ItemListing {
				Size = itemSizeBytes,
				ClusterStartIndex = clusterStartIndex
			};
		}
	}
}
