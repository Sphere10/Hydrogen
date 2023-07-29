// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class ActionHasher<T> : IItemHasher<T> {
	private readonly Func<T, byte[]> _hasher;

	public ActionHasher(Func<T, byte[]> hasher, int digestLength) {
		_hasher = hasher;
		DigestLength = digestLength;
	}

	public byte[] Hash(T item) => _hasher.Invoke(item);

	public int DigestLength { get; }

}
