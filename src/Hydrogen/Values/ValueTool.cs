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
using System.Runtime.CompilerServices;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools;

public class Values {
	public const double MAX_SIMPSON_RECURSION = 100;
	public const char INFINITY_SYMBOL = '∞';
	public const string INFINITY_STRING = "∞";
	public const string NEGATIVE_INFINITY_STRING = "-∞";
	public const string POSITIVE_INFINITY_STRING = "+∞";
	public const string UNDEFINED_STRING = "undef";

	/// <summary>
	/// Visual Basic like 'With' statement.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool With<T>(T value, out T valueAsOut) {
		valueAsOut = value;
		return true;
	}

	/// <summary>
	/// Converts an integer into into another base. 
	/// </summary>
	/// <param name="number"></param>
	/// <param name="N">Base to convert <see cref="number"/> in</param>
	/// <returns>Array containing each digit in base <see cref="N"/> representation. The digits are represented as decimal numbers.</returns>
	public static uint[] ToBaseN(uint number, uint N) {
		var digitsList = new List<uint>();
		if (number == 0) {
			digitsList.Add(0);
		}
		while (number > 0) {
			digitsList.Add(number % N);
			number /= N;
		}
		digitsList.Reverse();
		return digitsList.ToArray();
	}

	public static T Min<T>(params T[] values) {
		return values.Min();
	}

	public static T Max<T>(params T[] values) {
		return values.Max();
	}

	public static int ClipValue(int value, int minValue, int maxValue) {
		if (value < minValue) {
			return minValue;
		} else if (value > maxValue) {
			return maxValue;
		}
		return value;
	}

	public static uint ClipValue(uint value, uint minValue, uint maxValue) {
		if (value < minValue) {
			return minValue;
		} else if (value > maxValue) {
			return maxValue;
		}
		return value;
	}

	public static long ClipValue(long value, long minValue, long maxValue) {
		if (value < minValue) {
			return minValue;
		} else if (value > maxValue) {
			return maxValue;
		}
		return value;
	}

	public static decimal ClipValue(decimal value, decimal minValue, decimal maxValue) {
		if (value < minValue) {
			return minValue;
		} else if (value > maxValue) {
			return maxValue;
		}
		return value;
	}

	public static float ClipValue(float value, float minValue, float maxValue) {
		if (value < minValue) {
			return minValue;
		} else if (value > maxValue) {
			return maxValue;
		}
		return value;
	}

	public static double ClipValue(double value, double minValue, double maxValue) {
		if (value < minValue) {
			return minValue;
		} else if (value > maxValue) {
			return maxValue;
		}
		return value;
	}

	public static void Swap<T>(ref T fromX, ref T fromY) {
		T temp = fromX;
		fromX = fromY;
		fromY = temp;
	}

	public static bool AnyUndefined(params double[] values) {
		bool retval = false;
		if (values != null) {
			if (values.Any(double.IsNaN)) {
				retval = true;
			}
		}
		return retval;
	}

	public static bool IsNumber(double d) {
		return !(double.IsNaN(d) || double.IsInfinity(d));
	}

	public static bool IsIn(double x, double lowerBound, double upperBound) {
		if (x < lowerBound || x > upperBound) {
			return false;
		}
		return true;
	}

	public static bool TryParseDoubleNiceString(string str, out double val) {
		bool retval = false;
		if (str == INFINITY_STRING || str == POSITIVE_INFINITY_STRING) {
			val = double.PositiveInfinity;
			retval = true;
		} else if (str == NEGATIVE_INFINITY_STRING) {
			val = double.NegativeInfinity;
			retval = true;
		} else if (str == UNDEFINED_STRING) {
			val = double.NaN;
			retval = true;
		} else {
			retval = double.TryParse(str, out val);
		}
		return retval;
	}

	public static string DoubleToNiceString(double val) {
		string retval = val.ToString();
		if (!IsNumber(val)) {
			if (double.IsNaN(val)) {
				retval = UNDEFINED_STRING;
			} else {
				retval = double.IsNegativeInfinity(val) ? NEGATIVE_INFINITY_STRING : POSITIVE_INFINITY_STRING;
			}
		}
		return retval;
	}

	public static string DoubleToShortNiceString(double val) {
		string retval = $"{val:N}";
		if (!IsNumber(val)) {
			if (double.IsNaN(val)) {
				retval = UNDEFINED_STRING;
			} else {
				retval = double.IsNegativeInfinity(val) ? NEGATIVE_INFINITY_STRING : POSITIVE_INFINITY_STRING;
			}
		}
		return retval;
	}


	public static class Future {

		public static ExplicitFuture<T> Explicit<T>() => new();

		public static ExplicitFuture<T> Explicit<T>(T value) => ExplicitFuture<T>.For(value);

		public static LazyLoad<T> LazyLoad<T>(Func<T> valueLoader) => new(valueLoader);

		public static Reloadable<T> Reloadable<T>(Func<T> valueLoader) => new(valueLoader);

		public static ProxyValue<T> AlwaysLoad<T>(Func<T> valueLoader) => new(valueLoader);

		public static BackgroundFetchedValue<T> BackgroundFetched<TSource, T>(IFuture<TSource> future, Func<T> valueLoader) => new(valueLoader);

		public static IFuture<T> Projection<TSource, T>(IFuture<TSource> future, Func<TSource, T> projection) => AlwaysLoad(() => projection(future.Value));

	}

}
