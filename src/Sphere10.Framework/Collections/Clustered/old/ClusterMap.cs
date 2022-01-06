//using System;
//using System.Collections.Generic;
//using System.Diagnostics;

//namespace Sphere10.Framework {
//	internal class ClusterMap {
//		private ClusterRepository _repository;
//		private readonly BitVector _bits;
//		private readonly SortedList<int> _freeClustersCache;

//		public ClusterMap(ClusterRepository repository, int freeClustersCacheSizeBytes) {
//			_repository = repository;
//			_bits = new BitVector(new FragmentedStream(new ClusterMap.FragmentProvider(this)));
//			MaxCachedFreeClusters = freeClustersCacheSizeBytes / sizeof(int);
//			/// TODO: pre-load free clusters cache (via ILoadable)
//			_freeClustersCache = new SortedList<int>(MaxCachedFreeClusters, SortDirection.Ascending);
//			RebuildFreeClustersCache();

//		}

		

//		public int TotalClusters => _repository.Header.TotalClusters;

//		public int ClusterSize => _repository.Header.ClusterSize;

//		public int MaxCachedFreeClusters { get; }

//		public int ClusterMapBitsPerCluster => ClusterSize * 8;

//		public int CalculateFragmentCluster(int clusterMapFragment) => ClusterMapBitsPerCluster * clusterMapFragment;

//		public int CalculateTotalFragments() => (int)Math.Ceiling(TotalClusters / (double)ClusterMapBitsPerCluster);

//		public bool IsClusterMapCluster(int cluster) => (ClusterSize * 8) % (cluster + 1) == 0;

//		public int[] ConsumeFreeClusters(int quantity) {
//			if (quantity == 0)
//				return Array.Empty<int>();
//			var consumed = new List<int>();
//			Debug.Assert(_bits[_repository.Header.LastKnownFreeCluster] == false);
//			consumed.Add(_repository.Header.LastKnownFreeCluster);
//		}

//		public void MarkUsed(int cluster) {
//			_bits[cluster] = true;
//			_freeClustersCache.Remove(cluster);
//		}

//		public void MarkUnused(int cluster) {
//			_bits[cluster] = false;
//			if (_freeClustersCache.Count < MaxCachedFreeClusters)
//				_freeClustersCache.Add(cluster);
//		}


//		private void RebuildFreeClustersCache() {
//			_freeClustersCache.Clear();
//			for(var i = 0; i < TotalClusters && _freeClustersCache.Count < MaxCachedFreeClusters; i++)
//				if (_bits[i] == false)
//					_freeClustersCache.Add(i);
//		}


//		internal class FragmentProvider : IStreamFragmentProvider {
//			private readonly ClusterMap _clusterMap;
			

//			public FragmentProvider(ClusterMap map) {
//				_clusterMap = map;
//			}

//			public long TotalBytes => (long)(FragmentCount * _clusterMap.ClusterMapBitsPerCluster * 0.125F);

//			/// <summary>
//			/// The number of sectors which the cluster map consumes. Since 1 byte stores 8 cluster used bits, 
//			/// </summary>
//			public int FragmentCount => _clusterMap.CalculateTotalFragments();

//			public ReadOnlySpan<byte> GetFragment(int index) {
//				Guard.ArgumentInRange(index, 0, FragmentCount - 1, nameof(index));
//				var clusterIX = _clusterMap.CalculateFragmentCluster(index); ;
//				var cluster = _clusterMap._repository.LoadCluster(clusterIX);
//				return cluster.Data.AsSpan(sizeof(uint)..);  // first 4 bytes of cluster are next-cluster pointer
//			}

//			public bool TryMapStreamPosition(long position, out int fragmentIndex, out int fragmentPosition) {
//				var clusterBitsPerCluster = _clusterMap._repository.ClusterSize * 8;
//				var logicalClusterMapCluster = (int)(position / clusterBitsPerCluster);
//				fragmentIndex = logicalClusterMapCluster;
//				fragmentPosition = (int)(position % _clusterMap._repository.ClusterSize);
//				return true;
//			}

//			public void UpdateFragment(int fragmentIndex, int fragmentPosition, ReadOnlySpan<byte> updateSpan) {
//				var clusterIX = _clusterMap.CalculateFragmentCluster(fragmentIndex);
//				var cluster = _clusterMap._repository.LoadCluster(clusterIX);
//				updateSpan.CopyTo(cluster.Data[fragmentPosition..]);
//				_clusterMap._repository.SaveCluster(clusterIX, cluster);
//			}

//			public bool TrySetTotalBytes(long length, out int[] newFragments, out int[] deletedFragments) {
//				throw new NotSupportedException("ClusterMap clusters are allocated automatically");
//			}
//		}

//	}
//}
