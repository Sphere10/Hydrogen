//-----------------------------------------------------------------------
// <copyright file="SpanExtensions.cs" company="Sphere 10 Software">
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

using System.Runtime.CompilerServices;

namespace Sphere10.Framework {

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
