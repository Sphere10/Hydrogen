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

public class CompositeComparer<T> : IComparer<T> {
	private readonly IComparer<T> _primary;
	private readonly IComparer<T> _secondary;

	public CompositeComparer(IComparer<T> single) : this(single, IdempotentComparer<T>.Instance) {
	}

	public CompositeComparer(IEnumerable<IComparer<T>> comparers) 
		: this(From(comparers)) {
	}

	public CompositeComparer(IComparer<T> primary, IComparer<T> secondary) {
		Guard.ArgumentNotNull(primary, nameof(primary));
		Guard.ArgumentNotNull(secondary, nameof(secondary));
		_primary = primary;
		_secondary = secondary;
	}

	public int Compare(T x, T y) {
		var result = _primary.Compare(x, y);
		return result == 0 ? _secondary.Compare(x, y) : result;
	}

	public static CompositeComparer<T> From(IEnumerable<IComparer<T>> comparers) {
		Guard.ArgumentNotNull(comparers, nameof(comparers));
		var comparersArr = comparers as IComparer<T>[] ?? comparers.ToArray();
		switch(comparersArr.Count()) {
			case 0: 
				throw new ArgumentException("Must have at least one comparer", nameof(comparers));
			case 1:
				return new CompositeComparer<T>(comparersArr.Single());
			case 2:
				return new CompositeComparer<T>(comparersArr.Single(), comparersArr.Skip(1).Single());
			default:
				return new CompositeComparer<T>(comparersArr.First(), From(comparersArr.Skip(1)));
		}
	}

}
