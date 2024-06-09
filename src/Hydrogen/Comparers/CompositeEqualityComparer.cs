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

public class CompositeEqualityComparer<T> : IEqualityComparer<T> {
	private readonly IEqualityComparer<T> _primary;
	private readonly IEqualityComparer<T> _secondary;

	public CompositeEqualityComparer(IEqualityComparer<T> single) : this(single, IdempotentEqualityComparer<T>.Instance) {
	}

	public CompositeEqualityComparer(IEnumerable<IEqualityComparer<T>> comparers) 
		: this(From(comparers)) {
	}

	public CompositeEqualityComparer(IEqualityComparer<T> primary, IEqualityComparer<T> secondary) {
		Guard.ArgumentNotNull(primary, nameof(primary));
		Guard.ArgumentNotNull(secondary, nameof(secondary));
		_primary = primary;;
		_secondary = secondary;
	}

	public bool Equals(T x, T y) => _primary.Equals(x, y) && _secondary.Equals(x, y);

	public int GetHashCode(T obj) => HashCode.Combine(_primary.GetHashCode(obj), _secondary.GetHashCode(obj));

	public static CompositeEqualityComparer<T> From(IEnumerable<IEqualityComparer<T>> comparers) {
		Guard.ArgumentNotNull(comparers, nameof(comparers));
		var comparersArr = comparers as IEqualityComparer<T>[] ?? comparers.ToArray();
		switch(comparersArr.Count()) {
			case 0: 
				throw new ArgumentException("Must have at least one comparer", nameof(comparers));
			case 1:
				return new CompositeEqualityComparer<T>(comparersArr.Single());
			case 2:
				return new CompositeEqualityComparer<T>(comparersArr.Single(), comparersArr.Skip(1).Single());
			default:
				return new CompositeEqualityComparer<T>(comparersArr.First(), From(comparersArr.Skip(1)));
		}
		 
	}
}