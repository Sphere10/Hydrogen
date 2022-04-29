//#define OMIT_GUARD

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;


namespace Hydrogen {

	/// <summary>
	/// Class used to guard against unexpected argument values
	/// or operations by throwing an appropriate exception.
	/// </summary>
	public static class Guard {

#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CheckIndex(int index, int collectionStartIndex, int collectionCount, bool allowAtEnd) {
			if (allowAtEnd && index == collectionCount)
				return;
			CheckRange(index, 1, false, collectionStartIndex, collectionCount);
		}

#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CheckRange(int index, int count, bool rightMostAligned, int collectionStartIndex, int collectionCount) {
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
				throw new FileNotFoundException("File not found", path);
		}

#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void FileNotExists(string path) {
			if (!File.Exists(path))
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
		/// <param name="name">The name of the argument</param>
#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentNotNull(object value, string name) {
            if (value == null)
                throw new ArgumentNullException(name, "Argument must not be null");
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
				throw new ArgumentException( message ?? $"Argument must not be the empty enumerable", paramName);
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
				throw new ArgumentOutOfRangeException(paramName, value,
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TType ArgumentCast<TType>(object @object, string parameter) {
			ArgumentCast<TType>(@object, out var result, parameter);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentCast<TType>(object @object, out TType castedObject, string parameter)
			=> ArgumentCast(@object, out castedObject, parameter, $"Cannot be cast to {typeof(TType)}");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentCast<TType>(object @object, out TType castedObject, string parameter, string message) {
			if (!(@object is TType cobj))
				throw new ArgumentException(message, parameter);
			castedObject = cobj;
		}

#if OMIT_GUARD
		[Conditional("DEBUG")]
#endif
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentIsAssignable<TType>(object @object, string parameter) {
			Argument(@object.GetType().IsAssignableFrom(typeof(TType)), parameter, $"Not assignable from {typeof(TType).GetShortName()}");
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

	}
}
