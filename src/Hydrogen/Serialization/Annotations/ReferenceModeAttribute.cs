using System;

namespace Hydrogen;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ReferenceModeAttribute : Attribute {

	public bool Nullable {
		get => Mode.HasFlag(ReferenceSerializerMode.SupportNull);
		set => Mode = value ? Mode | ReferenceSerializerMode.SupportNull : Mode & ~ReferenceSerializerMode.SupportNull;
	}

	public bool AllowContextReference {
		get => Mode.HasFlag(ReferenceSerializerMode.SupportContextReferences);
		set => Mode = value ? Mode | ReferenceSerializerMode.SupportContextReferences : Mode & ~ReferenceSerializerMode.SupportContextReferences;
	}

	public bool AllowExternalReference {
		get => Mode.HasFlag(ReferenceSerializerMode.SupportExternalReferences);
		set => Mode = value ? Mode | ReferenceSerializerMode.SupportExternalReferences : Mode & ~ReferenceSerializerMode.SupportExternalReferences;
	}

	public ReferenceSerializerMode Mode { get; private set; } = ReferenceSerializerMode.Default;
}
