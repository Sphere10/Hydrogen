using System;

namespace Sphere10.Framework {
	[Flags]
	public enum ClusterTraits {
		Used = 1 << 0, // 00000001
		Listing = 1 << 1,
		Data = 1 << 2,
		Free = 1 << 3
	}
}
