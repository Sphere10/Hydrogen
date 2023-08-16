using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hydrogen;

internal class StreamMappedClusterMap : ClusterMap<StreamPagedList<Cluster>>, ILoadable {
	public event EventHandlerEx<object> Loading { add => _clusters.InternalCollection.Loading += value; remove => _clusters.InternalCollection.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => _clusters.InternalCollection.Loaded += value; remove => _clusters.InternalCollection.Loaded -= value; }

	public StreamMappedClusterMap(Stream rootStream, long offset, ClusterSerializer clusterSerializer, Endianness endianness = HydrogenDefaults.Endianness, bool autoLoad = false) 
		: base( 
			new StreamPagedList<Cluster>(
				clusterSerializer,
				rootStream
					.AsBounded(offset, long.MaxValue, useRelativeOffset: true, allowInnerResize: true)
					.AsNonClosing(),
				endianness,
				includeListHeader: false,
				autoLoad: autoLoad
			),
			clusterSerializer.ClusterDataSize
		) {
	}

	public bool RequiresLoad => _clusters.InternalCollection.RequiresLoad;

	public void Load() => _clusters.InternalCollection.Load();

	public Task LoadAsync() => _clusters.InternalCollection.LoadAsync();

	public override ClusterTraits ReadClusterTraits(long cluster) {
		_clusters.InternalCollection.ReadItemBytes(cluster, Cluster.TraitsOffset, Cluster.TraitsLength, out var bytes);
		var traits = (ClusterTraits)bytes[0];
		return traits;
	}

	protected override void WriteClusterTraitsInternal(long clusterIndex, ClusterTraits traits) {
		_clusters.InternalCollection.WriteItemBytes(clusterIndex, Cluster.TraitsOffset, new[] { (byte)traits });
	}

	protected override void MaskClusterTraitsInternal(long clusterIndex, ClusterTraits traits, bool on) {
		_clusters.InternalCollection.ReadItemBytes(clusterIndex, Cluster.TraitsOffset, Cluster.TraitsLength, out var traitBytes);
		var newTraits = ((ClusterTraits)traitBytes[0]).CopyAndSetFlags(traits, on);
		_clusters.InternalCollection.WriteItemBytes(clusterIndex, Cluster.TraitsOffset, new[] { (byte)newTraits });
	}

	public override long ReadClusterPrev(long cluster) {
		_clusters.InternalCollection.ReadItemBytes(cluster, Cluster.PrevOffset, Cluster.PrevLength, out var bytes);
		var prevCluster = _clusters.InternalCollection.Reader.BitConverter.ToInt64(bytes);
		return prevCluster;
	}

	protected override void WriteClusterPrevInternal(long clusterIndex, long prev) {
		var bytes = _clusters.InternalCollection.Writer.BitConverter.GetBytes(prev);
		_clusters.InternalCollection.WriteItemBytes(clusterIndex, Cluster.PrevOffset, bytes);
	}

	public override long ReadClusterNext(long cluster) {
		_clusters.InternalCollection.ReadItemBytes(cluster, Cluster.NextOffset, Cluster.NextLength, out var bytes);
		var nextValue = _clusters.InternalCollection.Reader.BitConverter.ToInt32(bytes);
		return nextValue;
	}

	protected override void WriteClusterNextInternal(long clusterIndex, long next) {
		var bytes = _clusters.InternalCollection.Writer.BitConverter.GetBytes(next);
		_clusters.InternalCollection.WriteItemBytes(clusterIndex, Cluster.NextOffset, bytes);
	}

	public override byte[] ReadClusterData(long clusterIndex, long offset, long size) {
		_clusters.InternalCollection.ReadItemBytes(clusterIndex, Cluster.DataOffset + offset, size, out var bytes);
		return bytes;
	}

	protected override void WriteClusterDataInternal(long clusterIndex, long offset, ReadOnlySpan<byte> data) {
		Guard.Argument(data.Length <= ClusterSize, nameof(data), "Data length exceeds cluster data length");
		_clusters.InternalCollection.WriteItemBytes(clusterIndex, Cluster.DataOffset + offset, data);
	}
}