// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Hydrogen;

/// <summary>
/// Extension methods for the <see cref="Type"/> class.
/// </summary>
/// <remarks></remarks>
public static class TypeExtensions {


	/// <summary>
	/// Gets generic arguments (and their generic arguments) transitively.
	/// </summary>
	public static Type[] GetGenericArgumentsTransitively(this Type type) {
		var alreadyVisited = new HashSet<Type>();
		return Get(type).ToArray();

		IEnumerable<Type> Get(Type type) {
			if (!alreadyVisited.Add(type))
				yield break;

			yield return type;
				foreach(var genericArgument in type.GetGenericArguments()) 
					foreach (var subType in Get(genericArgument))
						yield return subType;
			
		}
	}

	public static bool IsEnumOrNullableEnum(this Type type, out Type enumType) {
		enumType = Nullable.GetUnderlyingType(type) ?? type;
		return enumType.IsEnum;
	}

	public static MethodInfo GetGenericMethod(this Type type, string name, int genericArgs) {
		var method = type.GetGenericMethods(name, genericArgs).FirstOrDefault();
		if (method is null) 
			throw new MissingMethodException(type.FullName, $"{name}<{Tools.Collection.Generate(() => ",").Take(genericArgs).ToDelimittedString(string.Empty)}>");
		return method;
	}

	public static IEnumerable<MethodInfo> GetGenericMethods(this Type type, string name, int genericArgs)
		=> type.GetMethods().Where(m => m.Name == name && m.IsGenericMethod && m.GetGenericArguments().Length == genericArgs);

	public static bool IsCrossAssemblyType(this Type type) {
		if (!type.IsGenericType)
			return false;
		var typeArgAssemblies = type.GenericTypeArguments.Select(x => x.Assembly).Distinct().ToArray();
		return typeArgAssemblies.Length != 0 && !typeArgAssemblies.SequenceEqual([type.Assembly]);
	}

	public static bool IsAssignableTo(this Type type, [NotNullWhen(true)] Type? targetType) => targetType?.IsAssignableFrom(type) ?? false;

	public static string ToStringCS(this Type type) {
		if (!type.IsGenericType)
			return type.Name;

		var builder = new StringBuilder();
		var name = type.Name;
		var index = name.IndexOf('`');
		builder.Append(name.Substring(0, index));
		builder.Append('<');

		var genericArguments = type.GetGenericArguments();

		for (var i = 0; i < genericArguments.Length; i++) {
			if (i > 0)
				builder.Append(", ");

			// If it's a generic parameter, append its name
			if (genericArguments[i].IsGenericParameter) {
				builder.Append(genericArguments[i].Name);
			}
			else {
				builder.Append(genericArguments[i].ToStringCS());
			}
		}

		builder.Append('>');
		return builder.ToString();
	}

	public static ConstructorInfo FindCompatibleConstructor(this Type type, Type[] parameterTypes) 
		=> TypeActivator.FindCompatibleConstructor(type, parameterTypes);

	public static bool TryActivateWithCompatibleArgs(Type type, object[] args, out object instance) 
		=> TypeActivator.TryActivateWithCompatibleArgs(type, args, out instance);

	public static object ActivateWithCompatibleArgs(this Type type, params object[] args) 
		=> TypeActivator.ActivateWithCompatibleArgs(type, args);

	public static IEnumerable<Type> GetAncestorClasses(this Type type)
		=> type.GetAncestorTypes().Where(t => !t.IsInterface);

	public static IEnumerable<Type> GetAncestorTypes(this Type type) {
		// is there any base type?
		if (type == null) {
			yield break;
		}

		// return all implemented or inherited interfaces
		foreach (var i in type.GetInterfaces()) {
			yield return i;
		}

		// return all inherited types
		var currentBaseType = type.BaseType;
		while (currentBaseType != null) {
			yield return currentBaseType;
			currentBaseType = currentBaseType.BaseType;
		}
	}

	public static bool IsPartialTypeDefinition(this Type type) {
		return type.IsConstructedGenericType && type.ContainsGenericParameters;
	}

	public static bool IsPartialOrGenericTypeDefinition(this Type type) 
		=> type.IsGenericTypeDefinition || type.IsPartialTypeDefinition();


	public static bool HasSubType(this Type type, Type otherType) => otherType.IsAssignableFrom(type);

	public static Type GetInterface(this Type type, string name) {
		return type.GetInterface(name, true);
	}

