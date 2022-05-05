using System;
using System.Numerics;

namespace Hydrogen.DApp.Core.Maths {

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

}

