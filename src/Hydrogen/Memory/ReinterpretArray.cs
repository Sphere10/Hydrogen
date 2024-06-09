// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Runtime.InteropServices;

namespace Hydrogen;

[StructLayout(LayoutKind.Explicit)]
public struct ReinterpretArray {

	[FieldOffset(0)] public byte[] AsByteArray;

	[FieldOffset(0)] public short[] AsInt16Array;

	[FieldOffset(0)] public ushort[] AsUInt16Array;

	[FieldOffset(0)] public int[] AsInt32Array;

	[FieldOffset(0)] public uint[] AsUInt32Array;

	[FieldOffset(0)] public long[] AsInt64Array;

	[FieldOffset(0)] public ulong[] AsUInt64Array;

	[FieldOffset(0)] public float[] AsFloatArray;

	[FieldOffset(0)] public double[] AsDoubleArray;

	public static ReinterpretArray From(byte[] bytes) {
		return new ReinterpretArray { AsByteArray = bytes };
	}

	public static ReinterpretArray From(short[] shorts) {
		return new ReinterpretArray { AsInt16Array = shorts };
	}

	public static ReinterpretArray From(ushort[] ushorts) {
		return new ReinterpretArray { AsUInt16Array = ushorts };
	}

	public static ReinterpretArray From(int[] ints) {
		return new ReinterpretArray { AsInt32Array = ints };
	}

	public static ReinterpretArray From(uint[] uints) {
		return new ReinterpretArray { AsUInt32Array = uints };
	}

	public static ReinterpretArray From(long[] longs) {
		return new ReinterpretArray { AsInt64Array = longs };
	}

	public static ReinterpretArray From(ulong[] ulongs) {
		return new ReinterpretArray { AsUInt64Array = ulongs };
	}

	public static ReinterpretArray From(float[] floats) {
		return new ReinterpretArray { AsFloatArray = floats };
	}

	public static ReinterpretArray From(double[] doubles) {
		return new ReinterpretArray { AsDoubleArray = doubles };
	}

}
