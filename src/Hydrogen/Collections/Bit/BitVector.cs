// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hydrogen;

/*
 

Bit-ordering within `ReadBits` and `WriteBits` are such that the 
least-significant bit (LSB) is the left-most bit of that byte. 

For example, consider an array of two bytes C = [A,B]:

Memory layout of C=[a,b] with their in-byte indexes marked.
A:[7][6][5][4][3][2][1][0]  B:[7][6][5][4][3][2][1][0]
C:[0][1][2][3][4][5][6][7]    [8][9]...

The bit indexes of the 16-bit array C are such that:
 Bit 0 maps to A[7]
 Bit 1 maps to A[6]
 Bit 7 maps to A[0]
 Bit 8 maps to B[7]
 Bit 16 maps to B[0]

 */
/// <summary>
/// A replacement for <see cref="BitArray"/>
/// </summary>
public class BitVector : RangedListBase<bool> {

	private readonly Stream _stream;
	private long _count;

	public BitVector(Stream stream) {
		Guard.ArgumentNotNull(stream, nameof(stream));

		_stream = stream;
		_count = (int)(stream.Length * 8);
	}

	public override long Count => _count;

	public override IEnumerable<long> IndexOfRange(IEnumerable<bool> items) {
		Guard.ArgumentNotNull(items, nameof(items));

		var itemsArray = items as bool[] ?? items.ToArray();
		if (!itemsArray.Any()) {
			return new List<long>();
		}

		var results = new long[itemsArray.Length];
		_stream.Seek(0, SeekOrigin.Begin);

		for (var i = 0; i < _stream.Length; i++) {
			var current = _stream.ReadBytes(1);
			var bitsLength = Math.Min(8, _count - i * 8);

			for (var j = 0; j < bitsLength; j++) {
				var value = Bits.ReadBit(current, j);
				foreach (var (t, index) in itemsArray.WithIndex()) {
					if (value == t) {
						results[index] = i * 8 + j;
					}
				}
			}
		}
		return results;
	}

	public override void AddRange(IEnumerable<bool> items) {
		Guard.ArgumentNotNull(items, nameof(items));

		var itemsArray = items as bool[] ?? items.ToArray();
		if (!itemsArray.Any())
			return;

		var startBitIndex = _count % 8;
		var remainingBits = startBitIndex > 0 ? itemsArray.Length - (8 - startBitIndex) : itemsArray.Length;
		var bytesCount = (int)Math.Ceiling((decimal)remainingBits / 8);
		var byteIndex = (int)Math.Floor((decimal)_count / 8);

		if (startBitIndex > 0)
			bytesCount++;

		var buffer = Tools.Array.Gen(bytesCount, (byte)0);

		if (startBitIndex > 0) {
			_stream.Seek(byteIndex, SeekOrigin.Begin);
			_stream.Read(buffer, 0, 1);
		}

		for (var i = 0; i < itemsArray.Length; i++) {
			Bits.SetBit(buffer, i + startBitIndex, itemsArray[i]);
		}

		_stream.Seek(byteIndex, SeekOrigin.Begin);
		_stream.Write(buffer);

		_count += itemsArray.Length;
		UpdateVersion();
	}

	public override void InsertRange(long index, IEnumerable<bool> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		Guard.ArgumentInRange(index, 0, Math.Max(0, _count), nameof(index));

		var itemsArray = items as bool[] ?? items.ToArray();

		if (!itemsArray.Any())
			return;

		if (index == Count)
			AddRange(itemsArray);
		else
			throw new NotSupportedException("This collection can only be inserted from the end");
	}

	public override IEnumerable<bool> ReadRange(long index, long count) {
		CheckRange(index, count);

		var startBitIndex = index % 8;
		var remainingBits = startBitIndex > 0 ? count - (8 - startBitIndex) : count;
		var finalBitIndex = remainingBits % 8;
		var nonPartialBits = remainingBits - finalBitIndex;

		var byteCount = nonPartialBits / 8;
		var byteIndex = (int)Math.Floor((decimal)index / 8);

		if (startBitIndex > 0)
			byteCount++;

		if (finalBitIndex > 0)
			byteCount++;

		_stream.Seek(byteIndex, SeekOrigin.Begin);
		var bytes = _stream.ReadBytes(byteCount);

		for (var i = 0; i < count; i++) {
			yield return Bits.ReadBit(bytes, i + startBitIndex);
		}
	}

