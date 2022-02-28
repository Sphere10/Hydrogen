using System.Runtime.InteropServices;

namespace Sphere10.Framework {

	[StructLayout(LayoutKind.Sequential)]
	public struct ClusteredStorageRecord {
		
		public ClusteredStorageRecordTraits Traits { get; set; }
		
		public int Size { get; set; }
		
		public int StartCluster { get; set; }

		public int KeyChecksum { get; set; }

		public byte[] ValueDigest { get; set; }

		public override string ToString() => $"[StreamRecord] Size: {Size}, Start: {StartCluster}, Traits: {Traits}, KeyChecksum: {KeyChecksum}, ValueDigest: {ValueDigest?.ToHexString(true)}";
	}
}
