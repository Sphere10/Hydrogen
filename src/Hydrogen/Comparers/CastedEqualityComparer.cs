// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public class CastedEqualityComparer<TItem, TBase> : IEqualityComparer<TBase> where TItem : TBase {
	private readonly IEqualityComparer<TItem> _comparer;

	public CastedEqualityComparer(IEqualityComparer<TItem> equalityComparer) {
		Guard.ArgumentNotNull(equalityComparer, nameof(equalityComparer));
		_comparer = equalityComparer;
	}

	public bool Equals(TBase? x, TBase? y)
		=> _comparer.Equals((TItem)x, (TItem)y);

	public int GetHashCode(TBase obj)
		=> _comparer.GetHashCode((TItem)obj);
}
