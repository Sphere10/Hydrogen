// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class UrlID {

	public static string Generate(uint id, Format format = Format.Base62) {
		var permutedID = PermuteId(id);
		switch (format) {
			case Format.HexLE:
				return EndianBitConverter.Little.GetBytes(permutedID).ToHexString(true);
			case Format.HexBE:
				return EndianBitConverter.Big.GetBytes(permutedID).ToHexString(true);
			case Format.Base62:
				return Base62Encoding.ToBase62String(permutedID);
			default:
				throw new ArgumentOutOfRangeException(nameof(format), format, null);
		}

	}

	private static double RoundFunction(uint input) {
		// Must be a function in the mathematical sense (x=y implies f(x)=f(y))
		// but it doesn't have to be reversible.
		// Must return a value between 0 and 1
		return ((1369 * input + 150889) % 714025) / 714025.0;
	}


	public static uint PermuteId(uint id) {
		uint l1 = (id >> 16) & 65535;
		uint r1 = id & 65535;
		uint l2, r2;
		for (int i = 0; i < 3; i++) {
			l2 = r1;
			r2 = l1 ^ (uint)(RoundFunction(r1) * 65535);
			l1 = l2;
			r1 = r2;
		}
		return ((r1 << 16) + l1);
	}


	public enum Format {
		Base62 = 1,
		HexLE,
		HexBE
	}

}
