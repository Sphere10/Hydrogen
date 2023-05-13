// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Hydrogen {
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

		public static char NextAnsiChar(this Random random) => (char)random.Next(-127, 127);

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
			var reinterpretArray = new ReinterpretArray();
			reinterpretArray.AsByteArray = random.NextBytes(count * 4);
			int[] source = reinterpretArray.AsInt32Array;
			var result = new int[count];
			for (var i = 0; i < count; i++)
				result[i] = source[i];
			return result;
		}

		public static bool[] NextBools(this Random random, int count) =>
			Enumerable.Range(0, count)
				.Select(x => random.NextBool())
				.ToArray();

		public static ValueRange<int> NextRange(this Random rng, int maxLength, bool fromEndOnly = false, int? rangeLength = null) {
			Guard.ArgumentInRange(maxLength, 1, int.MaxValue, nameof(maxLength));
			return NextRangeBetween(rng, 0, maxLength - 1, fromEndOnly, rangeLength);
		}

		public static ValueRange<int> NextRangeBetween(this Random rng, int minIndex, int maxIndex, bool fromEndOnly = false, int? rangeLength = null) {
			Guard.Ensure(minIndex <= maxIndex, $"{nameof(minIndex)} must be smaller than or equal to {nameof(maxIndex)}");
			var length = maxIndex - minIndex + 1;

			if (!rangeLength.HasValue)
				rangeLength = rng.Next(0, length);
			else
				Guard.Ensure(rangeLength <= length, $"{nameof(rangeLength)} must be null or less than or equal to the length min the range");


			//    s       x********************y                 e
			//    <--A--->                     <-------B--------->
			//    s = minIndex  e = maxIndex   x = range start index     y = range end index
			//    A = x - s      B = e - y     A + B = length - rangeLength


			var remainingLength = length - rangeLength.Value;
			var A = rng.Next(0, remainingLength+1);
			var B = remainingLength - A;

			var endIX = fromEndOnly ? maxIndex : Math.Max(minIndex + A + rangeLength.Value - 1, 0);
			var startIX = (endIX - rangeLength.Value + 1).ClipTo(minIndex, endIX);
		
			return new ValueRange<int>(startIX, endIX, Comparer<int>.Default, true, true, checkOrder: false); 
		}

	}
}
