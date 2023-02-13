//-----------------------------------------------------------------------
// <copyright file="EnumerableSequenceEqualComparer.cs" company="Sphere 10 Software">
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
