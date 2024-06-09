// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public sealed class InMemoryClusterMap : ClusterMap {

	private readonly IExtendedList<Cluster> _clusters;

	public InMemoryClusterMap(IExtendedList<Cluster> clusters, int clusterDataSize) {
		_clusters = clusters;
		ClusterSize = clusterDataSize;
		ZeroClusterBytes = new byte[clusterDataSize];
	}

	public override int ClusterSize { get; }
	
	public override byte[] ZeroClusterBytes { get; }

	public override IReadOnlyExtendedList<Cluster> Clusters => _clusters;

	internal override long ClusterCount => _clusters.Count;

	internal override void AddCluster(Cluster cluster) {
		_clusters.Add(cluster);
	}

	internal override Cluster GetCluster(long index) => _clusters[index];

	internal override void UpdateCluster(long index, Cluster cluster) {
		_clusters.Update(index, cluster);
	}

	internal override void RemoveEndClusters(long quantity) {
		_clusters.RemoveRange(_clusters.Count - quantity, quantity);  // Migrate included removals in @event
	}

	internal override void ClearClusters() 
		=> _clusters.Clear();

	internal override ClusterTraits ReadClusterTraits(long cluster) 
		=> GetCluster(cluster).Traits;

	internal override void WriteClusterTraitsInternal(long clusterIndex, ClusterTraits traits) 
		=> GetCluster(clusterIndex).Traits = traits;

	internal override void MaskClusterTraitsInternal(long clusterIndex, ClusterTraits traits, bool on)  {
		var cluster = GetCluster(clusterIndex);
		cluster.Traits = cluster.Traits.CopyAndSetFlags(traits, on);
	}
			
	internal override long ReadClusterPrev(long cluster)
		=> GetCluster(cluster).Prev;

	internal override void WriteClusterPrevInternal(long clusterIndex, long prev) => GetCluster(clusterIndex).Prev = prev;
	

	internal override long ReadClusterNext(long cluster) => GetCluster(cluster).Next;

	internal override void WriteClusterNextInternal(long clusterIndex, long next) => GetCluster(clusterIndex).Next = next;
	
	internal override byte[] ReadClusterData(long clusterIndex, long offset, long size) 
		=> GetCluster(clusterIndex).Data.AsSpan().Slice((int)offset, (int)size).ToArray();

	internal override void WriteClusterDataInternal(long clusterIndex, long offset, ReadOnlySpan<byte> data)
		=> data.CopyTo(GetCluster(clusterIndex).Data.AsSpan().Slice((int)offset));
	
	
}
