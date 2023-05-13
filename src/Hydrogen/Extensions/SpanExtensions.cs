// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

using System.Runtime.CompilerServices;

namespace Hydrogen {

	public static class SpanExtensions {

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T> Slice<T>(this ReadOnlySpan<T> span, Index index) 
			=> span.Slice(index.GetOffset(span.Length));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T> Slice<T>(this ReadOnlySpan<T> span, Range range) {
			var (offset, length) = range.GetOffsetAndLength(span.Length);
			return span.Slice(offset, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> Slice<T>(this Span<T> span, Index index)
			=> span.Slice(index.GetOffset(span.Length));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> Slice<T>(this Span<T> span, Range range) {
			var (offset, length) = range.GetOffsetAndLength(span.Length);
			return span.Slice(offset, length);
		}
	}
}
