// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.
// Based off : https://github.com/asik/FixedMath.Net

using System;
using System.Runtime.CompilerServices;

namespace Hydrogen;

/// <summary>
/// Represents a Q31.32 fixed-point number.
/// </summary>
public partial struct FixedPoint : IEquatable<FixedPoint>, IComparable<FixedPoint> {
	const long MAX_VALUE = long.MaxValue;
	const long MIN_VALUE = long.MinValue;
	const int NUM_BITS = 64;
	const int FRACTIONAL_PLACES = 32;
	const long ONE = 1L << FRACTIONAL_PLACES;
	const long PI_TIMES_2 = 0x6487ED511;
	const long PI = 0x3243F6A88;
	const long PI_OVER_2 = 0x1921FB544;
	const long LN2 = 0xB17217F7;
	const long LOG2MAX = 0x1F00000000;
	const long LOG2MIN = -0x2000000000;
	const int LUT_SIZE = (int)(PI_OVER_2 >> 15);


	// Precision of this type is 2^-32, that is 2,3283064365386962890625E-10
	public static readonly decimal Precision = (decimal)(new FixedPoint(1L)); //0.00000000023283064365386962890625m;
	public static readonly FixedPoint MaxValue = new FixedPoint(MAX_VALUE);
	public static readonly FixedPoint MinValue = new FixedPoint(MIN_VALUE);
	public static readonly FixedPoint One = new FixedPoint(ONE);
	public static readonly FixedPoint Zero = new FixedPoint();
	public static readonly FixedPoint Pi = new FixedPoint(PI);
	public static readonly FixedPoint PiOver2 = new FixedPoint(PI_OVER_2);
	public static readonly FixedPoint PiTimes2 = new FixedPoint(PI_TIMES_2);
	public static readonly FixedPoint PiInv = (FixedPoint)0.3183098861837906715377675267M;
	public static readonly FixedPoint PiOver2Inv = (FixedPoint)0.6366197723675813430755350535M;
	private static readonly FixedPoint Log2Max = new FixedPoint(LOG2MAX);
	private static readonly FixedPoint Log2Min = new FixedPoint(LOG2MIN);
	private static readonly FixedPoint Ln2 = new FixedPoint(LN2);
	private static readonly FixedPoint LutInterval = (FixedPoint)(LUT_SIZE - 1) / PiOver2;


	readonly long _rawValue;

	public FixedPoint(int value) {
		_rawValue = value * ONE;
	}

	/// <summary>
	/// This is the constructor from raw value; it can only be used interally.
	/// </summary>
	/// <param name="rawValue"></param>
	FixedPoint(long rawValue) {
		_rawValue = rawValue;
	}


	public long RawValue => _rawValue;


	/// <summary>
	/// Returns a number indicating the sign of a Fix64 number.
	/// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
	/// </summary>
	public static int Sign(FixedPoint value) {
		return
			value._rawValue < 0 ? -1 :
			value._rawValue > 0 ? 1 :
			0;
	}


	/// <summary>
	/// Returns the absolute value of a Fix64 number.
	/// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
	/// </summary>
	public static FixedPoint Abs(FixedPoint value) {
		if (value._rawValue == MIN_VALUE) {
			return MaxValue;
		}

		// branchless implementation, see http://www.strchr.com/optimized_abs_function
		var mask = value._rawValue >> 63;
		return new FixedPoint((value._rawValue + mask) ^ mask);
	}

	/// <summary>
	/// Returns the absolute value of a Fix64 number.
	/// FastAbs(Fix64.MinValue) is undefined.
	/// </summary>
	public static FixedPoint FastAbs(FixedPoint value) {
		// branchless implementation, see http://www.strchr.com/optimized_abs_function
		var mask = value._rawValue >> 63;
		return new FixedPoint((value._rawValue + mask) ^ mask);
	}


	/// <summary>
	/// Returns the largest integer less than or equal to the specified number.
	/// </summary>
	public static FixedPoint Floor(FixedPoint value) {
		// Just zero out the fractional part
		return new FixedPoint((long)((ulong)value._rawValue & 0xFFFFFFFF00000000));
	}

