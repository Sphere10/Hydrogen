// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public sealed class BoundedEnumerator<T> : EnumeratorDecorator<T> {
	private readonly int _maxCount;
	private int _count;

	public BoundedEnumerator(IEnumerator<T> enumerator, int maxCount)
		: base(enumerator) {
		_maxCount = maxCount;
		_count = 0;
	}

	public override bool MoveNext() => _count++ < _maxCount && base.MoveNext();

	public override void Reset() {
		base.Reset();
		_count = 0;
	}
}
