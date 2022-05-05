using System;

namespace Hydrogen.Maths;

public interface ICRNG {
	byte[] NextBytes(int count);
}

public static class ICRNGExtensions {

	public static byte NextByte(this ICRNG randomNumberGenerator) 
		=> randomNumberGenerator.NextBytes(1)[0];

	public static char NextAsciiChar(this ICRNG randomNumberGenerator) 
		=> (char)randomNumberGenerator.NextByte();

	public static UInt16 NextUInt16(this ICRNG randomNumberGenerator) 
		=> EndianBitConverter.Little.ToUInt16(randomNumberGenerator.NextBytes(2));

	public static UInt32 NextUInt32(this ICRNG randomNumberGenerator) 
		=> EndianBitConverter.Little.ToUInt32(randomNumberGenerator.NextBytes(4));

	public static UInt64 NextUInt64(this ICRNG randomNumberGenerator) 
		=> EndianBitConverter.Little.ToUInt16(randomNumberGenerator.NextBytes(8));


}