﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Maths;

public interface IRandomNumberGenerator {
	byte[] NextBytes(int count);
}


public static class IRandomNumberGeneratorExtensions {

	public static byte NextByte(this IRandomNumberGenerator randomNumberGenerator)
		=> randomNumberGenerator.NextBytes(1)[0];

	public static char NextAsciiChar(this IRandomNumberGenerator randomNumberGenerator)
		=> (char)randomNumberGenerator.NextByte();

	public static UInt16 NextUInt16(this IRandomNumberGenerator randomNumberGenerator)
		=> EndianBitConverter.Little.ToUInt16(randomNumberGenerator.NextBytes(2));

	public static UInt32 NextUInt32(this IRandomNumberGenerator randomNumberGenerator)
		=> EndianBitConverter.Little.ToUInt32(randomNumberGenerator.NextBytes(4));

	public static UInt64 NextUInt64(this IRandomNumberGenerator randomNumberGenerator)
		=> EndianBitConverter.Little.ToUInt16(randomNumberGenerator.NextBytes(8));

	public static float NextFloat(this IRandomNumberGenerator randomNumberGenerator) {
		return (float)(randomNumberGenerator.NextUInt32() * 4.6566128730773926E-010);
	}
	public static float NextUFloat(this IRandomNumberGenerator randomNumberGenerator) {
		return (float)(randomNumberGenerator.NextUInt32() * 2.32830643653869628906E-10);
	}

}
