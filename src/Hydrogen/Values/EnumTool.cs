//-----------------------------------------------------------------------
// <copyright file="EnumTool.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Linq;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools {

	public static class Enums {

		public static IEnumerable<T> GetValues<T>() {
			return Enum.GetValues(typeof(T)).Cast<T>();
		}

		public static IEnumerable<int> GetIntValues(Enum @enum) {
			return Enum.GetValues(typeof(@Enum)).Cast<int>().OrderBy(x => x);
		}

		public static bool IsInRange(Type enumType, int value) {
			var values = Enum.GetValues(enumType).Cast<int>().OrderBy(x => x).ToArray();
			return value >= values.First() && value <= values.Last();
		}

		public static bool IsInRange<TEnum>(int value) {
			return IsInRange(typeof(TEnum), value);
		}

		public static IEnumerable<T> GetAttributes<T>(Enum value) where T : Attribute {
			return value.GetType().GetField(value.ToString()).GetCustomAttributesOfType<T>(false);
		}

		public static T GetAttribute<T>(Enum value) where T : Attribute {
			return GetAttributes<T>(value).First();
		}

		public static bool HasAttribute<T>(Enum value) where T : Attribute {
			return GetAttributes<T>(value).Any();
		}

		public static bool HasDescription(Enum value) {
			return GetAttributes<DescriptionAttribute>(value).Any();
		}

		public static IEnumerable<string> GetDescriptions(Enum value) {
			return GetAttributes<DescriptionAttribute>(value).Select(x => x.Description);
		}

		public static string GetDescription(Enum value, string @default = null) => GetDescriptions(value).FirstOrDefault() ?? @default ?? value.ToString();

		public static T GetValueFromDescription<T>(string description) {
			return (T)GetValueFromDescription(typeof(T), description);
		}

		public static object GetValueFromDescription(Type type, string description) {
			if (!type.IsEnum) throw new InvalidOperationException();
			foreach (var field in type.GetFields()) {
				if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute) {
					if (attribute.Description == description)
						return field.GetValue(null);
				} else {
					if (field.Name == description)
						return field.GetValue(null);
				}
			}
			throw new ArgumentException("Not found.", nameof(description));
		}

		public static IEnumerable<object> GetDefaultValues(Enum value) {
			return GetAttributes<DefaultValueAttribute>(value).Select(x => x.Value);
		}

		public static object GetDefaultValue(Enum value) {
			return GetDefaultValues(value).First();
		}


		public static string ToTextForm(Enum enumValue) {
			throw new NotImplementedException();
		}

		public static Enum ToTextForm(string enumValueText) {
			throw new NotImplementedException();
		}

	}
}

