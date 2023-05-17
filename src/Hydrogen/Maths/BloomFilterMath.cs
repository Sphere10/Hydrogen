// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen {

	public static class BloomFilterMath {

		public static decimal EstimateBloomFilterError(long bloomFilterLength, long maximumExpectedItems, int hashRounds) {
			return (decimal)Math.Pow(1 - Math.Exp((double)-hashRounds * maximumExpectedItems / bloomFilterLength), hashRounds);
		}

		public static long EstimateBloomFilterLength(decimal targetError, long maxItems, int hashRounds) {
			return (int)Math.Round(-hashRounds * maxItems / Math.Log(1 - Math.Pow((double)targetError, 1.0D / hashRounds)));
		}
	}

}