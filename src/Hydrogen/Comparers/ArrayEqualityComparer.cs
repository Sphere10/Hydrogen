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

namespace Hydrogen.Collections;

public class ArrayEqualityComparer<T> : IEqualityComparer<T[]> {
	private readonly IEqualityComparer<T> _elementComparer;

	public ArrayEqualityComparer() {
		_elementComparer = EqualityComparer<T>.Default;
	}

	public ArrayEqualityComparer(IEqualityComparer<T> comparer) {
		_elementComparer = comparer;
	}

	public bool Equals(T[] x, T[] y) {
		return Object.ReferenceEquals(x, y) || (x != null && y != null && x.SequenceEqual(y, _elementComparer));
	}

	public int GetHashCode(T[] obj) {
		if (obj == null)
			return 0;

		return unchecked(obj.Select(e => e.GetHashCode()).Aggregate(0, (a, b) => (((23 * 37) + a) * 37) + b));
	}
}
