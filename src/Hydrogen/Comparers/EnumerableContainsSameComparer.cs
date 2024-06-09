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

public class EnumerableContainsSameComparer<T> : IEqualityComparer<IEnumerable<T>> {
	public bool Equals(IEnumerable<T> x, IEnumerable<T> y) {
		var xArray = x as T[] ?? x.ToArray();
		var yArray = y as T[] ?? y.ToArray();
		return Object.ReferenceEquals(x, y) || (x != null && y != null && xArray.ContainsAll(yArray) && yArray.ContainsAll(xArray));
	}

	public int GetHashCode(IEnumerable<T> obj) {
		if (obj == null)
			return 0;

		return unchecked(obj.Select(e => e.GetHashCode()).Aggregate(0, (a, b) => (((23 * 37) + a) * 37) + b));
	}
}
