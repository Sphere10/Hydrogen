using System;
using System.Runtime.Serialization;

namespace Hydrogen.ObjectSpaces;

[Flags]
public enum ObjectSpaceTraits {
	[EnumMember(Value = "none")]
	None = 0,

	[EnumMember(Value = "merkleized")]
	Merkleized = 1 << 0,

	[EnumMember(Value = "autosave")]
	AutoSave = 1 << 1,

}
