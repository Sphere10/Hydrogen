// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools;

public static class Enums {

	public static string[] GetSerializableOrientedNames<T>() where T : struct
		=> GetSerializableOrientedNames(typeof(T)).ToArray();

	public static string[] GetSerializableOrientedNames(Type enumType)
		// here we reverse the set to pick EnumValueAttribute/Description names with precedence
		=> GetNameCandidates(!enumType.IsNullable() ? enumType : Nullable.GetUnderlyingType(enumType)).Select(x => x.Reverse().First()).ToArray();

	public static string GetSerializableOrientedName(Enum @enum)
		=> GetEnumNameCandidates(@enum).Reverse().First();

	public static string GetSerializableOrientedNameOrDefault(Enum @enum, string @default = "Unknown")
		=> @enum != null ? GetEnumNameCandidates(@enum).Reverse().First() : @default;

	public static string GetHumanReadableName(Enum @enum)
		=> GetDescription(@enum) ?? GetDisplayName(@enum) ?? @enum.ToString();

	/// <summary>
	/// For all enums, returns all their name candidates. An enum can have multiple serializable names based on attributes
	/// </summary>
	/// <param name="enumType"></param>
	/// <returns></returns>
	internal static IEnumerable<string[]> GetNameCandidates(Type enumType) {
		Guard.ArgumentNotNull(enumType, nameof(enumType));
		foreach (var value in Enum.GetValues(enumType))
			yield return GetEnumNameCandidates(value as Enum).ToArray();
	}

	public static IEnumerable<string> GetEnumNameCandidates(Enum @enum) {
		yield return @enum.ToString();
		var attributes = @enum.GetAttributes<EnumMemberAttribute>();
		if (attributes.Any())
			yield return attributes.First().Value;
	}

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

	public static string GetDisplayName(Enum value, string @default = null) {
		var displayNameAttrs = GetAttributes<DisplayNameAttribute>(value).ToArray();
		return displayNameAttrs.Length > 0 ? displayNameAttrs[0].DisplayName : @default;
	}


	public static string GetDescription(Enum value, string @default = null)
		=> GetDescriptions(value).FirstOrDefault() ?? @default ?? value.ToString();

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

	private static readonly char[] FlagDelimiter = new[] { ',' };

	public static bool TryParseEnum<TEnum>(string value, out TEnum result) where TEnum : struct {
		if (!TryParseEnum(typeof(TEnum), value, false, out var objResult)) {
			result = default;
			return false;
		}
		result = (TEnum)objResult;
		return true;
	}

