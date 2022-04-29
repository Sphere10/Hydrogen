using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen {
	/// <summary>
	/// Base class for dictionary implementation. Implements index operator and misc.
	/// </summary>
	public abstract class DictionaryBase<TKey, TValue> : IDictionary<TKey, TValue> {
		private readonly KeysCollection _keysCollectionProperty;
		private readonly ValuesCollection _valuesCollectionProperty;

		protected DictionaryBase() {
			_keysCollectionProperty = new KeysCollection(this);
			_valuesCollectionProperty = new ValuesCollection(this);
		}

		public virtual ICollection<TKey> Keys => _keysCollectionProperty;

		public virtual ICollection<TValue> Values => _valuesCollectionProperty;

		public abstract void Add(TKey key, TValue value);

		public abstract bool ContainsKey(TKey key);

		public abstract bool TryGetValue(TKey key, out TValue value);

		public virtual TValue this[TKey key] {
			get {
				if (TryGetValue(key, out var value))
					return value;
				throw new KeyNotFoundException($"The key '{key}' was not found");
			}
			set => Add(key, value);
		}

		public abstract void Add(KeyValuePair<TKey, TValue> item);

		public abstract bool Contains(KeyValuePair<TKey, TValue> item);

		public abstract void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex);

		public abstract bool Remove(KeyValuePair<TKey, TValue> item);

		public abstract bool Remove(TKey item);

		public abstract void Clear();

		public abstract int Count { get; }

		public abstract bool IsReadOnly { get; }

		public abstract IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();

		protected virtual IEnumerator<TKey> GetKeysEnumerator() => GetEnumerator().AsEnumerable().Select(x => x.Key).GetEnumerator();

		protected virtual IEnumerator<TValue> GetValuesEnumerator() => GetEnumerator().AsEnumerable().Select(x => x.Value).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private class KeysCollection : ICollection<TKey> {

			private readonly DictionaryBase<TKey, TValue> _parent;

			public KeysCollection(DictionaryBase<TKey, TValue> parent) {
				_parent = parent;
			}

			public IEnumerator<TKey> GetEnumerator() => _parent.GetKeysEnumerator();

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}

			public void Add(TKey item) => throw new NotSupportedException();

			public void Clear() => _parent.Clear();

			public bool Contains(TKey item) => _parent.ContainsKey(item);

			public void CopyTo(TKey[] array, int arrayIndex) {
				foreach(var key in this) 
					array[arrayIndex++] = key;
			}

			public bool Remove(TKey item) => _parent.Remove(item);

			public int Count => _parent.Count;

			public bool IsReadOnly => _parent.IsReadOnly;
		}

		private class ValuesCollection : ICollection<TValue> {

			private readonly DictionaryBase<TKey, TValue> _parent;

			public ValuesCollection(DictionaryBase<TKey, TValue> parent) {
				_parent = parent;
			}

			public IEnumerator<TValue> GetEnumerator() => _parent.GetValuesEnumerator();

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}

			public void Add(TValue item) => throw new NotSupportedException();

			public void Clear() => _parent.Clear();

			public bool Contains(TValue item) => throw new NotSupportedException();

			public void CopyTo(TValue[] array, int arrayIndex) {
				foreach (var value in this)
					array[arrayIndex++] = value;
			}

			public bool Remove(TValue item) => throw new NotSupportedException();

			public int Count => _parent.Count;

			public bool IsReadOnly => _parent.IsReadOnly;
		}

	}
}
