// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe, Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.CompilerServices;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;

namespace Hydrogen.CryptoEx;

public class RadioGatun32Digest : IDigest, IMemoable {

	#region Consts

	private const int ByteLength = 12;

	private const int DigestLengthBytes = 32;

	private const int MillSize = 19;
	private const int BeltWidth = 3;
	private const int BeltLength = 13;
	private const int NumberOfBlankIterations = 16;

	#endregion

	private int _bufferPos;
	private ulong _processedBytes;
	private readonly byte[] _buffer = new byte[ByteLength];
	private readonly uint[] _mMill = new uint[MillSize];
	private uint[][] _mBelt;


	private static uint RotateRight32(uint value, int distance) {
		return (value >> distance) | (value << (32 - distance));
	}

	private void RoundFunction() {
		uint[] q = _mBelt[BeltLength - 1];
		for (int i = BeltLength - 1; i > 0; i--)
			_mBelt[i] = _mBelt[i - 1];
		_mBelt[0] = q;

		for (int i = 0; i < 12; i++)
			_mBelt[i + 1][i % BeltWidth] ^= _mMill[i + 1];

		uint[] a = new uint[MillSize];

		for (int i = 0; i < MillSize; i++)
			a[i] = _mMill[i] ^ (_mMill[(i + 1) % MillSize] | ~_mMill[(i + 2) % MillSize]);

		for (int i = 0; i < MillSize; i++)
			_mMill[i] = RotateRight32(a[(7 * i) % MillSize], i * (i + 1) / 2);

		for (int i = 0; i < MillSize; i++)
			a[i] = _mMill[i] ^ _mMill[(i + 1) % MillSize] ^ _mMill[(i + 4) % MillSize];

		a[0] ^= 1;
		for (int i = 0; i < MillSize; i++)
			_mMill[i] = a[i];

		for (int i = 0; i < BeltWidth; i++)
			_mMill[i + 13] ^= q[i];
	}

	private void Finish() {
		int paddingSize = GetByteLength() - (((int)_processedBytes) % GetByteLength());

		byte[] pad = new byte[paddingSize];
		pad[0] = 0x01;
		BlockUpdate(pad, 0, paddingSize);

		for (int i = 0; i < NumberOfBlankIterations; i++)
			RoundFunction();
	}

	private void ProcessBlock() {
		RoundFunction();
	}

	// this takes a buffer of information and fills the block
	private void ProcessFilledBuffer() {
		int idx = 0;
		uint[] data = new uint[3];

		for (int i = 0; i < 3; i++, idx++) {
			data[idx] = Pack.LE_To_UInt32(_buffer, i * 4);
		}

		for (int i = 0; i < BeltWidth; i++) {
			_mMill[i + 16] ^= data[i];
			_mBelt[0][i] ^= data[i];
		}

		ProcessBlock();

		_bufferPos = 0;
		Array.Clear(_buffer, 0, _buffer.Length);
	}

	/**
        * Standard constructor
        */
	public RadioGatun32Digest() {
		_mBelt = new uint[BeltLength][];
		for (int i = 0; i < BeltLength; i++)
			_mBelt[i] = new uint[BeltWidth];
		Reset();
	}

	/**
         * Copy constructor.  This will copy the state of the provided
         * message digest.
         */
	public RadioGatun32Digest(RadioGatun32Digest t) {
		Reset(t);
	}

	public string AlgorithmName {
		get { return "RadioGatun32"; }
	}

	public int GetDigestSize() {
		return DigestLengthBytes;
	}

	public int GetByteLength() {
		return ByteLength;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Update(byte input) {
		_buffer[_bufferPos] = input;

		_processedBytes++;

		_bufferPos++;

		if (_bufferPos == _buffer.Length) {
			ProcessFilledBuffer();
		}
	}

	public void BlockUpdate(ReadOnlySpan<byte> input) {
		while (!input.IsEmpty) {
			Update(input[0]);
			input = input.Slice(1);
		}
	}

	public void BlockUpdate(byte[] input, int inOff, int length) {
		BlockUpdate(input.AsSpan(inOff, length));
	}

	public int DoFinal(Span<byte> output) {
		Finish();

		Span<uint> temp = stackalloc uint[8];

		for (var i = 0; i < 4; i++) {
			RoundFunction();
			_mMill.AsSpan(1, 2).CopyTo(temp.Slice(i * 2, 2));
		}

		Pack.UInt32_To_LE(temp, output, 0);

		Reset();

		return GetDigestSize();
	}

	public int DoFinal(byte[] output, int outOff) {
		return DoFinal(output.AsSpan(outOff));
	}

	public void Reset() {
		Array.Clear(_mMill, 0, _mMill.Length);

		for (int i = 0; i < BeltLength; i++)
			Array.Clear(_mBelt[i], 0, _mBelt[i].Length);

		_bufferPos = 0;
		_processedBytes = 0;
	}

	public IMemoable Copy() {
		return new RadioGatun32Digest(this);
	}

	public void Reset(IMemoable other) {
		RadioGatun32Digest originalDigest = (RadioGatun32Digest)other;

		Array.Copy(originalDigest._mMill, 0, _mMill, 0, _mMill.Length);
		Array.Copy(originalDigest._buffer, 0, _buffer, 0, _buffer.Length);

		int outerSourceArrayLength = originalDigest._mBelt.Length;
		_mBelt = new uint[outerSourceArrayLength][];

		for (var idx = 0; idx < outerSourceArrayLength; idx++) {
			uint[] innerSourceArray = originalDigest._mBelt[idx];
			int innerSourceArrayLength = innerSourceArray.Length;
			_mBelt[idx] = new uint[innerSourceArrayLength];
			Array.Copy(innerSourceArray, _mBelt[idx], innerSourceArrayLength);
		}

		_bufferPos = originalDigest._bufferPos;
		_processedBytes = originalDigest._processedBytes;
	}
}