	public static bool TryParseEnum(Type enumType, string value, bool ignoreValueCase, out object result) {
		Guard.ArgumentNotNull(enumType, nameof(enumType));
		Guard.Argument(enumType.IsEnum, nameof(enumType), "Not an Enum");

		result = default;
		if (string.IsNullOrEmpty(value)) {
			return false;
		}

		if (!enumType.IsEnum)
			throw new ArgumentException(string.Format("Type '{0}' is not an enum", enumType.FullName));

		// Try to parse the value directly 
		if (System.Enum.IsDefined(enumType, value)) {
			result = System.Enum.Parse(enumType, value);
			return true;
		}


		// Get some info on enum
		var enumValues = System.Enum.GetValues(enumType);
		if (enumValues.Length == 0)
			return false; // probably can't happen as you cant define empty enum?
		var enumTypeCode = Type.GetTypeCode(enumValues.GetValue(0).GetType());



		// Get all the possible names and their value for enum 
		// todo: cache this for efficiency
		var enumInfo = new Dictionary<string, object>(ignoreValueCase ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture);


		var enumNameCandidates = Tools.Enums.GetNameCandidates(enumType).ToArray();
		for (var i = 0; i < enumNameCandidates.Length; i++) {
			var enumNames = enumNameCandidates[i];
			var enumVal = enumValues.GetValue(i);
			foreach (var enumName in enumNames)
				if (!enumInfo.ContainsKey(enumName))
					enumInfo.Add(enumName, enumVal);
		}

		// Try to match name manually
		if (enumInfo.TryGetValue(value.Trim(), out result))
			return true;

		// Try to parse it as a flag 
		if (value.IndexOf(',') != -1) {
			if (!Attribute.IsDefined(enumType, typeof(FlagsAttribute)))
				return false; // value has flags but enum is not flags


			ulong retVal = 0;
			foreach (var name in value.Split(FlagDelimiter)) {
				var trimmedName = name.Trim();
				if (!enumInfo.ContainsKey(trimmedName))
					return false; // Enum has no such flag

				var enumValueObject = enumInfo[trimmedName];
				ulong enumValueLong;
				switch (enumTypeCode) {
					case TypeCode.Byte:
						enumValueLong = (byte)enumValueObject;
						break;
					case TypeCode.SByte:
						enumValueLong = (byte)((sbyte)enumValueObject);
						break;
					case TypeCode.Int16:
						enumValueLong = (ushort)((short)enumValueObject);
						break;
					case TypeCode.Int32:
						enumValueLong = (uint)((int)enumValueObject);
						break;
					case TypeCode.Int64:
						enumValueLong = (ulong)((long)enumValueObject);
						break;
					case TypeCode.UInt16:
						enumValueLong = (ushort)enumValueObject;
						break;
					case TypeCode.UInt32:
						enumValueLong = (uint)enumValueObject;
						break;
					case TypeCode.UInt64:
						enumValueLong = (ulong)enumValueObject;
						break;
					default:
						return false; // should never happen
				}
				retVal |= enumValueLong;
			}
			result = System.Enum.ToObject(enumType, retVal);
			return true;
		}

		// the value may be a number, so parse it directly
		switch (enumTypeCode) {
			case TypeCode.SByte:
				sbyte sb;
				if (!SByte.TryParse(value, out sb))
					return false;
				result = System.Enum.ToObject(enumType, sb);
				break;
			case TypeCode.Byte:
				byte b;
				if (!Byte.TryParse(value, out b))
					return false;
				result = System.Enum.ToObject(enumType, b);
				break;
			case TypeCode.Int16:
				short i16;
				if (!Int16.TryParse(value, out i16))
					return false;
				result = System.Enum.ToObject(enumType, i16);
				break;
			case TypeCode.UInt16:
				ushort u16;
				if (!UInt16.TryParse(value, out u16))
					return false;
				result = System.Enum.ToObject(enumType, u16);
				break;
			case TypeCode.Int32:
				int i32;
				if (!Int32.TryParse(value, out i32))
					return false;
				result = System.Enum.ToObject(enumType, i32);
				break;
			case TypeCode.UInt32:
				uint u32;
				if (!UInt32.TryParse(value, out u32))
					return false;
				result = System.Enum.ToObject(enumType, u32);
				break;
			case TypeCode.Int64:
				long i64;
				if (!Int64.TryParse(value, out i64))
					return false;
				result = System.Enum.ToObject(enumType, i64);
				break;
			case TypeCode.UInt64:
				ulong u64;
				if (!UInt64.TryParse(value, out u64))
					return false;
				result = System.Enum.ToObject(enumType, u64);
				break;
			default:
				return false; // should never happen
		}

		return true;
	}

	public static object ParseEnum(Type enumType, string value, bool ignoreValueCase)
		=> TryParseEnum(enumType, value, ignoreValueCase, out var result) ? result : throw new FormatException($"Invalid formatted {enumType.Name} enum: {value}");

	public static object ParseEnumOrDefault(Type enumType, string value, bool ignoreValueCase, object defaultValue)
		=> TryParseEnum(enumType, value, ignoreValueCase, out var result) ? result : defaultValue;

	public static TEnum ParseEnum<TEnum>(string value, bool ignoreValueCase)
		=> (TEnum)ParseEnum(typeof(TEnum), value, ignoreValueCase);

	public static TEnum ParseEnumOrDefault<TEnum>(Type enumType, string value, bool ignoreValueCase, TEnum defaultValue)
		=> ParseEnumOrDefault(typeof(TEnum), value, ignoreValueCase, defaultValue);

}
