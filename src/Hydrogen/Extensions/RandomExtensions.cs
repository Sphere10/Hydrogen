// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydrogen;

public static class RandomExtensions {

	/// <summary>
	/// Generate a random decimal between 0 (inclusive) and 1 (exclusive)
	/// </summary>
	public static decimal NextDecimal(this Random rng) {
		// Combine four random integers to fill the decimal's 128 bits.
		Span<int> parts = stackalloc int[4];
		parts[0] = rng.Next();
		parts[1] = rng.Next();
		parts[2] = rng.Next();
		parts[3] = rng.Next();
		
		// Construct a decimal from parts, with scaling to ensure it's between 0 and 1.
		// Note: Adjust the scaling factor based on how you combine the parts.
		return new decimal(parts[0], parts[1], parts[2], false, 28) / decimal.MaxValue;
	}


	/// <summary>
	/// Generates a random long, ensuring selected value approximates a 1/2^64 probability.
	/// </summary>
	public static long NextLong(this Random random) {
		Span<byte> buffer = stackalloc byte[8];
		random.NextBytes(buffer);
		return EndianBitConverter.Little.ToInt64(buffer);
	}

	/// <summary>
	/// Generates a random long, within a range, attempting uniform distribution by scaling a random normalized decimal (<see cref="NextDecimal"/>). Both arguments are inclusive.
	/// </summary>
	/// <remarks>Distribution is not uniform for very large ranges between <see cref="minValue"/> and <see cref="maxValue"/>.</remarks>
	public static long NextLong(this Random rng, long minValue, long maxValue) {
		Guard.ArgumentLT(minValue, maxValue, nameof(minValue), $"Must be less than or equal to argument '{nameof(maxValue)}'");

		// If no range, 
		if (minValue == maxValue)
			return minValue;

		// Generate a random decimal between 0 (inclusive) and 1 (exclusive)
		var randomValue = rng.NextDecimal();

		// Calculate range and scale
		var range = (decimal)maxValue - minValue + 1;
		var scaled = randomValue * range;

		// Result
		return minValue + (long)scaled;
	}

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

	public static byte NextByte(this Random random) => (byte)random.Next(0, byte.MaxValue + 1);

	public static bool NextBool(this Random random) {
		return random.Next(0, 2) > 0;
	}

	public static string NextString(this Random random, int size) {
		return Encoding.ASCII.GetString(random.NextBytes(size));
	}

	public static string NextString(this Random random, int minSize, int maxSize) {
		return Encoding.ASCII.GetString(random.NextBytes(random.Next(minSize, maxSize)));
	}


	/// <summary>
	/// Generates a byte array filled with random bytes.
	/// </summary>
	/// <param name="random">The instance of the Random class.</param>
	/// <param name="count">The number of random bytes to generate.</param>
	/// <returns>A byte array filled with 'count' random bytes.</returns>
	public static byte[] NextBytes(this Random random, int count)
	{
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
		var A = rng.Next(0, remainingLength + 1);
		var B = remainingLength - A;

		var endIX = fromEndOnly ? maxIndex : Math.Max(minIndex + A + rangeLength.Value - 1, 0);
		var startIX = (endIX - rangeLength.Value + 1).ClipTo(minIndex, endIX);

		return new ValueRange<int>(startIX, endIX, Comparer<int>.Default, true, true, checkOrder: false);
	}

}
