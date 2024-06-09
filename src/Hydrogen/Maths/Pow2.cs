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

namespace Hydrogen;

public static class Pow2 {

	public static IEnumerable<int> CalculatePow2Partition(long number) {
		while (number >= 1) {
			var log2 = Tools.Maths.EpsilonTrunc(Math.Log(number, 2));
			var x = Math.Floor(log2); // x of 2^x of this term
			yield return (int)x;

			if (log2 == x) // safe FP comparison due to trunc & floor
				yield break; // end of sum (no more terms detected)
			number -= 1 << (int)x; // (int)Math.Pow(2, x);
		}
	}

	public static void Reduce(IList<int> exponents) {
		var finished = false;
		restart:
		while (!finished) {
			for (var i = exponents.Count - 1; i > 0; i--) {
				var lexp = exponents[i - 1];
				var rexp = exponents[i];
				if (lexp < rexp) {
					exponents.Swap(i - 1, i);
					goto restart;
				}
				if (lexp == rexp) {
					exponents[i - 1]++;
					exponents.RemoveAt(i);
					goto restart;
				}

			}
			finished = true;
		}
	}

	/// <summary>
	/// Adds a leaf node to a set of sub-roots.
	/// </summary>
	public static IEnumerable<int> AddOne(IEnumerable<int> pow2Partition) {
		var exponents = new List<int>(pow2Partition);
		var newExponent = 0;
		while (true) {
			if (exponents.Count == 0 || exponents[^1] > newExponent) {
				exponents.Add(newExponent);
				break;
			}
			newExponent = exponents[^1] + 1;
			exponents.RemoveAt(^1);
		}
		return exponents;
	}

	/// <summary>
	/// Adds a leaf node to a set of sub-roots.
	/// </summary>
	public static IEnumerable<int> Add(IEnumerable<int> left, IEnumerable<int> right) {
		var exponents = new List<int>();
		exponents.AddRange(left);
		exponents.AddRange(right);
		Reduce(exponents);
		return exponents;
	}

	/// <summary>
	/// Adds a leaf node to a set of sub-roots.
	/// </summary>
	public static IEnumerable<int> Mul(IEnumerable<int> left, IEnumerable<int> right) {
		var exponents = new List<int>();
		exponents.AddRange(left.SelectMany(x => right, (x, y) => x + y));
		Reduce(exponents);
		return exponents;
	}
}
