using System;

namespace Sphere10.Framework {

	public static class BloomFilterMath {

		public static decimal EstimateBloomFilterError(long bloomFilterLength, long maximumExpectedItems, int hashRounds) {
			return (decimal)Math.Pow(1 - Math.Exp((double)-hashRounds * maximumExpectedItems / bloomFilterLength), hashRounds);
		}

		public static long EstimateBloomFilterLength(decimal targetError, long maxItems, int hashRounds) {
			return (int)Math.Round(-hashRounds * maxItems / Math.Log(1 - Math.Pow((double)targetError, 1.0D / hashRounds)));
		}
	}

}