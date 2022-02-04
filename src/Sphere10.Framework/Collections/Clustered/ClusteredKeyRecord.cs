using System.Runtime.InteropServices;

namespace Sphere10.Framework {

	[StructLayout(LayoutKind.Sequential)]
	public struct ClusteredKeyRecord : IClusteredKeyRecord {
		public StreamRecordTraits Traits { get; set; }
		public int Size { get; set; }
		public int StartCluster { get; set; }
		public int KeyChecksum { get; set; }
	}
}
