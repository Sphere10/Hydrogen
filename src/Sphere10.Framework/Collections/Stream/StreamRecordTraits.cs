using System;

namespace Sphere10.Framework {
	[Flags]
	public enum StreamRecordTraits : byte {
		None  = 0,
		
		IsNull = 1 << 0,
		IsUsed = 1 << 1,
		Bit2 = 1 << 2,
		Bit3 = 1 << 3,
		Bit4 = 1 << 4,
		Bit5 = 1 << 5,
		Bit6 = 1 << 6,
		Bit7 = 1 << 7,

		Default = None,
	}
}
