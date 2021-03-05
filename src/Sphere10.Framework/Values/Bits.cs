//-----------------------------------------------------------------------
// <copyright file="BitStream.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Framework {

	/// <summary>
    /// Utility that read and write binary bits to and from byte arrays
    /// </summary>
    public class Bits  {


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
		/// <param name="sourceBitOffset"></param>
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
		/// <param name="readRightToLeft">Reads backwards when true</param>
		/// <param name="writeRightToLeft">Writes backwards when true</param>
		/// <returns>Number of bits copied</returns>
		public static int CopyBits(ReadOnlySpan<byte> source, int sourceBitOffset, Span<byte> dest, int destBitOffset, int bitCount, IterateDirection readDirection = IterateDirection.LeftToRight, IterateDirection writeDirection = IterateDirection.LeftToRight) {
			// Base-case
			if (source.Length == 0 || bitCount == 0)
				return 0;

			// Setup the byte, in-byte and boundary indexes
			var totalBits = source.Length << 3;
			var readBitIndex = sourceBitOffset;  // absolute bit index within source
			var readByteIndex = sourceBitOffset >> 3;
			var readByteBitIndex = sourceBitOffset - (readByteIndex << 3);
			var readLeftBitBoundary = (readDirection == IterateDirection.LeftToRight ? sourceBitOffset : sourceBitOffset - bitCount + 1).ClipTo(0, totalBits - 1);
			var readRightBitBoundary = (readDirection == IterateDirection.LeftToRight ? sourceBitOffset + bitCount - 1 : sourceBitOffset).ClipTo(readLeftBitBoundary, totalBits - 1);

			var writeBitIndex = destBitOffset;  // absolute bit index within dest
			var writeByteIndex = destBitOffset >> 3;
			var writeByteBitIndex = destBitOffset - (writeByteIndex << 3);
			var writeLeftBitBoundary = writeDirection == IterateDirection.LeftToRight ? destBitOffset : (destBitOffset - bitCount + 1).ClipTo(0, Int32.MaxValue); ;
			var writeRightBitBoundary = writeDirection == IterateDirection.LeftToRight ? destBitOffset + bitCount - 1 : destBitOffset;
			
			// Copy the bits
			var bitsCopied = 0;
			while (readLeftBitBoundary <= readBitIndex && readBitIndex <= readRightBitBoundary &&
				   writeLeftBitBoundary <= writeBitIndex && writeBitIndex <= writeRightBitBoundary &&
				   bitsCopied < bitCount) {
				
				// Copy current bit 
				if ((source[readByteIndex] & (0x1 << (7 - readByteBitIndex))) != 0) {
					// Read a 1, so write a 1
					dest[writeByteIndex] = (byte)(dest[writeByteIndex] | (0x1 << (7 - writeByteBitIndex)));
				} else {
					// Read a 0, so write a 0
					dest[writeByteIndex] = (byte)(dest[writeByteIndex] & (0xffffffff - (0x1 << (7 - writeByteBitIndex))));
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
	}
}