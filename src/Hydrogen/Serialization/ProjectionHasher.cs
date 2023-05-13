// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public sealed class ProjectionHasher<TItem, TKey> : IItemHasher<TItem> {
	private readonly Func<TItem, TKey> _projection;
	private readonly IItemHasher<TKey> _hasher;

	public ProjectionHasher(Func<TItem, TKey> projection, IItemHasher<TKey> hasher) {
		Guard.ArgumentNotNull(projection, nameof(projection));
		Guard.ArgumentNotNull(hasher, nameof(hasher));
		_projection = projection;
		_hasher = hasher;
	}

	public byte[] Hash(TItem item) => _hasher.Hash(_projection(item));

	public int DigestLength => _hasher.DigestLength;
}
