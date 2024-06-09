// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public sealed class BoundedEnumerator<T> : EnumeratorDecorator<T> {
	private readonly long _maxCount;
	private long _count;

	public BoundedEnumerator(IEnumerator<T> enumerator, long maxCount)
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
