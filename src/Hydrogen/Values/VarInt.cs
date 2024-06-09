// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Hydrogen;

/// <summary>
/// Variable-sized integer. A given ulong when converted to bytes, will take only the required
/// number of bytes plus a header. Values less than 0xFC will require only a single byte and no header. Use when
/// serializing variable ulong values.
/// </summary>
public readonly struct VarInt {

	private readonly ulong _value;

	private static readonly EndianBitConverter BitConverter = EndianBitConverter.Little;

	public VarInt() : this(0UL) {
	}

	public VarInt(ulong value) {
		_value = value;
	}

	public static int SizeOf(ulong value)
		=> value switch {
			< 0xFD => sizeof(byte),
			<= 0xFFFF => sizeof(byte) + sizeof(ushort),
			<= 0xFFFFFFFF => sizeof(byte) + sizeof(uint),
			_ => sizeof(byte) + sizeof(ulong)
		};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static VarInt From(ReadOnlySpan<byte> bytes) => new(Read(bytes));

	public static ulong Read(ReadOnlySpan<byte> bytes) {
		ulong result;
		if (bytes.Length == 1) {
			result = bytes[0];
		} else {
			var prefix = bytes[0];
			if (prefix == 0xFD)
				result = BitConverter.ToUInt16(bytes[1..3]);
			else if (prefix == 0xFE)
				result = BitConverter.ToUInt32(bytes[1..5]);
			else {
				result = BitConverter.ToUInt64(bytes[1..9]);
			}
		}
		return result;
	}


	public static ulong Read(Stream stream) {
		Guard.ArgumentNotNull(stream, nameof(stream));
		var reader = new EndianBinaryReader(BitConverter, stream);
		var prefix = stream.ReadByte();

		if (prefix < 0xFD)
			return new VarInt((byte)prefix);

		if (prefix == 0xFD) {
			return reader.ReadUInt16();
		}

		if (prefix == 0xFE)
			return reader.ReadUInt32();

		return reader.ReadUInt64();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Write(Stream stream) => Write(_value, stream);

	public static void Write(ulong value, Stream stream) {
		if (value < 0xFD)
			stream.WriteByte((byte)value);
		else if (value <= 0xFFFF) {
			stream.WriteByte(0xFD);
			stream.Write(BitConverter.GetBytes((ushort)value));
		} else if (value <= 0xFFFFFFFF) {
			stream.WriteByte(0xFE);
			stream.Write(BitConverter.GetBytes((uint)value));
		} else {
			stream.WriteByte(0xFF);
			stream.Write(BitConverter.GetBytes(value));
		}
	}

	public byte[] ToBytes() {
		using var memoryStream = new MemoryStream();
		Write(memoryStream);
		return memoryStream.ToArray();
	}


	public ulong ToLong() => _value;


	// Operator overloads

	public static implicit operator ulong(VarInt v) => v._value;

	public static implicit operator VarInt(ulong v) => new(v);

	public static VarInt operator +(VarInt a, VarInt b) => new(a._value + b._value);

	//public static VarInt operator +(VarInt a, ulong b) => new (a._value + b);

	public static VarInt operator -(VarInt a, VarInt b) => new(a._value - b._value);

	//public static VarInt operator -(VarInt a, ulong b) => new (a._value - b);

	public static VarInt operator *(VarInt a, VarInt b) => new(a._value * b._value);

	//public static VarInt operator *(VarInt a, ulong b) => new (a._value * b);

	public static VarInt operator /(VarInt a, VarInt b) => b._value == 0 ? throw new DivideByZeroException() : new VarInt(a._value / b._value);

	//	public static VarInt operator /(VarInt a, ulong b) => b == 0 ? throw new DivideByZeroException() : new VarInt(a._value / b);

	public static VarInt operator ++(VarInt a) => new VarInt(a._value + 1);

	public static VarInt operator --(VarInt a) => new VarInt(a._value - 1);
}
