using System.Collections;
using System.Collections.Generic;

namespace Sphere10.Framework {
	/// <summary>
	/// Base class for dictionary implementation. Implements index operator and misc.
	/// </summary>
	public abstract class DictionaryBase<TKey, TValue> : IDictionary<TKey, TValue> {

		public abstract ICollection<TKey> Keys { get; }

		public abstract ICollection<TValue> Values { get; }

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

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	}
}
