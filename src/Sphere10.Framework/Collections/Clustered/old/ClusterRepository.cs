//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//namespace Sphere10.Framework {

//	/// <summary>
//	/// Organizes clusters of arrays for usage in <see cref="StreamedList{TItem,TListing}"/>. It is designed as follows:
//	/// - All data is stored in allocation units called "Clusters" which form a forward-pointing linked-list.
//	/// - A logical stream of data is fragmented across arbitrary clusters. These are called "FragmentedStreams" such that fragment 0 maps to cluster X, fragment 1 to cluster Y, etc..
//	/// - The first fragmented stream is a "cluster map" is a bit-vector that tracks which sector is used or not. The cluster map starts at cluster 0.
//	/// - The second fragmented stream is the "registry" which is a serialized list of "stream headers" which describe a stream and the first cluster of that stream.
//	/// - All data streams are subsequently stored in clusters having their starting cluster remembered in their listing.
//	///
//	///  To retrieve the N'th stream:
//	///		- first a lookup of the N'th item in the registry is done.
//	///		- This then points to the start cluster of the stream, and the total bytes of that stream.
//	///		- Those clusters are traversed until all the bytes are recovered.
//	///
//	/// To store a stream
//	/// Example:
//	/// [M][L][D][D][D][D][L][D][D][D][M]......
//	/// </summary>
//	internal class ClusterRepository {
	
//		private readonly StreamPagedList<Cluster> _clusters;
		
//		public ClusterRepository(Stream rootStream, BlobContainerHeader header, int clusterSize, Endianness endianness) {
//			Guard.ArgumentNotNull(rootStream, nameof(rootStream));
//			Guard.ArgumentNotNull(header, nameof(header));
//			Guard.ArgumentInRange(clusterSize, 1, int.MaxValue, nameof(clusterSize));
//			Header = header;
//			ClusterSize = clusterSize;
//			Endianness = endianness;
//			_clusters = new StreamPagedList<Cluster>(
//				new ClusterSerializer(clusterSize),
//				new NonClosingStream(new BoundedStream(rootStream, BlobContainerHeader.ByteLength, long.MaxValue) { UseRelativeOffset = true })
//			);
//			Map = new ClusterMap(this);
//		}

//		public int ClusterCount => _clusters.Count;

//		public int ClusterSize { get; }

//		public Endianness Endianness { get; }

//		public BlobContainerHeader Header { get; }

//		public ClusterMap Map { get; }

//		public Cluster LoadCluster(int index) => _clusters[index];

//		public void SaveCluster(int index, Cluster cluster) => _clusters[index] = cluster;

//		public void ReleaseClusters(int[] clusterIndices) {
//			foreach (var clusterIX in clusterIndices) 
//				Map.MarkUnused(clusterIX);
//			Header.UsedClusters -= clusterIndices.Length;
//			Header.LastKnownFreeCluster = Math.Min(Header.LastKnownFreeCluster, clusterIndices.Min());
//		}

//		public int[] AllocateClusters(int quantity) {
//			Map
//		}

//		public int FetchNextClusterIndex(int cluster) {
//			_clusters.ReadItemRaw(cluster, 0, sizeof(int), out var result);
//			return (int)_clusters.Reader.BitConverter.ToUInt32(result);
//		}

//		public void IncreaseClusters(int quantity) {
//			// Clusters can only grow by h
//			var used = 0;
//			for (var i = 0; i < quantity; i++) {
//				var nextClusterIX = _clusters.Count;
//				if (Map.IsClusterMapCluster(nextClusterIX)) {
//					// Add a cluster for the cluster map
//					_clusters.Add(NewCluster());
//					Map.MarkUsed(nextClusterIX);
//					used++;

//					// Mark the previous cluster pointer
//					if (nextClusterIX > 0) {
//						var prevClusterMapClusterIX = Map.CalculateFragmentCluster(nextClusterIX - 1);
//						_clusters.WriteItemBytes(prevClusterMapClusterIX, 0, EndianBitConverter.For(Endianness).GetBytes((uint)nextClusterIX));
//					}

//				}
//				_clusters.Add(NewCluster());
//			}
//			Header.TotalClusters = _clusters.Count;
//			Header.UsedClusters += used;
//		}

//		public void DecreaseClusters(int quantity) => throw new NotImplementedException();


//		public Cluster NewCluster(int next = 0) => new() { Next = next, Data = Tools.Array.Gen<byte>(ClusterSize, 0) };

	

//	}


//}
