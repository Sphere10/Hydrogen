using System;

namespace Hydrogen.Tests.ObjectSpaces;

[Flags]
public enum TestTraits {
	MemoryMapped,
	FileMapped,
	Merklized,
	PersistentIgnorant
}
