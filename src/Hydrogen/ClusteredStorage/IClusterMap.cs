using System;
using System.Collections.Generic;

namespace Hydrogen;

public delegate void ClustersCountChangedEventHandler(object sender, long clusterCountDelta, long? terminalValue);
public delegate void ClusterChainCreatedEventHandler(object sender, long startCluster, long endCluster, long totalClusters, long terminalValue);
public delegate void ClusterChainRemovedEventHandler(object sender, long terminalValue);
public delegate void ClusterChainBoundaryMovedEventHandler(object sender, long cluster, long terminalValue, long clusterCountDelta);
public delegate void ClusterMovedEventHandler(object sender, long fromCluster, long toCluster, ClusterTraits traits, long? terminalValue);

public interface IClusterMap : ISynchronizedObject {
	event ClustersCountChangedEventHandler ClusterCountChanged;
	event ClusterChainCreatedEventHandler ClusterChainCreated;
	event ClusterChainRemovedEventHandler ClusterChainRemoved;
	event ClusterChainBoundaryMovedEventHandler ClusterChainStartChanged;
	event ClusterChainBoundaryMovedEventHandler ClusterChainEndChanged;
	event ClusterMovedEventHandler ClusterMoved;

	IReadOnlyExtendedList<Cluster> Clusters { get; }

	public int ClusterSize { get; }

	public byte[] ZeroClusterBytes { get; }

	public bool PreventClusterNavigation { get; set; }

	(long, long) NewClusterChain(long quantity, long terminalValue);

	long AppendClustersToEnd(long fromEnd, long quantity);

	long RemoveBackwards(long fromCluster, long quantity);

	void Clear();

	ClusterTraits FastReadClusterTraits(long clusterIndex);

	void FastWriteClusterTraits(long clusterIndex, ClusterTraits traits);
	
	void FastMaskClusterTraits(long clusterIndex, ClusterTraits traits, bool on);

	long FastReadClusterPrev(long clusterIndex);
	
	void FastWriteClusterPrev(long clusterIndex, long prev);

	long FastReadClusterNext(long clusterIndex);	
	
	void FastWriteClusterNext(long clusterIndex, long next);

	byte[] FastReadClusterData(long clusterIndex, long offset, long size);

	void FastWriteClusterData(long clusterIndex, long offset, ReadOnlySpan<byte> data);

	long CalculateClusterChainLength(long byteLength);

	string ToStringFullContents();
}
