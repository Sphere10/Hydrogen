using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hydrogen;

public class StreamMappedClusterMap : ClusterMap, ILoadable {

	public event EventHandlerEx<object> Loading { add => _innerStreamPagedList.Loading += value; remove => _innerStreamPagedList.Loading -= value; }
	public event EventHandlerEx<object> Loaded { add => _innerStreamPagedList.Loaded += value; remove => _innerStreamPagedList.Loaded -= value; }

	private readonly StreamPagedList<Cluster> _innerStreamPagedList;

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
		_innerStreamPagedList = (StreamPagedList<Cluster>)((SynchronizedExtendedList<Cluster>)Clusters).InternalCollection;
	}

	public bool RequiresLoad => _innerStreamPagedList.RequiresLoad;

	public void Load() => _innerStreamPagedList.Load();

	public Task LoadAsync() => _innerStreamPagedList.LoadAsync();

	public override ClusterTraits ReadClusterTraits(long cluster) {
		_innerStreamPagedList.ReadItemBytes(cluster, ClusterSerializer.TraitsOffset, ClusterSerializer.TraitsLength, out var bytes);
		var traits = (ClusterTraits)bytes[0];
		return traits;
	}

	protected override void WriteClusterTraitsInternal(long clusterIndex, ClusterTraits traits) {
		_innerStreamPagedList.WriteItemBytes(clusterIndex, ClusterSerializer.TraitsOffset, new[] { (byte)traits });
	}

	protected override void MaskClusterTraitsInternal(long clusterIndex, ClusterTraits traits, bool on) {
		_innerStreamPagedList.ReadItemBytes(clusterIndex, ClusterSerializer.TraitsOffset, ClusterSerializer.TraitsLength, out var traitBytes);
		var newTraits = ((ClusterTraits)traitBytes[0]).CopyAndSetFlags(traits, on);
		_innerStreamPagedList.WriteItemBytes(clusterIndex, ClusterSerializer.TraitsOffset, new[] { (byte)newTraits });
	}

	public override long ReadClusterPrev(long cluster) {
		_innerStreamPagedList.ReadItemBytes(cluster, ClusterSerializer.PrevOffset, ClusterSerializer.PrevLength, out var bytes);
		var prevCluster = _innerStreamPagedList.Reader.BitConverter.ToInt64(bytes);
		return prevCluster;
	}

	protected override void WriteClusterPrevInternal(long clusterIndex, long prev) {
		var bytes = _innerStreamPagedList.Writer.BitConverter.GetBytes(prev);
		_innerStreamPagedList.WriteItemBytes(clusterIndex, ClusterSerializer.PrevOffset, bytes);
	}

	public override long ReadClusterNext(long cluster) {
		_innerStreamPagedList.ReadItemBytes(cluster, ClusterSerializer.NextOffset, ClusterSerializer.NextLength, out var bytes);
		var nextValue = _innerStreamPagedList.Reader.BitConverter.ToInt32(bytes);
		return nextValue;
	}

	protected override void WriteClusterNextInternal(long clusterIndex, long next) {
		var bytes = _innerStreamPagedList.Writer.BitConverter.GetBytes(next);
		_innerStreamPagedList.WriteItemBytes(clusterIndex, ClusterSerializer.NextOffset, bytes);
	}

	public override byte[] ReadClusterData(long clusterIndex, long offset, long size) {
		_innerStreamPagedList.ReadItemBytes(clusterIndex, ClusterSerializer.DataOffset + offset, size, out var bytes);
		return bytes;
	}

	protected override void WriteClusterDataInternal(long clusterIndex, long offset, ReadOnlySpan<byte> data) {
		Guard.Argument(data.Length <= ClusterSize, nameof(data), "Data length exceeds cluster data length");
		_innerStreamPagedList.WriteItemBytes(clusterIndex, ClusterSerializer.DataOffset + offset, data);
	}
}