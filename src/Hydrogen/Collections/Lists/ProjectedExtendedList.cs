// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public sealed class ProjectedExtendedList<TFrom, TTo> : IExtendedList<TTo> {
	private readonly IExtendedList<TFrom> _source;
	private readonly Func<TFrom, TTo> _projection;
	private readonly Func<TTo, TFrom> _inverseProjection;

	public ProjectedExtendedList(IExtendedList<TFrom> source, Func<TFrom, TTo> projection, Func<TTo, TFrom> inverseProjection) {
		Guard.ArgumentNotNull(source, nameof(source));
		Guard.ArgumentNotNull(projection, nameof(projection));
		Guard.ArgumentNotNull(inverseProjection, nameof(inverseProjection));
		_source = source;
		_projection = projection;
		_inverseProjection = inverseProjection;
	}

	public long Count => _source.Count;

	int ICollection<TTo>.Count => ((ICollection<TFrom>)_source).Count;

	int IReadOnlyCollection<TTo>.Count => ((ICollection<TFrom>)_source).Count;

	public bool IsReadOnly => _source.IsReadOnly;

	public int IndexOf(TTo item) => _source.IndexOf(_inverseProjection(item));

	public long IndexOfL(TTo item) => _source.IndexOfL(_inverseProjection(item));

	public IEnumerable<long> IndexOfRange(IEnumerable<TTo> items) => _source.IndexOfRange(items.Select(_inverseProjection));

	public bool Contains(TTo item) => _source.Contains(_inverseProjection(item));

	public IEnumerable<bool> ContainsRange(IEnumerable<TTo> items) => _source.ContainsRange(items.Select(_inverseProjection));

	public TTo Read(long index) => _projection(_source.Read(index));

	public IEnumerable<TTo> ReadRange(long index, long count) => _source.ReadRange(index, count).Select(_projection);

	public void Add(TTo item) => _source.Add(_inverseProjection(item));

	public void AddRange(IEnumerable<TTo> items) => _source.AddRange(items.Select(_inverseProjection));

	public void Update(long index, TTo item) => _source.Update(index, _inverseProjection(item));

	public void UpdateRange(long index, IEnumerable<TTo> items) => _source.UpdateRange(index, items.Select(_inverseProjection));

	public void Insert(int index, TTo item) => _source.Insert(index, _inverseProjection(item));

	public void Insert(long index, TTo item) => _source.Insert(index, _inverseProjection(item));

	public void InsertRange(long index, IEnumerable<TTo> items) => _source.InsertRange(index, items.Select(_inverseProjection));

	public bool Remove(TTo item) => _source.Remove(_inverseProjection(item));

	public IEnumerable<bool> RemoveRange(IEnumerable<TTo> items) => _source.RemoveRange(items.Select(_inverseProjection));

	public void RemoveAt(int index) => _source.RemoveAt(index);

	public void RemoveAt(long index) => _source.RemoveAt(index);

	public void RemoveRange(long index, long count) => _source.RemoveRange(index, count);

	public void Clear() => _source.Clear();

	public void CopyTo(TTo[] array, int arrayIndex) => _source.CopyTo(System.Array.ConvertAll(array, x => _inverseProjection(x)), arrayIndex);

	IEnumerator IEnumerable.GetEnumerator() => _source.GetEnumerator();

	public IEnumerator<TTo> GetEnumerator() => _source.Cast<TTo>().GetEnumerator();

	public TTo this[int index] {
		get => this[(long)index];
		set => this[(long)index] = value;
	}

	public TTo this[long index] {
		get => _projection(_source[index]);
		set => _source[index] = _inverseProjection(value);
	}

	TTo IWriteOnlyExtendedList<TTo>.this[long index] {
		set => this[index] = value;
	}

	TTo IReadOnlyList<TTo>.this[int index] => this[index];

}
