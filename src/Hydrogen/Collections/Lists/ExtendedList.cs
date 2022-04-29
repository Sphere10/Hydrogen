using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// A generic ExtendedList implementation that uses an underlying array for storage.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ExtendedList<T> : RangedListBase<T> {
		private T[] _internalArray;
		private int _length;

		public ExtendedList() 
			: this(1024) {
		}

		public ExtendedList(int capacity)
			: this(capacity, int.MaxValue) {
		}

		public ExtendedList(int capacity, int maxCapacity)
			: this(capacity, capacity, maxCapacity) {
		}

		public ExtendedList(int capacity, int capacityGrowthSize, int maxCapacity)
			: this(new T[capacity], 0, capacityGrowthSize, maxCapacity) {
		}

		public ExtendedList(T[] sourceArray)
			: this(sourceArray, sourceArray.Length, 0, sourceArray.Length) {
		}

		private ExtendedList(T[] internalArray, int currentLogicalSize, int capacityGrowthSize, int maxCapacity) {
			Guard.ArgumentNotNull(internalArray, nameof(internalArray));
			Guard.ArgumentInRange(currentLogicalSize, 0, internalArray.Length, nameof(currentLogicalSize));
			Guard.ArgumentInRange(capacityGrowthSize, 0, int.MaxValue, nameof(capacityGrowthSize));
			Guard.ArgumentInRange(maxCapacity, currentLogicalSize, int.MaxValue, nameof(maxCapacity));
			_internalArray = internalArray;
			_length = currentLogicalSize;
			CapacityGrowthSize = capacityGrowthSize;
			MaxCapacity = maxCapacity;
		}

		public int CapacityGrowthSize { get; }

		public int MaxCapacity { get; }

		public override int Count => _length;

		public override bool IsReadOnly => false;

		public override IEnumerable<int> IndexOfRange(IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			var comparer = EqualityComparer<T>.Default;
			var itemsArr = items as T[] ?? items.ToArray();
			foreach (var item in itemsArr) {
				var itemIndex = -1;
				for (var j = 0; j < _length; j++) {
					if (comparer.Equals(item, _internalArray[j])) {
						itemIndex = j;
						break;
					}
				}
				yield return itemIndex;
			}
		}

		public override IEnumerable<T> ReadRange(int index, int count) {
			CheckRange(index, count);
			
			// optimize for single read
			if (count == 1)
				return new[] { _internalArray[index] };

			var endIndex = Math.Min(index + count - 1, _length - 1);
			var readCount = endIndex - index + 1;
			if (readCount <= 0)
				return new T[0];
			var readResult = new T[readCount];
			Array.Copy(_internalArray, index, readResult, 0, readCount);
			return readResult;
		}

		public override void AddRange(IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			var itemsArr = items as T[] ?? items.ToArray();

			if (itemsArr.Length == 0)
				return;
			
			GrowSpaceIfRequired(itemsArr.Length);
			UpdateVersion();
			if (itemsArr.Length == 1)
				// single access optimization
				_internalArray[_length] = itemsArr[0];
			else
				Array.Copy(itemsArr, 0, _internalArray, _length, itemsArr.Length);
			_length += itemsArr.Length;
		}

		public override void UpdateRange(int index, IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			var itemsArr = items as T[] ?? items.ToArray();
			CheckRange(index, itemsArr.Length);
			
			if (itemsArr.Length == 0)
				return;

			UpdateVersion();
			Array.Copy(itemsArr, 0, _internalArray, index, itemsArr.Length);
		}

		public override void InsertRange(int index, IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			Guard.ArgumentInRange(index, 0, Math.Max(0, _length), nameof(index));

			var itemsArr = items as T[] ?? items.ToArray();
			GrowSpaceIfRequired(itemsArr.Length);
			UpdateVersion();

			var capacity = _internalArray.Length - _length;
			if (itemsArr.Length > capacity)
				throw new ArgumentException("Insufficient capacity", nameof(items));

			// Use Array.Copy to move original items, since handles overlap scenarios
			Array.Copy(_internalArray, index, _internalArray, index + itemsArr.Length, _length - index);
			Array.Copy(itemsArr, 0, _internalArray, index, itemsArr.Length);
			_length += itemsArr.Length;
		}

		public override void RemoveRange(int index, int count) {
			CheckRange(index, count);

			if (count == 0)
				return;

			// Use Array.Copy to move original items, since handles overlap scenarios
			Array.Copy(_internalArray, index + count, _internalArray, index, (_length - (index + count)));
			_length -= count;
			UpdateVersion();
		}

		private void GrowSpaceIfRequired(int newItems) {
			var newCapacity = _length + newItems;

			// check if don't need to grow capacity
			if (newCapacity <= _internalArray.Length)
				return;

			// check if growth exceeds max
			if (newCapacity > MaxCapacity || CapacityGrowthSize == 0)
				throw new ArgumentException("Insufficient capacity");

			// determine how many new bytes are actually needed
			var actualNewBytes = newItems - (_internalArray.Length - _length);
			Debug.Assert(actualNewBytes > 0);

			// calc the new capacity (grows in blocks)
			var remainingGrowthCapacity = MaxCapacity - _internalArray.Length;
			var growAmount = (int)Math.Min(Math.Ceiling(actualNewBytes / (double)CapacityGrowthSize) * CapacityGrowthSize, remainingGrowthCapacity);
			Debug.Assert(_internalArray.Length + growAmount <= MaxCapacity);

			// Resize store
			Array.Resize(ref _internalArray, _internalArray.Length + growAmount);
		}

	}


}