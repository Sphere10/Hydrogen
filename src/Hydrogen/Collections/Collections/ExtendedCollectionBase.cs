// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

public abstract class ExtendedCollectionBase<T> : IExtendedCollection<T> {

	public abstract int Count { get; }

	public abstract bool IsReadOnly { get; }

	public abstract void Add(T item);

	public abstract void AddRange(IEnumerable<T> items);

	public abstract IEnumerable<bool> RemoveRange(IEnumerable<T> items);

	public abstract void Clear();

	public abstract bool Remove(T item);

	public abstract bool Contains(T item);

	public abstract IEnumerable<bool> ContainsRange(IEnumerable<T> items);

	public abstract void CopyTo(T[] array, int arrayIndex);

	public abstract IEnumerator<T> GetEnumerator();

	#region ICollection wrapper

	int ICollection<T>.Count => Count;

	void ICollection<T>.Add(T item) {
		Add(item);
	}

	void ICollection<T>.Clear() {
		Clear();
	}

	bool ICollection<T>.Contains(T item) {
		return Contains(item);
	}

	void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
		CopyTo(array, arrayIndex);
	}

	bool ICollection<T>.Remove(T item) {
		return Remove(item);
	}

	#endregion

	#region IReadOnlyExtendedCollection wrapper

	int IReadOnlyCollection<T>.Count => Count;

	void IReadOnlyExtendedCollection<T>.CopyTo(T[] array, int arrayIndex) {
		CopyTo(array, arrayIndex);
	}

	bool IReadOnlyExtendedCollection<T>.Contains(T item) {
		return Contains(item);
	}
	IEnumerator IEnumerable.GetEnumerator() {
		return ((IEnumerable<T>)this).GetEnumerator();
	}

	#endregion

	#region IWriteOnlyExtendedCollection

	void IWriteOnlyExtendedCollection<T>.Add(T item) {
		Add(item);
	}

	bool IWriteOnlyExtendedCollection<T>.Remove(T item) {
		return Remove(item);
	}

	void IWriteOnlyExtendedCollection<T>.Clear() {
		Clear();
	}

	#endregion

}
