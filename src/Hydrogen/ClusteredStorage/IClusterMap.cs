using System;

namespace Hydrogen;

public interface IClusterMap : ISynchronizedObject {
	event EventHandlerEx<object, ClusterMapChangedEventArgs> Changed;

	IReadOnlyExtendedList<Cluster> Clusters { get; }

	public int ClusterSize { get; }

	public byte[] ZeroClusterBytes { get; }

	(long, long) NewClusterChain(long quantity, long terminalValue);

	long AppendClustersToEnd(long fromEnd, long quantity);

	long RemoveBackwards(long fromCluster, long quantity);

	long CalculateClusterChainLength(long byteLength);

	void Clear();

	ClusterTraits ReadClusterTraits(long cluster);

	void WriteClusterTraits(long cluster, ClusterTraits traits);
	
	void MaskClusterTraits(long cluster, ClusterTraits traits, bool on);

	long ReadClusterPrev(long cluster);
	
	void WriteClusterPrev(long cluster, long prev);

	long ReadClusterNext(long cluster);	
	
	void WriteClusterNext(long cluster, long next);

	byte[] ReadClusterData(long cluster, long offset, long size);

	void WriteClusterData(long cluster, long offset, ReadOnlySpan<byte> data);

}
