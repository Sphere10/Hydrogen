// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Globalization;
using System.Numerics;

namespace Hydrogen.DApp.Core.Maths;

/// <summary>
/// Represent the challenge that miners must solve for finding a new block. The Target/CompactTarget algorithm is
/// different to Bitcoin's nbits approach and is the same as PascalCoin's, invented by Albert Molina.
///
/// Of note, the CompactTarget represents a UInt32 version of target but retains orderable properties, such that
/// the higher the CompactTarget the "more difficult" it is. Bitcoin's nBits does not retain this property.
/// </summary>
public class MolinaTargetAlgorithm : ICompactTargetAlgorithm {
	readonly uint _bitmask;
	readonly uint _minZeros;
	readonly uint _maxZeros;

	public MolinaTargetAlgorithm() {
		_bitmask = (1 << 24) - 1; // 0x00FFFFFF in LE
		_minZeros = MinCompactTarget >> 24;
		_maxZeros = 256 - 24 - 1;
	}

	public uint MinCompactTarget => 134217728;

	public uint MaxCompactTarget => 3892314111;

	public uint FromTarget(BigInteger target) {
		// Count the number of bitwise 0's bignumber has
		// Represents a 256 digit binary number starting with 1 followed by 255 0's
		var bn2 = BigInteger.Parse("08000000000000000000000000000000000000000000000000000000000000000", NumberStyles.AllowHexSpecifier);
		var numZeros = (uint)0;
		const uint maxZeros = 256 - 24 - 1;
		while (target < bn2 && numZeros < maxZeros) {
			bn2 >>= 1;
			numZeros++;
		}

		// If smaller than minimum, return minimum
		var min_compact_target = MinCompactTarget;
		var numZerosInMinCompactTarget = min_compact_target >> 24;
		if (numZeros < numZerosInMinCompactTarget) {
			return min_compact_target;
		}

		// Get the compacted numerical value (loses precision)
		var compactValue = (uint)(target >> (int)(maxZeros - numZeros));

		// First 8 bits encodes the number of leading 0's and last 24 bits encode the bitwise inverted numerical value (preserve ordering).
		// Details:
		//  - (compactValue & bitmask) zero's the first 8 bits from value
		//  - the (... ^ bitmask) flips the bitsto preserve orderable property of compact target  (i.e. FF (large) becomes 00 (small)
		//
		const uint bitmask = (1 << 24) - 1; // 0x00FFFFFF in LE
		return (numZeros << 24) | ((compactValue & bitmask) ^ bitmask);
	}

	public uint FromDigest(ReadOnlySpan<byte> digest) {
		if (digest.Length > 32)
			throw new ArgumentOutOfRangeException(nameof(digest), "Must be 32 bytes");

		// Add 0-padding to target on the right until it's 32bytes
		var raw = new byte[32];
		Tools.Array.Fill(raw, (byte)0);
		for (var j = 0; j < digest.Length; j++) {
			raw[j + 32 - digest.Length] = digest[j];
		}
		return FromTarget(new BigInteger(raw, true, true));
	}

	public BigInteger ToTarget(uint compactTarget) {
		var numZeros = (compactTarget >> 24).ClipTo(_minZeros, _maxZeros);
		var compactValue = (compactTarget << 8) >> 8; // remove first byte 

		// XOR the compactValue prepend the implicit bit "1" 
		var uncompactedValue = (compactValue ^ _bitmask) | (_bitmask + 1);

		// Calculate target
		return new BigInteger(uncompactedValue) << (int)(_maxZeros - numZeros);
	}

	public void ToDigest(uint compactTaget, Span<byte> digest)
		=> ToDigest(ToTarget(compactTaget), digest);

	public void ToDigest(BigInteger targetBI, Span<byte> digest) {
		Guard.ArgumentInRange(digest.Length, 32, 32, nameof(digest));
		var bigIntBytes = targetBI.ToByteArray(isUnsigned: true, isBigEndian: true);
		digest.Fill(0);
		for (var i = 0; i < bigIntBytes.Length; i++) {
			digest[i + 32 - bigIntBytes.Length] = bigIntBytes[i];
		}
	}

	public uint AggregateWork(uint aggregation, uint newBlockCompactTarget) {
		return FromTarget(ToTarget(aggregation) + ToTarget(newBlockCompactTarget));
	}

}
