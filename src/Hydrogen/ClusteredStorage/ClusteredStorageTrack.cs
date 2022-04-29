using System.Runtime.InteropServices;

namespace Hydrogen;

[StructLayout(LayoutKind.Sequential)]
public struct ClusteredStorageTrack {

	public int StartCluster { get; set; }

	public int RecordCount { get; set; }

	public override string ToString() => $"[{nameof(ClusteredStorageTrack)}] {nameof(StartCluster)}: {StartCluster}, {nameof(RecordCount)}: {RecordCount}";
}