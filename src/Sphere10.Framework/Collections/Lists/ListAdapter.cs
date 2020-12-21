using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Framework.Collections.Lists {

	public sealed class ListAdapter<T> : IList {
		private readonly IList<T> _genericList;

		public ListAdapter(IList<T> wrappedList) {
			Guard.ArgumentNotNull(wrappedList, nameof(wrappedList));
			_genericList = wrappedList;
		}

		public int Add(object value) {
			_genericList.Add((T)value);
			return _genericList.Count - 1;
		}

		public void Clear() {
			_genericList.Clear();
		}

		public bool Contains(object value) {
			return _genericList.Contains((T)value);
		}

		public int IndexOf(object value) {
			return _genericList.IndexOf((T)value);
		}

		public void Insert(int index, object value) {
			_genericList.Insert(index, (T)value);
		}

		public bool IsFixedSize => false;

		public bool IsReadOnly => _genericList.IsReadOnly;

		public void Remove(object value) {
			_genericList.Remove((T)value);
		}

		public void RemoveAt(int index) {
			_genericList.RemoveAt(index);
		}

		public object this[int index] {
			get => _genericList[index];
			set => _genericList[index] = (T)value;
		}

		public void CopyTo(Array array, int index) {
			_genericList.CopyTo((T[])array, index);
		}

		public int Count => _genericList.Count;

		public bool IsSynchronized => false;

		public object SyncRoot => this;

		public IEnumerator GetEnumerator() {
			return _genericList.GetEnumerator();
		}
	}
}
