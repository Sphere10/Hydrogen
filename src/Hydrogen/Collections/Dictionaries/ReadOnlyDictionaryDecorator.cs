//-----------------------------------------------------------------------
// <copyright file="ReadOnlyDictionaryDecorator.cs" company="Sphere 10 Software">
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

	public class ReadOnlyDictionaryDecorator<TKey, TValue, TReadOnlyDictionary> : IReadOnlyDictionary<TKey, TValue> 
		where TReadOnlyDictionary : IReadOnlyDictionary<TKey, TValue> {

		protected readonly TReadOnlyDictionary Internal;
        public ReadOnlyDictionaryDecorator(TReadOnlyDictionary internalDictionary) { 
			Internal = internalDictionary;
		}

		public virtual TValue this[TKey key] => Internal[key];

		public virtual IEnumerable<TKey> Keys => Internal.Keys;

		public virtual IEnumerable<TValue> Values => Internal.Values;

		public virtual int Count => Internal.Count;

		public virtual bool ContainsKey(TKey key) => Internal.ContainsKey(key);

		public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Internal.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public virtual bool TryGetValue(TKey key, out TValue value) => Internal.TryGetValue(key, out value);

	}
}
