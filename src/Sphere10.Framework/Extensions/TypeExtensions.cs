//-----------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Sphere10.Framework {

	/// <summary>
	/// Extension methods for the <see cref="Type"/> class.
	/// </summary>
	/// <remarks></remarks>
	public static class TypeExtensions {

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

		/// <summary>
		/// Determines whether <paramref name="type"/> is a constructed type of <paramref name="genericTypeDefinition"/>.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="genericTypeDefinition">The generic type definition.</param>
		/// <returns><see langword="true" /> if <paramref name="type"/> is a constructed type of <paramref name="genericTypeDefinition"/>; otherwise, <see langword="false" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsConstructedGenericTypeOf(this Type type, Type genericTypeDefinition)
			=> type.IsConstructedGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition;

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
}