	/// <summary>
	/// Returns the smallest integral value that is greater than or equal to the specified number.
	/// </summary>
	public static FixedPoint Ceiling(FixedPoint value) {
		var hasFractionalPart = (value._rawValue & 0x00000000FFFFFFFF) != 0;
		return hasFractionalPart ? Floor(value) + One : value;
	}

	/// <summary>
	/// Rounds a value to the nearest integral value.
	/// If the value is halfway between an even and an uneven value, returns the even value.
	/// </summary>
	public static FixedPoint Round(FixedPoint value) {
		var fractionalPart = value._rawValue & 0x00000000FFFFFFFF;
		var integralPart = Floor(value);
		if (fractionalPart < 0x80000000) {
			return integralPart;
		}
		if (fractionalPart > 0x80000000) {
			return integralPart + One;
		}
		// if number is halfway between two values, round to the nearest even number
		// this is the method used by System.Math.Round().
		return (integralPart._rawValue & ONE) == 0
			? integralPart
			: integralPart + One;
	}

	/// <summary>
	/// Adds x and y. Performs saturating addition, i.e. in case of overflow, 
	/// rounds to MinValue or MaxValue depending on sign of operands.
	/// </summary>
	public static FixedPoint operator +(FixedPoint x, FixedPoint y) {
		var xl = x._rawValue;
		var yl = y._rawValue;
		var sum = xl + yl;
		// if signs of operands are equal and signs of sum and x are different
		if (((~(xl ^ yl) & (xl ^ sum)) & MIN_VALUE) != 0) {
			sum = xl > 0 ? MAX_VALUE : MIN_VALUE;
		}
		return new FixedPoint(sum);
	}

	/// <summary>
	/// Adds x and y witout performing overflow checking. Should be inlined by the CLR.
	/// </summary>
	public static FixedPoint FastAdd(FixedPoint x, FixedPoint y) {
		return new FixedPoint(x._rawValue + y._rawValue);
	}

	/// <summary>
	/// Subtracts y from x. Performs saturating substraction, i.e. in case of overflow, 
	/// rounds to MinValue or MaxValue depending on sign of operands.
	/// </summary>
	public static FixedPoint operator -(FixedPoint x, FixedPoint y) {
		var xl = x._rawValue;
		var yl = y._rawValue;
		var diff = xl - yl;
		// if signs of operands are different and signs of sum and x are different
		if ((((xl ^ yl) & (xl ^ diff)) & MIN_VALUE) != 0) {
			diff = xl < 0 ? MIN_VALUE : MAX_VALUE;
		}
		return new FixedPoint(diff);
	}

	/// <summary>
	/// Subtracts y from x witout performing overflow checking. Should be inlined by the CLR.
	/// </summary>
	public static FixedPoint FastSub(FixedPoint x, FixedPoint y) {
		return new FixedPoint(x._rawValue - y._rawValue);
	}

	static long AddOverflowHelper(long x, long y, ref bool overflow) {
		var sum = x + y;
		// x + y overflows if sign(x) ^ sign(y) != sign(sum)
		overflow |= ((x ^ y ^ sum) & MIN_VALUE) != 0;
		return sum;
	}

