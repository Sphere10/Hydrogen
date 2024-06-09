// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//#define OMIT_GUARD

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;


namespace Hydrogen;

/// <summary>
/// Class used to guard against unexpected argument values
/// or operations by throwing an appropriate exception.
/// </summary>
public static class Guard {

#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CheckIndex(long index, long collectionStartIndex, long collectionCount, bool allowAtEnd) {
		if (allowAtEnd && index == collectionCount)
			return;
		CheckRange(index, 1, false, collectionStartIndex, collectionCount);
	}

#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CheckRange(long index, long count, bool rightMostAligned, long collectionStartIndex, long collectionCount) {
		ArgumentGTE(index, collectionStartIndex, nameof(index));
		ArgumentGTE(count, 0, nameof(count));
		if (rightMostAligned)
			ArgumentEquals(collectionCount - index, count, nameof(count), "Specified range must be aligned to right-most of collection");
		else
			ArgumentGTE(collectionCount - index, count, nameof(count), "Specified range is beyond the boundaries of the collection");
	}

#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FileExists(string path) {
		if (!File.Exists(path))
			throw new FileNotFoundException($"File not found: {path} ", path);
	}

#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FileNotExists(string path) {
		if (File.Exists(path))
			throw new FileAlreadyExistsException("File already exists", path);
	}

#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DirectoryExists(string path) {
		if (!Directory.Exists(path))
			throw new DirectoryNotFoundException($"Directory not found: '{path}'");
	}

#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DirectoryNotExists(string path) {
		if (Directory.Exists(path))
			throw new DirectoryNotFoundException($"Directory already exists: '{path}'");
	}

	/// <summary>
	/// Throws an exception if an argument is null
	/// </summary>
	/// <param name="value">The value to be tested</param>
	/// <param name="paramName">The name of the argument</param>
#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ArgumentNotNull(object value, string paramName) {
		if (value == null)
			throw new ArgumentNullException(paramName, "Argument must not be null");
	}

	/// <summary>
	/// Throws an exception if an argument does not parse as the given type.
	/// </summary>
	/// <param name="value">The value to be tested</param>
	/// <param name="name">The name of the argument</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ArgumentParse<T>(string value, string name, out T parsedValue) {
		if (!GenericParser.TryParse(value, out parsedValue))
			throw new ArgumentException($"Argument could not be parsed as {typeof(T).Name}", name);
	}

	/// <summary>
	/// Throws an exception if a string argument is null or empty
	/// </summary>
	/// <param name="value">The value to be tested</param>
	/// <param name="paramName">The name of the argument</param>
#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ArgumentNotNullOrEmpty(string value, string paramName, string message = null) {
		if (string.IsNullOrEmpty(value))
			throw new ArgumentException(message ?? $"Argument must not be the empty string", paramName);
	}

	/// <summary>
	/// Throws an exception if a string argument is null or empty
	/// </summary>
	/// <param name="value">The value to be tested</param>
	/// <param name="paramName">The name of the argument</param>
#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ArgumentNotNullOrWhitespace(string value, string paramName, string message = null) {
		if (string.IsNullOrWhiteSpace(value))
			throw new ArgumentException(message ?? $"Argument must not be the empty string", paramName);
	}

#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ArgumentNotNullOrEmpty<T>(IEnumerable<T> items, string paramName, string message = null) {
		ArgumentNotNull(items, paramName);
		if (!items.Any())
			throw new ArgumentException(message ?? $"Argument must not be the empty enumerable", paramName);
	}

	/// <summary>
	/// Throws an ArgumentOutOfRangeException if the specified condition is not met.
	/// </summary>
	/// <param name="value">The value of the argument</param>
	/// <param name="minInclusive">The minimum allowed value of the argument</param>
	/// <param name="maxInclusive">The maximum allowed value of the argument</param>
	/// <param name="paramName">The name of the argument</param>
