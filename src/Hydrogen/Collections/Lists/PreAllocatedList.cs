using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Hydrogen {

	/// <summary>
	/// A wrapper for <see cref="IExtendedList{T}"/> that implements insertion, deletion and append as update operations over a pre-allocated collection of items.
	/// This is useful for converting an <see cref="IExtendedList{T}"/> that can only be mutated via "UPDATE" operations into one that supports INSERT/UPDATE/DELETE
	/// via sequential UPDATE operations. This is achieved by maintaining a local <see cref="Count"/> property and by overwriting pre-allocated items on
	/// append/insert, and "forgetting" them on delete. When the pre-allocated items are exhaused, a <see cref="PreallocationGrowthPolicy"/> is used to grow
	/// the underlying list. 
	/// </summary>
	/// <remarks>
	/// When shuffling items around via copy/paste operations, they are done "one at a time" rather than in "in ranges" so as not to exhaust memory on
	/// unbounded lists. Thus this class is suitable for wrapping unbounded lists of data without memory/computational complexity blowout.
	/// Additionally, <see cref="Contains"/> and <see cref="ContainsRange"/> are overriden and implemented based on <see cref="IndexOf"/> and <see cref="IndexOfRange"/>
	/// so as to ensure only the logical objects are searched (avoids false positives). Same is true for <see cref="Remove"/> and <see cref="RemoveRange(int,int)"/>.
	/// </remarks>
	public class PreAllocatedList<TItem> : ExtendedListDecorator<TItem> {
		private int _count;
		private readonly PreAllocationPolicy _preAllocationPolicy;
		private readonly int _blockSize;
		private readonly Func<TItem> _activator;

		public PreAllocatedList(Func<TItem> itemActivator) 
			: this(PreAllocationPolicy.MinimumRequired, 0, itemActivator)  {
		}

		public PreAllocatedList(int preallocatedItemCount, Func<TItem> itemActivator) 
			: this(PreAllocationPolicy.Fixed, preallocatedItemCount, itemActivator) {
		}

		public PreAllocatedList(PreAllocationPolicy preAllocationPolicy, int blockSize, Func<TItem> itemActivator)
			: this(new ExtendedList<TItem>(), 0, preAllocationPolicy, blockSize, itemActivator) {
		}

		public PreAllocatedList(IExtendedList<TItem> internalStore, int internalStoreCount, PreAllocationPolicy preAllocationPolicy, int blockSize, Func<TItem> itemActivator)
			: base(internalStore) {
			_count = internalStoreCount;
			if (preAllocationPolicy.IsIn(PreAllocationPolicy.ByBlock, PreAllocationPolicy.Fixed)) {
				Guard.Argument(blockSize > 0, nameof(blockSize), $"Must be greater than 0 for policy {preAllocationPolicy}");
			}

			_preAllocationPolicy = preAllocationPolicy;
			_blockSize = blockSize;
			_activator = itemActivator;

			// Ensure enough capacity when in Fixed mode (since never allocates again)
			if (preAllocationPolicy == PreAllocationPolicy.Fixed) {
				internalStore.AddRange(Enumerable.Repeat(_activator(), Math.Max(0, _blockSize - internalStore.Count)));
			}
		}

		public override int Count => _count;

		public virtual int Capacity => base.Count;

		public override int IndexOf(TItem item) => ToLogicalIndex(base.IndexOf(item));

		public override IEnumerable<int> IndexOfRange(IEnumerable<TItem> items) => base.IndexOfRange(items).Select(ToLogicalIndex);

		public override bool Contains(TItem item) => IndexOf(item) > 0;

		public override IEnumerable<bool> ContainsRange(IEnumerable<TItem> items) => IndexOfRange(items).Select(ix => ix > 0);

		public override TItem Read(int index) {
			CheckIndex(index);
			return base.Read(index);
		}

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
			CheckIndex(index);
			UpdateRange(index, new[] { item });
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
			// TODO: this could be optimized by copying bounded ranges instead of 1-by-1. Will 
			// improve stream record performance in ClusteredStorage
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

		public override void Clear() {
			_count = 0;
			if (_preAllocationPolicy == PreAllocationPolicy.MinimumRequired)
				ReduceExcessCapacity();
		}

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
					PreAllocationPolicy.Fixed => Enumerable.Empty<TItem>().ToArray(), 
					PreAllocationPolicy.MinimumRequired => Enumerable.Repeat(_activator(), required).ToArray(),
					PreAllocationPolicy.ByBlock => Enumerable.Repeat(_activator(), _blockSize * (int)Math.Ceiling(required / (float)_blockSize)).ToArray(),
					_ => throw new ArgumentOutOfRangeException(nameof(_preAllocationPolicy), _preAllocationPolicy, null)
				};
				InternalCollection.AddRange(newPreAllocatedItems);
				spareCapacity = (Capacity - Count) - quantity;
				Guard.Ensure(spareCapacity >= 0, "Insufficient space");
			}
		}

		private void ReduceExcessCapacity() {
			Debug.Assert(_preAllocationPolicy == PreAllocationPolicy.MinimumRequired);
			var spareCapacity = (Capacity - Count);
			if (spareCapacity > 0) {
				if (typeof(TItem).HasSubType(typeof(IDisposable))) 
					foreach (var item in InternalCollection.ReadRange(^spareCapacity..).Cast<IDisposable>())
						item.Dispose();

				InternalCollection.RemoveRange(^spareCapacity..);
			}
		}

		private int ToLogicalIndex(int index) {
			if (0 <= index && index <= _count - 1)
				return index;
			return -1;
		}

		protected bool ValidIndex(int index, bool allowAtEnd = false) => 0 <= index && (allowAtEnd ? index <= Count : index < Count);

		protected void CheckIndex(int index, bool allowAtEnd = false) => Guard.CheckIndex(index, 0, Count, allowAtEnd);

		protected void CheckRange(int index, int count, bool rightAligned = false) => Guard.CheckRange(index, count, rightAligned, 0, Count);

	}

}
