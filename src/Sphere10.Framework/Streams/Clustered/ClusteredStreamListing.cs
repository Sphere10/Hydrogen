using System.Runtime.InteropServices;

namespace Sphere10.Framework {
	[StructLayout(LayoutKind.Sequential)]
	public struct ClusteredStreamListing : IStreamListing {
		public int Size { get; set; }
		public int StartCluster { get; set; }

		public override string ToString() => $"[StreamListing] Size: {Size}, Start: {StartCluster}";
	}
}
