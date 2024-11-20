// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe, Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;

namespace Hydrogen.CryptoEx;

public abstract class HavalDigest
	: IDigest, IMemoable {

	#region Consts

	private const int HavalVersion = 1;
	private const int ByteLength = 128;

	#endregion

	private int _rounds, _digestLength, _bufferPos;
	private ulong _processedBytes;
	private readonly byte[] _buffer = new byte[ByteLength];

	protected readonly uint[] MHash = new uint[8];

	private static ulong ReverseBytesUInt64(ulong value) {
		return (value & 0x00000000000000FF) << 56 |
		       (value & 0x000000000000FF00) << 40 |
		       (value & 0x0000000000FF0000) << 24 |
		       (value & 0x00000000FF000000) << 8 |
		       (value & 0x000000FF00000000) >> 8 |
		       (value & 0x0000FF0000000000) >> 24 |
		       (value & 0x00FF000000000000) >> 40 |
		       (value & 0xFF00000000000000) >> 56;
	}

	private static ulong le2me_64(ulong value) {
		return !BitConverter.IsLittleEndian ? ReverseBytesUInt64(value) : value;
	}


	private void Finish() {
		ulong bits = _processedBytes * 8;
		int padIndex;

		if (_bufferPos < 118) {
			padIndex = 118 - _bufferPos;
		} else {
			padIndex = 246 - _bufferPos;
		}


		byte[] pad = new byte[padIndex + 10];
		pad[0] = 0x01;

		pad[padIndex++] = (byte)((_rounds << 3) | (HavalVersion & 0x07));
		pad[padIndex++] = (byte)(GetDigestSize() << 1);

		bits = le2me_64(bits);

		Pack.UInt64_To_LE(bits, pad, padIndex);

		padIndex += 8;

		BlockUpdate(pad, 0, padIndex);
	}


	// this takes a buffer of information and fills the block
	private void ProcessFilledBuffer() {
		uint[] temp = new uint[32];
		// copies into the block...
		int idx = 0;
		for (int i = 0; i < 32; i++, idx++) {
			temp[idx] = Pack.LE_To_UInt32(_buffer, i * 4);
		}

		ProcessBlock(ref temp);
		_bufferPos = 0;
		Array.Clear(_buffer, 0, _buffer.Length);
	}


	private void TailorDigestBits() {
		uint t;

		switch (GetDigestSize()) {
			case 16:
				t = (MHash[7] & 0x000000FF) | (MHash[6] & 0xFF000000) | (MHash[5] & 0x00FF0000) |
				    (MHash[4] & 0x0000FF00);
				MHash[0] += t >> 8 | t << 24;
				t = (MHash[7] & 0x0000FF00) | (MHash[6] & 0x000000FF) | (MHash[5] & 0xFF000000) |
				    (MHash[4] & 0x00FF0000);
				MHash[1] += t >> 16 | t << 16;
				t = (MHash[7] & 0x00FF0000) | (MHash[6] & 0x0000FF00) | (MHash[5] & 0x000000FF) |
				    (MHash[4] & 0xFF000000);
				MHash[2] += t >> 24 | t << 8;
				t = (MHash[7] & 0xFF000000) | (MHash[6] & 0x00FF0000) | (MHash[5] & 0x0000FF00) |
				    (MHash[4] & 0x000000FF);
				MHash[3] += t;
				break;
			case 20:
				t = MHash[7] & 0x3F | (uint)(MHash[6] & (0x7F << 25)) | MHash[5] & (0x3F << 19);
				MHash[0] += t >> 19 | t << 13;
				t = MHash[7] & (0x3F << 6) | MHash[6] & 0x3F | (uint)(MHash[5] & (0x7F << 25));
				MHash[1] += t >> 25 | t << 7;
				t = (MHash[7] & (0x7F << 12)) | (MHash[6] & (0x3F << 6)) | (MHash[5] & 0x3F);
				MHash[2] += t;
				t = (MHash[7] & (0x3F << 19)) | (MHash[6] & (0x7F << 12)) | (MHash[5] & (0x3F << 6));
				MHash[3] += (t >> 6);
				t = (MHash[7] & ((uint)0x7F << 25)) | MHash[6] & (0x3F << 19) | MHash[5] & (0x7F << 12);
				MHash[4] += (t >> 12);
				break;
			case 24:
				t = MHash[7] & 0x1F | (uint)(MHash[6] & (0x3F << 26));
				MHash[0] += t >> 26 | t << 6;
				t = (MHash[7] & (0x1F << 5)) | (MHash[6] & 0x1F);
				MHash[1] += t;
				t = (MHash[7] & (0x3F << 10)) | (MHash[6] & (0x1F << 5));
				MHash[2] += (t >> 5);
				t = (MHash[7] & (0x1F << 16)) | (MHash[6] & (0x3F << 10));
				MHash[3] += (t >> 10);
				t = (MHash[7] & (0x1F << 21)) | (MHash[6] & (0x1F << 16));
				MHash[4] += (t >> 16);
				t = (uint)(MHash[7] & (0x3F << 26)) | MHash[6] & (0x1F << 21);
				MHash[5] += (t >> 21);
				break;
			case 28:
				MHash[0] += ((MHash[7] >> 27) & 0x1F);
				MHash[1] += ((MHash[7] >> 22) & 0x1F);
				MHash[2] += ((MHash[7] >> 18) & 0x0F);
				MHash[3] += ((MHash[7] >> 13) & 0x1F);
				MHash[4] += ((MHash[7] >> 9) & 0x0F);
				MHash[5] += ((MHash[7] >> 4) & 0x1F);
				MHash[6] += (MHash[7] & 0x0F);
				break;
		}
	}

	protected static uint RotateRight32(uint value, int distance) {
		return (value >> distance) | (value << (32 - distance));
	}

	/**
        * Standard constructor
        */
	protected HavalDigest(int rounds, int digestLength) {
		_rounds = rounds;
		_digestLength = digestLength;
		_buffer = new byte[GetByteLength()];
		Reset();
	}


	protected abstract void ProcessBlock(ref uint[] block);

	/**
         * Copy constructor.  This will copy the state of the provided
         * message digest.
         */
	protected HavalDigest(HavalDigest t) {
		Reset(t);
	}

	public string AlgorithmName {
		get { return "Haval_" + _rounds + "_" + (GetDigestSize() * 8); }
	}

	public int GetDigestSize() {
		return _digestLength;
	}

	public int GetByteLength() {
		return ByteLength;
	}

	public void Update(byte input) {
		_buffer[_bufferPos] = input;

		_processedBytes++;

		_bufferPos++;

		if (_bufferPos == _buffer.Length) {
			ProcessFilledBuffer();
		}
	}

	public void BlockUpdate(ReadOnlySpan<byte> input) {
		for (int i = 0; i < input.Length; i++) {
			Update(input[i]);
		}
	}

	public void BlockUpdate(byte[] input, int inOff, int length) {
		BlockUpdate(input.AsSpan(inOff, length));
	}

	public void BlockUpdate(byte[] input) {
		BlockUpdate(input.AsSpan());
	}


	public int DoFinal(Span<byte> output) {
		Finish();

		TailorDigestBits();

		Pack.UInt32_To_LE(MHash, output, 0);

		Reset();

		return GetDigestSize();
	}

	public int DoFinal(byte[] output, int outOff) {
		return DoFinal(output.AsSpan(outOff));
	}

	public void Reset() {
		MHash[0] = 0x243F6A88;
		MHash[1] = 0x85A308D3;
		MHash[2] = 0x13198A2E;
		MHash[3] = 0x03707344;
		MHash[4] = 0xA4093822;
		MHash[5] = 0x299F31D0;
		MHash[6] = 0x082EFA98;
		MHash[7] = 0xEC4E6C89;
		Array.Clear(_buffer, 0, _buffer.Length);
		_bufferPos = 0;
		_processedBytes = 0;
	}

	public abstract IMemoable Copy();

	public void Reset(IMemoable other) {
		HavalDigest originalDigest = (HavalDigest)other;

		Array.Copy(originalDigest.MHash, 0, MHash, 0, MHash.Length);
		Array.Copy(originalDigest._buffer, 0, _buffer, 0, _buffer.Length);

		_rounds = originalDigest._rounds;
		_digestLength = originalDigest._digestLength;
		_bufferPos = originalDigest._bufferPos;
		_processedBytes = originalDigest._processedBytes;
	}
}


