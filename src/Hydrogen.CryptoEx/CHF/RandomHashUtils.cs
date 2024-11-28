// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Hydrogen.CryptoEx;

public static class RandomHashUtils {

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint GetLastDWordLE(byte[] chunk) {
		var chunkLength = chunk.Length;

		if (chunkLength < 4)
			throw new ArgumentException($"{nameof(chunk)} needs to be at least 4 bytes");

		// Last 4 bytes are nonce (LE)
		return (uint)(chunk[chunkLength - 4] |
		              (chunk[chunkLength - 3] << 8) |
		              (chunk[chunkLength - 2] << 16) |
		              (chunk[chunkLength - 1] << 24));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint GetDWordLE(byte[] chunk, int offset) {
		var chunkLength = chunk.Length;
		if (chunkLength < offset + 3)
			throw new ArgumentException($"{nameof(chunk)}[{nameof(offset)}] needs at least 4 more bytes");

		// Last 4 bytes are nonce (LE)
		return (uint)(chunk[offset + 0] |
		              (chunk[offset + 1] << 8) |
		              (chunk[offset + 2] << 16) |
		              (chunk[offset + 3] << 24));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] SetLastDWordLE(byte[] chunk, uint value) {
		// Clone the original header
		var result = Clone(chunk);

		// If digest not big enough to contain a nonce, just return the clone
		var chunkLength = chunk.Length;
		if (chunkLength < 4)
			return result;

		// Overwrite the nonce in little-endian
		result[chunkLength - 4] = (byte)value;
		result[chunkLength - 3] = (byte)((value >> 8) & 255);
		result[chunkLength - 2] = (byte)((value >> 16) & 255);
		result[chunkLength - 1] = (byte)((value >> 24) & 255);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte RotateRight8(byte value, int distance) {
		Debug.Assert(distance >= 0);
		distance &= 7;
		return (byte)((value >> distance) | (value << (8 - distance)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte RotateLeft8(byte value, int distance) {
		Debug.Assert(distance >= 0);
		distance &= 7;
		return (byte)((value << distance) | (value >> (8 - distance)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] Clone<T>(T[] buffer) => (T[])buffer?.Clone();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] Concatenate<T>(T[] left, T[] right) {
		if (left == null)
			return Clone(right);
		if (right == null)
			return Clone(left);

		var result = new T[left.Length + right.Length];
		Array.Copy(left, 0, result, 0, left.Length);
		Array.Copy(right, 0, result, left.Length, right.Length);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool BytesEqual(byte[] left, byte[] right) => BytesEqual(left, right, 0, (uint)left.Length);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool BytesEqual(byte[] left, byte[] right, uint from, uint length) {
		if (length == 0)
			return false;
		var leftLength = left.Length;
		var rightLength = right.Length;
		if (leftLength - from < length || rightLength - from < length)
			return false;
		for (var idx = (int)from; idx <= length; idx++) {
			if (left[idx] != right[idx])
				return false;
		}

		return true;
	}
}
