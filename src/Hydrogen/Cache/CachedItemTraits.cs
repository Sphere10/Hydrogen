using System;

namespace Hydrogen {
	[Flags]
	public enum CachedItemTraits {
		Invalidated = 1 << 0,
		CanPurge = 1 << 1,
		Purged = 1 << 2,
		Default = CanPurge,
	}
}
