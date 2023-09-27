// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Numerics;

namespace Hydrogen.DApp.Core.Maths;

public interface ICompactTargetAlgorithm {

	uint MinCompactTarget { get; }

	uint MaxCompactTarget { get; }

	uint FromTarget(BigInteger target);

	uint FromDigest(ReadOnlySpan<byte> digest);

	BigInteger ToTarget(uint compactTarget);

	void ToDigest(uint compactTaget, Span<byte> digest);

	void ToDigest(BigInteger targetBI, Span<byte> digest);

	uint AggregateWork(uint compactAggregation, uint newBlockCompactWork);
}


public static class ICompactTargetAlgorithmExtensions {
	public static byte[] ToDigest(this ICompactTargetAlgorithm alg, uint compactTaget) {
		var bytes = new byte[32];
		alg.ToDigest(compactTaget, bytes);
		return bytes;

	}

	public static byte[] ToDigest(this ICompactTargetAlgorithm alg, BigInteger target) {
		var bytes = new byte[32];
		alg.ToDigest(target, bytes);
		return bytes;
	}

}
