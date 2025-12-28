using System;
using System.Runtime.Serialization;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Flags describing optional behaviors for an object space.
/// </summary>
[Flags]
public enum ObjectSpaceTraits {
	[EnumMember(Value = "none")]
	None = 0,

	[EnumMember(Value = "merkleized")]
	/// <summary>
	/// Enables merkle-tree indexing for integrity verification.
	/// </summary>
	Merkleized = 1 << 0,

	[EnumMember(Value = "autosave")]
	/// <summary>
	/// Automatically persists tracked changes without manual save calls.
	/// </summary>
	AutoSave = 1 << 1,

}
