// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
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
