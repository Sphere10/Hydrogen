// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class ItemHasherDecorator<TItem, TItemHasher> : IItemHasher<TItem>
	where TItemHasher : IItemHasher<TItem> {

	protected readonly TItemHasher InternalHasher;

	public ItemHasherDecorator(TItemHasher internalHasher) {
		InternalHasher = internalHasher;
	}

	public virtual byte[] Hash(TItem item) => InternalHasher.Hash(item);

	public int DigestLength => InternalHasher.DigestLength;
}
