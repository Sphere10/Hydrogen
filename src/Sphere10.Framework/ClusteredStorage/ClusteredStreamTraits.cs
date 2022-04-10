using System;

namespace Sphere10.Framework {
	[Flags]
	public enum ClusteredStreamTraits : byte {
		/// <summary>
		/// Stream is standard
		/// </summary>
		None  = 0,

		/// <summary>
		/// Stream should be interpreted as null (not empty)
		/// </summary>
		IsNull = 1 << 0,

		/// <summary>
		/// In Dictionary usage, this bit indicates whether Stream is a part of the Dictionary. When 0, it is available to be used as a slot for the dictionary item.
		/// </summary>
		/// TODO: rename to IsTombstone and invert usage
		IsUsed = 1 << 1,


		Default = None,
	}
}
