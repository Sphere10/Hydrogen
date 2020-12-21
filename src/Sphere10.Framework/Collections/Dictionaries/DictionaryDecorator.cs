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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Reflection;

namespace Sphere10.Framework {


	public abstract class DictionaryDecorator<K, V> : IDictionary<K, V> {
		protected readonly IDictionary<K,V> InternalDictionary;

		protected DictionaryDecorator() : this(new Dictionary<K, V>()) {
        }

		protected DictionaryDecorator(IDictionary<K, V> internalDictionary) {
			InternalDictionary = internalDictionary;
        }

		#region IDictionary Implementation
		public virtual void Add(K key, V value) { InternalDictionary.Add(key, value); }
		public virtual bool ContainsKey(K key) { return InternalDictionary.ContainsKey(key); }
		public virtual ICollection<K> Keys => InternalDictionary.Keys;
		public virtual bool TryGetValue(K key, out V value) { return InternalDictionary.TryGetValue(key, out value); }
		public virtual ICollection<V> Values => InternalDictionary.Values;
		public virtual V this[K key] {
			get => InternalDictionary[key];
			set => InternalDictionary[key] = value;
		}
		public virtual void Add(KeyValuePair<K, V> item) { InternalDictionary.Add(item); }
		public virtual bool Contains(KeyValuePair<K, V> item) { return InternalDictionary.Contains(item); }
		public virtual void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) { InternalDictionary.CopyTo(array, arrayIndex); }
		public virtual bool Remove(KeyValuePair<K, V> item) { return InternalDictionary.Remove(item); }
		public virtual bool Remove(K item) { return InternalDictionary.Remove(item); }
		public virtual void Clear() { InternalDictionary.Clear(); }
		public virtual int Count => InternalDictionary.Count;
		public virtual bool IsReadOnly => InternalDictionary.IsReadOnly;
		public virtual IEnumerator<KeyValuePair<K, V>> GetEnumerator() { return InternalDictionary.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return (InternalDictionary as IEnumerable).GetEnumerator(); }
		#endregion
    }
}
