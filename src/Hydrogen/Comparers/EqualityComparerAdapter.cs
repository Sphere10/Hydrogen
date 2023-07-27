// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

// Based on Jon Skeet's library

using System;
using System.Collections.Generic;

namespace Hydrogen;


public class EqualityComparerAdapter<T> : IEqualityComparer<T> {
	private readonly IComparer<T> _comparer;
	private readonly IItemChecksummer<T> _checksummer;

	public EqualityComparerAdapter(IComparer<T> comparer, IItemChecksummer<T> checksummer = null) {
		_comparer = comparer;
		_checksummer = checksummer ?? new ObjectHashCodeChecksummer<T>();
	}

	public bool Equals(T x, T y)  => _comparer.Compare(x, y) == 0;

	public int GetHashCode(T obj) => _checksummer.CalculateChecksum(obj);
}
