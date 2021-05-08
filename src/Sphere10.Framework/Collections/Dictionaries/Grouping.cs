//-----------------------------------------------------------------------
// <copyright file="Grouping.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Collections.Generic;

namespace Sphere10.Framework {

	public sealed class Grouping<TKey, TElement> : IGrouping<TKey, TElement> {
		private readonly IEnumerable<TElement> _elements;

		public Grouping(TKey key, IEnumerable<TElement> elements) {
			Key = key;
			_elements = elements;
		}

		public TKey Key { get; }

		public IEnumerator<TElement> GetEnumerator() {
			return _elements.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}

}

