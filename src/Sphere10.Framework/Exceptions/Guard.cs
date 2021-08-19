using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sphere10.Framework {

	/// <summary>
	/// Class used to guard against unexpected argument values
	/// or operations by throwing an appropriate exception.
	/// </summary>
	public static class Guard {

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void FileExists(string path) {
			if (!File.Exists(path))
				throw new FileNotFoundException("File not found", path);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void FileNotExists(string path) {
			if (!File.Exists(path))
				throw new FileAlreadyExistsException("File already exists", path);
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DirectoryExists(string path) {
			if (!Directory.Exists(path))
				throw new DirectoryNotFoundException($"Directory not found: '{path}'");
		}

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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentNotNullOrWhitespace(string value, string paramName, string message = null) {
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException(message ?? $"Argument must not be the empty string", paramName);
		}

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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentInRange(long value, long minInclusive, long maxInclusive, string paramName) {
			if (value < minInclusive || value > maxInclusive) {
				throw new ArgumentOutOfRangeException(paramName, value,
					$"Value should be in range [{minInclusive} - {maxInclusive}]");
			}
		}

		/// <summary>
		/// Throws an ArgumentOutOfRangeException if the specified condition is not met.
		/// </summary>
		/// <param name="value">The value of the argument</param>
		/// <param name="minInclusive">The minimum allowed value of the argument</param>
		/// <param name="maxInclusive">The maximum allowed value of the argument</param>
		/// <param name="paramName">The name of the argument</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentInRange(ulong value, ulong minInclusive, ulong maxInclusive, string paramName) {
			if (value < minInclusive || value > maxInclusive) {
				throw new ArgumentOutOfRangeException(paramName, value,
					$"Value should be in range [{minInclusive} - {maxInclusive}]");
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentEquals(long value, long expected, string paramName) {
			if (value != expected) {
				throw new ArgumentOutOfRangeException(paramName, value,
					$"Value should be {expected}");
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentInRange(int value, int minInclusive, int maxInclusive, string paramName) {
			ArgumentInRange(value, minInclusive, maxInclusive, paramName, $"Value should be in range [{minInclusive} - {maxInclusive}]");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentInRange(int value, int minInclusive, int maxInclusive, string paramName, string message) {
			if (value < minInclusive || value > maxInclusive) {
				throw new ArgumentOutOfRangeException(paramName, value, message);
			}
		}

		/// <summary>
		/// Throws an ArgumentException if the specified condition is not met.
		/// </summary>
		/// <param name="condition">The condition that must be met</param>
		/// <param name="paramName">The name of the argument</param>
		/// <param name="message">The exception message to be used</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Argument(bool condition, string paramName, string message) {
            if (!condition)
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentIsAssignable<TType>(object @object, string parameter) {
			Argument(@object.GetType().IsAssignableFrom(typeof(TType)), parameter, $"Not assignable from {typeof(TType).GetShortName()}");
		}

		/// <summary>
		/// Throws an InvalidOperationException if the specified condition is not met.
		/// </summary>
		/// <param name="condition">The condition that must be met</param>
		/// <param name="message">The exception message to be used</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Ensure(bool condition, string message = null) {
            if (!condition)
                throw new InternalErrorException(message);
        }

    }
}
