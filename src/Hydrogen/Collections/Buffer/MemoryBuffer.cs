// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hydrogen;

/// <summary>
/// A memory buffer whose entire contents reside in a contiguous block of memory. Operations are batch optimized thus suitable for image/data manipulations of small - mid size.
/// For larger buffers whose contents should be paged use <see cref="MemoryPagedBuffer"/>.
/// <remarks>
/// - Setting the initialCapacity accurately can yield 100x performance gains since resizing the array can be expensive.
/// - All enumerable results are implemented as arrays and arguments are not checked for performance.
/// </remarks>
/// </summary>
public sealed class MemoryBuffer : RangedListBase<byte>, IBuffer {
	public const long DefaultBlockGrowth = 65536;
	public const long DefaultMaxSize = long.MaxValue;
	private byte[] _internalArray;
	private long _length;
	private readonly long _initialCapacity;
	private readonly long _blockGrowthSize;
	private readonly long _maxCapacity;

	public MemoryBuffer() : this(0, DefaultBlockGrowth) {
	}

	public MemoryBuffer(long initialCapacity, long blockGrowthSize)
		: this(initialCapacity, blockGrowthSize, DefaultMaxSize) {
	}

	public MemoryBuffer(long initialCapacity, long blockGrowthSize, long maxCapacity)
		: this(new byte[initialCapacity], 0, blockGrowthSize, maxCapacity) {
	}

	public MemoryBuffer(byte[] fixedArray)
		: this(fixedArray, fixedArray.Length, 0, fixedArray.Length) {
	}

	private MemoryBuffer(byte[] internalArray, long currentSize, long capacityGrowthSize, long maxCapacity) {
		_internalArray = internalArray;
		_initialCapacity = internalArray.Length;
		_length = currentSize;
		_blockGrowthSize = capacityGrowthSize;
		_maxCapacity = maxCapacity;
	}

	public long CapacityGrowthSize => _blockGrowthSize;

	public long MaxCapacity => _maxCapacity;

	public override long Count => _length;

	public override IEnumerable<long> IndexOfRange(IEnumerable<byte> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		var results = new List<long>();
		var itemsArr = items as byte[] ?? items.ToArray();
		for (var i = 0L; i < _internalArray.Length; i++) {
			var arrayByte = _internalArray[i];
			foreach (var compareByte in itemsArr) {
				if (compareByte == arrayByte) {
					results.Add(i);
					break;
				}
			}
		}
		return results.ToArray();
	}

