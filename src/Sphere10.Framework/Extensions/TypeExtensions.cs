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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Sphere10.Framework.FastReflection;

namespace Sphere10.Framework {

	/// <summary>
	/// Extension methods for the <see cref="Type"/> class.
	/// </summary>
	/// <remarks></remarks>
	public static class TypeExtensions {

	    public static Type GetInterface(this Type type, string name) {
	        return type.GetInterface(name, true);
	    }

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
					if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
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

		/// <summary>
		/// Determines whether this type is nullable.
		/// </summary>
		/// <returns><c>true</c> if the specified source is nullable; otherwise, <c>false</c>.</returns>
		/// <remarks></remarks>
		public static bool IsNullable(this Type source) {
			return source.IsGenericType && source.GetGenericTypeDefinition() == typeof(Nullable<>);			
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
	}
}
