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
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools;

public class Maths {
	public const float EPSILON_F = 0.000001f;
	public const double EPSILON_D = 0.000001d;
	public const decimal EPSILON_M = 0.000001m;

	public const double MAX_SIMPSON_RECURSION = 100;

	private static readonly Random _globalRandom = new Random(Environment.TickCount);

	[ThreadStatic] private static Random _threadRandom;

	public static Random RNG {
		get {
			if (_threadRandom != null)
				return _threadRandom;

			int seed;
			lock (_globalRandom) seed = _globalRandom.Next();
			_threadRandom = new Random(seed);
			return _threadRandom;
		}
	}

	public static float Abs(float value) {
		return value < 0 ? -value : value;
	}

	public static double Abs(double value) {
		return value < 0 ? -value : value;
	}

	public static decimal Abs(decimal value) {
		return value < 0 ? -value : value;
	}

	public static double EpsilonTrunc(double value) {
		//return (double)Math.Round((decimal)value, (int)Math.Log(1.0D / EPSILON_D, 10.0D));
		const double INVERSE_EPSILON = 1 / EPSILON_D;
		var whole = (long)value;
		var mantissa = (value - whole);
		return whole + (long)(mantissa * INVERSE_EPSILON) / INVERSE_EPSILON;
	}

	public static float EpsilonTrunc(float value) {
		//return (float)Math.Round((decimal)value, (int)Math.Log(1.0D / EPSILON_F, 10.0F));
		const float INVERSE_EPSILON = 1 / EPSILON_F;
		var whole = (long)value;
		var mantissa = (value - whole);
		return whole + (long)(mantissa * INVERSE_EPSILON) / INVERSE_EPSILON;
	}

	public static decimal EpsilonTrunc(decimal value) {
		//return Math.Round(value, (int)Math.Log(1.0D / EPSILON_D, 10.0D));
		const decimal INVERSE_EPSILON = 1 / EPSILON_M;
		var whole = (long)value;
		var mantissa = (value - whole);
		return whole + (long)(mantissa * INVERSE_EPSILON) / INVERSE_EPSILON;
	}

	public static float EpsilonClip(float val) {
		return Math.Abs(val) < EPSILON_F ? EPSILON_F : val;
	}

	public static double EpsilonClip(double val) {
		return Math.Abs(val) < EPSILON_D ? EPSILON_D : val;
	}

	public static decimal EpsilonClip(decimal val) {
		return Math.Abs(val) < EPSILON_M ? EPSILON_M : val;
	}

	public static void Rotate2D(double x, double y, double angle, out double x_bar, out double y_bar) {
		double c = System.Math.Cos(angle);
		double s = System.Math.Sin(angle);
		x_bar = c * x - s * y;
		y_bar = s * x + c * y;
	}

	public static void HyperbolicRotate2D(double x, double y, double angle, out double x_bar, out double y_bar) {
		double c = System.Math.Cosh(angle);
		double s = System.Math.Sinh(angle);
		x_bar = c * x - s * y;
		y_bar = s * x + c * y;
	}

	public static double Asinh(double x) {
		return Math.Log(x + Math.Sqrt(x * x + 1));
	}

	public static double Acosh(double x) {
		return Math.Log(x + Math.Sqrt(x * x - 1));
	}

	public static double Atanh(double x) {
		return 0.5 * Math.Log((1 + x) / (1 - x));
	}

	/// <summary>
	/// Same as ceiling except but also increments perfect integer parameters.
	/// </summary>
	/// <param name="arcLength"></param>
	/// <returns></returns>
	public static double NextInt(double x) {
		double y = System.Math.Ceiling(x);
		if (System.Math.Abs(y - x) < EPSILON_D) {
			y += 1.0;
		}
		return y;
	}

	public static bool Gamble(double winProbability) {
		return RNG.Next(1, 1000001) / 1000000.0D <= winProbability;
	}

	public static int GambleOdds(params double[] pieChances) {
		if (Math.Abs(pieChances.Sum() - 1.0D) > EPSILON_D)
			throw new ArgumentException("Must sum to 1.0D", nameof(pieChances));

		var draw = (double)RNG.Next(1, 1000001) / 1000000.0D;
		for (var i = 0; i < pieChances.Length; i++) {
			var start = i > 0 ? pieChances.Take(i - 1).Sum() : 0D;
			var end = start + pieChances[i];
			if (start <= draw && draw <= end)
				return i;
		}
		throw new Exception("Should not happen");
	}

	public static int EuclideanDistance(int x0, int y0, int x1, int y1) {
		return (int)Math.Round(EuclideanDistance((double)x0, (double)y0, (double)x1, (double)y1), 0);
	}

	public static float EuclideanDistance(float x0, float y0, float x1, float y1) {
		return (float)EuclideanDistance((double)x0, (double)y0, (double)x1, (double)y1);
	}

	public static double EuclideanDistance(double x0, double y0, double x1, double y1) {
		return Math.Sqrt(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2));
	}


	/// <summary>
	/// Enumerates all permutations of M-tuple indices of a collection of N elements.
	/// </summary>
	/// <param name="N">Total number of indexes available in collection</param>
	/// <param name="M">The number of indexes per permutation</param>
	/// <returns></returns>
	/// <remarks>Not memory efficient and can bloat for large N, M.</remarks>

	public static IEnumerable<long[]> GenerateIndexPermutations(long N, long M)
		=> IterateIndexPermutationsMemoryEfficiently(N, M).Select(x => x.ToArray());

	/// <summary>
	/// Iterates all permutations of M-tuple indices of a collection of N elements.
	/// </summary>
	/// <param name="N">Total number of indexes available in collection</param>
	/// <param name="M">The tuple size of indexes to fetch</param>
	/// <returns></returns>
	/// <remarks>Memory efficient implementation which yields same array instance in every iteration. Careful if converting an array/list as all elements will be same.</remarks>
	public static IEnumerable<long[]> IterateIndexPermutationsMemoryEfficiently(long N, long M) {
		Guard.ArgumentLTE(M, N, nameof(M));

		if (M == 0)
			yield break;

		var result = new long[M];
		var stack = new Stack<long>();
		stack.Push(0);

		while (stack.Count > 0) {
			var index = stack.Count - 1;
			var value = stack.Pop();

			while (value < N) {
				result[index++] = value;
				stack.Push(++value);

				if (index == M) {
					yield return result;
					break;
				}
			}
		}
	}
}