	public override IEnumerable<byte> ReadRange(long index, long count) {
		CheckRange(index, count);
		var indexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(index);
		// optimize for single read
		if (count == 1)
			return new[] { _internalArray[index] };

		var endIndex = Math.Min(index + count - 1, _length - 1);
		var readCount = endIndex - index + 1;
		if (readCount <= 0)
			return Array.Empty<byte>();
		var readCountI = Tools.Collection.CheckNotImplemented64bitAddressingLength(readCount);
		var readResult = new byte[readCount];
		Buffer.BlockCopy(_internalArray, indexI, readResult, 0, readCountI);
		return readResult;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<byte> ReadSpan(long index, long count) {
		return AsSpan(index, count);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void AddRange(IEnumerable<byte> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		AddRange(items as byte[] ?? items.ToArray());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddRange(ReadOnlySpan<byte> items) {
		Write(_length, items);
	}

	public void Write(long index, ReadOnlySpan<byte> items) {
		var newBytesCount = Math.Max(index + items.Length, _length) - _length;

		GrowSpaceIfRequired(newBytesCount);
		UpdateVersion();

		if (items.Length == 1)
			_internalArray[index] = items[0];
		else {
			var indexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(index);
			items.CopyTo(_internalArray.AsSpan(indexI, items.Length));
		}

		_length += newBytesCount;
	}

	public void ExpandTo(long totalBytes) {
		Guard.ArgumentInRange(totalBytes, _length, MaxCapacity, nameof(totalBytes), "Allocated space would either not contain existing items or exceed max capacity");
		var newBytes = totalBytes - _internalArray.Length;
		if (newBytes > 0)
			ExpandBy(newBytes);
	}

	public void ExpandBy(long newBytes) {
		Guard.ArgumentInRange(newBytes, 0, long.MaxValue, nameof(newBytes));
		GrowSpaceIfRequired(newBytes);
		_length += newBytes;
	}

	public override void UpdateRange(long index, IEnumerable<byte> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		UpdateRange(index, items as byte[] ?? items.ToArray());
	}

	public void UpdateRange(long index, ReadOnlySpan<byte> items) {
		CheckRange(index, items.Length);
		var indexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(index);
		UpdateVersion();
		items.CopyTo(_internalArray.AsSpan(indexI, items.Length));
	}

	public override void InsertRange(long index, IEnumerable<byte> items) {
		InsertRange(index, items as byte[] ?? items.ToArray());
	}

	public void InsertRange(long index, ReadOnlySpan<byte> items) {
		CheckIndex(index, allowAtEnd: true);
		var indexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(index);
		GrowSpaceIfRequired(items.Length);
		UpdateVersion();
		var capacity = _internalArray.Length - _length;
		if (items.Length > capacity)
			throw new ArgumentException("Insufficient capacity", nameof(items));

		// Use Array.Copy to move original items, since handles overlap scenarios
		Array.Copy(_internalArray, index, _internalArray, index + items.Length, _length - index);
		items.CopyTo(_internalArray.AsSpan(indexI, items.Length));
		_length += items.Length;
	}

	public override void RemoveRange(long index, long count) {
		CheckRange(index, count);
		var indexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(index);
		var countI = Tools.Collection.CheckNotImplemented64bitAddressingLength(count);
		UpdateVersion();

		// Use Array.Copy to move original items, since handles overlap scenarios
		Array.Copy(_internalArray, indexI + countI, _internalArray, index, (_length - (index + count)));
		_length -= count;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<byte> AsSpan() => AsSpan(0);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<byte> AsSpan(long index) {
		var indexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(index);
		var lengthI = Tools.Collection.CheckNotImplemented64bitAddressingLength(_length - index);
		return AsSpan(indexI, lengthI);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<byte> AsSpan(long index, long count) {
		if (index == _length && count == 0)
			return Span<byte>.Empty;

		var indexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(index);
		var countI = Tools.Collection.CheckNotImplemented64bitAddressingLength(count);
		return _internalArray.AsSpan(indexI, countI);
	}

	public override void Clear() {
		var initialCapacityI = Tools.Collection.CheckNotImplemented64bitAddressingLength(_initialCapacity);
		Array.Resize(ref _internalArray, initialCapacityI);
		_length = 0;
	}

	private void GrowSpaceIfRequired(long newBytes) {
		var newCapacity = _length + newBytes;

		// check if don't need to grow capacity
		if (newCapacity <= _internalArray.Length)
			return;

		// check if growth exceeds max
		if (newCapacity > _maxCapacity || _blockGrowthSize == 0)
			throw new ArgumentException("Insufficient capacity");

		// determine how many new bytes are actually needed
		var actualNewBytes = newBytes - (_internalArray.Length - _length);
		Debug.Assert(actualNewBytes > 0);

		// calc the new capacity (grows in blocks)
		var remainingGrowthCapacity = _maxCapacity - _internalArray.Length;
		var growAmount = (int)Math.Min(Math.Ceiling(actualNewBytes / (double)_blockGrowthSize) * _blockGrowthSize, remainingGrowthCapacity);
		Debug.Assert(_internalArray.Length + growAmount <= _maxCapacity);

		// Re-allocate internal array
		Array.Resize(ref _internalArray, _internalArray.Length + growAmount);
	}

}
