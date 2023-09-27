namespace Hydrogen;

using System;
using System.Collections.Generic;

/// <summary>
/// When dealing with partially constructed generic types, the .NET type system equality can treat
/// to logically equivalent types as different. This comparer will treat them as equivalent.
/// </summary>
public class TypeEquivalenceComparer : IEqualityComparer<Type> {

	public static readonly TypeEquivalenceComparer Instance = new TypeEquivalenceComparer();

	public bool Equals(Type typeA, Type typeB) {
		// If both are null, or the same instance, they're equivalent.
		if (typeA == typeB) return true;

		// If either is null (but not both, based on the above check), they're not equivalent.
		if (typeA == null || typeB == null) return false;

		// Ensure both types are generic and have the same generic type definition.
		if (!typeA.IsGenericType || !typeB.IsGenericType || typeA.GetGenericTypeDefinition() != typeB.GetGenericTypeDefinition())
			return false;

		// Compare the generic arguments.
		var argsA = typeA.GetGenericArguments();
		var argsB = typeB.GetGenericArguments();

		// This check is mostly redundant due to the GetGenericTypeDefinition() check, but added for clarity.
		if (argsA.Length != argsB.Length) return false;

		for (int i = 0; i < argsA.Length; i++) {
			// If both are unspecified, treat them as equivalent.
			if (argsA[i].IsGenericParameter && argsB[i].IsGenericParameter) continue;

			// If one is unspecified and the other isn't, they're not equivalent.
			if (argsA[i].IsGenericParameter || argsB[i].IsGenericParameter) return false;

			// Now, check for deeper equivalence, including potential recursive checks.
			if (!this.Equals(argsA[i], argsB[i])) return false;
		}

		return true;
	}

	public int GetHashCode(Type type) {
		var hash = 0;
		if (type == null)
			return hash;

		// If the type is generic, use the generic type definition's hash code.
		if (!type.IsGenericType) {
			// If it's not a generic type, just use the type's hash code.
			hash = type.GetHashCode();
		} else {
			// Build a logically consistent hashcode
			hash = type.GetGenericTypeDefinition().GetHashCode();

			// For each generic argument, combine its hash code.
			foreach (var arg in type.GetGenericArguments()) {
				if (arg.IsGenericParameter) {
					// If it's an unspecified type argument, use a constant value to represent it.
					hash = HashCode.Combine(hash, -1);
				} else {
					// Otherwise, combine the hash code of the type argument recursively.
					hash = HashCode.Combine(hash, GetHashCode(arg));
				}
			}
		}

		return hash;
	}
}