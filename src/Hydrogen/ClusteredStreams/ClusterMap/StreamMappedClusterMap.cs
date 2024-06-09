// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hydrogen;

public sealed class StreamMappedClusterMap : ClusterMap, ILoadable {

	public event EventHandlerEx<object> Loading { add => _clusters.InternalCollection.Loading += value; remove => _clusters.InternalCollection.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => _clusters.InternalCollection.Loaded += value; remove => _clusters.InternalCollection.Loaded -= value; }

	private readonly SynchronizedExtendedList<Cluster, StreamPagedList<Cluster>> _clusters;
	private readonly IDictionary<long, ClusterHeader> _clusterCache;
	private readonly bool _enableCache;

	private record ClusterHeader(ClusterTraits Traits, long Prev , long Next);

	public StreamMappedClusterMap(Stream rootStream, long offset, ClusterSerializer clusterSerializer, bool enableCache, Endianness endianness = HydrogenDefaults.Endianness, bool autoLoad = false) {
		_clusters = new StreamPagedList<Cluster>(
			clusterSerializer,
			rootStream
				.AsBounded(offset, long.MaxValue - offset, useRelativeOffset: true, allowInnerResize: true)
				.AsNonClosing(),
			endianness,
			includeListHeader: false,
			autoLoad: autoLoad
		).AsSynchronized<Cluster, StreamPagedList<Cluster>>();
		ClusterSize = clusterSerializer.ClusterDataSize;
		ZeroClusterBytes = new byte[clusterSerializer.ClusterDataSize];
		_enableCache = enableCache;
		_clusterCache = new Dictionary<long, ClusterHeader>();
			
	}

	public bool RequiresLoad => _clusters.InternalCollection.RequiresLoad;

	public void Load() => _clusters.InternalCollection.Load();

	public Task LoadAsync() => _clusters.InternalCollection.LoadAsync();

	public override int ClusterSize { get; }
	
	public override byte[] ZeroClusterBytes { get; }

	public override IReadOnlyExtendedList<Cluster> Clusters => _clusters;


	internal override long ClusterCount => _clusters.Count;

	internal override void AddCluster(Cluster cluster) {
		_clusters.Add(cluster);
		
		if (_enableCache)
			_clusterCache[_clusters.Count - 1] = new ClusterHeader(cluster.Traits, cluster.Prev, cluster.Next);
	}

	internal override Cluster GetCluster(long index) {
		var cluster = _clusters[index];

		if (_enableCache)
			_clusterCache[index] = new ClusterHeader(cluster.Traits, cluster.Prev, cluster.Next);
		return cluster;
	}

	internal override void UpdateCluster(long index, Cluster cluster) {
		if (_enableCache)
			_clusterCache.Remove(index);

		_clusters.Update(index, cluster);

		if (_enableCache)
			_clusterCache[index] = new ClusterHeader(cluster.Traits, cluster.Prev, cluster.Next);
	}

	internal override void RemoveEndClusters(long quantity) {
		if (_enableCache)
			Tools.Collection.RangeL(ClusterCount - quantity, ClusterCount).ForEach(x => _clusterCache.Remove(x));

		_clusters.RemoveRange(ClusterCount - quantity, quantity);  // Migrate included removals in @event
	}

	internal override void ClearClusters() {
		if (_enableCache)
			_clusterCache.Clear();

		_clusters.Clear();
	}

	internal override ClusterTraits ReadClusterTraits(long cluster) {
		if (_enableCache) 
			return GetCachedClusterHeader(cluster).Traits;

		_clusters.InternalCollection.ReadItemBytes(cluster, ClusterSerializer.TraitsOffset, ClusterSerializer.TraitsLength, out var bytes);
		var traits = (ClusterTraits)bytes[0];
		return traits;
	}

