// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.CompilerServices;

namespace Hydrogen.Collections.Spans;

public class ByteSpanReader {
	private int _ix;
	private readonly EndianBitConverter _converter;

	public ByteSpanReader(EndianBitConverter converter, int offset = 0) {
		_ix = offset;
		_converter = converter;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte ReadByte(ReadOnlySpan<byte> span) {
		return span[_ix++];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public char ReadChar(ReadOnlySpan<byte> span) {
		var val = _converter.ToChar(span, _ix);
		_ix += sizeof(char);
		return val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte[] ReadBytes(ReadOnlySpan<byte> span, int length) {
		var bytes = new byte[length];
		ReadBytes(span, bytes);
		return bytes;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ReadBytes(ReadOnlySpan<byte> span, Span<byte> result) {
		span.Slice(_ix, result.Length).CopyTo(result);
		_ix += result.Length;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte[,] ReadBytes2D(ReadOnlySpan<byte> span, int rows, int columns) {
		var result = new byte[rows, columns];
		ReadBytes2D(span, ref result);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ReadBytes2D(ReadOnlySpan<byte> span, ref byte[,] result) {
		for (var i = 0; i < result.GetLength(0); i++)
			ReadBytes(span, result.GetRow(i));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Int16 ReadInt16(ReadOnlySpan<byte> span) {
		var val = _converter.ToInt16(span, _ix);
		_ix += sizeof(Int16);
		return val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UInt16 ReadUInt16(ReadOnlySpan<byte> span) {
		var val = _converter.ToUInt16(span, _ix);
		_ix += sizeof(UInt16);
		return val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Int32 ReadInt32(ReadOnlySpan<byte> span) {
		var val = _converter.ToInt32(span, _ix);
		_ix += sizeof(Int32);
		return val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UInt32 ReadUInt32(ReadOnlySpan<byte> span) {
		var val = _converter.ToUInt32(span, _ix);
		_ix += sizeof(UInt32);
		return val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Int64 ReadInt64(ReadOnlySpan<byte> span) {
		var val = _converter.ToInt64(span, _ix);
		_ix += sizeof(Int64);
		return val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UInt64 ReadUInt64(ReadOnlySpan<byte> span) {
		var val = _converter.ToUInt64(span, _ix);
		_ix += sizeof(UInt64);
		return val;
	}
}