	public static FixedPoint operator *(FixedPoint x, FixedPoint y) {

		var xl = x._rawValue;
		var yl = y._rawValue;

		var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
		var xhi = xl >> FRACTIONAL_PLACES;
		var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
		var yhi = yl >> FRACTIONAL_PLACES;

		var lolo = xlo * ylo;
		var lohi = (long)xlo * yhi;
		var hilo = xhi * (long)ylo;
		var hihi = xhi * yhi;

		var loResult = lolo >> FRACTIONAL_PLACES;
		var midResult1 = lohi;
		var midResult2 = hilo;
		var hiResult = hihi << FRACTIONAL_PLACES;

		bool overflow = false;
		var sum = AddOverflowHelper((long)loResult, midResult1, ref overflow);
		sum = AddOverflowHelper(sum, midResult2, ref overflow);
		sum = AddOverflowHelper(sum, hiResult, ref overflow);

		bool opSignsEqual = ((xl ^ yl) & MIN_VALUE) == 0;

		// if signs of operands are equal and sign of result is negative,
		// then multiplication overflowed positively
		// the reverse is also true
		if (opSignsEqual) {
			if (sum < 0 || (overflow && xl > 0)) {
				return MaxValue;
			}
		} else {
			if (sum > 0) {
				return MinValue;
			}
		}

		// if the top 32 bits of hihi (unused in the result) are neither all 0s or 1s,
		// then this means the result overflowed.
		var topCarry = hihi >> FRACTIONAL_PLACES;
		if (topCarry != 0 && topCarry != -1 /*&& xl != -17 && yl != -17*/) {
			return opSignsEqual ? MaxValue : MinValue;
		}

		// If signs differ, both operands' magnitudes are greater than 1,
		// and the result is greater than the negative operand, then there was negative overflow.
		if (!opSignsEqual) {
			long posOp, negOp;
			if (xl > yl) {
				posOp = xl;
				negOp = yl;
			} else {
				posOp = yl;
				negOp = xl;
			}
			if (sum > negOp && negOp < -ONE && posOp > ONE) {
				return MinValue;
			}
		}

		return new FixedPoint(sum);
	}

