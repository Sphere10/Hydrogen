using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {
	public abstract class StreamMappedClusteredListBase<T> : RangedListBase<T> {

		protected readonly IObjectSerializer<T> Serializer;
		protected readonly Stream InnerStream;
		protected readonly int ClusterDataSize;

		public event EventHandlerEx<object, byte[]> ItemRead;

		public event EventHandlerEx<object, byte[]> ItemWrite;

		public event EventHandlerEx<object, byte[]> ItemRemove;

		public event EventHandlerEx<object, byte[]> ItemUpdate;

		public StreamMappedClusteredListBase(int clusterDataSize, IObjectSerializer<T> serializer, Stream stream) {
			Serializer = serializer;
			InnerStream = stream;
			ClusterDataSize = clusterDataSize;
		}

		public virtual bool RequiresLoad => InnerStream.Length > 0 && !Loaded;

		public bool Loaded { get; protected set; }

		protected abstract IEnumerable<int> GetFreeClusterNumbers(int numberRequired);

		protected bool SuppressNotifications;

		internal abstract StreamMappedPagedList<Cluster> Clusters { get; set; }

		public abstract void Load();

		protected abstract void MarkClusterFree(int clusterNumber);

		protected ItemListing AddItemToClusters(T item) {
			if (item is null) {
				return new ItemListing {
					Size = -1,
					ClusterStartIndex = -1
				};
			}
			
			byte[] data = SerializeItem(item);
			return AddItemInternal(data);
		}

		protected ItemListing UpdateItemInClusters(ItemListing itemListing, T update) {
			RemoveItemFromClusters(itemListing.ClusterStartIndex, itemListing.Size);
			byte[] updatedData = SerializeItem(update);
			NotifyItemUpdate(updatedData);
			return AddItemInternal(updatedData);
		}

		protected T ReadItemFromClusters(int startCluster, int size) {
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
			NotifyItemRead(data);

			T item = Serializer.Deserialize(size,
				new EndianBinaryReader(EndianBitConverter.Little, new MemoryStream(data)));

			return item;
		}

		protected void RemoveItemFromClusters(int startClusterNumber, int size) {
			var next = startClusterNumber;

			ByteArrayBuilder builder = new ByteArrayBuilder();

			while (next != -1) {
				var cluster = Clusters[next];
				var data = cluster.Data.Take(Math.Min(size, cluster.Data.Length)).ToArray();
				builder.Append(data);
				size -= data.Length;

				MarkClusterFree(cluster.Number);
				next = cluster.Next;
			}

			NotifyItemRemove(builder.ToArray());
		}

		private ItemListing AddItemInternal(byte[] data) {
			var clusters = new List<Cluster>();
			var segments = data.Partition(ClusterDataSize)
				.ToList();

			var numbers = GetFreeClusterNumbers(segments.Count).ToArray();

			for (var i = 0; i < segments.Count; i++) {
				var segment = segments[i].ToArray();
				var clusterData = new byte[ClusterDataSize];
				segment.CopyTo(clusterData, 0);

				clusters.Add(new Cluster {
					Number = numbers[i],
					Data = clusterData,
					Next = segments.Count - 1 == i ? -1 : numbers[i + 1]
				});
			}

			foreach (var cluster in clusters)
				if (!Clusters.Any())
					Clusters.Add(cluster);
				else if (cluster.Number >= Clusters.Count)
					Clusters.Add(cluster);
				else
					Clusters[cluster.Number] = cluster;

			NotifyItemWrite(data);

			return new ItemListing {
				Size = data.Length,
				ClusterStartIndex = clusters.FirstOrDefault()?.Number ?? -1
			};
		}

		private byte[] SerializeItem(T item) {
			using var stream = new MemoryStream();
			Serializer.Serialize(item, new EndianBinaryWriter(EndianBitConverter.Little, stream));
			return stream.ToArray();
		}

		protected void CheckRange(int index, int count) {
			Guard.Argument(count >= 0, nameof(index), "Must be greater than or equal to 0");
			if (index == Count && count == 0) return; // special case: at index of "next item" with no count, this is valid
			Guard.ArgumentInRange(index, 0, Count - 1, nameof(index));
			if (count > 0)
				Guard.ArgumentInRange(index + count - 1, 0, Count - 1, nameof(count));
		}

		protected void CheckLoaded() {
			if (RequiresLoad) {
				throw new InvalidOperationException("List requires loading as stream contains existing data.");
			}
		}

		protected virtual void OnItemRead(byte[] item) {
		}

		protected virtual void OnItemWrite(byte[] item) {
		}

		protected virtual void OnItemRemove(byte[] item) {
		}

		protected virtual void OnItemUpdate(byte[] item) {
		}


		protected void NotifyItemRead(byte[] item) {
			if (SuppressNotifications)
				return;

			OnItemRead(item);
			ItemRead?.Invoke(this, item);
		}

		protected void NotifyItemWrite(byte[] item) {
			if (SuppressNotifications)
				return;

			OnItemWrite(item);
			ItemWrite?.Invoke(this, item);
		}

		protected void NotifyItemUpdate(byte[] item) {
			if (SuppressNotifications)
				return;

			OnItemUpdate(item);
			ItemUpdate?.Invoke(this, item);
		}

		protected void NotifyItemRemove(byte[] item) {
			if (SuppressNotifications)
				return;

			OnItemRemove(item);
			ItemRemove?.Invoke(this, item);
		}
	}
}
