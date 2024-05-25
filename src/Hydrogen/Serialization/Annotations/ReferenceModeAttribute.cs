using System;
using System.Reflection;
using Hydrogen.Mapping;

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

	public static ReferenceSerializerMode GetReferenceModeOrDefault(MemberInfo memberInfo)  {
		var result = memberInfo.TryGetCustomAttributeOfType<ReferenceModeAttribute>(false, out var attr) ? attr.Mode : ReferenceSerializerMode.Default;
		if (attr is not null && memberInfo is FieldInfo { FieldType.IsValueType: true } or PropertyInfo { PropertyType.IsValueType: true }) 
			throw new InvalidOperationException($"Type member {memberInfo.DeclaringType.ToStringCS()}.{memberInfo.Name} incorrectly specifies a {nameof(ReferenceModeAttribute)} for a value-type");
		return result;		

	}
}