	public static string GetShortName(this Type type) {
		var fullName = type.FullName;
		var name = type.Name;
		var nameSpace = type.Namespace;
		if (string.IsNullOrWhiteSpace(nameSpace))
			return name;

		return name.Length < fullName.Length ? fullName.Substring(nameSpace.Length + 1) : name;

	}

	public static IEnumerable<PropertyInfo> GetProperties(this Type type, BindingFlags bindingFlags, bool includeInherited) {
		var dictionary = new Dictionary<string, List<PropertyInfo>>();

		Type? currType = type;

		while (currType != null) {
			var properties =
				currType
					.GetProperties(bindingFlags)
					.Where(prop => prop.DeclaringType == currType);

			foreach (var property in properties) {
				if (!dictionary.TryGetValue(property.Name, out var others)) {
					others = new List<PropertyInfo>();
					dictionary.Add(property.Name, others);
				}

				if (others.Any(other => other.GetMethod?.GetBaseDefinition() == property.GetMethod?.GetBaseDefinition())) {
					// This is an inheritance case. We can safely ignore the value of property since
					// we have seen a more derived value.
					continue;
				}

				others.Add(property);
			}

			currType = includeInherited ? currType.BaseType : null;
		}
		return dictionary.Values.SelectMany(p => p);
	}

	private static FieldInfo? GetField(this Type type, string name, BindingFlags bindingFlags, bool includeInherited) {
		FieldInfo? fi;
		while ((fi = type?.GetField(name, bindingFlags)) == null && (type = type?.BaseType) != null && includeInherited)
			;
		return fi;
	}

	public static bool IsActivatable(this Type type) {
		Guard.ArgumentNotNull(type, nameof(type));
		return !type.IsAbstract &&
		       !type.IsInterface &&
		       !type.IsGenericTypeDefinition &&
		       !type.ContainsGenericParameters;
	}

	public static bool IsFullyConstructed(this Type type) {
		Guard.ArgumentNotNull(type, nameof(type));
		return !type.IsGenericType || 
				type.IsConstructedGenericType && !type.ContainsGenericParameters;
	}

