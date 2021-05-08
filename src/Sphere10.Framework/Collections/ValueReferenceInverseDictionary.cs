//-----------------------------------------------------------------------
// <copyright file="ValueReferenceInverseDictionary.cs" company="Sphere 10 Software">
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
using System.Linq;

namespace Sphere10.Framework.Collections {
	public class ValueReferenceInverseDictionary<TKey, TValue> : ILookup<TValue, TKey> where TValue : class {
	    private readonly ILookup<Reference<TValue>, TKey> _valueReferenceToKeyLookup;

		public ValueReferenceInverseDictionary(IDictionary<TKey, TValue> dictionary) {
		    _valueReferenceToKeyLookup = dictionary.ToLookup(kv => Reference.For(kv.Value), kv => kv.Key);
		}

	    #region ILookup<TValue, TKey>

		public IEnumerator<IGrouping<TValue, TKey>> GetEnumerator() {
			return _valueReferenceToKeyLookup.Select(g => (IGrouping<TValue, TKey>)new Grouping<TValue, TKey>(g.Key.Object, g)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public bool Contains(TValue key) {
			return _valueReferenceToKeyLookup.Contains(Reference.For(key));
		}

		public int Count => _valueReferenceToKeyLookup.Count;

		public IEnumerable<TKey> this[TValue key] => _valueReferenceToKeyLookup[Reference.For(key)];

		#endregion
	}
}
