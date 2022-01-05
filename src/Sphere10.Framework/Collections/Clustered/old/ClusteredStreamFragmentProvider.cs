//using System;
//using System.Collections.Generic;

//namespace Sphere10.Framework {
//	/// <summary>
//	/// </summary>
//	internal class ClusteredStreamFragmentProvider : IStreamFragmentProvider, ILoadable {
//		public event EventHandlerEx<object> Loading;
//		public event EventHandlerEx<object> Loaded;

//		private readonly ClusterRepository _clusterRepository;
//		private readonly IList<int> _clusterMapClusters;

//		public ClusteredStreamFragmentProvider(ClusterRepository clusterRepository, int startCluster) {
//			_clusterRepository = clusterRepository;
//			_clusterMapClusters = new List<int> { 0 }; // cluster map always starts at cluster 0
//		}

//		public bool RequiresLoad { get; private set; }

//		public long TotalBytes => FragmentCount * _clusterRepository.ClusterSize;

//		public int FragmentCount => _clusterRepository.ClusterCount - _clusterRepository.CalculateTotalClusterMapClusters();

//		public void Load() {
//			if (!RequiresLoad)
//				throw new InvalidOperationException("Already loaded");
//			Loading?.Invoke(this);

//			// Keep track of all the clustermap cluster (this will never be large, even in a muti-terrabyte stream)
//			var nextCluster = _clusterMapClusters[0];
//			while ((nextCluster = _clusterRepository.FetchNextClusterIndex(nextCluster)) != 0)
//				_clusterMapClusters.Add(nextCluster);
//			RequiresLoad = false;
//			Loaded?.Invoke(this);
//		}

//		public ReadOnlySpan<byte> GetFragment(int index) {
//			CheckLoaded();
//			Guard.ArgumentInRange(index, 0, _clusterMapClusters.Count - 1, nameof(index));
//			var clusterIX = _clusterMapClusters[index];
//			var cluster = _clusterRepository.GetCluster(clusterIX);
//			return cluster.Data.AsSpan(sizeof(uint)..);  // first 4 bytes of cluster are next-cluster pointer
//		}

//		public bool TryMapStreamPosition(long position, out int fragmentIndex, out int fragmentPosition) {
//			CheckLoaded();
//			fragmentIndex = (int)(position / _clusterRepository.ClusterSize);
//			fragmentPosition = (int)(position % _clusterRepository.ClusterSize);
//			return true;
//		}

//		public void UpdateFragment(int fragmentIndex, int fragmentPosition, ReadOnlySpan<byte> updateSpan) {
//			CheckLoaded();
//			throw new NotImplementedException();
//		}


//		public bool TrySetTotalBytes(long length, out int[] newFragments, out int[] deletedFragments) {
//			CheckLoaded();
//			throw new NotImplementedException();
//		}

//		private void CheckLoaded() {
//			if (RequiresLoad)
//				throw new InvalidOperationException("Not loaded");
//		}
//	}
//}
