﻿using System;

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
