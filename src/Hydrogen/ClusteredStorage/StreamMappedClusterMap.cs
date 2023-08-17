using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hydrogen;

public class StreamMappedClusterMap : ClusterMap, ILoadable {

	public event EventHandlerEx<object> Loading { add => InnerStreamPagedList.Loading += value; remove => InnerStreamPagedList.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => InnerStreamPagedList.Loaded += value; remove => InnerStreamPagedList.Loaded -= value; }

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

	private StreamPagedList<Cluster> InnerStreamPagedList => (StreamPagedList<Cluster>)_clusters.InternalCollection;

	public bool RequiresLoad => InnerStreamPagedList.RequiresLoad;

	public void Load() => InnerStreamPagedList.Load();

	public Task LoadAsync() => InnerStreamPagedList.LoadAsync();

	public override ClusterTraits ReadClusterTraits(long cluster) {
		InnerStreamPagedList.ReadItemBytes(cluster, Cluster.TraitsOffset, Cluster.TraitsLength, out var bytes);
		var traits = (ClusterTraits)bytes[0];
		return traits;
	}

	protected override void WriteClusterTraitsInternal(long clusterIndex, ClusterTraits traits) {
		InnerStreamPagedList.WriteItemBytes(clusterIndex, Cluster.TraitsOffset, new[] { (byte)traits });
	}

	protected override void MaskClusterTraitsInternal(long clusterIndex, ClusterTraits traits, bool on) {
		InnerStreamPagedList.ReadItemBytes(clusterIndex, Cluster.TraitsOffset, Cluster.TraitsLength, out var traitBytes);
		var newTraits = ((ClusterTraits)traitBytes[0]).CopyAndSetFlags(traits, on);
		InnerStreamPagedList.WriteItemBytes(clusterIndex, Cluster.TraitsOffset, new[] { (byte)newTraits });
	}

	public override long ReadClusterPrev(long cluster) {
		InnerStreamPagedList.ReadItemBytes(cluster, Cluster.PrevOffset, Cluster.PrevLength, out var bytes);
		var prevCluster = InnerStreamPagedList.Reader.BitConverter.ToInt64(bytes);
		return prevCluster;
	}

	protected override void WriteClusterPrevInternal(long clusterIndex, long prev) {
		var bytes = InnerStreamPagedList.Writer.BitConverter.GetBytes(prev);
		InnerStreamPagedList.WriteItemBytes(clusterIndex, Cluster.PrevOffset, bytes);
	}

	public override long ReadClusterNext(long cluster) {
		InnerStreamPagedList.ReadItemBytes(cluster, Cluster.NextOffset, Cluster.NextLength, out var bytes);
		var nextValue = InnerStreamPagedList.Reader.BitConverter.ToInt32(bytes);
		return nextValue;
	}

	protected override void WriteClusterNextInternal(long clusterIndex, long next) {
		var bytes = InnerStreamPagedList.Writer.BitConverter.GetBytes(next);
		InnerStreamPagedList.WriteItemBytes(clusterIndex, Cluster.NextOffset, bytes);
	}

	public override byte[] ReadClusterData(long clusterIndex, long offset, long size) {
		InnerStreamPagedList.ReadItemBytes(clusterIndex, Cluster.DataOffset + offset, size, out var bytes);
		return bytes;
	}

	protected override void WriteClusterDataInternal(long clusterIndex, long offset, ReadOnlySpan<byte> data) {
		Guard.Argument(data.Length <= ClusterSize, nameof(data), "Data length exceeds cluster data length");
		InnerStreamPagedList.WriteItemBytes(clusterIndex, Cluster.DataOffset + offset, data);
	}
}