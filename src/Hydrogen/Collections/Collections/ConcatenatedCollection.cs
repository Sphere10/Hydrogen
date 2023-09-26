using System;
using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

public class ConcatenatedCollection<T> : ICollection<T> {
	private readonly ICollection<T> _first;
	private readonly ICollection<T> _second;

	public ConcatenatedCollection(ICollection<T> first, ICollection<T> second) {
		_first = first ?? throw new ArgumentNullException(nameof(first));
		_second = second ?? throw new ArgumentNullException(nameof(second));
	}

	public int Count => _first.Count + _second.Count;

	public bool IsReadOnly => true;

	public void Add(T item) {
		throw new NotSupportedException("This collection is read-only.");
	}

	public void Clear() {
		throw new NotSupportedException("This collection is read-only.");
	}

	public bool Contains(T item) {
		return _first.Contains(item) || _second.Contains(item);
	}

	public void CopyTo(T[] array, int arrayIndex) {
		_first.CopyTo(array, arrayIndex);
		_second.CopyTo(array, arrayIndex + _first.Count);
	}

	public IEnumerator<T> GetEnumerator() {
		foreach (var item in _first) {
			yield return item;
		}

		foreach (var item in _second) {
			yield return item;
		}
	}

	public bool Remove(T item) {
		throw new NotSupportedException("This collection is read-only.");
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}


}
