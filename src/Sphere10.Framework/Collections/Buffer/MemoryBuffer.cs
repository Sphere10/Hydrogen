using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// A memory buffer whose entire contents reside in a contiguous block of memory. Operations are batch optimized thus suitable for image/data manipulations of small - mid size.
	/// For larger buffers whose contents should be paged use <see cref="MemoryPagedBuffer"/>.
	/// <remarks>
	/// All enumerable results are implemented as arrays and arguments are not checked for performance.
	/// </remarks>
	/// </summary>
	public sealed class MemoryBuffer : RangedListBase<byte>, IBuffer {
		public const int DefaultBlockGrowth = 4096;
		public const int DefaultMaxSize = int.MaxValue;
		private byte[] _internalArray;
		private int _length;
		private readonly int _initialCapacity;
		private readonly int _blockGrowthSize;
		private readonly int _maxCapacity;

		public MemoryBuffer() : this(0, DefaultBlockGrowth) {
		}

		public MemoryBuffer(int initialCapacity, int blockGrowthSize) 
			: this(initialCapacity, blockGrowthSize, DefaultMaxSize) {
		}

		public MemoryBuffer(int initialCapacity, int blockGrowthSize, int maxCapacity)
			: this(new byte[initialCapacity], 0, blockGrowthSize, maxCapacity) {
		}

		public MemoryBuffer(byte[] fixedArray) 
			: this(fixedArray, fixedArray.Length, 0, fixedArray.Length) {
		}

		private MemoryBuffer(byte[] internalArray, int currentSize, int capacityGrowthSize, int maxCapacity) {
			_internalArray = internalArray;
			_initialCapacity = internalArray.Length;
			_length = currentSize;
			_blockGrowthSize = capacityGrowthSize;
			_maxCapacity = maxCapacity;
		}

		public int CapacityGrowthSize => _blockGrowthSize;

		public int MaxCapacity => _maxCapacity;

		public override int Count => _length;

		public override IEnumerable<int> IndexOfRange(IEnumerable<byte> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			var results = new List<int>();
			var itemsArr = items as byte[] ?? items.ToArray();
			for (var i = 0; i <_internalArray.Length; i++) {
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

		public override IEnumerable<byte> ReadRange(int index, int count) {
			CheckRange(index, count);
			// optimize for single read
			if (count == 1)
				return new[] { _internalArray[index] };

			var endIndex = Math.Min(index + count - 1, _length - 1);
			var readCount = endIndex - index + 1;
			if (readCount <= 0)
				return new byte[0];
			var readResult = new byte[readCount];
			System.Buffer.BlockCopy(_internalArray, index, readResult, 0, readCount);
			return readResult;
		}

		public ReadOnlySpan<byte> ReadSpan(int index, int count) => AsSpan(index, count);

		public override void AddRange(IEnumerable<byte> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			AddRange(items as byte[] ?? items.ToArray());
		}

		public void AddRange(ReadOnlySpan<byte> items) {
			Write(_length, items);
		}

		public void Write(int index, ReadOnlySpan<byte> items) {
			int newBytesCount = Math.Max(index + items.Length, _length) - _length;
			
			GrowSpaceIfRequired(newBytesCount);
			UpdateVersion();
			
			if (items.Length == 1)
			{
				_internalArray[index] = items[0];
			}
			else
			{
				items.CopyTo(_internalArray.AsSpan(index, items.Length));
			}

			_length += newBytesCount;
		}

		public void ExpandTo(int totalBytes) {
			Guard.ArgumentInRange(totalBytes, _length, MaxCapacity, nameof(totalBytes), "Allocated space would either not contain existing items or exceed max capacity");
			var newBytes = totalBytes - _internalArray.Length;
			if (newBytes > 0) 
				Expand(newBytes);
		}

		public void Expand(int newBytes) {
			Guard.ArgumentInRange(newBytes, 0, int.MaxValue, nameof(newBytes));
			GrowSpaceIfRequired(newBytes);
			_length += newBytes;
		}


		public override void UpdateRange(int index, IEnumerable<byte> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			UpdateRange(index, items as byte[] ?? items.ToArray()) ;
		}

		public void UpdateRange(int index, ReadOnlySpan<byte> items) {
			CheckRange(index, items.Length);
			UpdateVersion();
			items.CopyTo(_internalArray.AsSpan(index, items.Length));
		}

		public override void InsertRange(int index, IEnumerable<byte> items) {
			InsertRange(index, items as byte[] ?? items.ToArray() );
		}

		public void InsertRange(int index, ReadOnlySpan<byte> items) {
			CheckIndex(index, allowAtEnd: true);
			GrowSpaceIfRequired(items.Length);
			UpdateVersion();
			var capacity = _internalArray.Length - _length;
			if (items.Length > capacity)
				throw new ArgumentException("Insufficient capacity", nameof(items));

			// Use Array.Copy to move original items, since handles overlap scenarios
			Array.Copy(_internalArray, index, _internalArray, index + items.Length, _length - index);
			items.CopyTo(_internalArray.AsSpan(index, items.Length));
			_length += items.Length;
		}

		public override void RemoveRange(int index, int count) {
			CheckRange(index, count);
			UpdateVersion();

			// Use Array.Copy to move original items, since handles overlap scenarios
			Array.Copy(_internalArray, index + count, _internalArray, index, (_length - (index + count)));
			_length -= count;
		}

        public Span<byte> AsSpan() => AsSpan(0);

		public Span<byte> AsSpan(int index) => AsSpan(index, _length - index);

		public Span<byte> AsSpan(int index, int count) {
			if (index == _length && count == 0)
				return Span<byte>.Empty;
			Guard.ArgumentInRange(index, 0, Math.Max(_length - 1, 0), nameof(index));
			Guard.ArgumentInRange(count, 0, Math.Max(_length - index, 0), nameof(count));
			return _internalArray.AsSpan(index, count);
		}

		public override void Clear() {
			_internalArray = new byte[_initialCapacity];
			_length = 0;
		}

		private void GrowSpaceIfRequired(int newBytes) {
			var newCapacity = _length + newBytes;

			// check if don't need to grow capacity
			if (newCapacity <= _internalArray.Length) 
				return;

			// check if growth exceeds max
			if (newCapacity > _maxCapacity || _blockGrowthSize == 0)
				throw new ArgumentException("Insufficient capacity");

			// determine how many new bytes are actually needed
			var actualNewBytes = newBytes - (_internalArray.Length - _length);
			Debug.Assert(actualNewBytes>0);

			// calc the new capacity (grows in blocks)
			var remainingGrowthCapacity = _maxCapacity - _internalArray.Length;
			var growAmount = (int)Math.Min(Math.Ceiling(actualNewBytes / (double)_blockGrowthSize)*_blockGrowthSize, remainingGrowthCapacity);
			Debug.Assert(_internalArray.Length + growAmount <= _maxCapacity);

			// Re-allocate internal array
			Array.Resize(ref _internalArray, _internalArray.Length + growAmount);
		}

	}
}