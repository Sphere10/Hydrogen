//-----------------------------------------------------------------------
// <copyright file="DictionaryDecorator.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace Sphere10.Framework {


	public abstract class DictionaryDecorator<TKey, TValue, TDictionary> : IDictionary<TKey, TValue> where TDictionary : IDictionary<TKey, TValue> {
		protected readonly TDictionary InternalDictionary;

		protected DictionaryDecorator(TDictionary internalDictionary) {
			InternalDictionary = internalDictionary;
        }

		#region IDictionary Implementation
		public virtual void Add(TKey key, TValue value) { InternalDictionary.Add(key, value); }
		public virtual bool ContainsKey(TKey key) { return InternalDictionary.ContainsKey(key); }
		public virtual ICollection<TKey> Keys => InternalDictionary.Keys;
		public virtual bool TryGetValue(TKey key, out TValue value) { return InternalDictionary.TryGetValue(key, out value); }
		public virtual ICollection<TValue> Values => InternalDictionary.Values;
		public virtual TValue this[TKey key] {
			get => InternalDictionary[key];
			set => InternalDictionary[key] = value;
		}
		public virtual void Add(KeyValuePair<TKey, TValue> item) { InternalDictionary.Add(item); }
		public virtual bool Contains(KeyValuePair<TKey, TValue> item) { return InternalDictionary.Contains(item); }
		public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { InternalDictionary.CopyTo(array, arrayIndex); }
		public virtual bool Remove(KeyValuePair<TKey, TValue> item) { return InternalDictionary.Remove(item); }
		public virtual bool Remove(TKey item) { return InternalDictionary.Remove(item); }
		public virtual void Clear() { InternalDictionary.Clear(); }
		public virtual int Count => InternalDictionary.Count;
		public virtual bool IsReadOnly => InternalDictionary.IsReadOnly;
		public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { return InternalDictionary.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return (InternalDictionary as IEnumerable).GetEnumerator(); }
		#endregion
    }

	public abstract class DictionaryDecorator<TKey, TValue> : DictionaryDecorator<TKey, TValue, IDictionary<TKey, TValue>> {
	
		protected DictionaryDecorator()
			: this(new Dictionary<TKey, TValue>()) {
		}

		protected DictionaryDecorator(IDictionary<TKey, TValue> internalDictionary) 
			: base(internalDictionary) {
		}

	}

}
