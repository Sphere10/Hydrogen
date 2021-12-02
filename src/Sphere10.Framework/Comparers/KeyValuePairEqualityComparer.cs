//-----------------------------------------------------------------------
// <copyright file="ActionEqualityComparer.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;

namespace Sphere10.Framework {

	public class KeyValuePairEqualityComparer<TKey, TValue> : IEqualityComparer<KeyValuePair<TKey, TValue>> {
		private readonly IEqualityComparer<TKey> _keyComparer;
		private readonly IEqualityComparer<TValue> _valueComparer;

		public KeyValuePairEqualityComparer(IEqualityComparer<TKey> keyComparer = null, IEqualityComparer<TValue> valueComparer = null) {
			_keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
			_valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
		}

		public bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y) {
			return _keyComparer.Equals(x.Key, y.Key) && _valueComparer.Equals(x.Value, y.Value);
		}

		public int GetHashCode(KeyValuePair<TKey, TValue> obj) {
			return HashCode.Combine(_keyComparer.GetHashCode(obj.Key), _valueComparer.GetHashCode(obj.Value));
		}
	}
}
