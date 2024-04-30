using System;
using System.Linq;
using System.Reflection;

using Hydrogen.Mapping;

namespace Hydrogen;
internal static class SerializationHelper {
	public static Member[] GetSerializableMembers(Type type)
		=> type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
			.Where(x => x.CanRead && x.CanWrite)
			.Where(x => !x.HasAttribute<TransientAttribute>(false))
			.Select(x => x.ToMember())
			.ToArray();

}