	/// <summary>
	/// Determines whether <paramref name="type"/> is a constructed type of <paramref name="genericTypeDefinition"/>.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="genericTypeDefinition">The generic type definition.</param>
	/// <returns><see langword="true" /> if <paramref name="type"/> is a constructed type of <paramref name="genericTypeDefinition"/>; otherwise, <see langword="false" />.</returns>
	/// <remarks>This should be interepreted as "is a constructed generic type of the following pure generic type".</remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsConstructedGenericTypeOf(this Type type, Type genericTypeDefinition) {
		Guard.ArgumentNotNull(type, nameof(type));
		Guard.ArgumentNotNull(genericTypeDefinition, nameof(genericTypeDefinition));
		Guard.Argument(genericTypeDefinition.IsGenericTypeDefinition, nameof(genericTypeDefinition), "Must be a generic type definition.");
		return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition;
	}

	/// <summary>
	/// Determines whether the specified type is a subtype of a specified base type.
	/// </summary>
	public static bool IsSubTypeOf(this Type type, Type baseType) {
		Guard.ArgumentNotNull(type, nameof(type));
		Guard.ArgumentNotNull(baseType, nameof(baseType));
		Guard.Argument(!baseType.IsGenericTypeDefinition, nameof(baseType), "Must not be be a generic type definition. Use IsSubTypeOfGenericType instead.");
		
		// A type is a subtype of itself
		if (type == baseType)
			return true;

		// first check through the implemented interfaces.
		if (baseType.IsInterface) 
			if (type.GetInterfaces().Any(interfaceType => interfaceType == baseType)) 
				return true;
			
		// Check base types
		while (type != null) {
			if (type == baseType) {
				return true;
			}
			type = type.BaseType;
		}
		
		return false;
	}

	/// <summary>
	/// Determines whether the specified type is a subtype of a specified generic type definition.
	/// </summary>
	/// <param name="type">The type to check.</param>
	/// <param name="genericTypeDefinition">The generic type definition to check against.</param>
	/// <returns>
	/// <c>true</c> if the specified type is a subtype of the specified generic type definition;
	/// otherwise, <c>false</c>.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when either <paramref name="type"/> or <paramref name="genericTypeDefinition"/> is <c>null</c>.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// Thrown when <paramref name="genericTypeDefinition"/> is not a generic type definition.
	/// </exception>
	/// <remarks>
	/// This method checks both the inheritance hierarchy and the interfaces implemented by the specified type.
	/// A type is considered a subtype of itself.
	/// </remarks>
	public static bool IsSubtypeOfGenericType(this Type type, Type genericTypeDefinition) => IsSubtypeOfGenericType(type, genericTypeDefinition, out _);


	/// <summary>
	/// Determines whether the specified type is a subtype of a specified generic type definition.
	/// </summary>
	/// <param name="type">The type to check.</param>
	/// <param name="genericTypeDefinition">The generic type definition to check against.</param>
	/// <param name="matchedGenericType">Out parameter that returns the first generic type in the inheritance or interface hierarchy of the specified type that matches the specified generic type definition. It returns <c>null</c> if no such type is found.</param>
	/// <returns>
	/// <c>true</c> if the specified type is a subtype of the specified generic type definition;
	/// otherwise, <c>false</c>.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown when either <paramref name="type"/> or <paramref name="genericTypeDefinition"/> is <c>null</c>.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// Thrown when <paramref name="genericTypeDefinition"/> is not a generic type definition.
	/// </exception>
	/// <remarks>
	/// This method checks both the inheritance hierarchy and the interfaces implemented by the specified type.
	/// A type is considered a subtype of itself.
	/// </remarks>
	public static bool IsSubtypeOfGenericType(this Type type, Type genericTypeDefinition, out Type matchedGenericType) {
		Guard.ArgumentNotNull(type, nameof(type));
		Guard.ArgumentNotNull(genericTypeDefinition, nameof(genericTypeDefinition));
		Guard.Argument(genericTypeDefinition.IsPartialOrGenericTypeDefinition(), nameof(genericTypeDefinition), "Must be a generic type definition");
		matchedGenericType = null!;

		// first check through the implemented interfaces.
		foreach (var interfaceType in type.GetInterfaces()) {  // GetInterfaces returns flattened list
			if (interfaceType.IsConstructedGenericType && interfaceType.GetGenericTypeDefinition() == genericTypeDefinition ||
				interfaceType.IsPartialOrGenericTypeDefinition() && TypeEquivalenceComparer.Instance.Equals(interfaceType, genericTypeDefinition)) {
				matchedGenericType = interfaceType;
				return true;
			}
		}
		
		// Check base types
		while (type != null) {
			if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition) {
				matchedGenericType = type;
				return true;
			}
			type = type.BaseType;
		}
		
		return false;
	}


	/// <summary>
	/// Determines whether <paramref name="type"/> is a nullable value type.
	/// </summary>
	/// <param name="type">The self.</param>
	/// <returns><see langword="true" /> if <paramref name="type"/> is a nullable value type; otherwise, <see langword="false" />.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullable(this Type type)
		=> type.IsValueType && type.IsConstructedGenericTypeOf(typeof(Nullable<>));


	/// <summary>
	/// Determines if a type is numeric.  Nullable numeric types are considered numeric.
	/// </summary>
	/// <remarks>
	/// Boolean is not considered numeric.
	/// </remarks>
	public static bool IsNumeric(this Type type) {
		if (type == null) {
			return false;
		}

		switch (Type.GetTypeCode(type)) {
			case TypeCode.Byte:
			case TypeCode.Decimal:
			case TypeCode.Double:
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.SByte:
			case TypeCode.Single:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
			case TypeCode.UInt64:
				return true;
			case TypeCode.Object:
				if (type.IsNullable()) {
					return Nullable.GetUnderlyingType(type).IsNumeric();
				}
				return false;
		}
		return false;
	}

	/// <summary>
	/// Determines if a type is numeric.  Nullable numeric types are considered numeric.
	/// </summary>
	/// <remarks>
	/// Boolean is not considered numeric.
	/// </remarks>
	public static bool IsIntegerNumeric(this Type type) {
		if (type == null) {
			return false;
		}

		switch (Type.GetTypeCode(type)) {
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.SByte:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
			case TypeCode.UInt64:
				return true;
			case TypeCode.Object:
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
					return Nullable.GetUnderlyingType(type).IsIntegerNumeric();
				}
				return false;
		}
		return false;
	}

	public static bool IsAnonymousType(this Type t) {
		var name = t.Name;
		if (name.Length < 3) {
			return false;
		}
		return name[0] == '<'
		       && name[1] == '>'
		       && name.IndexOf("AnonymousType", StringComparison.Ordinal) > 0;
	}

	public static bool IsCollection(this Type type) {
		return typeof(IEnumerable).IsAssignableFrom(type);
	}
}
