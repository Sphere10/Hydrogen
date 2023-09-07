// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// A generic ExtendedList implementation that uses an underlying array for streams.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ExtendedList<T> : RangedListBase<T> {
	private T[] _internalArray;
	private long _length;
	private readonly IEqualityComparer<T> _comparer;

	public ExtendedList(IEqualityComparer<T> comparer = null)
		: this(4096, comparer) {
	}

	public ExtendedList(long capacity, IEqualityComparer<T> comparer = null)
		: this(capacity, int.MaxValue, comparer) {
	}

	public ExtendedList(long capacity, long maxCapacity, IEqualityComparer<T> comparer = null)
		: this(capacity, capacity, maxCapacity,	comparer) {
	}

	public ExtendedList(long capacity, long capacityGrowthSize, long maxCapacity, IEqualityComparer<T> comparer = null)
		: this(new T[capacity], 0, capacityGrowthSize, maxCapacity, comparer) {
	}

	public ExtendedList(T[] sourceArray, IEqualityComparer<T> comparer = null) {
		Guard.ArgumentNotNull(sourceArray, nameof(sourceArray));
		_internalArray = sourceArray;
		_comparer = comparer ?? EqualityComparer<T>.Default;
	}

	private ExtendedList(T[] internalArray, long currentLogicalSize, long capacityGrowthSize, long maxCapacity, IEqualityComparer<T> comparer = null) {
		Guard.ArgumentNotNull(internalArray, nameof(internalArray));
		Guard.ArgumentInRange(capacityGrowthSize, 0, long.MaxValue, nameof(capacityGrowthSize));
		Guard.ArgumentInRange(maxCapacity, currentLogicalSize, long.MaxValue, nameof(maxCapacity));
		Guard.ArgumentInRange(currentLogicalSize, 0, internalArray.LongLength, nameof(currentLogicalSize));

		_internalArray = internalArray;
		_length = currentLogicalSize;
		CapacityGrowthSize = capacityGrowthSize;
		MaxCapacity = maxCapacity;
		_comparer = comparer ?? EqualityComparer<T>.Default;
	}

	public long CapacityGrowthSize { get; }

	public long MaxCapacity { get; }

	public override long Count => _length;

	public override bool IsReadOnly => false;

	public override IEnumerable<long> IndexOfRange(IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		var itemsArr = items as T[] ?? items.ToArray();
		foreach (var item in itemsArr) {
			var itemIndex = -1;
			for (var j = 0; j < _length; j++) {
				if (_comparer.Equals(item, _internalArray[j])) {
					itemIndex = j;
					break;
				}
			}
			yield return itemIndex;
		}
	}

	public override IEnumerable<T> ReadRange(long index, long count) {
		CheckRange(index, count);

		// optimize for single read
		if (count == 1)
			return new[] { _internalArray[index] };

		var endIndex = Math.Min(index + count - 1, _length - 1);
		var readCount = endIndex - index + 1;
		if (readCount <= 0)
			return Array.Empty<T>();
		var readResult = new T[readCount];
		Array.Copy(_internalArray, index, readResult, 0, readCount);
		return readResult;
	}

	public override void AddRange(IEnumerable<T> items) {
		// TODO: needs updating to support multiple _internalArray's and 2^64-1 addressable items
		Guard.ArgumentNotNull(items, nameof(items));
		var itemsArr = items as T[] ?? items.ToArray();

		if (itemsArr.LongLength == 0)
			return;

		GrowSpaceIfRequired(itemsArr.LongLength);
		UpdateVersion();
		if (itemsArr.LongLength == 1)
			// single access optimization
			_internalArray[_length] = itemsArr[0];
		else
			Array.Copy(itemsArr, 0, _internalArray, _length, itemsArr.LongLength);
		_length += itemsArr.LongLength;
	}

	public override void UpdateRange(long index, IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		var itemsArr = items as T[] ?? items.ToArray();
		CheckRange(index, itemsArr.LongLength);

		if (itemsArr.LongLength == 0)
			return;

		UpdateVersion();
		Array.Copy(itemsArr, 0, _internalArray, index, itemsArr.LongLength);
	}

	public override void InsertRange(long index, IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		Guard.ArgumentInRange(index, 0, Math.Max(0, _length), nameof(index));

		var itemsArr = items as T[] ?? items.ToArray();
		GrowSpaceIfRequired(itemsArr.LongLength);
		UpdateVersion();

		var capacity = _internalArray.LongLength - _length;
		if (itemsArr.LongLength > capacity)
			throw new ArgumentException("Insufficient capacity", nameof(items));

		// Use Array.Copy to move original items, since handles overlap scenarios
		Array.Copy(_internalArray, index, _internalArray, index + itemsArr.LongLength, _length - index);
		Array.Copy(itemsArr, 0, _internalArray, index, itemsArr.LongLength);
		_length += itemsArr.LongLength;
	}

	public override void RemoveRange(long index, long count) {
		CheckRange(index, count);

		if (count == 0)
			return;

		// Use Array.Copy to move original items, since handles overlap scenarios
		Array.Copy(_internalArray, index + count, _internalArray, index, (_length - (index + count)));
		_length -= count;
		UpdateVersion();
	}

	private void GrowSpaceIfRequired(long newItems) {
		var newCapacity = _length + newItems;

		// check if don't need to grow capacity
		if (newCapacity <= _internalArray.LongLength)
			return;

		// check if growth exceeds max
		if (newCapacity > MaxCapacity || CapacityGrowthSize == 0)
			throw new ArgumentException("Insufficient capacity");

		// determine how many new bytes are actually needed
		var actualNewBytes = newItems - (_internalArray.LongLength - _length);
		Debug.Assert(actualNewBytes > 0);

		// calc the new capacity (grows in blocks)
		var remainingGrowthCapacity = MaxCapacity - _internalArray.LongLength;
		var growAmount = (int)Math.Min(Math.Ceiling(actualNewBytes / (double)CapacityGrowthSize) * CapacityGrowthSize, remainingGrowthCapacity);
		Debug.Assert(_internalArray.LongLength + growAmount <= MaxCapacity);

		// Resize store
		Tools.Collection.ResizeArray(ref _internalArray, _internalArray.LongLength + growAmount);
	}

}