	/// <summary>
	/// Performs multiplication without checking for overflow.
	/// Useful for performance-critical code where the values are guaranteed not to cause overflow
	/// </summary>
	public static FixedPoint FastMul(FixedPoint x, FixedPoint y) {

		var xl = x._rawValue;
		var yl = y._rawValue;

		var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
		var xhi = xl >> FRACTIONAL_PLACES;
		var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
		var yhi = yl >> FRACTIONAL_PLACES;

		var lolo = xlo * ylo;
		var lohi = (long)xlo * yhi;
		var hilo = xhi * (long)ylo;
		var hihi = xhi * yhi;

		var loResult = lolo >> FRACTIONAL_PLACES;
		var midResult1 = lohi;
		var midResult2 = hilo;
		var hiResult = hihi << FRACTIONAL_PLACES;

		var sum = (long)loResult + midResult1 + midResult2 + hiResult;
		return new FixedPoint(sum);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static int CountLeadingZeroes(ulong x) {
		int result = 0;
		while ((x & 0xF000000000000000) == 0) {
			result += 4;
			x <<= 4;
		}
		while ((x & 0x8000000000000000) == 0) {
			result += 1;
			x <<= 1;
		}
		return result;
	}

	public static FixedPoint operator /(FixedPoint x, FixedPoint y) {
		var xl = x._rawValue;
		var yl = y._rawValue;

		if (yl == 0) {
			throw new DivideByZeroException();
		}

		var remainder = (ulong)(xl >= 0 ? xl : -xl);
		var divider = (ulong)(yl >= 0 ? yl : -yl);
		var quotient = 0UL;
		var bitPos = NUM_BITS / 2 + 1;


		// If the divider is divisible by 2^n, take advantage of it.
		while ((divider & 0xF) == 0 && bitPos >= 4) {
			divider >>= 4;
			bitPos -= 4;
		}

		while (remainder != 0 && bitPos >= 0) {
			int shift = CountLeadingZeroes(remainder);
			if (shift > bitPos) {
				shift = bitPos;
			}
			remainder <<= shift;
			bitPos -= shift;

			var div = remainder / divider;
			remainder = remainder % divider;
			quotient += div << bitPos;

			// Detect overflow
			if ((div & ~(0xFFFFFFFFFFFFFFFF >> bitPos)) != 0) {
				return ((xl ^ yl) & MIN_VALUE) == 0 ? MaxValue : MinValue;
			}

			remainder <<= 1;
			--bitPos;
		}

		// rounding
		++quotient;
		var result = (long)(quotient >> 1);
		if (((xl ^ yl) & MIN_VALUE) != 0) {
			result = -result;
		}

		return new FixedPoint(result);
	}

	public static FixedPoint operator %(FixedPoint x, FixedPoint y) {
		return new FixedPoint(
			x._rawValue == MIN_VALUE & y._rawValue == -1 ? 0 : x._rawValue % y._rawValue);
	}

	/// <summary>
	/// Performs modulo as fast as possible; throws if x == MinValue and y == -1.
	/// Use the operator (%) for a more reliable but slower modulo.
	/// </summary>
	public static FixedPoint FastMod(FixedPoint x, FixedPoint y) {
		return new FixedPoint(x._rawValue % y._rawValue);
	}

	public static FixedPoint operator -(FixedPoint x) {
		return x._rawValue == MIN_VALUE ? MaxValue : new FixedPoint(-x._rawValue);
	}

	public static bool operator ==(FixedPoint x, FixedPoint y) {
		return x._rawValue == y._rawValue;
	}

	public static bool operator !=(FixedPoint x, FixedPoint y) {
		return x._rawValue != y._rawValue;
	}

	public static bool operator >(FixedPoint x, FixedPoint y) {
		return x._rawValue > y._rawValue;
	}

	public static bool operator <(FixedPoint x, FixedPoint y) {
		return x._rawValue < y._rawValue;
	}

	public static bool operator >=(FixedPoint x, FixedPoint y) {
		return x._rawValue >= y._rawValue;
	}

	public static bool operator <=(FixedPoint x, FixedPoint y) {
		return x._rawValue <= y._rawValue;
	}

	/// <summary>
	/// Returns 2 raised to the specified power.
	/// Provides at least 6 decimals of accuracy.
	/// </summary>
	internal static FixedPoint Pow2(FixedPoint x) {
		if (x._rawValue == 0) {
			return One;
		}

		// Avoid negative arguments by exploiting that exp(-x) = 1/exp(x).
		bool neg = x._rawValue < 0;
		if (neg) {
			x = -x;
		}

		if (x == One) {
			return neg ? One / 2 : 2;
		}
		if (x >= Log2Max) {
			return neg ? One / MaxValue : MaxValue;
		}
		if (x <= Log2Min) {
			return neg ? MaxValue : Zero;
		}

		/* The algorithm is based on the power series for exp(x):
		 * http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
		 * 
		 * From term n, we get term n+1 by multiplying with x/n.
		 * When the sum term drops to zero, we can stop summing.
		 */

		int integerPart = (int)Floor(x);
		// Take fractional part of exponent
		x = new FixedPoint(x._rawValue & 0x00000000FFFFFFFF);

		var result = One;
		var term = One;
		int i = 1;
		while (term._rawValue != 0) {
			term = FastMul(FastMul(x, term), Ln2) / i;
			result += term;
			i++;
		}

		result = FromRaw(result._rawValue << integerPart);
		if (neg) {
			result = One / result;
		}

		return result;
	}

	/// <summary>
	/// Returns the base-2 logarithm of a specified number.
	/// Provides at least 9 decimals of accuracy.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">
	/// The argument was non-positive
	/// </exception>
	internal static FixedPoint Log2(FixedPoint x) {
		if (x._rawValue <= 0) {
			throw new ArgumentOutOfRangeException("Non-positive value passed to Ln", "x");
		}

		// This implementation is based on Clay. S. Turner's fast binary logarithm
		// algorithm (C. S. Turner,  "A Fast Binary Logarithm Algorithm", IEEE Signal
		//     Processing Mag., pp. 124,140, Sep. 2010.)

		long b = 1U << (FRACTIONAL_PLACES - 1);
		long y = 0;

		long rawX = x._rawValue;
		while (rawX < ONE) {
			rawX <<= 1;
			y -= ONE;
		}

		while (rawX >= (ONE << 1)) {
			rawX >>= 1;
			y += ONE;
		}

		var z = new FixedPoint(rawX);

		for (int i = 0; i < FRACTIONAL_PLACES; i++) {
			z = FastMul(z, z);
			if (z._rawValue >= (ONE << 1)) {
				z = new FixedPoint(z._rawValue >> 1);
				y += b;
			}
			b >>= 1;
		}

		return new FixedPoint(y);
	}

	/// <summary>
	/// Returns the natural logarithm of a specified number.
	/// Provides at least 7 decimals of accuracy.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">
	/// The argument was non-positive
	/// </exception>
	public static FixedPoint Ln(FixedPoint x) {
		return FastMul(Log2(x), Ln2);
	}

	/// <summary>
	/// Returns a specified number raised to the specified power.
	/// Provides about 5 digits of accuracy for the result.
	/// </summary>
	/// <exception cref="DivideByZeroException">
	/// The base was zero, with a negative exponent
	/// </exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// The base was negative, with a non-zero exponent
	/// </exception>
	public static FixedPoint Pow(FixedPoint b, FixedPoint exp) {
		if (b == One) {
			return One;
		}
		if (exp._rawValue == 0) {
			return One;
		}
		if (b._rawValue == 0) {
			if (exp._rawValue < 0) {
				throw new DivideByZeroException();
			}
			return Zero;
		}

		FixedPoint log2 = Log2(b);
		return Pow2(exp * log2);
	}

	public static FixedPoint Exp(FixedPoint x) {
		return Pow2(x / Ln2);
	}

	public static FixedPoint Sqrt(FixedPoint x) {
		var xl = x._rawValue;
		if (xl < 0) {
			// We cannot represent infinities like Single and Double, and Sqrt is
			// mathematically undefined for x < 0. So we just throw an exception.
			throw new ArgumentOutOfRangeException("Negative value passed to Sqrt", "x");
		}

		var num = (ulong)xl;
		var result = 0UL;

		// second-to-top bit
		var bit = 1UL << (NUM_BITS - 2);

		while (bit > num) {
			bit >>= 2;
		}

		// The main part is executed twice, in order to avoid
		// using 128 bit values in computations.
		for (var i = 0; i < 2; ++i) {
			// First we get the top 48 bits of the answer.
			while (bit != 0) {
				if (num >= result + bit) {
					num -= result + bit;
					result = (result >> 1) + bit;
				} else {
					result = result >> 1;
				}
				bit >>= 2;
			}

			if (i == 0) {
				// Then process it again to get the lowest 16 bits.
				if (num > (1UL << (NUM_BITS / 2)) - 1) {
					// The remainder 'num' is too large to be shifted left
					// by 32, so we have to add 1 to result manually and
					// adjust 'num' accordingly.
					// num = a - (result + 0.5)^2
					//       = num + result^2 - (result + 0.5)^2
					//       = num - result - 0.5
					num -= result;
					num = (num << (NUM_BITS / 2)) - 0x80000000UL;
					result = (result << (NUM_BITS / 2)) + 0x80000000UL;
				} else {
					num <<= (NUM_BITS / 2);
					result <<= (NUM_BITS / 2);
				}

				bit = 1UL << (NUM_BITS / 2 - 2);
			}
		}
		// Finally, if next bit would have been 1, round the result upwards.
		if (num > result) {
			++result;
		}
		return new FixedPoint((long)result);
	}

	public override bool Equals(object obj) {
		return obj is FixedPoint && ((FixedPoint)obj)._rawValue == _rawValue;
	}

	public override int GetHashCode() {
		return _rawValue.GetHashCode();
	}

	public bool Equals(FixedPoint other) {
		return _rawValue == other._rawValue;
	}

	public int CompareTo(FixedPoint other) {
		return _rawValue.CompareTo(other._rawValue);
	}

	public override string ToString() {
		// Up to 10 decimal places
		return ((decimal)this).ToString("0.##########");
	}

	public static FixedPoint FromRaw(long rawValue) {
		return new FixedPoint(rawValue);
	}

	public static implicit operator FixedPoint(long value) {
		return new FixedPoint(value * ONE);
	}
	public static implicit operator long(FixedPoint value) {
		return value._rawValue >> FRACTIONAL_PLACES;
	}
	public static implicit operator FixedPoint(float value) {
		return new FixedPoint((long)(value * ONE));
	}
	public static implicit operator float(FixedPoint value) {
		return (float)value._rawValue / ONE;
	}
	public static implicit operator FixedPoint(double value) {
		return new FixedPoint((long)(value * ONE));
	}
	public static implicit operator double(FixedPoint value) {
		return (double)value._rawValue / ONE;
	}
	public static implicit operator FixedPoint(decimal value) {
		return new FixedPoint((long)(value * ONE));
	}
	public static implicit operator decimal(FixedPoint value) {
		return (decimal)value._rawValue / ONE;
	}


}
