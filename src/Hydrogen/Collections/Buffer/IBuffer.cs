// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public interface IBuffer : IExtendedList<byte> {

	ReadOnlySpan<byte> ReadSpan(int index, int count);

	void AddRange(ReadOnlySpan<byte> span);

	void UpdateRange(int index, ReadOnlySpan<byte> items);

	void InsertRange(int index, ReadOnlySpan<byte> items);

	Span<byte> AsSpan(int index, int count);

	void ExpandTo(int totalBytes);

	void ExpandBy(int newBytes);
}


public static class IBufferExtensions {

	public static ReadOnlySpan<byte> ReadSpan(this IBuffer memoryBuffer, Range range) {
		var (start, count) = range.GetOffsetAndLength(memoryBuffer.Count);
		return memoryBuffer.ReadSpan(start, count);
	}


	public static Span<byte> AsSpan(this IBuffer memoryBuffer, Range range) {
		var (start, count) = range.GetOffsetAndLength(memoryBuffer.Count);
		return memoryBuffer.AsSpan(start, count);
	}

	public static Span<byte> AsSpan(this IBuffer memoryBuffer, Index startIndex)
		=> memoryBuffer.AsSpan(Range.StartAt(startIndex));
}
