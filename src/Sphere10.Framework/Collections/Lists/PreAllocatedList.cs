using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sphere10.Framework {

	/// <summary>
	/// A list implementation that implements inserts/deletes/adds as updates over an underlying fixed-size list. It works by shuffling
	/// objects around and maintaining it's own count. The only mutation methods called on the decorated inner list are <seealso  cref="IExtendedList{T}.Update"/> and <see cref="IExtendedList{T}.UpdateRange"/>
	/// it's own count and shuffles objects around using updates. The algorithms are optimized to avoid loading objects in memory.
	/// </summary>
	/// <remarks>
	/// <see cref="Contains"/> and <see cref="ContainsRange"/> are overriden and implemented based on <see cref="IndexOf"/> and <see cref="IndexOfRange"/> in order to ensure only
	/// the logical objects are searched (avoids false positives). Same for <see cref="Remove"/> and <see cref="RemoveRange(int,int)"/>.
	/// </remarks>
	public class PreAllocatedList<TItem> : ExtendedListDecorator<TItem> {

		private int _count;


		/// <summary>
		/// Creates a PreAllocatedList.
		/// </summary>
		/// <param name="maxCount">Number of items to pre-allocate.</param>
		public PreAllocatedList(int maxCount)
			: this(new ExtendedList<TItem>(Tools.Array.Gen<TItem>(maxCount, default))) {
		}


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fixedSizeStore">This is the pre-allocated list that is used to add/update/insert/remote from. This list is never changed and only mutated via update operations.</param>
		public PreAllocatedList(IExtendedList<TItem> fixedSizeStore)
			: base(fixedSizeStore) {
			_count = 0;
		}

		public override int Count => _count;

		public virtual int MaxCount => base.Count;

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
			var remaining = MaxCount - Count;
			Guard.ArgumentInRange(itemsArr.Length, 0, remaining, nameof(items), "Insufficient space");
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
			if (_count + itemsArr.Length > MaxCount)
				throw new ArgumentException("Insufficient space");


			// shuffle the items forward

			// aaaa            ;; _count = 4  max = 10   fromStartIndex = 2   fromEndIndex = _count - 1 
			// 0123456789
			// insert 3 b's at index 2
			// aabbbaa         ;; _count = 7  max = 10   toStartIndex = fromStartIndex + itemsArr.Length   toEndIndex = fromEndIndex + itemsArr.Length   
			// 0123456789

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
		}

		public override void Clear() => _count = 0;

		public override void CopyTo(TItem[] array, int arrayIndex) {
			foreach (var item in this)
				array[arrayIndex++] = item;
		}

		public override IEnumerator<TItem> GetEnumerator() => base.GetEnumerator().WithBoundary(_count);

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
}
