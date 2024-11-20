// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;

namespace Hydrogen.CryptoEx.HF;

public class Murmur3_x86_32Digest : IDigest, IMemoable {

	#region Consts

	private const int ByteLength = 4;

	private const int DigestLengthBytes = 4;

	private const uint C1 = 0xCC9E2D51;
	private const uint C2 = 0x1B873593;
	private const uint C3 = 0xE6546B64;
	private const uint C4 = 0x85EBCA6B;
	private const uint C5 = 0xC2B2AE35;

	#endregion

	private uint _seed, _h, _processedBytes;
	private int _idx;
	private readonly byte[] _buffer = new byte[ByteLength];


	private static uint RotateLeft32(uint value, int distance) {
		return (value << distance) | (value >> (32 - distance));
	}

	private void TransformUInt32Fast(uint data) {
		uint k = data;

		k = k * C1;
		k = RotateLeft32(k, 15);
		k = k * C2;

		_h = _h ^ k;
		_h = RotateLeft32(_h, 13);
		_h = (_h * 5) + C3;
	}

	private void ByteUpdate(byte b) {
		_buffer[_idx] = b;
		_idx++;
		if (_idx >= 4) {
			uint k = Pack.LE_To_UInt32(_buffer, 0);
			TransformUInt32Fast(k);
			_idx = 0;
		}
	}

	private void Finish() {
		if (_idx != 0) {
			switch (_idx) {
				case 3: {
					uint k = (uint)_buffer[2] << 16 | ((uint)_buffer[1] << 8) | _buffer[0];
					k *= C1;
					k = RotateLeft32(k, 15);
					k *= C2;
					_h ^= k;
					break;
				}
				case 2: {
					uint k = ((uint)_buffer[1] << 8) | _buffer[0];
					k *= C1;
					k = RotateLeft32(k, 15);
					k *= C2;
					_h ^= k;
					break;
				}
				case 1: {
					uint k = _buffer[0];
					k *= C1;
					k = RotateLeft32(k, 15);
					k *= C2;
					_h ^= k;
					break;
				}
			}
		}

		_h ^= _processedBytes;

		_h ^= _h >> 16;
		_h *= C4;
		_h ^= _h >> 13;
		_h *= C5;
		_h ^= _h >> 16;
	}

	/**
        * Standard constructor
        */
	public Murmur3_x86_32Digest() : this(0) {
	}

	public Murmur3_x86_32Digest(uint seed) {
		_seed = seed;
		Reset();
	}

	/**
        * Copy constructor.  This will copy the state of the provided
        * message digest.
        */
	public Murmur3_x86_32Digest(Murmur3_x86_32Digest t) {
		Reset(t);
	}

	public string AlgorithmName {
		get { return "Murmur3_x86_32"; }
	}

	public int GetDigestSize() {
		return DigestLengthBytes;
	}

	public int GetByteLength() {
		return ByteLength;
	}

	public void Update(byte input) {
		BlockUpdate([input], 0, 1);
	}

	public void BlockUpdate(ReadOnlySpan<byte> input) {
		int len = input.Length;
		int i = 0;

		// consume last pending bytes
		if ((_idx != 0) & (len != 0)) {
			while ((_idx < 4) & (len != 0)) {
				_buffer[_idx] = input[i];
				_idx++;
				i++;
				len--;
			}

			if (_idx == 4) {
				uint k = Pack.LE_To_UInt32(_buffer, 0);
				TransformUInt32Fast(k);
				_idx = 0;
			}
		}

		int nBlocks = len >> 2;

		// body
		for (int j = 0; j < nBlocks; j++) {
			uint k = Pack.LE_To_UInt32(input.Slice(i + (j * 4), 4));
			TransformUInt32Fast(k);
		}

		// save pending end bytes
		int offset = i + (nBlocks * 4);
		while (offset < input.Length) {
			ByteUpdate(input[offset]);
			offset++;
		}

		_processedBytes += (uint)input.Length;
	}

	public void BlockUpdate(byte[] input, int inOff, int length) {
		BlockUpdate(input.AsSpan(inOff, length));
	}

	public int DoFinal(Span<byte> output) {
		Finish();

		uint[] tempBuf = { _h };
		Pack.UInt32_To_BE(tempBuf, output, 0);

		Reset();

		return GetDigestSize();
	}

	public int DoFinal(byte[] output, int outOff) {
		return DoFinal(output.AsSpan(outOff));
	}

	public uint DoFinal() {
		Span<byte> tempBuf = stackalloc byte[GetDigestSize()];

		DoFinal(tempBuf);

		return ((uint)(tempBuf[0]) << 24) | ((uint)(tempBuf[1]) << 16) |
		       ((uint)(tempBuf[2]) << 8) | tempBuf[3];
	}
	public void Reset() {
		_h = _seed;
		_processedBytes = 0;
		_idx = 0;
		Array.Clear(_buffer, 0, _buffer.Length);
	}

	public IMemoable Copy() {
		return new Murmur3_x86_32Digest(this);
	}

	public void Reset(IMemoable other) {
		Murmur3_x86_32Digest originalDigest = (Murmur3_x86_32Digest)other;
		Array.Copy(originalDigest._buffer, 0, _buffer, 0, _buffer.Length);
		_seed = originalDigest._seed;
		_h = originalDigest._h;
		_processedBytes = originalDigest._processedBytes;
		_idx = originalDigest._idx;
	}
}
