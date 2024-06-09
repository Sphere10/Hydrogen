// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public static class GenericParser {

	public static bool TryParse<T>(string input, out T value) {
		if (TryParse(typeof(T), input, out var objValue)) {
			value = (T)objValue;
			return true;
		}
		value = default;
		return false;
	}

	public static T Parse<T>(string input) {
		if (!TryParse<T>(input, out var value))
			throw new FormatException((input == null ? "Null string" : $"String '{input}'") + $" could not be parsed into an {typeof(T).Name}");
		return value;
	}

	public static T SafeParse<T>(this string input, T defaultValue = default) {
		if (!TryParse<T>(input, out var value))
			return defaultValue;
		return value;
	}

	public static bool TryParse(Type type, string input, out object value) {
		// Special case: when input is empty and T is nullable, then parsed correctly as null
		if (input == string.Empty && type.IsNullable()) {
			value = default;
			return true;
			// Note: the NULL case is handled by nullable converter
		}

		// Use component model type convertors
		var converter = TypeDescriptorEx.GetConverter(type);
		if (converter != null && converter.IsValid(input)) {
			value = converter.ConvertFromString(input);
			return true;
		}

		// Try enum 
		if (type.IsEnum)
			if (Enum.TryParse(type, input, out value))
				return true;

		value = default;
		return false;
	}

	public static object Parse(Type type, string input) {
		if (!TryParse(type, input, out var value))
			throw new FormatException((input == null ? "Null string" : $"String '{input}'") + $" could not be parsed into an {type.Name}");
		return value;
	}

	public static object SafeParse(Type type, string input) {
		TryParse(type, input, out var value);
		return value;
	}

}