#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ArgumentInRange(ulong value, ulong minInclusive, ulong maxInclusive, string paramName) {
		if (value < minInclusive || value > maxInclusive) {
			throw new ArgumentOutOfRangeException(paramName,
				value,
				$"Value should be in range [{minInclusive} - {maxInclusive}]");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	public static void ArgumentEquals(long value, long expected, string paramName, string message = null) {
		if (value != expected)
			throw new ArgumentOutOfRangeException(paramName, value, message ?? $"Value should be {expected}");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	public static void ArgumentNotNegative(long value, string paramName, string message = null) {
		if (value < 0)
			throw new ArgumentOutOfRangeException(paramName, value, message ?? $"Must not be negative (was {value})");
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	public static void ArgumentLT(long value, long operand, string paramName, string message = null) {
		if (value >= operand)
			throw new ArgumentOutOfRangeException(paramName, value, message ?? $"Must be less than {operand} but was {value}");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	public static void ArgumentLTE(long value, long operand, string paramName, string message = null) {
		if (value > operand)
			throw new ArgumentOutOfRangeException(paramName, value, message ?? $"Must be less than or equal to {operand} but was {value}");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	public static void ArgumentGT(long value, long operand, string paramName, string message = null) {
		if (value <= operand)
			throw new ArgumentOutOfRangeException(paramName, value, message ?? $"Must be greater than {operand} but was {value}");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	public static void ArgumentGTE(long value, long operand, string paramName, string message = null) {
		if (value < operand)
			throw new ArgumentOutOfRangeException(paramName, value, message ?? $"Must be greater than or equal to {operand} but was {value}");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	public static void ArgumentInRange(long value, long minInclusive, long maxInclusive, string paramName, string message = null) {
		if (value < minInclusive || value > maxInclusive)
			throw new ArgumentOutOfRangeException(paramName, value, message ?? $"Value should be in range [{minInclusive} - {maxInclusive}]");
	}

	/// <summary>
	/// Throws an ArgumentException if the specified condition is not met.
	/// </summary>
	/// <param name="condition">The condition that must be met</param>
	/// <param name="paramName">The name of the argument</param>
	/// <param name="message">The exception message to be used</param>
#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Argument(bool condition, string paramName, string message) {
		if (!condition)
			throw new ArgumentException(message, paramName);
	}

	/// <summary>
	/// Throws an ArgumentException if the specified condition is met.
	/// </summary>
	/// <param name="condition">The condition that must be met</param>
	/// <param name="paramName">The name of the argument</param>
	/// <param name="message">The exception message to be used</param>
#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ArgumentNot(bool condition, string paramName, string message) {
		if (condition)
			throw new ArgumentException(message, paramName);
	}

#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TType ArgumentCast<TType>(object @object, string paramName) {
		ArgumentCast<TType>(@object, out var result, paramName);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ArgumentCast<TType>(object @object, out TType castedObject, string paramName)
		=> ArgumentCast(@object, out castedObject, paramName, $"Cannot be cast to {typeof(TType)}");

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ArgumentCast<TType>(object @object, out TType castedObject, string paramName, string message) {
		if (!(@object is TType cobj))
			throw new ArgumentException(message, paramName);
		castedObject = cobj;
	}

#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ArgumentIsAssignable<TType>(object @object, string paramName) {
		Argument(@object.GetType().IsAssignableFrom(typeof(TType)), paramName, $"Not assignable from {typeof(TType).GetShortName()}");
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ArgumentNotThrows(Action action, string paramName, string message) {
		try {
			action();
		} catch {
			throw new ArgumentException(message, paramName);
		}
	}


#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Against(bool condition, string message = null)
		=> Ensure(!condition, message);


	/// <summary>
	/// Throws an InvalidOperationException if the specified condition is not met.
	/// </summary>
	/// <param name="condition">The condition that must be met</param>
	/// <param name="message">The exception message to be used</param>
#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Ensure(bool condition, string message = null) {
		if (!condition)
			throw new InvalidOperationException(message ?? "Internal error");
	}


#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TType EnsureCast<TType>(object @object, string message) {
		EnsureCast<TType>(@object, out var result, message);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void EnsureCast<TType>(object @object, out TType castedObject, string message) {
		Ensure(@object is TType, message);
		castedObject = (TType)@object;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void EnsureNotThrows(Action action, string message) {
		try {
			action();
		} catch {
			throw new InvalidOperationException(message);
		}
	}
}
