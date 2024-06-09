// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// Implementation of EndianBitConverter which converts to/from little-endian
/// byte arrays.
/// </summary>
public sealed class LittleEndianBitConverter : EndianBitConverter {
	/// <summary>
	/// Indicates the byte order ("endianess") in which data is converted using this class.
	/// </summary>
	/// <remarks>
	/// Different computer architectures store data using different byte orders. "Big-endian"
	/// means the most significant byte is on the left end of a word. "Little-endian" means the 
	/// most significant byte is on the right end of a word.
	/// </remarks>
	/// <returns>true if this converter is little-endian, false otherwise.</returns>
	public override bool IsLittleEndian() => true;

	/// <summary>
	/// Indicates the byte order ("endianess") in which data is converted using this class.
	/// </summary>
	public override Endianness Endianness => Endianness.LittleEndian;

	/// <summary>
	/// Copies the specified number of bytes from value to buffer, starting at index.
	/// </summary>
	/// <param name="value">The value to copy</param>
	/// <param name="bytes">The number of bytes to copy</param>
	/// <param name="buffer">The buffer to copy the bytes into</param>
	/// <param name="index">The index to start at</param>
	protected override void WriteToImpl(long value, int bytes, Span<byte> buffer, int index) {
		for (var i = 0; i < bytes; i++) {
			buffer[i + index] = unchecked((byte)(value & 0xff));
			value = value >> 8;
		}
	}

	/// <summary>
	/// Returns a value built from the specified number of bytes from the given buffer,
	/// starting at index.
	/// </summary>
	/// <param name="buffer">The data in byte array format</param>
	/// <param name="startIndex">The first index to use</param>
	/// <param name="bytesToConvert">The number of bytes to use</param>
	/// <returns>The value built from the given bytes</returns>
	protected override long FromBytes(ReadOnlySpan<byte> buffer, int startIndex, int bytesToConvert) {
		long ret = 0;
		for (var i = 0; i < bytesToConvert; i++) {
			ret = unchecked((ret << 8) | buffer[startIndex + bytesToConvert - 1 - i]);
		}
		return ret;
	}
}
