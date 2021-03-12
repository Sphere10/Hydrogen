using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

<<<<<<< HEAD
namespace Sphere10.Framework.Collections.Lists {
=======
namespace Sphere10.Framework.Collections {
>>>>>>> 31261811913d43c84cbc72ce9ab75c6658c08004

	/// <summary>
	/// A list implementation that implements inserts/deletes/adds as updates over an underlying fixed-size list. This list maintains
	/// it's own count and shuffles objects around using updates.
	/// </summary>
	/// <remarks>
	/// <see cref="Contains"/> and <see cref="ContainsRange"/> are overriden and implemented based on <see cref="IndexOf"/> and <see cref="IndexOfRange"/> in order to ensure only
	/// the logical objects are searched (avoids false positives). Same for <see cref="Remove"/> and <see cref="RemoveRange(int,int)"/>.
	/// </remarks>
	public class PreallocatedList<TItem> : ExtendedListDecorator<TItem> {

		private int _count;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fixedSizeStore">The underlying list which remains same size. List operations are implemented as updates of <see cref="fixedSizeStore"/>.</param>
		public PreallocatedList(IExtendedList<TItem> fixedSizeStore)
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
			CheckIndex(index);
			if (_count + itemsArr.Length > MaxCount)
				throw new ArgumentException("Insufficient space");
			var itemsToMove = ReadRange(index, itemsArr.Length).ToArray();
			base.UpdateRange(index, itemsArr);
			base.UpdateRange(index+itemsArr.Length, itemsToMove);
		}

		public override bool Remove(TItem item) => this.RemoveRange(new[] { item }).First();

		public override IEnumerable<bool> RemoveRange(IEnumerable<TItem> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			var indexes = IndexOfRange(items).WithIndex();
			// need to delete items in descending order
			throw new NotImplementedException();
		}

		public override void RemoveAt(int index) => this.RemoveRange(index,1);

		public override void RemoveRange(int index, int count) {
			CheckRange(index, count);

			var toMoveIX = index + count;
			var toMoveCount = _count - toMoveIX;
			var itemsToMove = ReadRange(toMoveIX, toMoveCount).ToArray();
			base.UpdateRange(index, itemsToMove);
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

		private int CheckIndex(int index) {
			Guard.ArgumentInRange(index, 0, Math.Max(0, _count - 1), nameof(index));
			return index;
		}

		private void CheckRange(int index, int count) {
			var startIX = 0;
			var lastIX = startIX + (_count - 1).ClipTo(startIX, int.MaxValue);
			Guard.ArgumentInRange(index, startIX, lastIX, nameof(index));
			if (count > 0)
				Guard.ArgumentInRange(index + count - 1, startIX, lastIX, nameof(count));
		}


	}
}
