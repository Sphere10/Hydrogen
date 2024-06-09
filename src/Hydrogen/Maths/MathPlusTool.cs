// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools;

public class MathPlus {

	public static double Integrate(IFunction f, double a, double b) {
		return AdaptiveSimpsonsRule(f, a, b, 0.001);
	}

	public static double SafeIntegrate(IFunction f, double a, double b, double minRange, double maxRange) {
		return SafeAdaptiveSimpsonsRule(f, a, b, minRange, maxRange, 0.001);
	}

	public static double Derivative(IFunction f, double x) {
		return (f.Eval(x + Tools.Maths.EPSILON_D) - f.Eval(x)) / Tools.Maths.EPSILON_D;
	}

	public static double ArcLength(IFunction f, double a, double b) {
		return ArcLength(f, Metric.Euclidean, a, b);
	}

	public static double ArcLength(IFunction f, Metric metric, double a, double b) {

		return Integrate(ConstructArcLengthDerivativeForFunction(f, metric), a, b);
	}

	public static double SafeArcLength(IFunction f, double a, double b, double minRange, double maxRange) {
		return SafeArcLength(f, a, b, Metric.Euclidean, minRange, maxRange);
	}

	public static double SafeArcLength(IFunction f, double a, double b, Metric metric, double minRange, double maxRange) {
		return SafeIntegrate(ConstructArcLengthDerivativeForFunction(f, metric), a, b, minRange, maxRange);
	}

	public static IFunction ConstructArcLengthDerivativeForFunction(IFunction sourceFuntion, Metric metric) {
		switch (metric) {
			case Metric.Euclidean:
				return new ArcLengthDerivativeFunction(sourceFuntion);
			case Metric.SpacelikeMinkowski:
				return new SpacelikeMinkowskiDerivativeFunction(sourceFuntion);
			case Metric.TimelikeMinkowski:
				return new TimelikeMinkowskiDerivativeFunction(sourceFuntion);
			default:
				throw new ApplicationException($"Unsupported metric {metric}");
		}
	}

	#region Integration auxillary functions

	/// <summary>
	/// http://en.wikipedia.org/wiki/Adaptive_Simpson%27s_method
	/// </summary>
	/// <param name="f"></param>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	private static double SimpsonsRule(IFunction f, double a, double b) {
		double c = (a + b) / 2.0;
		double h3 = System.Math.Abs(b - a) / 6.0;
		return h3 * (f.Eval(a) + 4.0 * f.Eval(c) + f.Eval(b));
	}

	private static double RecursiveAdaptiveSimpsonsRule(IFunction f, double a, double b, double eps, double sum) {
		double c = (a + b) / 2.0;
		double left = SimpsonsRule(f, a, c);
		double right = SimpsonsRule(f, c, b);
		if (System.Math.Abs(left + right - sum) <= 15 * eps || !Tools.Values.IsNumber(sum)) {
			return left + right + (left + right - sum) / 15;
		}
		return RecursiveAdaptiveSimpsonsRule(f, a, c, eps / 2, left) + RecursiveAdaptiveSimpsonsRule(f, c, b, eps / 2, right);
	}

	/// <summary>
	/// Takes the simpson rule over the entire interval, and of the two half-intervals.
	/// If the sum of half-intervals exceed tolerance it indicates that we need to subdivide the intervals.
	/// </summary>
	/// <param name="f"></param>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="eps"></param>
	/// <returns></returns>
	private static double AdaptiveSimpsonsRule(IFunction f, double a, double b, double eps) {
		return RecursiveAdaptiveSimpsonsRule(f, a, b, eps, SimpsonsRule(f, a, b));
	}

	private static double SafeSimpsonsRule(IFunction f, double a, double b, double minRange, double maxRange, ref bool encounteredDiscontinuity) {
		double result = 0.0;
		double c = (a + b) / 2.0;
		double f_a = f.Eval(a);
		double f_b = f.Eval(b);
		double f_c = f.Eval(c);
		if (Tools.Values.IsIn(f_a, minRange, maxRange) && Tools.Values.IsIn(f_b, minRange, maxRange) && Tools.Values.IsIn(f_c, minRange, maxRange)) {
			encounteredDiscontinuity = false;
			var h3 = Tools.Maths.Abs(b - a) / 6.0;
			result = h3 * (f_a + 4.0 * f_c + f_b);
		} else {
			encounteredDiscontinuity = true;
		}
		return result;
	}

	private static double SafeRecursiveAdaptiveSimpsonsRule(IFunction f, double a, double b, double minRange, double maxRange, double eps, double sum) {
		var encounteredDiscontinuity = false;
		var c = (a + b) / 2.0;
		var left = SafeSimpsonsRule(f, a, c, minRange, maxRange, ref encounteredDiscontinuity);
		if (encounteredDiscontinuity) {
			return double.NaN;
		}

		var right = SafeSimpsonsRule(f, c, b, minRange, maxRange, ref encounteredDiscontinuity);
		if (encounteredDiscontinuity) {
			return double.NaN;
		}

		if (Tools.Maths.Abs(left + right - sum) <= 15 * eps || !Tools.Values.IsNumber(sum)) {
			return left + right + (left + right - sum) / 15;
		}
		return SafeRecursiveAdaptiveSimpsonsRule(f, a, c, minRange, maxRange, eps / 2, left) + SafeRecursiveAdaptiveSimpsonsRule(f, c, b, minRange, maxRange, eps / 2, right);
	}

	private static double SafeAdaptiveSimpsonsRule(IFunction f, double a, double b, double minRange, double maxRange, double eps) {
		return SafeRecursiveAdaptiveSimpsonsRule(f, a, b, minRange, maxRange, eps, SimpsonsRule(f, a, b));
	}

	#endregion

}
