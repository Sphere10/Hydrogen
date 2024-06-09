// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Bloom filter base implementation with parameterized length, hashing rounds. Sub-classes need only implement hashing function itself.
/// </summary>
/// <remarks>
/// The below approximation is used to determine filter length based on parameterized false positive error probability (see https://en.wikipedia.org/wiki/Bloom_filter#Probability_of_false_positives)
///
///  E = (1 - exp(-k*n/m))^k
///
///  where
/// 
///    E = Acceptable false positive probability 
///    N = maximum number of items expected in the set
///    M = length of the bloom filter (i.e. number of bits) 
///    k = the number hash rounds (how many bits will flip to 1 on the filter after adding 1 item)
///
///  When parameterizing by E, N, k the bloom filter length becomes
///
///   m = -k*n / ln( 1 - E^(1/k) ) 
/// 
/// </remarks>
public abstract class BloomFilterBase<TItem> : IBloomFilter<TItem> {

	private readonly BitArray _bitArray;

	protected BloomFilterBase(decimal targetError, int maximumExpectedItems, int hashRounds)
		: this((int)BloomFilterMath.EstimateBloomFilterLength(targetError, maximumExpectedItems, hashRounds), hashRounds) {
	}

	protected BloomFilterBase(int messageLength, int hashRounds) {
		Guard.ArgumentInRange(messageLength, 1, int.MaxValue, nameof(messageLength));
		Guard.ArgumentInRange(hashRounds, 1, messageLength, nameof(hashRounds));
		_bitArray = new BitArray(messageLength);
		HashRounds = hashRounds;
		Count = 0;
	}

	public int HashRounds { get; }

	public int FilterLength => _bitArray.Length;

	public int Count { get; private set; }

	public bool IsReadOnly => false;

	public decimal Error => BloomFilterMath.EstimateBloomFilterError(_bitArray.Length, Count, HashRounds);

	protected abstract int[] Hash(TItem item);

	public void Add(TItem item) {
		for (var i = 0; i < HashRounds; i++) {
			foreach (var index in Hash(item)) {
				_bitArray.Set(index, true);
			}
		}
		Count++;
	}

	public void Clear() {
		_bitArray.SetAll(false);
		Count = 0;
	}

	public bool Contains(TItem item) {
		return Hash(item).All(index => _bitArray[index]);
	}

	public void UnionWith(IEnumerable<TItem> other) {
		Guard.ArgumentCast<BloomFilterBase<TItem>>(other, out var otherBloomFilter, nameof(other));
		Guard.Argument(FilterLength == otherBloomFilter.FilterLength, nameof(other), "Mismatched filter lengths");
		this._bitArray.Or(otherBloomFilter._bitArray);
	}

	public IEnumerator<bool> GetEnumerator() {
		return _bitArray.Cast<bool>().GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}
