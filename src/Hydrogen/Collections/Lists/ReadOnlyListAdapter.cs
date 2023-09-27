// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

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
