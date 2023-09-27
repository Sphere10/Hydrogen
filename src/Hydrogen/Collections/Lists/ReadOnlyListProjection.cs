// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Hydrogen;

public class ReadOnlyListProjection<TFrom, TTo> : IReadOnlyList<TTo>  {

	private readonly Func<TFrom, TTo> _projection;

	public ReadOnlyListProjection(IReadOnlyList<TFrom> internalList, Func<TFrom, TTo> projection) {
		_projection = projection;
		InternalList = internalList;
	}

	protected IReadOnlyList<TFrom> InternalList;

	public TTo this[int index] => _projection(InternalList[index]);

	public int Count => InternalList.Count;

	public IEnumerator<TTo> GetEnumerator() => new ProjectedEnumerator<TFrom, TTo>(InternalList.GetEnumerator(), _projection);

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
