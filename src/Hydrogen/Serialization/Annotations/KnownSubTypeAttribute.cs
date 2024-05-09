using System;
using System.Collections.Generic;

namespace Hydrogen;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public class KnownSubTypeAttribute(Type subType) : Attribute {
	public Type Type { get; set; } = subType;

	public static IEnumerable<Type> GetKnownSubTypes(Type type) {
		foreach(var subTypeAttribute in type.GetCustomAttributesOfType<KnownSubTypeAttribute>()) {
			Guard.Against(type.IsSealed, $"Invalid {typeof(KnownSubTypeAttribute).ToStringCS()} specified on {type.ToStringCS()}. The type {type.ToStringCS()} is sealed and has no sub-types.");
			Guard.Ensure(subTypeAttribute.Type.IsSubclassOf(type), $"Invalid {typeof(KnownSubTypeAttribute).ToStringCS()} specified on {type.ToStringCS()}. The type {subTypeAttribute.Type.ToStringCS()} is not a sub-type of {type.ToStringCS()}.");
			yield return subTypeAttribute.Type;
		}
	}
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public class KnownSubTypeAttribute<T>() : KnownSubTypeAttribute(typeof(T));
