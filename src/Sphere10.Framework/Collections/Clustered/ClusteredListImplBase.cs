using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	internal abstract class ClusteredListImplBase<TItem, TListing> : RangedListBase<TItem>, ILoadable where TListing : IClusteredItemListing {
		public event EventHandlerEx<object> Loading;
		public event EventHandlerEx<object> Loaded;

		private ClusteredListingActivator<TItem, TListing> _listingActivator;
		protected readonly IItemSerializer<TListing> ListingSerializer;
		protected readonly IItemSerializer<TItem> ItemSerializer;
		protected readonly IEqualityComparer<TItem> ItemComparer;
		protected readonly Stream InnerStream;
		protected readonly int ClusterDataSize;
		protected StreamPagedList<Cluster> Clusters;

		public event EventHandlerEx<object, ClusteredItemAccessedArgs<TItem, TListing>> ItemAccess;

		protected ClusteredListImplBase(int clusterDataSize, Stream stream, IItemSerializer<TItem> itemSerializer, IItemSerializer<TListing> listingSerializer, IEqualityComparer<TItem> itemComparer = null) {
			Guard.ArgumentInRange(clusterDataSize, 1, int.MaxValue, nameof(clusterDataSize));
			Guard.ArgumentNotNull(stream, nameof(stream));
			Guard.ArgumentNotNull(itemSerializer, nameof(itemSerializer));
			Guard.ArgumentNotNull(listingSerializer, nameof(listingSerializer));
			Guard.Argument(listingSerializer.IsStaticSize, nameof(listingSerializer), "Listing objects must be fixed size");
			_listingActivator = null; // Must be set post-construction by implementation class (TODO: refactor this away with better pattern)
			ItemSerializer = itemSerializer;
			ListingSerializer = listingSerializer;
			ItemComparer = itemComparer ?? EqualityComparer<TItem>.Default;
			InnerStream = stream;
			ClusterDataSize = clusterDataSize;
			RequiresLoad = stream.Length != 0;  // stream length may be -1 if it's an ExtendedStream of an unloaded ILoadableList
		}

		internal ClusteredListingActivator<TItem, TListing> ListingActivator { 
			get {
				Guard.Ensure(_listingActivator != null, "Listing activator has not been configured by client class");
				return _listingActivator;
			}
			set => _listingActivator = value;
		}

		public abstract IReadOnlyList<TListing> Listings { get; }

		public bool RequiresLoad { get; protected set; }
	
		protected abstract IEnumerable<int> GetFreeClusters(int numberRequired);

		protected bool SuppressNotifications;

		public virtual void Load() {
			if (InnerStream is ILoadable { RequiresLoad: true } loadable) {
				NotifyLoading();
				loadable.Load();
				NotifyLoaded();
			}
			RequiresLoad = false;
		}

		//TODO: add ability to read raw byte segments that by-pass expensive serialization
		//public abstract void UpdateItemBytes(int index, int byteOffset, byte[] bytes);

		//public abstract void ReadItemBytes(int index, int byteOffset, int byteCount, out byte[] bytes);

		protected virtual void Initialize() {
			RequiresLoad = false;
		}

		protected TListing NewListingInstance(TItem item, int itemSizeBytes, int clusterStartIndex)
			=> ListingActivator(this, item, itemSizeBytes, clusterStartIndex);

		internal abstract void UpdateListing(int index, TListing listing);

		protected abstract void MarkClusterFree(int clusterNumber);

		protected TListing AddItemToClusters(int listingIndex, TItem item) {
			if (item is null) {
				// TODO: size should be 0?
				return NewListingInstance(item, -1, -1);
			}

			var data = ItemSerializer.Serialize(item, Endianness.LittleEndian);
			var listing = AddItemInternal(item, data);
			NotifyItemAccess(ListOperationType.Add, listingIndex, listing, item, data);
			return listing;
		}

		protected TListing UpdateItemInClusters(int listingIndex, TListing itemListing, TItem update) {
			// note: implementation here removes items (frees up clusters) and adds it again (consumes free clusters).
			var updatedData = ItemSerializer.Serialize(update, Endianness.LittleEndian);
			RemoveItemFromClusters(listingIndex, itemListing);
			var listing = AddItemInternal(update, updatedData);
			NotifyItemAccess(ListOperationType.Update, listingIndex, listing, update, updatedData);
			return listing;
		}

		protected TItem ReadItemFromClusters(int listingIndex, TListing listing) {
			var size = listing.Size;
			var startCluster = listing.ClusterStartIndex;
			if (size == -1 && startCluster == -1)
				return default;

			int? next = startCluster;
			var remaining = size;

			var builder = new ByteArrayBuilder();

			while (next != -1) {
				var cluster = Clusters[next.Value];
				next = cluster.Next;
				if (cluster.Next < 0) {
					builder.Append(cluster.Data.Take(remaining).ToArray());
				} else {
					builder.Append(cluster.Data);
					remaining -= cluster.Data.Length;
				}
			}

			var data = builder.ToArray();
			Guard.Ensure(data.Length == size, "Read item was not same size as listing");
			var item = ItemSerializer.Deserialize(data, Endianness.LittleEndian);
			NotifyItemAccess(ListOperationType.Read, listingIndex, listing, item, data);
			return item;
		}

		protected void RemoveItemFromClusters(int listingIndex, TListing listing) {
			var startClusterNumber = listing.ClusterStartIndex;
			var next = startClusterNumber;

			while (next != -1) {
				var cluster = Clusters[next];
				// Removed since old data is unnecessary burden (for now)
				//var data = cluster.Data.Take(Math.Min(size, cluster.Data.Length)).ToArray();
				//builder.Append(data);
				//size -= data.Length;

				MarkClusterFree(cluster.Number);
				next = cluster.Next;
			}

			NotifyItemAccess(ListOperationType.Remove, listingIndex, listing, default, default);
		}

		private TListing AddItemInternal(TItem item, byte[] data) {
			var clusters = new List<Cluster>();
			var segments = data.Partition(ClusterDataSize).ToArray();
			var numbers = GetFreeClusters(segments.Length).ToArray();

			for (var i = 0; i < segments.Length; i++) {
				var segment = segments[i].ToArray();
				var clusterData = new byte[ClusterDataSize];
				segment.CopyTo(clusterData, 0);

				clusters.Add(new Cluster {
					Number = numbers[i],
					Data = clusterData,
					Next = i == segments.Length - 1 ? -1 : numbers[i + 1],
					Traits = ClusterTraits.Used
				});
			}

			foreach (var cluster in clusters)
				if (!Clusters.Any())
					Clusters.Add(cluster);
				else if (cluster.Number >= Clusters.Count)
					Clusters.Add(cluster);
				else
					Clusters[cluster.Number] = cluster;

			return NewListingInstance(item, data.Length, clusters.FirstOrDefault()?.Number ?? -1);
		}

		protected virtual void OnLoading() {
		}

		protected virtual void OnLoaded() {
		}

		protected virtual void OnItemAccess(ListOperationType operationType, int listingIndex, TListing listing, TItem item, byte[] serializedItem) {
		}

		protected void NotifyItemAccess(ListOperationType operationType, int listingIndex, TListing listing, TItem item, byte[] serializedItem) {
			if (SuppressNotifications)
				return;

			OnItemAccess(operationType, listingIndex, listing, item, serializedItem);
			var args = new ClusteredItemAccessedArgs<TItem, TListing> { ListingIndex = listingIndex, Listing = listing, Item = item, SerializedItem = serializedItem };
			ItemAccess?.Invoke(this, args);
		}

		protected void NotifyLoading() {
			if (SuppressNotifications)
				return;

			OnLoading();
			Loading?.Invoke(this);
		}

		protected void NotifyLoaded() {
			if (SuppressNotifications)
				return;

			OnLoaded();
			Loaded?.Invoke(this);
		}

		protected void CheckLoaded() {
			if (InnerStream.Length == 0)
				Initialize(); // only initialize when stream is empty

			if (RequiresLoad) 
				throw new InvalidOperationException("List requires loading as stream contains existing data.");
		}

		protected void CheckRange(int index, int count) {
			var startIx = 0;
			var lastIx = (Count - 1).ClipTo(startIx, int.MaxValue);
			Guard.ArgumentInRange(index, startIx, lastIx, nameof(index));
			if (count > 0)
				Guard.ArgumentInRange(index + count - 1, startIx, lastIx, nameof(count));
		}

		public class Cluster {
			public ClusterTraits Traits { get; set; }
			// will pad 3 bytes after traits when serialized
			public int Number { get; set; }
			public byte[] Data { get; set; }
			public int Next { get; set; }
		}

		public class ClusterSerializer : StaticSizeObjectSerializer<Cluster> {
			private readonly int _clusterDataSize;

			public ClusterSerializer(int clusterSize) : base(sizeof(int) + clusterSize + sizeof(int) + sizeof(int)) {
				Guard.ArgumentInRange(clusterSize, 1, int.MaxValue, nameof(clusterSize));
				_clusterDataSize = clusterSize;
			}
			
			public override bool TrySerialize(Cluster item, EndianBinaryWriter writer, out int bytesWritten) {
				try {
					Guard.ArgumentNotNull(item, nameof(item));
					Guard.ArgumentNotNull(writer, nameof(writer));
				
					Debug.Assert(item.Data.Length == _clusterDataSize);

					writer.Write((byte)item.Traits);
					writer.Write((ushort)0); // padding
					writer.Write((byte)0); // padding
					writer.Write(item.Number);
					writer.Write(item.Data);
					writer.Write(item.Next);

					bytesWritten = sizeof(int) + _clusterDataSize + sizeof(int) + sizeof(int);
					
					return true;
				} catch (Exception) {
					bytesWritten = 0;
					return false;
				}
			}

			public override bool TryDeserialize(int byteSize, EndianBinaryReader reader, out Cluster item) {
				try {
					Guard.ArgumentNotNull(reader, nameof(reader));

					var traits = (ClusterTraits)reader.ReadByte();
					reader.ReadUInt16(); // PADDING	
					reader.ReadByte(); // PADDING
					var number = reader.ReadInt32();
					var data = reader.ReadBytes(_clusterDataSize);
					var next = reader.ReadInt32();

					var cluster = new Cluster {
						Traits = traits,
						Number = number,
						Data = data,
						Next = next
					};
					item = cluster;
					return true;
				} catch (Exception) {
					item = default;
					return false;
				}
			}
		}

		[Flags]
		public enum ClusterTraits : byte {
			Used = 1 << 0, // 00000001
			Listing = 1 << 1,
			Data = 1 << 2,
			Free = 1 << 3
		}

	}
}
