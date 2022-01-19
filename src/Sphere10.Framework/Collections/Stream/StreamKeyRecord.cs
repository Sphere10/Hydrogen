using System.Runtime.InteropServices;

namespace Sphere10.Framework {

	[StructLayout(LayoutKind.Sequential)]
	public struct StreamKeyRecord : IStreamKeyRecord {
		public StreamRecordTraits Traits { get; set; }
		public int Size { get; set; }
		public int StartCluster { get; set; }
		public int KeyChecksum { get; set; }
	}
}
