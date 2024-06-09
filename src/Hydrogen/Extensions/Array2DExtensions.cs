// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hydrogen;

public static class Array2DExtensions {

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> GetRow<T>(this T[,] array2D, int row)
		=> GetRow(array2D, row, 0, array2D.GetLength(1));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> GetRow<T>(this T[,] array2D, int row, int rowOffset, int length)
		=> MemoryMarshal.CreateSpan(ref array2D[row, rowOffset], length);

	public static IEnumerable<T[]> GetRows<T>(this T[,] array2d) {
		for (var i = 0; i < array2d.GetLength(0); i++) {
			yield return GetRow(array2d, i).ToArray();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetRow<T>(this T[,] array2D, int row, ReadOnlySpan<T> values)
		=> SetRow(array2D, row, 0, values);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SetRow<T>(this T[,] array2D, int row, int rowOffset, ReadOnlySpan<T> values)
		=> values.CopyTo(array2D.GetRow(row, rowOffset, values.Length));

	public static bool SequenceEqual<T>(this T[,] array2D, T[,] other) where T : IEquatable<T> {
		if (ReferenceEquals(array2D, other))
			return true;

		if (array2D.Length != other.Length)
			return false;

		if (array2D.GetLength(0) != other.GetLength(0) ||
		    array2D.GetLength(1) != other.GetLength(1))
			return false;

		for (var row = 0; row < array2D.GetLength(0); row++) {
			if (!array2D.GetRow(row).SequenceEqual(other.GetRow(row)))
				return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> AsFlatSpan<T>(this T[,] array2D)
		=> MemoryMarshal.CreateSpan(ref array2D[0, 0], array2D.Length);


	public static T[] ToFlatArray<T>(this T[,] array2D) {
		var dest = new T[array2D.Length];
		var destOffset = 0;
		for (var i = 0; i < array2D.GetLength(0); i++) {
			var row = array2D.GetRow(i);
			row.CopyTo(dest.AsSpan(destOffset));
			destOffset += row.Length;
		}
		return dest;
	}

}