	public override void RemoveRange(long index, long count) {
		CheckRange(index, count, rightAligned: true);

		if (index + count != Count)
			throw new NotSupportedException("This collection can only be removed from the end");

		var bytesToRemove = _stream.Length - (int)Math.Ceiling((decimal)(_count - count) / 8);
		_stream.SetLength(_stream.Length - bytesToRemove);
		_count -= count;
		UpdateVersion();
	}

	public override void UpdateRange(long index, IEnumerable<bool> items) {
		Guard.ArgumentNotNull(items, nameof(items));
		var itemsArray = items as bool[] ?? items.ToArray();
		CheckRange(index, itemsArray.Length);

		if (itemsArray.Length == 0)
			return;

		var endIndex = index + itemsArray.Length;
		if (endIndex > _count)
			throw new ArgumentOutOfRangeException(nameof(index), "Update range is out of bounds");

		var startBitIndex = index % 8;
		var remainingBits = startBitIndex > 0 ? itemsArray.Length - (8 - startBitIndex) : itemsArray.Length;
		var finalBitIndex = remainingBits % 8;
		var nonPartialBits = remainingBits - finalBitIndex;

		var byteIndex = (int)Math.Floor((decimal)index / 8);
		var bytesCount = nonPartialBits / 8;

		if (startBitIndex > 0)
			bytesCount++;

		if (finalBitIndex > 0)
			bytesCount++;

		var buffer = Tools.Array.Gen(bytesCount, (byte)0);

		if (startBitIndex > 0) {
			_stream.Seek(byteIndex, SeekOrigin.Begin);
			_stream.Read(buffer, 0, 1);
		}

		if (finalBitIndex > 0) {
			_stream.Seek(byteIndex + bytesCount - 1, SeekOrigin.Begin);
			_stream.Read(buffer, buffer.Length - 1, 1);
		}

		for (var i = 0; i < itemsArray.Length; i++) {
			Bits.SetBit(buffer, i + startBitIndex, itemsArray[i]);
		}

		_stream.Seek(byteIndex, SeekOrigin.Begin);
		_stream.Write(buffer);
		UpdateVersion();
	}

	public override void Clear() {
		_count = 0;
		_stream.SetLength(0);
		base.Clear();
		UpdateVersion();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="fromOffset"></param>
	/// <param name="quantity"></param>
	/// <param name="searchValue"></param>
	/// <param name="value"></param>
	/// <param name="indices"></param>
	/// <returns></returns>
	/// <remarks>NOT TESTED</remarks>
	public long FastFindBits(long fromOffset, long quantity, bool searchValue, bool value, out long[] indices) {
		fromOffset = FastFindStreamOffsetContainingBitValue(fromOffset, value);
		// TODO: update this to search bit values using QWORD's
		var indicesL = new List<long>();
		var startSearchBit = fromOffset * 8;
		for (var i = startSearchBit; i < _count || indicesL.Count < quantity; i++)
			if (Read((int)i) == searchValue)
				indicesL.Add((int)i);
		indices = indicesL.ToArray();
		return _stream.Position;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="offset"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	/// <remarks>NOT TESTED</remarks>
	public long FastFindStreamOffsetContainingBitValue(long offset, bool value) {
		_stream.Seek(offset, SeekOrigin.Begin);
		var converter = EndianBitConverter.For(Endianness.LittleEndian);
		var mask = 0b11111111111111111111111111111111;
		var bytesRead = new byte[64];
		var blank = Tools.Array.Gen<byte>(64, 0);
		var foundBit = false;
		var readCount = 0;
		while (!foundBit && (offset + readCount) < Count) {
			blank.AsSpan().CopyTo(bytesRead);
			readCount = _stream.Read(bytesRead);
			var qwordRead = converter.ToUInt64(bytesRead);
			foundBit = value ? (qwordRead & mask) > 0 : (~qwordRead & mask) > 0;
			if (!foundBit) {
				offset += sizeof(UInt64);
			}
		}

		if (foundBit)
			return offset;
		return -1;
	}


	//private void CheckRange(int index, int count) {
	//	Guard.Argument(count >= 0, nameof(index), "Must be greater than or equal to 0");
	//	if (index == Count && count == 0) return; // special case: at index of "next item" with no count, this is valid
	//	Guard.ArgumentInRange(index, 0, Count - 1, nameof(index));
	//	if (count > 0)
	//		Guard.ArgumentInRange(index + count - 1, 0, Count - 1, nameof(count));
	//}


}
