// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class ColumnarList : RangedListBase<object[]> {
	private readonly IExtendedList<object>[] _columnStore;

	public ColumnarList(params IExtendedList<object>[] columnStore) {
		Guard.ArgumentNotNull(columnStore, nameof(columnStore));
		Guard.Argument(columnStore.Length > 0, nameof(columnStore), "Must contain at least 1 column store");
		_columnStore = columnStore;
	}

	public override long Count => _columnStore[0].Count;

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
		UpdateVersion();
	}

	public override IEnumerable<long> IndexOfRange(IEnumerable<object[]> items) {
		throw new NotSupportedException();
	}

	public override void InsertRange(long index, IEnumerable<object[]> items) {
		CheckIndex(index, true);
		var rowItems = items as object[][] ?? items.ToArray();
		if (rowItems.Length == 0)
			return;
		var columnarItems = Tools.Array.Transpose(rowItems, _columnStore.Length);
		CheckDimension(columnarItems.Length);
		for (var i = 0; i < columnarItems.Length; i++)
			_columnStore[i].InsertRange(index, columnarItems[i]);
		UpdateVersion();
	}

	public override IEnumerable<object[]> ReadRange(long index, long count) {
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

	public override void RemoveRange(long index, long count) {
		CheckRange(index, count);

		if (count == 0)
			return;

		for (var i = 0; i < _columnStore.Length; i++)
			_columnStore[i].RemoveRange(index, count);
	}

	public override void UpdateRange(long index, IEnumerable<object[]> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		var rowItems = items as object[][] ?? items.ToArray();
		CheckRange(index, rowItems.Length);

		if (rowItems.Length == 0)
			return;

		var columnarItems = Tools.Array.Transpose(rowItems, _columnStore.Length);
		CheckDimension(columnarItems.Length);
		for (var i = 0; i < columnarItems.Length; i++)
			_columnStore[i].UpdateRange(index, columnarItems[i]);
		UpdateVersion();
	}

	public override void Clear() {
		foreach (var col in _columnStore)
			col.Clear();
		UpdateVersion();
	}

	private void CheckDimension(int dim) {
		Guard.ArgumentEquals(dim, Columns.Length, nameof(dim));
	}
}