public class Haval5_256Digest : HavalDigest {
	/**
        * Standard Haval5_256 constructor
        */
	public Haval5_256Digest()
		: base(5, 32) {
	}

	protected override void ProcessBlock(ref uint[] block) {
		uint a = MHash[0];
		uint b = MHash[1];
		uint c = MHash[2];
		uint d = MHash[3];
		uint e = MHash[4];
		uint f = MHash[5];
		uint g = MHash[6];
		uint h = MHash[7];

		uint t = c & (g ^ b) ^ f & e ^ a & d ^ g;
		h = block[0] + RotateRight32(t, 7) + RotateRight32(h, 11);
		t = b & (f ^ a) ^ e & d ^ h & c ^ f;
		g = block[1] + RotateRight32(t, 7) + RotateRight32(g, 11);

		t = a & (e ^ h) ^ d & c ^ g & b ^ e;
		f = block[2] + RotateRight32(t, 7) + RotateRight32(f, 11);

		t = h & (d ^ g) ^ c & b ^ f & a ^ d;
		e = block[3] + RotateRight32(t, 7) + RotateRight32(e, 11);

		t = g & (c ^ f) ^ b & a ^ e & h ^ c;
		d = block[4] + RotateRight32(t, 7) + RotateRight32(d, 11);

		t = f & (b ^ e) ^ a & h ^ d & g ^ b;
		c = block[5] + RotateRight32(t, 7) + RotateRight32(c, 11);

		t = e & (a ^ d) ^ h & g ^ c & f ^ a;
		b = block[6] + RotateRight32(t, 7) + RotateRight32(b, 11);

		t = d & (h ^ c) ^ g & f ^ b & e ^ h;
		a = block[7] + RotateRight32(t, 7) + RotateRight32(a, 11);

		t = c & (g ^ b) ^ f & e ^ a & d ^ g;
		h = block[8] + RotateRight32(t, 7) + RotateRight32(h, 11);

		t = b & (f ^ a) ^ e & d ^ h & c ^ f;
		g = block[9] + RotateRight32(t, 7) + RotateRight32(g, 11);

		t = a & (e ^ h) ^ d & c ^ g & b ^ e;
		f = block[10] + RotateRight32(t, 7) + RotateRight32(f, 11);

		t = h & (d ^ g) ^ c & b ^ f & a ^ d;
		e = block[11] + RotateRight32(t, 7) + RotateRight32(e, 11);

		t = g & (c ^ f) ^ b & a ^ e & h ^ c;
		d = block[12] + RotateRight32(t, 7) + RotateRight32(d, 11);

		t = f & (b ^ e) ^ a & h ^ d & g ^ b;
		c = block[13] + RotateRight32(t, 7) + RotateRight32(c, 11);

		t = e & (a ^ d) ^ h & g ^ c & f ^ a;
		b = block[14] + RotateRight32(t, 7) + RotateRight32(b, 11);

		t = d & (h ^ c) ^ g & f ^ b & e ^ h;
		a = block[15] + RotateRight32(t, 7) + RotateRight32(a, 11);

		t = c & (g ^ b) ^ f & e ^ a & d ^ g;
		h = block[16] + RotateRight32(t, 7) + RotateRight32(h, 11);

		t = b & (f ^ a) ^ e & d ^ h & c ^ f;
		g = block[17] + RotateRight32(t, 7) + RotateRight32(g, 11);

		t = a & (e ^ h) ^ d & c ^ g & b ^ e;
		f = block[18] + RotateRight32(t, 7) + RotateRight32(f, 11);

		t = h & (d ^ g) ^ c & b ^ f & a ^ d;
		e = block[19] + RotateRight32(t, 7) + RotateRight32(e, 11);

		t = g & (c ^ f) ^ b & a ^ e & h ^ c;
		d = block[20] + RotateRight32(t, 7) + RotateRight32(d, 11);

		t = f & (b ^ e) ^ a & h ^ d & g ^ b;
		c = block[21] + RotateRight32(t, 7) + RotateRight32(c, 11);

		t = e & (a ^ d) ^ h & g ^ c & f ^ a;
		b = block[22] + RotateRight32(t, 7) + RotateRight32(b, 11);

		t = d & (h ^ c) ^ g & f ^ b & e ^ h;
		a = block[23] + RotateRight32(t, 7) + RotateRight32(a, 11);

		t = c & (g ^ b) ^ f & e ^ a & d ^ g;
		h = block[24] + RotateRight32(t, 7) + RotateRight32(h, 11);

		t = b & (f ^ a) ^ e & d ^ h & c ^ f;
		g = block[25] + RotateRight32(t, 7) + RotateRight32(g, 11);

		t = a & (e ^ h) ^ d & c ^ g & b ^ e;
		f = block[26] + RotateRight32(t, 7) + RotateRight32(f, 11);

		t = h & (d ^ g) ^ c & b ^ f & a ^ d;
		e = block[27] + RotateRight32(t, 7) + RotateRight32(e, 11);

		t = g & (c ^ f) ^ b & a ^ e & h ^ c;
		d = block[28] + RotateRight32(t, 7) + RotateRight32(d, 11);

		t = f & (b ^ e) ^ a & h ^ d & g ^ b;
		c = block[29] + RotateRight32(t, 7) + RotateRight32(c, 11);

		t = e & (a ^ d) ^ h & g ^ c & f ^ a;
		b = block[30] + RotateRight32(t, 7) + RotateRight32(b, 11);

		t = d & (h ^ c) ^ g & f ^ b & e ^ h;
		a = block[31] + RotateRight32(t, 7) + RotateRight32(a, 11);

		t = d & (e & ~a ^ b & c ^ g ^ f) ^ b & (e ^ c)
		                                 ^ a & c ^ f;
		h = block[5] + 0x452821E6 + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = c & (d & ~h ^ a & b ^ f ^ e) ^ a & (d ^ b)
		                                 ^ h & b ^ e;
		g = block[14] + 0x38D01377 + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = b & (c & ~g ^ h & a ^ e ^ d) ^ h & (c ^ a)
		                                 ^ g & a ^ d;
		f = block[26] + 0xBE5466CF + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = a & (b & ~f ^ g & h ^ d ^ c) ^ g & (b ^ h)
		                                 ^ f & h ^ c;
		e = block[18] + 0x34E90C6C + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = h & (a & ~e ^ f & g ^ c ^ b) ^ f & (a ^ g)
		                                 ^ e & g ^ b;
		d = block[11] + 0xC0AC29B7 + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = g & (h & ~d ^ e & f ^ b ^ a) ^ e & (h ^ f)
		                                 ^ d & f ^ a;
		c = block[28] + 0xC97C50DD + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = f & (g & ~c ^ d & e ^ a ^ h) ^ d & (g ^ e)
		                                 ^ c & e ^ h;
		b = block[7] + 0x3F84D5B5 + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = e & (f & ~b ^ c & d ^ h ^ g) ^ c & (f ^ d)
		                                 ^ b & d ^ g;
		a = block[16] + 0xB5470917 + RotateRight32(t, 7) +
		    RotateRight32(a, 11);

		t = d & (e & ~a ^ b & c ^ g ^ f) ^ b & (e ^ c)
		                                 ^ a & c ^ f;
		h = block[0] + 0x9216D5D9 + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = c & (d & ~h ^ a & b ^ f ^ e) ^ a & (d ^ b)
		                                 ^ h & b ^ e;
		g = block[23] + 0x8979FB1B + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = b & (c & ~g ^ h & a ^ e ^ d) ^ h & (c ^ a)
		                                 ^ g & a ^ d;
		f = block[20] + 0xD1310BA6 + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = a & (b & ~f ^ g & h ^ d ^ c) ^ g & (b ^ h)
		                                 ^ f & h ^ c;
		e = block[22] + 0x98DFB5AC + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = h & (a & ~e ^ f & g ^ c ^ b) ^ f & (a ^ g)
		                                 ^ e & g ^ b;
		d = block[1] + 0x2FFD72DB + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = g & (h & ~d ^ e & f ^ b ^ a) ^ e & (h ^ f)
		                                 ^ d & f ^ a;
		c = block[10] + 0xD01ADFB7 + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = f & (g & ~c ^ d & e ^ a ^ h) ^ d & (g ^ e)
		                                 ^ c & e ^ h;
		b = block[4] + 0xB8E1AFED + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = e & (f & ~b ^ c & d ^ h ^ g) ^ c & (f ^ d)
		                                 ^ b & d ^ g;
		a = block[8] + 0x6A267E96 + RotateRight32(t, 7) +
		    RotateRight32(a, 11);

		t = d & (e & ~a ^ b & c ^ g ^ f) ^ b & (e ^ c)
		                                 ^ a & c ^ f;
		h = block[30] + 0xBA7C9045 + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = c & (d & ~h ^ a & b ^ f ^ e) ^ a & (d ^ b)
		                                 ^ h & b ^ e;
		g = block[3] + 0xF12C7F99 + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = b & (c & ~g ^ h & a ^ e ^ d) ^ h & (c ^ a)
		                                 ^ g & a ^ d;
		f = block[21] + 0x24A19947 + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = a & (b & ~f ^ g & h ^ d ^ c) ^ g & (b ^ h)
		                                 ^ f & h ^ c;
		e = block[9] + 0xB3916CF7 + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = h & (a & ~e ^ f & g ^ c ^ b) ^ f & (a ^ g)
		                                 ^ e & g ^ b;
		d = block[17] + 0x0801F2E2 + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = g & (h & ~d ^ e & f ^ b ^ a) ^ e & (h ^ f)
		                                 ^ d & f ^ a;
		c = block[24] + 0x858EFC16 + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = f & (g & ~c ^ d & e ^ a ^ h) ^ d & (g ^ e)
		                                 ^ c & e ^ h;
		b = block[29] + 0x636920D8 + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = e & (f & ~b ^ c & d ^ h ^ g) ^ c & (f ^ d)
		                                 ^ b & d ^ g;
		a = block[6] + 0x71574E69 + RotateRight32(t, 7) +
		    RotateRight32(a, 11);

		t = d & (e & ~a ^ b & c ^ g ^ f) ^ b & (e ^ c)
		                                 ^ a & c ^ f;
		h = block[19] + 0xA458FEA3 + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = c & (d & ~h ^ a & b ^ f ^ e) ^ a & (d ^ b)
		                                 ^ h & b ^ e;
		g = block[12] + 0xF4933D7E + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = b & (c & ~g ^ h & a ^ e ^ d) ^ h & (c ^ a)
		                                 ^ g & a ^ d;
		f = block[15] + 0x0D95748F + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = a & (b & ~f ^ g & h ^ d ^ c) ^ g & (b ^ h)
		                                 ^ f & h ^ c;
		e = block[13] + 0x728EB658 + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = h & (a & ~e ^ f & g ^ c ^ b) ^ f & (a ^ g)
		                                 ^ e & g ^ b;
		d = block[2] + 0x718BCD58 + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = g & (h & ~d ^ e & f ^ b ^ a) ^ e & (h ^ f)
		                                 ^ d & f ^ a;
		c = block[25] + 0x82154AEE + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = f & (g & ~c ^ d & e ^ a ^ h) ^ d & (g ^ e)
		                                 ^ c & e ^ h;
		b = block[31] + 0x7B54A41D + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = e & (f & ~b ^ c & d ^ h ^ g) ^ c & (f ^ d)
		                                 ^ b & d ^ g;
		a = block[27] + 0xC25A59B5 + RotateRight32(t, 7) +
		    RotateRight32(a, 11);

		t = e & (b & d ^ c ^ f) ^ b & a ^ d & g ^ f;
		h = block[19] + 0x9C30D539 + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = d & (a & c ^ b ^ e) ^ a & h ^ c & f ^ e;
		g = block[9] + 0x2AF26013 + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = c & (h & b ^ a ^ d) ^ h & g ^ b & e ^ d;
		f = block[4] + 0xC5D1B023 + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = b & (g & a ^ h ^ c) ^ g & f ^ a & d ^ c;
		e = block[20] + 0x286085F0 + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = a & (f & h ^ g ^ b) ^ f & e ^ h & c ^ b;
		d = block[28] + 0xCA417918 + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = h & (e & g ^ f ^ a) ^ e & d ^ g & b ^ a;
		c = block[17] + 0xB8DB38EF + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = g & (d & f ^ e ^ h) ^ d & c ^ f & a ^ h;
		b = block[8] + 0x8E79DCB0 + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = f & (c & e ^ d ^ g) ^ c & b ^ e & h ^ g;
		a = block[22] + 0x603A180E + RotateRight32(t, 7) +
		    RotateRight32(a, 11);

		t = e & (b & d ^ c ^ f) ^ b & a ^ d & g ^ f;
		h = block[29] + 0x6C9E0E8B + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = d & (a & c ^ b ^ e) ^ a & h ^ c & f ^ e;
		g = block[14] + 0xB01E8A3E + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = c & (h & b ^ a ^ d) ^ h & g ^ b & e ^ d;
		f = block[25] + 0xD71577C1 + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = b & (g & a ^ h ^ c) ^ g & f ^ a & d ^ c;
		e = block[12] + 0xBD314B27 + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = a & (f & h ^ g ^ b) ^ f & e ^ h & c ^ b;
		d = block[24] + 0x78AF2FDA + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = h & (e & g ^ f ^ a) ^ e & d ^ g & b ^ a;
		c = block[30] + 0x55605C60 + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = g & (d & f ^ e ^ h) ^ d & c ^ f & a ^ h;
		b = block[16] + 0xE65525F3 + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = f & (c & e ^ d ^ g) ^ c & b ^ e & h ^ g;
		a = block[26] + 0xAA55AB94 + RotateRight32(t, 7) +
		    RotateRight32(a, 11);

		t = e & (b & d ^ c ^ f) ^ b & a ^ d & g ^ f;
		h = block[31] + 0x57489862 + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = d & (a & c ^ b ^ e) ^ a & h ^ c & f ^ e;
		g = block[15] + 0x63E81440 + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = c & (h & b ^ a ^ d) ^ h & g ^ b & e ^ d;
		f = block[7] + 0x55CA396A + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = b & (g & a ^ h ^ c) ^ g & f ^ a & d ^ c;
		e = block[3] + 0x2AAB10B6 + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = a & (f & h ^ g ^ b) ^ f & e ^ h & c ^ b;
		d = block[1] + 0xB4CC5C34 + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = h & (e & g ^ f ^ a) ^ e & d ^ g & b ^ a;
		c = block[0] + 0x1141E8CE + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = g & (d & f ^ e ^ h) ^ d & c ^ f & a ^ h;
		b = block[18] + 0xA15486AF + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = f & (c & e ^ d ^ g) ^ c & b ^ e & h ^ g;
		a = block[27] + 0x7C72E993 + RotateRight32(t, 7) +
		    RotateRight32(a, 11);

		t = e & (b & d ^ c ^ f) ^ b & a ^ d & g ^ f;
		h = block[13] + 0xB3EE1411 + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = d & (a & c ^ b ^ e) ^ a & h ^ c & f ^ e;
		g = block[6] + 0x636FBC2A + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = c & (h & b ^ a ^ d) ^ h & g ^ b & e ^ d;
		f = block[21] + 0x2BA9C55D + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = b & (g & a ^ h ^ c) ^ g & f ^ a & d ^ c;
		e = block[10] + 0x741831F6 + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = a & (f & h ^ g ^ b) ^ f & e ^ h & c ^ b;
		d = block[23] + 0xCE5C3E16 + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = h & (e & g ^ f ^ a) ^ e & d ^ g & b ^ a;
		c = block[11] + 0x9B87931E + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = g & (d & f ^ e ^ h) ^ d & c ^ f & a ^ h;
		b = block[5] + 0xAFD6BA33 + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = f & (c & e ^ d ^ g) ^ c & b ^ e & h ^ g;
		a = block[2] + 0x6C24CF5C + RotateRight32(t, 7) +
		    RotateRight32(a, 11);

		t = d & (f & ~a ^ c & ~b ^ e ^ b ^ g) ^ c &
			(e & a ^ f ^ b) ^ a & b ^ g;
		h = block[24] + 0x7A325381 + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = c & (e & ~h ^ b & ~a ^ d ^ a ^ f) ^ b &
			(d & h ^ e ^ a) ^ h & a ^ f;
		g = block[4] + 0x28958677 + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = b & (d & ~g ^ a & ~h ^ c ^ h ^ e) ^ a &
			(c & g ^ d ^ h) ^ g & h ^ e;
		f = block[0] + 0x3B8F4898 + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = a & (c & ~f ^ h & ~g ^ b ^ g ^ d) ^ h &
			(b & f ^ c ^ g) ^ f & g ^ d;
		e = block[14] + 0x6B4BB9AF + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = h & (b & ~e ^ g & ~f ^ a ^ f ^ c) ^ g &
			(a & e ^ b ^ f) ^ e & f ^ c;
		d = block[2] + 0xC4BFE81B + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = g & (a & ~d ^ f & ~e ^ h ^ e ^ b) ^ f &
			(h & d ^ a ^ e) ^ d & e ^ b;
		c = block[7] + 0x66282193 + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = f & (h & ~c ^ e & ~d ^ g ^ d ^ a) ^ e &
			(g & c ^ h ^ d) ^ c & d ^ a;
		b = block[28] + 0x61D809CC + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = e & (g & ~b ^ d & ~c ^ f ^ c ^ h) ^ d &
			(f & b ^ g ^ c) ^ b & c ^ h;
		a = block[23] + 0xFB21A991 + RotateRight32(t, 7) +
		    RotateRight32(a, 11);

		t = d & (f & ~a ^ c & ~b ^ e ^ b ^ g) ^ c &
			(e & a ^ f ^ b) ^ a & b ^ g;
		h = block[26] + 0x487CAC60 + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = c & (e & ~h ^ b & ~a ^ d ^ a ^ f) ^ b &
			(d & h ^ e ^ a) ^ h & a ^ f;
		g = block[6] + 0x5DEC8032 + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = b & (d & ~g ^ a & ~h ^ c ^ h ^ e) ^ a &
			(c & g ^ d ^ h) ^ g & h ^ e;
		f = block[30] + 0xEF845D5D + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = a & (c & ~f ^ h & ~g ^ b ^ g ^ d) ^ h &
			(b & f ^ c ^ g) ^ f & g ^ d;
		e = block[20] + 0xE98575B1 + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = h & (b & ~e ^ g & ~f ^ a ^ f ^ c) ^ g &
			(a & e ^ b ^ f) ^ e & f ^ c;
		d = block[18] + 0xDC262302 + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = g & (a & ~d ^ f & ~e ^ h ^ e ^ b) ^ f &
			(h & d ^ a ^ e) ^ d & e ^ b;
		c = block[25] + 0xEB651B88 + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = f & (h & ~c ^ e & ~d ^ g ^ d ^ a) ^ e &
			(g & c ^ h ^ d) ^ c & d ^ a;
		b = block[19] + 0x23893E81 + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = e & (g & ~b ^ d & ~c ^ f ^ c ^ h) ^ d &
			(f & b ^ g ^ c) ^ b & c ^ h;
		a = block[3] + 0xD396ACC5 + RotateRight32(t, 7) +
		    RotateRight32(a, 11);

		t = d & (f & ~a ^ c & ~b ^ e ^ b ^ g) ^ c &
			(e & a ^ f ^ b) ^ a & b ^ g;
		h = block[22] + 0x0F6D6FF3 + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = c & (e & ~h ^ b & ~a ^ d ^ a ^ f) ^ b &
			(d & h ^ e ^ a) ^ h & a ^ f;
		g = block[11] + 0x83F44239 + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = b & (d & ~g ^ a & ~h ^ c ^ h ^ e) ^ a &
			(c & g ^ d ^ h) ^ g & h ^ e;
		f = block[31] + 0x2E0B4482 + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = a & (c & ~f ^ h & ~g ^ b ^ g ^ d) ^ h &
			(b & f ^ c ^ g) ^ f & g ^ d;
		e = block[21] + 0xA4842004 + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = h & (b & ~e ^ g & ~f ^ a ^ f ^ c) ^ g &
			(a & e ^ b ^ f) ^ e & f ^ c;
		d = block[8] + 0x69C8F04A + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = g & (a & ~d ^ f & ~e ^ h ^ e ^ b) ^ f &
			(h & d ^ a ^ e) ^ d & e ^ b;
		c = block[27] + 0x9E1F9B5E + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = f & (h & ~c ^ e & ~d ^ g ^ d ^ a) ^ e &
			(g & c ^ h ^ d) ^ c & d ^ a;
		b = block[12] + 0x21C66842 + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = e & (g & ~b ^ d & ~c ^ f ^ c ^ h) ^ d &
			(f & b ^ g ^ c) ^ b & c ^ h;
		a = block[9] + 0xF6E96C9A + RotateRight32(t, 7) +
		    RotateRight32(a, 11);

		t = d & (f & ~a ^ c & ~b ^ e ^ b ^ g) ^ c &
			(e & a ^ f ^ b) ^ a & b ^ g;
		h = block[1] + 0x670C9C61 + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = c & (e & ~h ^ b & ~a ^ d ^ a ^ f) ^ b &
			(d & h ^ e ^ a) ^ h & a ^ f;
		g = block[29] + 0xABD388F0 + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = b & (d & ~g ^ a & ~h ^ c ^ h ^ e) ^ a &
			(c & g ^ d ^ h) ^ g & h ^ e;
		f = block[5] + 0x6A51A0D2 + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = a & (c & ~f ^ h & ~g ^ b ^ g ^ d) ^ h &
			(b & f ^ c ^ g) ^ f & g ^ d;
		e = block[15] + 0xD8542F68 + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = h & (b & ~e ^ g & ~f ^ a ^ f ^ c) ^ g &
			(a & e ^ b ^ f) ^ e & f ^ c;
		d = block[17] + 0x960FA728 + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = g & (a & ~d ^ f & ~e ^ h ^ e ^ b) ^ f &
			(h & d ^ a ^ e) ^ d & e ^ b;
		c = block[10] + 0xAB5133A3 + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = f & (h & ~c ^ e & ~d ^ g ^ d ^ a) ^ e &
			(g & c ^ h ^ d) ^ c & d ^ a;
		b = block[16] + 0x6EEF0B6C + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = e & (g & ~b ^ d & ~c ^ f ^ c ^ h) ^ d &
			(f & b ^ g ^ c) ^ b & c ^ h;
		a = block[13] + 0x137A3BE4 + RotateRight32(t, 7) +
		    RotateRight32(a, 11);

		t = b & (d & e & g ^ ~f) ^ d & a ^ e & f ^ g & c;
		h = block[27] + 0xBA3BF050 + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = a & (c & d & f ^ ~e) ^ c & h ^ d & e ^ f & b;
		g = block[3] + 0x7EFB2A98 + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = h & (b & c & e ^ ~d) ^ b & g ^ c & d ^ e & a;
		f = block[21] + 0xA1F1651D + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = g & (a & b & d ^ ~c) ^ a & f ^ b & c ^ d & h;
		e = block[26] + 0x39AF0176 + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = f & (h & a & c ^ ~b) ^ h & e ^ a & b ^ c & g;
		d = block[17] + 0x66CA593E + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = e & (g & h & b ^ ~a) ^ g & d ^ h & a ^ b & f;
		c = block[11] + 0x82430E88 + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = d & (f & g & a ^ ~h) ^ f & c ^ g & h ^ a & e;
		b = block[20] + 0x8CEE8619 + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = c & (e & f & h ^ ~g) ^ e & b ^ f & g ^ h & d;
		a = block[29] + 0x456F9FB4 + RotateRight32(t, 7) +
		    RotateRight32(a, 11);

		t = b & (d & e & g ^ ~f) ^ d & a ^ e & f ^ g & c;
		h = block[19] + 0x7D84A5C3 + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = a & (c & d & f ^ ~e) ^ c & h ^ d & e ^ f & b;
		g = block[0] + 0x3B8B5EBE + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = h & (b & c & e ^ ~d) ^ b & g ^ c & d ^ e & a;
		f = block[12] + 0xE06F75D8 + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = g & (a & b & d ^ ~c) ^ a & f ^ b & c ^ d & h;
		e = block[7] + 0x85C12073 + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = f & (h & a & c ^ ~b) ^ h & e ^ a & b ^ c & g;
		d = block[13] + 0x401A449F + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = e & (g & h & b ^ ~a) ^ g & d ^ h & a ^ b & f;
		c = block[8] + 0x56C16AA6 + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = d & (f & g & a ^ ~h) ^ f & c ^ g & h ^ a & e;
		b = block[31] + 0x4ED3AA62 + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = c & (e & f & h ^ ~g) ^ e & b ^ f & g ^ h & d;
		a = block[10] + 0x363F7706 + RotateRight32(t, 7) +
		    RotateRight32(a, 11);

		t = b & (d & e & g ^ ~f) ^ d & a ^ e & f ^ g & c;
		h = block[5] + 0x1BFEDF72 + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = a & (c & d & f ^ ~e) ^ c & h ^ d & e ^ f & b;
		g = block[9] + 0x429B023D + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = h & (b & c & e ^ ~d) ^ b & g ^ c & d ^ e & a;
		f = block[14] + 0x37D0D724 + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = g & (a & b & d ^ ~c) ^ a & f ^ b & c ^ d & h;
		e = block[30] + 0xD00A1248 + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = f & (h & a & c ^ ~b) ^ h & e ^ a & b ^ c & g;
		d = block[18] + 0xDB0FEAD3 + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = e & (g & h & b ^ ~a) ^ g & d ^ h & a ^ b & f;
		c = block[6] + 0x49F1C09B + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = d & (f & g & a ^ ~h) ^ f & c ^ g & h ^ a & e;
		b = block[28] + 0x075372C9 + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = c & (e & f & h ^ ~g) ^ e & b ^ f & g ^ h & d;
		a = block[24] + 0x80991B7B + RotateRight32(t, 7) +
		    RotateRight32(a, 11);

		t = b & (d & e & g ^ ~f) ^ d & a ^ e & f ^ g & c;
		h = block[2] + 0x25D479D8 + RotateRight32(t, 7) +
		    RotateRight32(h, 11);

		t = a & (c & d & f ^ ~e) ^ c & h ^ d & e ^ f & b;
		g = block[23] + 0xF6E8DEF7 + RotateRight32(t, 7) +
		    RotateRight32(g, 11);

		t = h & (b & c & e ^ ~d) ^ b & g ^ c & d ^ e & a;
		f = block[16] + 0xE3FE501A + RotateRight32(t, 7) +
		    RotateRight32(f, 11);

		t = g & (a & b & d ^ ~c) ^ a & f ^ b & c ^ d & h;
		e = block[22] + 0xB6794C3B + RotateRight32(t, 7) +
		    RotateRight32(e, 11);

		t = f & (h & a & c ^ ~b) ^ h & e ^ a & b ^ c & g;
		d = block[4] + 0x976CE0BD + RotateRight32(t, 7) +
		    RotateRight32(d, 11);

		t = e & (g & h & b ^ ~a) ^ g & d ^ h & a ^ b & f;
		c = block[1] + 0x04C006BA + RotateRight32(t, 7) +
		    RotateRight32(c, 11);

		t = d & (f & g & a ^ ~h) ^ f & c ^ g & h ^ a & e;
		b = block[25] + 0xC1A94FB6 + RotateRight32(t, 7) +
		    RotateRight32(b, 11);

		t = c & (e & f & h ^ ~g) ^ e & b ^ f & g ^ h & d;
		a = block[15] + 0x409F60C4 + RotateRight32(t, 7) +
		    RotateRight32(a, 11);


		MHash[0] += a;
		MHash[1] += b;
		MHash[2] += c;
		MHash[3] += d;
		MHash[4] += e;
		MHash[5] += f;
		MHash[6] += g;
		MHash[7] += h;

		Array.Clear(block, 0, block.Length);
	}

	/**
         * Copy constructor.  This will copy the state of the provided
         * message digest.
         */
	public Haval5_256Digest(Haval5_256Digest t) : base(t) {
	}


	public override IMemoable Copy() {
		return new Haval5_256Digest(this);
	}
}
