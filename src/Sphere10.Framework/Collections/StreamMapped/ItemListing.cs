using System.Runtime.InteropServices;

namespace Sphere10.Framework {

	[StructLayout(LayoutKind.Sequential)]
	public struct ItemListing {
		public int ClusterStartIndex { get; set; }

		public int Size { get; set; }
	}

}
