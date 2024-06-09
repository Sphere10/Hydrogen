// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.InteropServices;

namespace Hydrogen;

public static class FastLog {
	[StructLayout(LayoutKind.Explicit)]
	private struct Ieee754 {
		[FieldOffset(0)] public float Single;
		[FieldOffset(0)] public uint UnsignedBits;
		[FieldOffset(0)] public int SignedBits;

		public uint Sign {
			get { return UnsignedBits >> 31; }
		}

		public int Exponent {
			get { return (SignedBits >> 23) & 0xFF; }
		}

		public uint Mantissa {
			get { return UnsignedBits & 0x007FFFFF; }
		}
	}


	private static readonly float[] MantissaLogs = new float[(int)System.Math.Pow(2, 23)];
	private const float Base10 = 3.321928F;
	private const float BaseE = 1.442695F;

	static FastLog() {
		//creating lookup table
		for (uint i = 0; i < MantissaLogs.Length; i++) {
			var n = new Ieee754 { UnsignedBits = i | 0x3F800000 }; //added the implicit 1 leading bit
			MantissaLogs[i] = (float)Math.Log(n.Single, 2);
		}
	}

	public static float Log2(float value) {
		if (value == 0F)
			return float.NegativeInfinity;

		var number = new Ieee754 { Single = value };

		if (number.UnsignedBits >> 31 == 1) //NOTE: didn't call Sign property for higher performance
			return float.NaN;

		return (((number.SignedBits >> 23) & 0xFF) - 127) + MantissaLogs[number.UnsignedBits & 0x007FFFFF];
		//NOTE: didn't call Exponent and Mantissa properties for higher performance
	}

	public static float Log10(float value) {
		return Log2(value) / Base10;
	}

	public static float Ln(float value) {
		return Log2(value) / BaseE;
	}

	public static float Log(float value, float valueBase) {
		return Log2(value) / Log2(valueBase);
	}
}
