// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Base class for singular item-by-item based extended collection implementations. This is not optimized for batch access.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingularCollectionBase<T> : ExtendedCollectionBase<T> {

	public override bool IsReadOnly => false;

	public override IEnumerable<bool> ContainsRange(IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		return items.Select(Contains).ToArray();
	}


	public override void AddRange(IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		foreach (var item in items)
			Add(item);
	}

	public override IEnumerable<bool> RemoveRange(IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		foreach (var item in items)
			yield return Remove(item);
	}

	protected IEnumerable<T> EnsureSafe(IEnumerable<T> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		return items;
	}
}