// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.using System;

namespace Hydrogen;

/// <summary>
/// Hashes items in an <see cref="ObjectStream"/> by serializing the item bytes and applying the configured hash algorithm.
/// </summary>
internal class ObjectStreamItemHasher : IItemHasher<long> {
	private readonly ObjectStream _objectStream;
	private readonly CHF _chf;

	public ObjectStreamItemHasher(ObjectStream objectStream, CHF chf) {
		_objectStream = objectStream;
		_chf = chf;
		DigestLength = Hashers.GetDigestSizeBytes(chf);	
	}

	public int DigestLength { get; }

	public byte[] Hash(long index) {
		var bytes = _objectStream.GetItemBytes(index);
		return Hashers.HashWithNullSupport(_chf, bytes);
	}
	
}
