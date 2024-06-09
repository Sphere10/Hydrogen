// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Hydrogen;

public static class ByteArrayExtensions {

	public static int GetHashCodeSimple(this byte[] buffer) {
		const int CONSTANT = 17;
		unchecked {
			int hashCode = 37;

			ReinterpretArray common = new ReinterpretArray();
			common.AsByteArray = buffer;
			int[] array = common.AsInt32Array;

			int length = buffer.Length;
			int remainder = length & 3;
			int len = length >> 2;

			int i = 0;

			while (i < len) {
				hashCode = CONSTANT * hashCode + array[i];
				i++;
			}

			if (remainder > 0) {
				int shift = sizeof(uint) - remainder;
				hashCode = CONSTANT * hashCode + ((array[i] << shift) >> shift);
			}

			return hashCode;
		}
	}

	/// http://en.wikipedia.org/wiki/MurmurHash
	/// </summary>
	/// <param name="buffer"></param>
	/// <param name="seed"></param>
	/// <returns></returns>
	public static int GetMurMurHash3(this byte[] buffer, int seed = 37) {
		return MURMUR3_32.Execute(buffer, seed);
	}

	public static byte[] SubArray(this byte[] buffer, int offset, int length) {
		byte[] middle = new byte[length];
		System.Buffer.BlockCopy(buffer, offset, middle, 0, length);
		return middle;
	}

	public static byte[] Left(this byte[] buffer, int length) {
		return buffer.SubArray(0, length);
	}

	public static byte[] Right(this byte[] buffer, int length) {
		return buffer.SubArray(buffer.Length - length, length);
	}

	public static int GetBit(this byte[] map, int bitIndex) {
		return (map[bitIndex >> 3] >> (bitIndex & 7)) & 1;
	}

	public static void SetBit(this byte[] map, int bitIndex, int value) {
		int bitMask = 1 << (bitIndex & 7);
		if (value != 0)
			map[bitIndex >> 3] |= (byte)bitMask;
		else
			map[bitIndex >> 3] &= (byte)(~bitMask);
	}

	public static string ToBase62String(this byte[] buffer) {
		return Base62Encoding.ToBase62String(buffer);
	}

	public static string ToBase32String(this byte[] buffer) {
		return Base32Encoding.Encode(buffer);
	}

	public static BinaryReader AsReader(this byte[] buffer, EndianBitConverter bitConverter) {
		return new BinaryReader(new MemoryStream(buffer));
	}

	public static EndianBinaryReader AsEndianReader(this byte[] buffer, EndianBitConverter bitConverter) {
		return new EndianBinaryReader(bitConverter, new MemoryStream(buffer));
	}

	public static string ToASCIIString(this byte[] asciiByteArray) {
		var enc = new ASCIIEncoding();
		return enc.GetString(asciiByteArray);
	}

	public static string ToHexString(this byte[] byteArray, bool ommit_0x = false) => HexEncoding.Encode(byteArray, !ommit_0x);

	/// <summary>
	/// Converts a byte array to an object
	/// </summary>
	/// <param name="bytes">The array of bytes to be converted</param>
	/// <returns>An object that represents the byte array</returns>
	public static object DeserializeToObject(this byte[] bytes) {
		return Tools.Object.DeserializeFromByteArray(bytes);
	}

	public static byte[] Xor(this byte[] left, byte[] right) {
#warning Should auto-wrap the right array

		#region Pre-conditions

		Debug.Assert(right.Length >= left.Length);
		if (!(right.Length >= left.Length)) {
			throw new ArgumentOutOfRangeException("right", "Parameter must be less than or equal to the size of source array");
		}

		#endregion

		var result = new List<byte>();
		for (var i = 0; i < left.Length; i++) {
			result.Add((byte)(left[i] ^ right[i]));
		}
		return result.ToArray();
	}
}
