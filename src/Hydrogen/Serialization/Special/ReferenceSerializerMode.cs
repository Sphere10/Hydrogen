using System;

namespace Hydrogen;

[Flags]
public enum ReferenceSerializerMode {

	SupportNull = 1 << 0,
	SupportContextReferences = 1 << 1,   
	SupportExternalReferences = 1 << 1,

	None = 0,
	Default = SupportNull | SupportContextReferences | SupportExternalReferences,
}
