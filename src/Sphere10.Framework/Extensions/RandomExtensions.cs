//-----------------------------------------------------------------------
// <copyright file="RandomExtensions.cs" company="Sphere 10 Software">
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
using System.Text;

namespace Sphere10.Framework {
	public static class RandomExtensions {
		public static int NextIn(this Random random, int minInclusive, int maxInclusive) {
			if (minInclusive == int.MinValue && maxInclusive == int.MaxValue) {
				return random.Next();
			} else if (minInclusive > int.MinValue && maxInclusive == int.MaxValue) {
				return random.Next(minInclusive - 1, maxInclusive) + 1;
			} else {
				return random.Next(minInclusive, maxInclusive + 1);
			}
		}

		public static bool NextBool(this Random random) {
			return random.Next(0, 2) > 0;
		}

		public static string NextString(this Random random, int size) {
			return Encoding.ASCII.GetString(random.NextBytes(size));
		}

		public static string NextString(this Random random, int minSize, int maxSize) {
			return Encoding.ASCII.GetString(random.NextBytes(random.Next(minSize, maxSize)));
		}

		public static byte[] NextBytes(this Random random, int count) {
            var buff = new byte[count];
            random.NextBytes(buff);
            return buff;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="random"></param>
		/// <param name="size"></param>
		/// <param name="minCount"></param>
		/// <param name="maxCount">Exclusive upper bound</param>
		/// <returns></returns>
		public static byte[][] NextByteArrays(this Random random, int size, int minCount, int maxCount) 
			=> NextByteArrays(random, size, random.Next(minCount, maxCount));

		public static byte[][] NextByteArrays(this Random random, int size, int count) 
			=> Tools.Collection.Generate(() => random.NextBytes(size)).Take(count).ToArray();

		public static int[] NextInts(this Random random, int count) {
			var xxx = new ReinterpretArray();
			xxx.AsByteArray = random.NextBytes(count * 4);
			int[] source = xxx.AsInt32Array;
			var result = new int[count];
			for (var i = 0; i < count; i++)
				result[i] = source[i];
			return result;
		}


		public static ValueRange<int> RandomRange(this Random rng, int count) {
			var index1 = rng.Next(0, count);
			var index2 = rng.Next(0, count);
			return new ValueRange<int>(Math.Min(index1, index2), Math.Max(index1, index2), Comparer<int>.Default, true, true);
		}

		public static ValueRange<int> RandomSegment(this Random rng, int collectionSize, int segmentSize) {
			if (collectionSize < 0)
				throw new ArgumentOutOfRangeException(nameof(collectionSize), collectionSize, "Must be positive");
			var size = collectionSize - segmentSize;
			if (size < 0)
				throw new ArgumentOutOfRangeException(nameof(segmentSize), segmentSize, "Cannot fit in collection size");
			var index = rng.Next(0, size);
		
			return new ValueRange<int>(index, index + segmentSize - 1);
		}
	}
}
