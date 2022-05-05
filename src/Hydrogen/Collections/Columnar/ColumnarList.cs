using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Hydrogen {

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

			if (count == 0)
				return Enumerable.Empty<object[]>();

			var actualCount = Math.Min(count, Count - index);

			var colData = new object[_columnStore.Length][];
			for (var i = 0; i < _columnStore.Length; i++) {
				colData[i] = _columnStore[i].ReadRange(index, actualCount).ToArray();
			}
			return Tools.Array.Transpose(colData);
		}

		public override void RemoveRange(int index, int count) {
			CheckRange(index, count);
			
			if (count == 0)
				return;

			for (var i = 0; i < _columnStore.Length; i++)
				_columnStore[i].RemoveRange(index, count);
		}

		public override void UpdateRange(int index, IEnumerable<object[]> items) {
			Guard.ArgumentNotNull(items, nameof(items));
			var rowItems = items as object[][] ?? items.ToArray();
			CheckRange(index, rowItems.Length);
		
			if (rowItems.Length == 0)
				return;

			var columnarItems = Tools.Array.Transpose(rowItems, _columnStore.Length);
			CheckDimension(columnarItems.Length);
			for (var i = 0; i < columnarItems.Length; i++)
				_columnStore[i].UpdateRange(index, columnarItems[i]);
		}

		public override void Clear() {
			foreach (var col in _columnStore)
				col.Clear();
		}


		private void CheckDimension(int dim) {
			Guard.ArgumentEquals(dim, Columns.Length, nameof(dim));
		}
	}

}
