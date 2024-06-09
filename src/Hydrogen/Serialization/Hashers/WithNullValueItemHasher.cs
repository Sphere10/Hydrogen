// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

internal sealed class WithNullValueItemHasher<TItem> : ItemHasherDecorator<TItem> {

	private readonly byte[] _nullItemHash;
	public WithNullValueItemHasher(IItemHasher<TItem> internalHasher, byte[] nullItemHash)
		: base(internalHasher) {
		_nullItemHash = nullItemHash;
	}

	public override byte[] Hash(TItem item)
		=> item == null ? Tools.Array.Clone(_nullItemHash) : base.Hash(item);
}
