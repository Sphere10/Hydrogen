// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public abstract class ExtendedListBase<T> : ExtendedCollectionBase<T>, IExtendedList<T> {

	public int IndexOf(T item) {
		checked {
			return (int)IndexOfL(item);
		}
	}

	public abstract long IndexOfL(T item);

	public abstract IEnumerable<long> IndexOfRange(IEnumerable<T> items);

	public abstract T Read(long index);

	public abstract IEnumerable<T> ReadRange(long index, long count);

	public abstract void Update(long index, T item);

	public abstract void UpdateRange(long index, IEnumerable<T> items);

	public void Insert(int index, T item) => Insert((long)index, item);

	public abstract void Insert(long index, T item);

	public abstract void InsertRange(long index, IEnumerable<T> items);

	public void RemoveAt(int index) => RemoveAt((long)index);

	public abstract void RemoveAt(long index);

	public abstract void RemoveRange(long index, long count);

	public T this[int index] {
		get => this[(long)index];
		set => this[(long)index] = value;
	}

	public T this[long index] {
		get => Read(index);
		set => Update(index, value);
	}

	protected virtual void CheckIndex(long index, bool allowAtEnd = false) => Guard.CheckIndex(index, 0, Count, allowAtEnd);

	protected virtual void CheckRange(long index, long count, bool rightAligned = false) => Guard.CheckRange(index, count, rightAligned, 0, Count);


}
