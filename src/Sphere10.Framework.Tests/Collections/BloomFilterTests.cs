//-----------------------------------------------------------------------
// <copyright file="LargebinaryFileTests.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.IO;
using System.Text;
using NUnit.Framework.Constraints;
using Sphere10.Framework;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.UnitTests {

    [TestFixture]
    public class BloomFilterTests {

		[Test]
		public void Test_Consistency([Values(0.01, 0.05, 0.1, 0.5)] decimal targetError,
									[Values(100, 1000)]  int maxExpectedItems,
									[Values(1,2,3)] int hashRounds,
									[Values] BloomFilterType bloomFilterType) {
			var filter = CreateFilter(targetError, maxExpectedItems, hashRounds, bloomFilterType);
			var RNG = new Random(31337);
			var data = RNG.NextString(100);
			filter.Add(data);
			var expected = filter.ToArray();
			for (var i = 0; i < 5; i++) {
				filter.Add(data);
				Assert.AreEqual(expected, filter.ToArray()); // same data doesnt change filter
			}
		}

		[Test]
		public void Test_FalseNegatives([Values(0.01, 0.05, 0.1, 0.5)] decimal targetError,
									[Values(100)]  int maxExpectedItems,
									[Values(1, 2, 3)] int hashRounds,
									[Values] BloomFilterType bloomFilterType) {
			var filter = CreateFilter(targetError, maxExpectedItems, hashRounds, bloomFilterType);
			var RNG = new Random(31337);
			for (var i = 0; i < maxExpectedItems; i++) {
				var data = RNG.NextString(10, 50);
				filter.Add(data);
				Assert.IsTrue(filter.Contains(data));
			}
		}

		[Test]
        public void IntegrationTest([Values(0.01, 0.05, 0.1, 0.5)] decimal targetError, 
									[Values(100, 1000, 10000)]  int maxExpectedItems, 
									[Values(2,3,5)] int hashRounds, 
									[Values] BloomFilterType bloomFilterType) {
			const int FalsePositiveSampleSize = 10000;

			var filter = CreateFilter(targetError, maxExpectedItems, hashRounds, bloomFilterType);
			var RNG = new Random(31337);
			var data = Tools.Collection.Generate(() => RNG.NextString(10, 50)).Take(maxExpectedItems).ToArray();
			foreach(var datum in data) { 
				filter.Add(datum);
				Assert.IsTrue(filter.Contains(datum));
			}

			// No false negatives
			var falseNegatives = data.Count(x => !filter.Contains(x));
			Assert.AreEqual(0, falseNegatives);

			// False positive error rate is tolerable
			var diffData = Enumerable.Range(0, int.MaxValue).Zip(data.Loop(), (i, s) => $"__{i}_{s}").Take(FalsePositiveSampleSize).ToArray();
			var falsePositives = diffData.Count(filter.Contains);
			var actualError = falsePositives / (decimal)diffData.Length;
			var errorUpperBound = targetError + 0.05M; // allow 5% tolerance for testing (happens when filter is small)
			Assert.LessOrEqual(actualError, errorUpperBound);
		}

		public enum BloomFilterType {
			MurMur3,
			HashBased_SHA2_256
		}

		private IBloomFilter<string> CreateFilter(decimal targetError, int maxExpectedItems, int hashRounds, BloomFilterType bloomFilterType) {
			switch (bloomFilterType) {
				case BloomFilterType.MurMur3:
					return new MurMur3BloomFilter<string>(targetError, maxExpectedItems, hashRounds, new StringSerializer(Encoding.ASCII));
				case BloomFilterType.HashBased_SHA2_256:
					return new HashedBloomFilter<string>(targetError, maxExpectedItems, hashRounds, CHF.SHA2_256, new StringSerializer(Encoding.ASCII));
				default:
					throw new ArgumentOutOfRangeException(nameof(bloomFilterType), bloomFilterType, null);
			}
		}
	}

}
