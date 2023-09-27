using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hydrogen.Mapping;

public static class MemberExtensions {
	public static Member ToMember(this PropertyInfo propertyInfo) {
		if (propertyInfo == null)
			throw new NullReferenceException("Cannot create member from null.");

		return new PropertyMember(propertyInfo);
	}

	public static Member ToMember(this MethodInfo methodInfo) {
		if (methodInfo == null)
			throw new NullReferenceException("Cannot create member from null.");

		return new MethodMember(methodInfo);
	}

	public static Member ToMember(this FieldInfo fieldInfo) {
		if (fieldInfo == null)
			throw new NullReferenceException("Cannot create member from null.");

		return new FieldMember(fieldInfo);
	}

	public static Member ToMember(this MemberInfo memberInfo) {
		if (memberInfo == null)
			throw new NullReferenceException("Cannot create member from null.");

		if (memberInfo is PropertyInfo)
			return ((PropertyInfo)memberInfo).ToMember();
		if (memberInfo is FieldInfo)
			return ((FieldInfo)memberInfo).ToMember();
		if (memberInfo is MethodInfo)
			return ((MethodInfo)memberInfo).ToMember();

		throw new InvalidOperationException("Cannot convert MemberInfo '" + memberInfo.Name + "' to Member.");
	}

	public static IEnumerable<Member> GetInstanceFields(this Type type) {
		foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			if (!field.Name.StartsWith("<"))
				yield return field.ToMember();
	}

	public static IEnumerable<Member> GetInstanceMethods(this Type type) {
		foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			if (!method.Name.StartsWith("get_") && !method.Name.StartsWith("set_") && method.ReturnType != typeof(void) && method.GetParameters().Length == 0)
				yield return method.ToMember();
	}

	public static IEnumerable<Member> GetInstanceProperties(this Type type) {
		foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			yield return property.ToMember();
	}

	public static IEnumerable<Member> GetInstanceMembers(this Type type) {
		var members = new HashSet<Member>(new MemberEqualityComparer());

		type.GetInstanceProperties().ForEach(x => members.Add(x));
		type.GetInstanceFields().ForEach(x => members.Add(x));
		type.GetInstanceMethods().ForEach(x => members.Add(x));

		if (type.BaseType != null && type.BaseType != typeof(object))
			type.BaseType.GetInstanceMembers().ForEach(x => members.Add(x));

		return members;
	}
}
