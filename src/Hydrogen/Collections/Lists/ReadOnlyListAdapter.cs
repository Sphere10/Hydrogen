//-----------------------------------------------------------------------
// <copyright file="ListDecorator.cs" company="Sphere 10 Software">
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

namespace Hydrogen {

	public class ReadOnlyListAdapter<TItem> : IReadOnlyList<TItem> {
		private readonly IList<TItem> _internalList;
		public ReadOnlyListAdapter(IList<TItem> internalList) {
			_internalList = internalList;
        }

        public TItem this[int index] => (TItem)_internalList[index];

        public int Count => _internalList.Count;

		public IEnumerator<TItem> GetEnumerator() => _internalList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
