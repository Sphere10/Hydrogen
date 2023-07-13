// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public sealed class CastedExtendedList<TFrom, TTo> : IExtendedList<TTo>
	where TFrom : class
	where TTo : class {
	internal readonly IExtendedList<TFrom> _from;

	public CastedExtendedList(IExtendedList<TFrom> from) {
		_from = from;
	}

	public long Count => _from.Count;

	int ICollection<TTo>.Count => ((ICollection<TFrom>)_from).Count;

	int IReadOnlyCollection<TTo>.Count => ((ICollection<TFrom>)_from).Count;

	public bool IsReadOnly => _from.IsReadOnly;

	public int IndexOf(TTo item) => _from.IndexOf(item as TFrom);

	public long IndexOfL(TTo item) => _from.IndexOfL(item as TFrom);

	public IEnumerable<long> IndexOfRange(IEnumerable<TTo> items) => _from.IndexOfRange(items.Cast<TFrom>());

	public bool Contains(TTo item) => _from.Contains(item as TFrom);

	public IEnumerable<bool> ContainsRange(IEnumerable<TTo> items) => _from.ContainsRange(items.Cast<TFrom>());

	public TTo Read(long index) => _from.Read(index) as TTo;

	public IEnumerable<TTo> ReadRange(long index, long count) => _from.ReadRange(index, count).Cast<TTo>();

	public void Add(TTo item) => _from.Add(item as TFrom);

	public void AddRange(IEnumerable<TTo> items) => _from.AddRange(items.Cast<TFrom>());

	public void Update(long index, TTo item) => _from.Update(index, item as TFrom);

	public void UpdateRange(long index, IEnumerable<TTo> items) => _from.UpdateRange(index, items.Cast<TFrom>());

	public void Insert(int index, TTo item) => _from.Insert(index, item as TFrom);

	public void Insert(long index, TTo item) => _from.Insert(index, item as TFrom);

	public void InsertRange(long index, IEnumerable<TTo> items) => _from.InsertRange(index, items.Cast<TFrom>());

	public bool Remove(TTo item) => _from.Remove(item as TFrom);

	public IEnumerable<bool> RemoveRange(IEnumerable<TTo> items) => _from.RemoveRange(items.Cast<TFrom>());

	public void RemoveAt(int index) => _from.RemoveAt(index);

	public void RemoveAt(long index) => _from.RemoveAt(index);

	public void RemoveRange(long index, long count) => _from.RemoveRange(index, count);

	public void Clear() => _from.Clear();

	public void CopyTo(TTo[] array, int arrayIndex) => _from.CopyTo(System.Array.ConvertAll(array, x => x as TFrom), arrayIndex);

	IEnumerator IEnumerable.GetEnumerator() => _from.GetEnumerator();

	public IEnumerator<TTo> GetEnumerator() => _from.Cast<TTo>().GetEnumerator();

	public TTo this[int index] {
		get => _from[index] as TTo;
		set => _from[index] = value as TFrom;
	}

	public TTo this[long index] {
		get => _from[index] as TTo;
		set => _from[index] = value as TFrom;
	}

	TTo IWriteOnlyExtendedList<TTo>.this[long index] {
		set => ((IWriteOnlyExtendedList<TFrom>)_from)[index] = value as TFrom;
	}

	TTo IReadOnlyList<TTo>.this[int index] => ((IReadOnlyList<TFrom>)_from)[index] as TTo;


}
