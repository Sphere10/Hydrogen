// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
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
