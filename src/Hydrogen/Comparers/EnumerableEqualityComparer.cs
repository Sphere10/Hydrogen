// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class EnumerableEqualityComparer<T> : IEqualityComparer<IEnumerable<T>> {
	private readonly IEqualityComparer<T> _elementComparer;

	public EnumerableEqualityComparer()
		: this(EqualityComparer<T>.Default) {
	}

	public EnumerableEqualityComparer(IEqualityComparer<T> comparer) {
		_elementComparer = comparer;
	}

	public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
		=> ReferenceEquals(x, y) || (x != null && y != null && x.SequenceEqual(y, _elementComparer));

	public int GetHashCode(IEnumerable<T> obj) {
		if (obj == null)
			return 0;

		return obj.Select(e => e.GetHashCode()).Aggregate(0, HashCode.Combine);
	}
}
