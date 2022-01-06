using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// A wrapper for <see cref="IExtendedList{T}"/> that implements insertion, deletion and append as update operations over a pre-allocated collection of items.
	/// This is useful for converting an <see cref="IExtendedList{T}"/> that can only be mutated via "UPDATE" operations into one that supports INSERT/UPDATE/DELETE.
	/// This is achieved by maintaining a local <see cref="Count"/> property and by overwriting pre-allocated items on append/insert, and "forgetting" them on delete.
	/// When the pre-allocated items are exhaused, a <see cref="PreallocationGrowthPolicy"/> is used to grow the underlying list. 
	/// </summary>
	/// <remarks>
	/// When shuffling items around via copy/paste operations, they are done "one at a time" rather than in ranges so as not to exhaust memory. Thus this class
	/// is suitable for wrapping arbitrarily large lists.
	/// Additionally, <see cref="Contains"/> and <see cref="ContainsRange"/> are overriden and implemented based on <see cref="IndexOf"/> and <see cref="IndexOfRange"/>
	/// so as to ensure only the logical objects are searched (avoids false positives). Same is true for <see cref="Remove"/> and <see cref="RemoveRange(int,int)"/>.
	/// </remarks>
	public class PreAllocatedList<TItem> : ExtendedListDecorator<TItem> where TItem : new() {
		private int _count;
		private readonly PreAllocationPolicy _preAllocationPolicy;
		private readonly int _blockSize;

		public PreAllocatedList() : this(PreAllocationPolicy.MinimumRequired, 0) {
		}

		public PreAllocatedList(int preallocatedItemCount) 
			: this(PreAllocationPolicy.Fixed, preallocatedItemCount) {
		}

		public PreAllocatedList(PreAllocationPolicy preAllocationPolicy, int blockSize)
			: this(new ExtendedList<TItem>(), 0, preAllocationPolicy, blockSize) {
		}

		public PreAllocatedList(IExtendedList<TItem> internalStore, int internalStoreCount, PreAllocationPolicy preAllocationPolicy, int blockSize)
			: base(internalStore) {
			_count = internalStoreCount;
			if (preAllocationPolicy.IsIn(PreAllocationPolicy.ByBlock, PreAllocationPolicy.Fixed)) {
				Guard.Argument(blockSize > 0, nameof(blockSize), $"Must be greater than 0 for policy {preAllocationPolicy}");
			}
			if (preAllocationPolicy == PreAllocationPolicy.Fixed) 
				internalStore.AddRange(Enumerable.Repeat(new TItem(), Math.Max(0, blockSize - internalStore.Count)));
			
			_preAllocationPolicy = preAllocationPolicy;
			_blockSize = blockSize;
		}

		public override int Count => _count;

		public virtual int Capacity => base.Count;

		public override int IndexOf(TItem item) => ToLogicalIndex(base.IndexOf(item));

		public override IEnumerable<int> IndexOfRange(IEnumerable<TItem> items) => base.IndexOfRange(items).Select(ToLogicalIndex);

		public override bool Contains(TItem item) => IndexOf(item) > 0;

		public override IEnumerable<bool> ContainsRange(IEnumerable<TItem> items) => IndexOfRange(items).Select(ix => ix > 0);

		public override TItem Read(int index) => base.Read(CheckIndex(index));

		public override IEnumerable<TItem> ReadRange(int index, int count) {
			CheckRange(index, count);
			return base.ReadRange(index, count);
		}

		public override void Add(TItem item) => AddRange(new[] { item });

		public override void AddRange(IEnumerable<TItem> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			var itemsArr = items as TItem[] ?? items.ToArray();
			EnsureSpace(itemsArr.Length);
			base.UpdateRange(_count, itemsArr);
			_count += itemsArr.Length;
		}

		public override void Update(int index, TItem item) {
			this.UpdateRange(index, new[] { item });
		}

		public override void UpdateRange(int index, IEnumerable<TItem> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			var itemsArr = items as TItem[] ?? items.ToArray();
			CheckRange(index, itemsArr.Length);
			base.UpdateRange(index, itemsArr);
		}

		public override void Insert(int index, TItem item) => this.InsertRange(index, new[] { item });

		public override void InsertRange(int index, IEnumerable<TItem> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			var itemsArr = items as TItem[] ?? items.ToArray();
			CheckIndex(index, true);
			EnsureSpace(itemsArr.Length);

			// shuffle item after insertion point forward
			var movedRegionFromStartIX = index;
			var movedRegionFromEndIX = _count - 1;
			var movedRegionToStartIX = movedRegionFromStartIX + itemsArr.Length;
			var movedRegionToEndIX = movedRegionFromEndIX + itemsArr.Length;

			for (var i = movedRegionToEndIX; i >= movedRegionToStartIX; i--) {
				var toCopy = base.Read(i - itemsArr.Length);
				base.Update(i, toCopy);
			}

			// finally, save the new items
			base.UpdateRange(index, itemsArr);

			_count += itemsArr.Length;
		}

		public override bool Remove(TItem item) => this.RemoveRange(new[] { item }).First();

		public override IEnumerable<bool> RemoveRange(IEnumerable<TItem> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			var indexes = IndexOfRange(items).WithIndex();
			// need to delete items in descending order
			throw new NotImplementedException();
		}

		public override void RemoveAt(int index) => this.RemoveRange(index, 1);

		public override void RemoveRange(int index, int count) {
			CheckRange(index, count);

			var movedRegionFromStartIX = index + count;
			var movedRegionFromEndIX = _count - 1;
			var movedRegionToStartIX = index;
			var movedRegionToEndIX = index + (movedRegionFromEndIX - movedRegionFromStartIX);

			for (var i = movedRegionToStartIX; i <= movedRegionToEndIX; i++) {
				var toCopy = base.Read(i + count);
				base.Update(i, toCopy);
			}
			_count -= count;
			if (_preAllocationPolicy == PreAllocationPolicy.MinimumRequired)
				ReduceExcessCapacity();
		}

		public override void Clear() => _count = 0;

		public override void CopyTo(TItem[] array, int arrayIndex) {
			foreach (var item in this)
				array[arrayIndex++] = item;
		}

		public override IEnumerator<TItem> GetEnumerator() => base.GetEnumerator().WithBoundary(_count);

		private void EnsureSpace(int quantity) {
			var spareCapacity = (Capacity - Count) - quantity;
			if (spareCapacity < 0) {
				var required = -spareCapacity;
				var newPreAllocatedItems = _preAllocationPolicy switch {
					PreAllocationPolicy.Fixed => Enumerable.Empty<TItem>(), 
					PreAllocationPolicy.MinimumRequired => Enumerable.Repeat(new TItem(), required),
					PreAllocationPolicy.ByBlock => Enumerable.Repeat(new TItem(), _blockSize * (int)Math.Ceiling(required / (float)_blockSize)),
					_ => throw new ArgumentOutOfRangeException(nameof(_preAllocationPolicy), _preAllocationPolicy, null)
				};

				InternalExtendedList.AddRange(newPreAllocatedItems);
				spareCapacity = (Capacity - Count) - quantity;
				Guard.Ensure(spareCapacity >= 0, "Insufficient space");
			}
		}

		private void ReduceExcessCapacity() {
			var spareCapacity = (Capacity - Count);
			if (spareCapacity > 0)
				InternalExtendedList.RemoveRange(^spareCapacity..);
		}

		private int ToLogicalIndex(int index) {
			if (0 <= index && index <= _count - 1)
				return index;
			return -1;
		}

		private int CheckIndex(int index, bool allowAtEnd = false) {
			if (allowAtEnd && index == _count) return index;
			Guard.ArgumentInRange(index, 0, Math.Max(0, _count - 1), nameof(index));
			return index;
		}

		private void CheckRange(int index, int count) {
			var startIX = 0;
			var lastIX = startIX + (_count - 1).ClipTo(startIX, int.MaxValue);
			if (index == lastIX + 1 && count == 0) return;  // special case: at index of "next item" with no count, this is valid
			Guard.ArgumentInRange(index, startIX, lastIX, nameof(index));
			if (count > 0)
				Guard.ArgumentInRange(index + count - 1, startIX, lastIX, nameof(count));
		}

	}

	public enum PreAllocationPolicy {
		/// <summary>
		/// The initial block of pre-allocated items is used, never grown or reduced.
		/// </summary>
		Fixed,

		/// <summary>
		/// The Capacity is grown in fixed-sized blocks as needed and never reduced.
		/// </summary>
		ByBlock,

		/// <summary>
		/// The Capacity is grown (and reduced) to meet the item Count.
		/// </summary>
		MinimumRequired,

	}

}
