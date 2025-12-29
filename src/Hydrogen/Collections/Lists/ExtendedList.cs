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

namespace Hydrogen;

/// <summary>
/// Resizable list backed by a contiguous array with long indexing support.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
/// <remarks>
/// Used widely in tests for capacity growth and range operations.
/// See tests/Hydrogen.Tests/Collections/Lists/ExtendedListTests.cs.
/// </remarks>
public class ExtendedList<T> : RangedListBase<T> {
	private const long DefaultCapacityGrowthSize = 4096;
	private T[] _internalArray;
	private long _length;
	private readonly IEqualityComparer<T> _comparer;


	/// <summary>
	/// Initializes an empty list with default growth settings.
	/// </summary>
	/// <param name="comparer">The comparer to use for item equality.</param>
	public ExtendedList(IEqualityComparer<T> comparer = null)
		: this(DefaultCapacityGrowthSize, comparer) {
	}

	/// <summary>
	/// Initializes an empty list with an initial capacity.
	/// </summary>
	/// <param name="capacity">The initial capacity.</param>
	/// <param name="comparer">The comparer to use for item equality.</param>
	public ExtendedList(long capacity, IEqualityComparer<T> comparer = null)
		: this(capacity, long.MaxValue, comparer) {
	}

	/// <summary>
	/// Initializes an empty list with a capacity and a maximum capacity.
	/// </summary>
	/// <param name="capacity">The initial capacity.</param>
	/// <param name="maxCapacity">The maximum capacity.</param>
	/// <param name="comparer">The comparer to use for item equality.</param>
	public ExtendedList(long capacity, long maxCapacity, IEqualityComparer<T> comparer = null)
		: this(capacity, DefaultCapacityGrowthSize, maxCapacity,	comparer) {
	}

	/// <summary>
	/// Initializes an empty list with capacity, growth size, and maximum capacity.
	/// </summary>
	/// <param name="capacity">The initial capacity.</param>
	/// <param name="capacityGrowthSize">The growth size for capacity increments.</param>
	/// <param name="maxCapacity">The maximum capacity.</param>
	/// <param name="comparer">The comparer to use for item equality.</param>
	public ExtendedList(long capacity, long capacityGrowthSize, long maxCapacity, IEqualityComparer<T> comparer = null)
		: this(new T[capacity], 0, capacityGrowthSize, maxCapacity, comparer) {
	}

	/// <summary>
	/// Initializes the list from an existing array.
	/// </summary>
	/// <param name="sourceArray">The source array.</param>
	/// <param name="comparer">The comparer to use for item equality.</param>
	public ExtendedList(T[] sourceArray, IEqualityComparer<T> comparer = null)
		: this(sourceArray, sourceArray.LongLength, 0L, sourceArray.LongLength, comparer) {
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

	/// <summary>
	/// Gets the growth increment used when resizing.
	/// </summary>
	public long CapacityGrowthSize { get; }

	/// <summary>
	/// Gets the maximum capacity allowed for this list.
	/// </summary>
	public long MaxCapacity { get; }

	/// <summary>
	/// Gets the number of elements in the list.
	/// </summary>
	public override long Count => _length;

	/// <summary>
	/// Always false for this mutable list implementation.
	/// </summary>
	public override bool IsReadOnly => false;

	/// <summary>
	/// Returns the index of each item, or -1 if not found.
	/// </summary>
	/// <param name="items">The items to locate.</param>
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

	/// <summary>
	/// Reads a contiguous range of items from the list.
	/// </summary>
	/// <param name="index">The starting index.</param>
	/// <param name="count">The number of items to read.</param>
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

	/// <summary>
	/// Appends a sequence of items to the list.
	/// </summary>
	/// <param name="items">The items to add.</param>
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

	/// <summary>
	/// Replaces a range of items starting at the specified index.
	/// </summary>
	/// <param name="index">The starting index.</param>
	/// <param name="items">The items to write.</param>
	public override void UpdateRange(long index, IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		var itemsArr = items as T[] ?? items.ToArray();
		CheckRange(index, itemsArr.LongLength);

		if (itemsArr.LongLength == 0)
			return;

		UpdateVersion();
		Array.Copy(itemsArr, 0, _internalArray, index, itemsArr.LongLength);
	}

	/// <summary>
	/// Inserts a range of items at the specified index.
	/// </summary>
	/// <param name="index">The insert index.</param>
	/// <param name="items">The items to insert.</param>
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

	/// <summary>
	/// Removes a range of items starting at the specified index.
	/// </summary>
	/// <param name="index">The starting index.</param>
	/// <param name="count">The number of items to remove.</param>
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
