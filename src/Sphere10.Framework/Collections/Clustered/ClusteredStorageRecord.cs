using System.Runtime.InteropServices;

namespace Sphere10.Framework {

	//[StructLayout(LayoutKind.Sequential)]
	public struct ClusteredStorageRecord {
		
		public ClusteredStorageRecordTraits Traits { get; set; }
		
		public int Size { get; set; }
		
		public int StartCluster { get; set; }

		public int KeyChecksum { get; set; }

		public byte[] Key { get; set; }

		public override string ToString() => $"[{nameof(ClusteredStorageRecord)}] {nameof(Size)}: {Size}, {nameof(StartCluster)}: {StartCluster}, {nameof(Traits)}: {Traits}, {nameof(KeyChecksum)}: {KeyChecksum}, {nameof(Key)}: {Key?.ToHexString(true)}";
	}
}
