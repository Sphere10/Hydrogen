// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public sealed class ProjectionHasher<TItem, TProjection> : IItemHasher<TItem> {
	private readonly Func<TItem, TProjection> _projection;
	private readonly IItemHasher<TProjection> _hasher;

	public ProjectionHasher(Func<TItem, TProjection> projection, IItemHasher<TProjection> hasher) {
		Guard.ArgumentNotNull(projection, nameof(projection));
		Guard.ArgumentNotNull(hasher, nameof(hasher));
		_projection = projection;
		_hasher = hasher;
	}

	public byte[] Hash(TItem item) => _hasher.Hash(_projection(item));

	public int DigestLength => _hasher.DigestLength;
}
