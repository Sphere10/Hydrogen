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
/// Compact Variable integer - LEB128 compression based on NBitcoin CompactVarInt. Useful for 
/// serializing small integers in a single byte. Larger integers may also be encoded but will be 
/// less efficient than other formats.
/// </summary>
public readonly struct CVarInt {
	private const int MaxBytesPermitted = 256;
	private readonly ulong _value;
	

	public CVarInt() : this(0UL) {
	}

	public CVarInt(ulong value) {
		_value = value;
	}

	public static int SizeOf(ulong value) {
		var len = 1;
		var n = value;
		while (n > 0x7F) {
			n = (n >> 7) - 1;
			len++;
		}

		return len;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CVarInt From(ReadOnlySpan<byte> bytes) => new(Read(bytes));

	public static ulong Read(ReadOnlySpan<byte> bytes) {
		ulong n = 0;
		for (var i = 0; i < bytes.Length; i++) {
			var chData = bytes[i];
			var a = n << 7;
			var b = (byte)(chData & 0x7F);
			n = (a | b);
			if ((chData & 0x80) != 0)
				n++;
			else
				break;
		}
		return n;
	}

	public static ulong Read(Stream stream) {
		ulong n = 0;
		var loopProtector = 0;
		while (true) {
			if (loopProtector++ > MaxBytesPermitted)
				throw new FormatException($"Reading {nameof(CVarInt)} failed (no terminating bit encountered)");
			var readByte = stream.ReadByte();
			if (readByte == -1)
				throw new InvalidOperationException($"Unexpected end of stream");
			var chData = readByte;
			var a = n << 7;
			var b = (byte)(chData & 0x7F);
			n = (a | b);
			if ((chData & 0x80) != 0)
				n++;
			else
				break;
		}
		return n;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Write(Stream stream) => Write(_value, stream);

	public static void Write(ulong value, Stream stream) {
		Span<byte> tmp = stackalloc byte[(sizeof(long) * 8 + 6) / 7];

		var len = 0;
		while (true) {
			var a = (byte)(value & 0x7F);
			var b = (byte)(len != 0 ? 0x80 : 0x00);
			tmp[len] = (byte)(a | b);
			if (value <= 0x7F)
				break;
			value = (value >> 7) - 1;
			len++;
		}
		do
			stream.WriteByte(tmp[len]);
		while (len-- != 0);
	}

	public byte[] ToBytes() {
		using var stream = new MemoryStream();
		Write(stream);
		return stream.ToArray();
	}


	public ulong ToLong() => _value;

	public static implicit operator ulong(CVarInt v) => v._value;

	public static implicit operator CVarInt(ulong v) => new(v);

	public static CVarInt operator +(CVarInt a, CVarInt b) => new(a._value + b._value);

	//public static VarInt operator +(VarInt a, ulong b) => new (a._value + b);

	public static CVarInt operator -(CVarInt a, CVarInt b) => new(a._value - b._value);

	//public static VarInt operator -(VarInt a, ulong b) => new (a._value - b);

	public static CVarInt operator *(CVarInt a, CVarInt b) => new(a._value * b._value);

	//public static VarInt operator *(VarInt a, ulong b) => new (a._value * b);

	public static CVarInt operator /(CVarInt a, CVarInt b) => b._value == 0 ? throw new DivideByZeroException() : new CVarInt(a._value / b._value);

	//	public static VarInt operator /(VarInt a, ulong b) => b == 0 ? throw new DivideByZeroException() : new VarInt(a._value / b);

	public static CVarInt operator ++(CVarInt a) => new CVarInt(a._value + 1);

	public static CVarInt operator --(CVarInt a) => new CVarInt(a._value - 1);


}
