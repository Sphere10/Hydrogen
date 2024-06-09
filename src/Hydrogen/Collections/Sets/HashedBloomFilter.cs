// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// A bloom filter base implementation that selects the "bloom hashes" using the last bytes of a cryptographic hash. 
/// </summary>
public class HashedBloomFilter<TItem> : BloomFilterBase<TItem> {
	private readonly CHF _hashAlgorithm;
	private readonly IItemSerializer<TItem> _serializer;

	public HashedBloomFilter(decimal targetError, int maximumExpectedItems, int hashRounds, CHF hashAlgorithm, IItemSerializer<TItem> serializer)
		: base(targetError, maximumExpectedItems, hashRounds) {
		Guard.ArgumentInRange(hashRounds, 1, 5, nameof(hashRounds));
		_hashAlgorithm = hashAlgorithm;
		_serializer = serializer;
	}

	public HashedBloomFilter(int messageLength, int hashRounds, CHF hashAlgorithm, IItemSerializer<TItem> serializer)
		: base(messageLength, hashRounds) {
		_hashAlgorithm = hashAlgorithm;
		_serializer = serializer;
	}

	protected override int[] Hash(TItem item) {
		var hash = Hashers.Hash(_hashAlgorithm, item, _serializer);
		if (hash.Length < 20)
			throw new InvalidOperationException("Hash is too short to select bloom filter bytes");

		return
			Enumerable
				.Range(0, HashRounds)
				.Select(index => (int)(EndianBitConverter.Little.ToUInt32(hash, index * 4) % FilterLength))
				.ToArray();

	}
}
