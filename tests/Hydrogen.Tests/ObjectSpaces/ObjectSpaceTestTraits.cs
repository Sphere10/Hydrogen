using System;

namespace Hydrogen.Tests;

[Flags]
public enum ObjectSpaceTestTraits {
	MemoryMapped,
	FileMapped,
	Merklized,
	PersistentIgnorant
}
