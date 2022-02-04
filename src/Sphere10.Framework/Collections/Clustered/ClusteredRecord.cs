using System.Runtime.InteropServices;

namespace Sphere10.Framework {

	[StructLayout(LayoutKind.Sequential)]
	public struct ClusteredRecord : IClusteredRecord {
		public StreamRecordTraits Traits { get; set; }
		public int Size { get; set; }
		public int StartCluster { get; set; }

		public override string ToString() => $"[StreamRecord] Size: {Size}, Start: {StartCluster}, Traits: {Traits}";
	}
}
