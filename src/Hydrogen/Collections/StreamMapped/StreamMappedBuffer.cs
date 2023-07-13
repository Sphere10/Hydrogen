// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hydrogen.Collections;

/// <summary>
/// A buffer whose contents are mapped onto a stream.
/// </summary>
/// <remarks>The following operations are not supported:
///  - <see cref="AsSpan"/>
///  - <see cref="IndexOfRange"/>
/// </remarks>
public class StreamMappedBuffer : RangedListBase<byte>, IBuffer {
	public const int DefaultBlockSize = 1 << 18; // 256 kb
	private readonly Stream _stream;
	private readonly int _blockSize;

	public StreamMappedBuffer(Stream stream)
		: this(stream, DefaultBlockSize) {
	}

	public StreamMappedBuffer(Stream stream, int blockSize) {
		Guard.ArgumentNotNull(stream, nameof(stream));
		Guard.ArgumentLTE(stream.Length, int.MaxValue, nameof(stream.Length));
		Guard.ArgumentGT(blockSize, 0, nameof(blockSize));
		_stream = stream;
		_blockSize = blockSize;
	}

	public override int Count => (int)_stream.Length;

	public override void AddRange(IEnumerable<byte> items)
		=> AddRange(items as byte[] ?? items?.ToArray() ?? throw new ArgumentNullException(nameof(items)));

	public void AddRange(ReadOnlySpan<byte> span)
		=> InsertRange(Count, span);

	public override IEnumerable<int> IndexOfRange(IEnumerable<byte> items)
		=> throw new NotSupportedException();

	public override IEnumerable<byte> ReadRange(int index, int count)
		=> ReadArray(index, count).ToArray();

	public ReadOnlySpan<byte> ReadSpan(int index, int count)
		=> ReadArray(index, count);

	public byte[] ReadArray(int index, int count) {
		CheckRange(index, count);
		_stream.Seek(index, SeekOrigin.Begin);
		return _stream.ReadBytes(count);
	}

	public override void UpdateRange(int index, IEnumerable<byte> items)
		=> UpdateRange(index, items as byte[] ?? items?.ToArray() ?? throw new ArgumentNullException(nameof(items)));

	public void UpdateRange(int index, ReadOnlySpan<byte> items) {
		CheckRange(index, items.Length);
		_stream.Seek(index, SeekOrigin.Begin);
		_stream.Write(items);
	}

	public override void InsertRange(int index, IEnumerable<byte> items)
		=> InsertRange(index, items as byte[] ?? items?.ToArray() ?? throw new ArgumentNullException(nameof(items)));

	public void InsertRange(int index, ReadOnlySpan<byte> items) {
		CheckIndex(index, true);
		var originalCount = Count;
		ExpandBy(items.Length);
		Tools.Streams.ShiftBytes(_stream, index, originalCount - index, index + items.Length, _blockSize);
		_stream.Seek(index, SeekOrigin.Begin);
		_stream.Write(items);
	}

	public override void RemoveRange(int index, int count) {
		CheckRange(index, count);
		var fromIndex = index + count;
		var shiftAmount = Count - fromIndex;
		Tools.Streams.ShiftBytes(_stream, fromIndex, shiftAmount, index, _blockSize);
		_stream.SetLength(Count - count);
	}

	public Span<byte> AsSpan(int index, int count) => throw new NotSupportedException();

	public void ExpandTo(int totalBytes) {
		Guard.ArgumentLTE(totalBytes, int.MaxValue, nameof(totalBytes));
		_stream.SetLength(totalBytes);
	}

	public void ExpandBy(int newBytes) => ExpandTo(Count + newBytes);


}