	internal override void WriteClusterTraitsInternal(long clusterIndex, ClusterTraits traits) {
		if (_enableCache)
			_clusterCache.Remove(clusterIndex);

		_clusters.InternalCollection.WriteItemBytes(clusterIndex, ClusterSerializer.TraitsOffset, new[] { (byte)traits });
	}

	internal override void MaskClusterTraitsInternal(long clusterIndex, ClusterTraits traits, bool on) {
		if (_enableCache)
			_clusterCache.Remove(clusterIndex);

		_clusters.InternalCollection.ReadItemBytes(clusterIndex, ClusterSerializer.TraitsOffset, ClusterSerializer.TraitsLength, out var traitBytes);
		var newTraits = ((ClusterTraits)traitBytes[0]).CopyAndSetFlags(traits, on);
		_clusters.InternalCollection.WriteItemBytes(clusterIndex, ClusterSerializer.TraitsOffset, new[] { (byte)newTraits });
		if (_enableCache)
			_clusterCache.Remove(clusterIndex);
	}

	internal override long ReadClusterPrev(long cluster) {
		if (_enableCache) 
			return GetCachedClusterHeader(cluster).Prev;

		_clusters.InternalCollection.ReadItemBytes(cluster, ClusterSerializer.PrevOffset, ClusterSerializer.PrevLength, out var bytes);
		var prevCluster = _clusters.InternalCollection.Reader.BitConverter.ToInt64(bytes);
		return prevCluster;
	}

	internal override void WriteClusterPrevInternal(long clusterIndex, long prev) {
		if (_enableCache)
			_clusterCache.Remove(clusterIndex);

		var bytes = _clusters.InternalCollection.Writer.BitConverter.GetBytes(prev);
		_clusters.InternalCollection.WriteItemBytes(clusterIndex, ClusterSerializer.PrevOffset, bytes);
	}

	internal override long ReadClusterNext(long cluster) {
		if (_enableCache) 
			return GetCachedClusterHeader(cluster).Next;

		_clusters.InternalCollection.ReadItemBytes(cluster, ClusterSerializer.NextOffset, ClusterSerializer.NextLength, out var bytes);
		var nextValue = _clusters.InternalCollection.Reader.BitConverter.ToInt32(bytes);
		return nextValue;
	}

	internal override void WriteClusterNextInternal(long clusterIndex, long next) {
		if (_enableCache)
			_clusterCache.Remove(clusterIndex);

		var bytes = _clusters.InternalCollection.Writer.BitConverter.GetBytes(next);
		_clusters.InternalCollection.WriteItemBytes(clusterIndex, ClusterSerializer.NextOffset, bytes);
	}

	internal override byte[] ReadClusterData(long clusterIndex, long offset, long size) {
		// cluster data is not cached
		_clusters.InternalCollection.ReadItemBytes(clusterIndex, ClusterSerializer.DataOffset + offset, size, out var bytes);
		return bytes;
	}

	internal override void WriteClusterDataInternal(long clusterIndex, long offset, ReadOnlySpan<byte> data) {
		// cluster data is not cached

		Guard.Argument(data.Length <= ClusterSize, nameof(data), "Data length exceeds cluster data length");
		_clusters.InternalCollection.WriteItemBytes(clusterIndex, ClusterSerializer.DataOffset + offset, data);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ClusterHeader GetCachedClusterHeader(long index) {
		if (!_clusterCache.TryGetValue(index, out var header)) {
			_clusters.InternalCollection.ReadItemBytes(index, 0, ClusterSerializer.TraitsLength + ClusterSerializer.PrevLength + ClusterSerializer.NextLength, out var bytes);
			header = new ClusterHeader(
				(ClusterTraits)bytes[0],
				_clusters.InternalCollection.Reader.BitConverter.ToInt64(bytes, (int)ClusterSerializer.PrevOffset),
				_clusters.InternalCollection.Reader.BitConverter.ToInt64(bytes, (int)ClusterSerializer.NextOffset)
			);
			_clusterCache[index] = header;
		}
		return header;
	}

	
	
}