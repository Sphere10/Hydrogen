// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace Hydrogen;

/// <summary>
/// Utility that read and write bits in byte array
/// </summary>
public class BitStream : Stream {
	private byte[] _source;

	/// <summary>
	/// Initialize the stream with capacity
	/// </summary>
	/// <param name="capacity">Capacity of the stream</param>
	public BitStream(int capacity) {
		_source = new byte[capacity];
	}

	/// <summary>
	/// Initialize the stream with a source byte array
	/// </summary>
	/// <param name="source"></param>
	public BitStream(byte[] source) {
		this._source = source;
	}

	public override bool CanRead => true;

	public override bool CanSeek => true;

	public override bool CanWrite => true;

	public override long Length => _source.Length * 8;

	public override long Position { get; set; }

	public override void SetLength(long value) {
		Array.Resize(ref _source, (int)((value - 1) >> 3) + 1);
	}

	/// <summary>
	/// Read the stream to the buffer
	/// </summary>
	/// <param name="buffer">Buffer</param>
	/// <param name="offset">Offset bit start position of the buffer</param>
	/// <param name="count">Number of bits to read</param>
	/// <returns>Number of bits read</returns>
	public override int Read(byte[] buffer, int offset, int count) {
		var bitsRead = Bits.CopyBits(_source, (int)Position, buffer, offset, count);
		Position += bitsRead;
		return bitsRead;
	}

	/// <summary>
	/// Write from buffer to the stream
	/// </summary>
	/// <param name="buffer"></param>
	/// <param name="offset">Offset start bit position of buffer</param>
	/// <param name="count">Number of bits</param>
	public override void Write(byte[] buffer, int offset, int count) {
		Guard.Argument(Position + count <= Length, nameof(count), "Insufficient space to write bits");
		var bitsWritten = Bits.CopyBits(buffer, offset, _source, (int)Position, count);
		Position += bitsWritten;
	}

	/// <summary>
	/// Set up the stream position
	/// </summary>
	/// <param name="offset">Position</param>
	/// <param name="origin">Position origin</param>
	/// <returns>Position after setup</returns>
	public override long Seek(long offset, SeekOrigin origin) {
		switch (origin) {
			case (SeekOrigin.Begin):
				Position = offset;
				break;
			case (SeekOrigin.Current):
				Position += offset;
				break;
			case (SeekOrigin.End):
				Position = Length + offset;
				break;
		}
		return Position;
	}

	public override void Flush() {
		// No op
	}

}
