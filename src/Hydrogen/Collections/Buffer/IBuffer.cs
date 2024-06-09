// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public interface IBuffer : IExtendedList<byte> {

	ReadOnlySpan<byte> ReadSpan(long index, long count);

	void AddRange(ReadOnlySpan<byte> span);

	void UpdateRange(long index, ReadOnlySpan<byte> items);

	void InsertRange(long index, ReadOnlySpan<byte> items);

	Span<byte> AsSpan(long index, long count);

	void ExpandTo(long totalBytes);

	void ExpandBy(long newBytes);
}


public static class IBufferExtensions {

	public static ReadOnlySpan<byte> ReadSpan(this IBuffer memoryBuffer, Range range) {
		var countI = Tools.Collection.CheckNotImplemented64bitAddressingLength(memoryBuffer.Count);
		var (start, count) = range.GetOffsetAndLength(countI);
		return memoryBuffer.ReadSpan(start, count);
	}

	public static Span<byte> AsSpan(this IBuffer memoryBuffer, Range range) {
		var countI = Tools.Collection.CheckNotImplemented64bitAddressingLength(memoryBuffer.Count);
		var (start, count) = range.GetOffsetAndLength(countI);
		return memoryBuffer.AsSpan(start, count);
	}

	public static Span<byte> AsSpan(this IBuffer memoryBuffer, Index startIndex)
		=> memoryBuffer.AsSpan(Range.StartAt(startIndex));
}
