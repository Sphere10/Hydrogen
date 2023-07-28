// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public sealed class NegatedEqualityComparer<T> : IEqualityComparer<T> {
	private readonly IEqualityComparer<T> _source;

	public NegatedEqualityComparer(IEqualityComparer<T> source) {
		_source = source;
	}

	public bool Equals(T x, T y) => !_source.Equals(x, y);

	public int GetHashCode(T obj) => _source.GetHashCode(obj);
}
