using System.Collections.Generic;
using System.Reflection;

namespace Hydrogen.FastReflection;

/// <summary>
/// Compares <see cref="MemberInfo"/> and ensures their owner types are also equal (.NET default does not do this).
/// </summary>
/// <typeparam name="T"></typeparam>
public class MemberInfoComparer<T> : IEqualityComparer<T> where T : MemberInfo {
	public bool Equals(T x, T y) =>  (x == null && y == null) || x.ReflectedType != null && x.Equals(y) && x.ReflectedType == y.ReflectedType;

	public int GetHashCode(T obj) => Tools.Object.CombineHashCodes(obj.GetHashCode(), obj.ReflectedType.GetHashCode());
		
}
