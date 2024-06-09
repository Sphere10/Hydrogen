// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hydrogen.Collections;

/// <summary>
/// A buffer whose contents are mapped onto a stream.
/// </summary>
/// <remarks>The following operations are not supported:
///  - <see cref="AsSpan"/>
///  - <see cref="IndexOfRange"/>
/// </remarks>
public class StreamMappedBuffer : RangedListBase<byte>, IBuffer {
	private readonly Stream _stream;
	private readonly int _streamOperationBlockSize;

	public StreamMappedBuffer(Stream stream)
		: this(stream, HydrogenDefaults.DefaultBufferOperationBlockSize) {
	}

	public StreamMappedBuffer(Stream stream, int streamOperationBlockSize) {
		Guard.ArgumentNotNull(stream, nameof(stream));
		Guard.ArgumentLTE(stream.Length, int.MaxValue, nameof(stream.Length));
		Guard.ArgumentGT(streamOperationBlockSize, 0, nameof(streamOperationBlockSize));
		_stream = stream;
		_streamOperationBlockSize = streamOperationBlockSize;
	}

	public override long Count => (int)_stream.Length;

	public override void AddRange(IEnumerable<byte> items)
		=> AddRange(items as byte[] ?? items?.ToArray() ?? throw new ArgumentNullException(nameof(items)));

	public void AddRange(ReadOnlySpan<byte> span)
		=> InsertRange(Count, span);

	public override IEnumerable<long> IndexOfRange(IEnumerable<byte> items)
		=> throw new NotSupportedException();

	public override IEnumerable<byte> ReadRange(long index, long count)
		=> ReadArray(index, count).ToArray();

	public ReadOnlySpan<byte> ReadSpan(long index, long count)
		=> ReadArray(index, count);

	public byte[] ReadArray(long index, long count) {
		CheckRange(index, count);
		_stream.Seek(index, SeekOrigin.Begin);
		return _stream.ReadBytes(count);
	}

	public override void UpdateRange(long index, IEnumerable<byte> items)
		=> UpdateRange(index, items as byte[] ?? items?.ToArray() ?? throw new ArgumentNullException(nameof(items)));

	public void UpdateRange(long index, ReadOnlySpan<byte> items) {
		CheckRange(index, items.Length);
		_stream.Seek(index, SeekOrigin.Begin);
		_stream.Write(items);
	}

	public override void InsertRange(long index, IEnumerable<byte> items)
		=> InsertRange(index, items as byte[] ?? items?.ToArray() ?? throw new ArgumentNullException(nameof(items)));

	public void InsertRange(long index, ReadOnlySpan<byte> items) {
		CheckIndex(index, true);
		var originalCount = Count;
		ExpandBy(items.Length);
		Tools.Streams.ShiftBytes(_stream, index, originalCount - index, index + items.Length, _streamOperationBlockSize);
		_stream.Seek(index, SeekOrigin.Begin);
		_stream.Write(items);
	}

	public override void RemoveRange(long index, long count) {
		CheckRange(index, count);
		var fromIndex = index + count;
		var shiftAmount = Count - fromIndex;
		Tools.Streams.ShiftBytes(_stream, fromIndex, shiftAmount, index, _streamOperationBlockSize);
		_stream.SetLength(Count - count);
	}

	public Span<byte> AsSpan(long index, long count) => throw new NotSupportedException();

	public void ExpandTo(long totalBytes) {
		Guard.ArgumentLTE(totalBytes, int.MaxValue, nameof(totalBytes));
		_stream.SetLength(totalBytes);
	}

	public void ExpandBy(long newBytes) => ExpandTo(Count + newBytes);


}
