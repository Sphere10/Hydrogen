using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sphere10.Framework {

	public class ColumnarList : RangedListBase<object[]> {
		private IExtendedList<object>[] _columnStore;

		public ColumnarList(params IExtendedList<object>[] columnStore) {
			Guard.ArgumentNotNull(columnStore, nameof(columnStore));
			Guard.Argument(columnStore.Length > 0, nameof(columnStore), "Must contain at least 1 column store");
			_columnStore = columnStore;
		}

		public override int Count => _columnStore[0].Count;

		public override bool IsReadOnly => false;

		public IReadOnlyExtendedList<object>[] Columns => _columnStore;

		public override void AddRange(IEnumerable<object[]> items) {
			var rowItems = items as object[][] ?? items.ToArray();
			if (rowItems.Length == 0)
				return;
			var columnarItems = Tools.Array.Transpose(rowItems, _columnStore.Length);
			CheckDimension(columnarItems.Length);
			for (var i = 0; i < columnarItems.Length; i++)
				_columnStore[i].AddRange(columnarItems[i]);
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<object[]> items) {
			throw new NotSupportedException();
		}

		public override void InsertRange(int index, IEnumerable<object[]> items) {
			CheckIndex(index, true);
			var rowItems = items as object[][] ?? items.ToArray();
			if (rowItems.Length == 0)
				return;
			var columnarItems = Tools.Array.Transpose(rowItems, _columnStore.Length);
			CheckDimension(columnarItems.Length);
			for (var i = 0; i < columnarItems.Length; i++)
				_columnStore[i].InsertRange(index, columnarItems[i]);
		}

		public override IEnumerable<object[]> ReadRange(int index, int count) {
			CheckRange(index, count);
			var actualCount = Math.Min(count, Count - index);

			var colData = new object[_columnStore.Length][];
			for (var i = 0; i < _columnStore.Length; i++) {
				colData[i] = _columnStore[i].ReadRange(index, actualCount).ToArray();
			}
			return Tools.Array.Transpose(colData);
		}

		public override void RemoveRange(int index, int count) {
			for (var i = 0; i < _columnStore.Length; i++)
				_columnStore[i].RemoveRange(index, count);
		}

		public override void UpdateRange(int index, IEnumerable<object[]> items) {
			CheckIndex(index, true);
			var rowItems = items as object[][] ?? items.ToArray();
			if (rowItems.Length == 0)
				return;
			var columnarItems = Tools.Array.Transpose(rowItems, _columnStore.Length);
			CheckDimension(columnarItems.Length);
			for (var i = 0; i < columnarItems.Length; i++)
				_columnStore[i].UpdateRange(index, columnarItems[i]);
		}

		public override void Clear() {
			_columnStore.ForEach(c => c.Clear());
		}

		private int CheckIndex(int index, bool allowAtEnd = false) {
			if (allowAtEnd && index == Count)
				return index;
			Guard.ArgumentInRange(index, 0, Math.Max(0, Count - 1), nameof(index));
			return index;
		}

		private void CheckRange(int index, int count) {
			var startIX = 0;
			var lastIX = startIX + (Count - 1).ClipTo(startIX, int.MaxValue);
			if (index == lastIX + 1 && count == 0)
				return;  // special case: at index of "next item" with no count, this is valid
			Guard.ArgumentInRange(index, startIX, lastIX, nameof(index));
			if (count > 0)
				Guard.ArgumentInRange(index + count - 1, startIX, lastIX, nameof(count));
		}


		public void CheckDimension(int dim) {
			Guard.ArgumentEquals(dim, Columns.Length, nameof(dim));
		}
	}

}
