// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Hydrogen;

/// <summary>
/// Utility that read and write binary bits to and from byte arrays
/// </summary>
public class Bits {

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPositivePowerOf2(int value)
		=> 0 < value && 0 == (value & (value - 1));


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte ReverseBits(byte value)
		=> (byte)((((ulong)value * 0x80200802) & 0x884422110) * 0x101010101 >> 32);


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint ReadBinaryNumber(ReadOnlySpan<byte> source, int sourceBitOffset, int binaryDigitsToRead, IterateDirection direction) {
		var dest = new byte[sizeof(uint)];
		CopyBits(source, sourceBitOffset, dest, sizeof(uint) * 8 - 1, binaryDigitsToRead, direction, IterateDirection.RightToLeft);
		return EndianBitConverter.Big.ToUInt32(dest);
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBinaryNumber(uint number, Span<byte> dest, int destBitOffset, int binaryDigitsToWrite, IterateDirection direction) {
		var source = EndianBitConverter.Big.GetBytes(number);
		CopyBits(source, sizeof(uint) * 8 - 1, dest, destBitOffset, binaryDigitsToWrite, IterateDirection.RightToLeft, direction);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong RotateRight(ulong value, int nBits) {
		return (value >> nBits) | (value << (64 - nBits));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte RotateLeft8(byte b, int i) {
		Debug.Assert(i >= 0);
		i &= 7;
		return (byte) ((b << i) | (b >> (8 - i)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte RotateRight8(byte b, int i) {
		Debug.Assert(i >= 0);
		i &= 7;
		return (byte) ((b >> i) | (b << (8 - i)));
	}


	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static ushort ReadBinaryNumberIntoWord(ReadOnlySpan<byte> source, int sourceBitOffset, int binaryDigitsToRead, IterateDirection direction) {
	//	var dest = new byte[2];
	//	var bitsRead = CopyBits(source, sourceBitOffset, dest, sizeof(short)*8 - 1, binaryDigitsToRead, direction, IterateDirection.RightToLeft);
	//	if (bitsRead != binaryDigitsToRead)
	//		throw new ArgumentException("Insufficient bits", nameof(source));
	//	return EndianBitConverter.Big.ToUInt16(dest);
	//}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static void WriteWordAsBinaryNumber(ushort number, Span<byte> dest, int bitOffset, int bitCount, IterateDirection direction) {
	//	var source = EndianBitConverter.Big.GetBytes(number);
	//	CopyBits(source, sizeof(short) * 8 - 1, dest, bitOffset, bitCount, IterateDirection.RightToLeft, direction);
	//}

	/// <summary>
	/// Reads the bits from the source byte array. Important to note that bits are indexed absolutely,
	/// thus a 8 byte array is really a 256 bit collection where "bit 0" is read from bit 7 of byte 0
	/// and bit 8 from bit 7 of byte 1, etc.  
	/// </summary>
	/// <param name="source"></param>
	/// <param name="bitOffset"></param>
	/// <param name="bitCount"></param>
	/// <param name="dest"></param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBits(ReadOnlySpan<byte> source, int bitOffset, int bitCount, out byte[] dest) {
		dest = new byte[((bitCount - 1) >> 3) + 1];
		return CopyBits(source, bitOffset, dest, 0, bitCount);
	}

	/// <summary>
	/// Copies bits from source array into dest array. Important to note that bits are indexed absolutely,
	/// thus a 8 byte array is really a 256 bit collection where "bit 0" is read from bit 7 of byte 0
	/// and bit 8 from bit 7 of byte 1, etc.  
	/// </summary>
	/// <param name="source">Byte array to copy from</param>
	/// <param name="sourceBitOffset">Bit offset when reading</param>
	/// <param name="bitCount">Number of bits to copy</param>
	/// <param name="dest">Byte array to copy to </param>
	/// <param name="destBitOffset">Bit offset when writing</param>
	/// <param name="readDirection">Reads backwards when true</param>
	/// <param name="writeDirection">Writes backwards when true</param>
	/// <returns>Number of bits copied</returns>
	public static int CopyBits(ReadOnlySpan<byte> source, long sourceBitOffset, Span<byte> dest, long destBitOffset, long bitCount, IterateDirection readDirection = IterateDirection.LeftToRight,
	                           IterateDirection writeDirection = IterateDirection.LeftToRight) {
		// Base-case
		if (source.Length == 0 || bitCount == 0)
			return 0;

		// Setup the byte, in-byte and boundary indexes
		var totalBits = source.Length << 3;
		var readBitIndex = sourceBitOffset; // absolute bit index within source
		var readByteIndex = sourceBitOffset >> 3;
		var readByteBitIndex = sourceBitOffset - (readByteIndex << 3);
		var readLeftBitBoundary = (readDirection == IterateDirection.LeftToRight ? sourceBitOffset : sourceBitOffset - bitCount + 1).ClipTo(0, totalBits - 1);
		var readRightBitBoundary = (readDirection == IterateDirection.LeftToRight ? sourceBitOffset + bitCount - 1 : sourceBitOffset).ClipTo(readLeftBitBoundary, totalBits - 1);

		var writeBitIndex = destBitOffset; // absolute bit index within dest
		var writeByteIndex = destBitOffset >> 3;
		var writeByteBitIndex = destBitOffset - (writeByteIndex << 3);
		var writeLeftBitBoundary = writeDirection == IterateDirection.LeftToRight ? destBitOffset : (destBitOffset - bitCount + 1).ClipTo(0, Int32.MaxValue);
		;
		var writeRightBitBoundary = writeDirection == IterateDirection.LeftToRight ? destBitOffset + bitCount - 1 : destBitOffset;

		// Copy the bits
		var bitsCopied = 0;
		while (readLeftBitBoundary <= readBitIndex && readBitIndex <= readRightBitBoundary &&
		       writeLeftBitBoundary <= writeBitIndex && writeBitIndex <= writeRightBitBoundary &&
		       bitsCopied < bitCount) {

			var readByteIndexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(readByteIndex);
			var readByteBitIndexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(readByteBitIndex);
			var writeByteIndexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(writeByteIndex);
			var writeByteBitIndexI = Tools.Collection.CheckNotImplemented64bitAddressingIndex(writeByteBitIndex);

			// Copy current bit 
			if ((source[readByteIndexI] & (0x1 << (7 - readByteBitIndexI))) != 0) {
				// Read a 1, so write a 1
				dest[writeByteIndexI] = (byte)(dest[writeByteIndexI] | (0x1 << (7 - writeByteBitIndexI)));
			} else {
				// Read a 0, so write a 0
				dest[writeByteIndexI] = (byte)(dest[writeByteIndexI] & (0xffffffff - (0x1 << (7 - writeByteBitIndexI))));
			}

			// Update read/write indexes
			switch (readDirection) {
				case IterateDirection.LeftToRight:
					readBitIndex++;
					if (readByteBitIndex == 7) {
						readByteBitIndex = 0;
						readByteIndex++;
					} else {
						readByteBitIndex++;
					}
					break;
				case IterateDirection.RightToLeft:
					readBitIndex--;
					if (readByteBitIndex == 0) {
						readByteBitIndex = 7;
						readByteIndex--;
					} else {
						readByteBitIndex--;
					}
					break;
			}

			switch (writeDirection) {
				case IterateDirection.LeftToRight:
					writeBitIndex++;
					if (writeByteBitIndex == 7) {
						writeByteBitIndex = 0;
						writeByteIndex++;
					} else {
						writeByteBitIndex++;
					}
					break;
				case IterateDirection.RightToLeft:
					writeBitIndex--;
					if (writeByteBitIndex == 0) {
						writeByteBitIndex = 7;
						writeByteIndex--;
					} else {
						writeByteBitIndex--;
					}
					break;
			}
			bitsCopied++;
		}
		return bitsCopied;
	}

	// TODO: needs testing
	public static void SetBit(Span<byte> dest, long index, bool value) {
		byte valueByte = value ? (byte)1 : (byte)0;
		// 00000000   (false)
		// 00000001   (true)

		// [ byte1  ] [ bytes 2] [byte 3  ]         (bytes) 
		// [76543210] [76543210] [76543210]         (in-byte bit index)
		//  01234567   89ABCDEF   .....             (bit index)

		CopyBits(new[] { valueByte }, 7, dest, index, 1);

	}

	// TODO: needs testing
	public static bool ReadBit(Span<byte> source, long index) {
		// 00000000   (false)
		// 00000001   (true)

		// [ byte1  ] [ bytes 2] [byte 3  ]         (bytes) 
		// [76543210] [76543210] [76543210]         (in-byte bit index)
		//  01234567   89ABCDEF   .....             (bit index)
		var valueByte = new byte[1];
		CopyBits(source, index, valueByte, 7, 1);
		Debug.Assert(valueByte[0].IsIn((byte)0, (byte)1));
		return valueByte[0] == 1;

	}
}
