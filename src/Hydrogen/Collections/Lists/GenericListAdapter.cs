using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

public sealed class GenericListAdapter<T> : IList<T> {

	private readonly IList _legacyList;

	public GenericListAdapter(IList legacyList) {
		_legacyList = legacyList;
	}

	public int Count => _legacyList.Count;

	public bool IsReadOnly => _legacyList.IsReadOnly;

	public IEnumerator<T> GetEnumerator() {
		foreach(var item in _legacyList) {
			yield return (T)item;
		}
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	public void Add(T item) => _legacyList.Add(item);

	public void Clear() => _legacyList.Clear();

	public bool Contains(T item) => _legacyList.Contains(item);

	public void CopyTo(T[] array, int arrayIndex) => _legacyList.CopyTo(array, arrayIndex);

	public bool Remove(T item) {
		var countBefore = _legacyList.Count;
		_legacyList.Remove(item);
		var countAfter = _legacyList.Count;
		return countBefore != countAfter;
	}

	public int IndexOf(T item) => _legacyList.IndexOf(item);

	public void Insert(int index, T item) => _legacyList.Insert(index, item);

	public void RemoveAt(int index) => _legacyList.RemoveAt(index);

	public T this[int index] {
		get => (T)_legacyList[index];
		set => _legacyList[index] = value;
	}
}
