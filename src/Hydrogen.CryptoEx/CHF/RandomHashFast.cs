// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen;
using System;
using System.Collections.Generic;
using System.Linq;
using Hydrogen.CryptoEx.HF;
using Hydrogen.Maths;
using Org.BouncyCastle.Utilities;

namespace Hydrogen.CryptoEx;


public sealed class RandomHashFast {
	private const int N = 5; // Number of hashing rounds required to compute a nonce (total rounds = 2^N - 1)
	private const int M = (10 * 1024) * 5; // 10KB The memory expansion unit (in bytes)

	private readonly Murmur3_x86_32Digest _murmurHash3;

	// declared here to avoid race-condition during mining
	private readonly Func<byte[], byte[]>[] _hashAlg = new Func<byte[], byte[]>[18];

	private byte[] _cachedHeader;
	private uint _cachedNonce;
	private ChecksummedByteCollection _cachedOutput;


	public byte[] NextHeader {
		get {
			byte[] result = new byte[0];
			if (_cachedHeader.Length > 0) {
				result = new byte[_cachedHeader.Length];
				Array.Copy(_cachedHeader, 0, result, 0, _cachedHeader.Length);
			}

			return result;
		}
	}

	private uint NextNonce => _cachedNonce;

	private static int ValidateInputData(int aBufferLength, int aReadStart, int aWriteStart, int aLength) {
		int readEnd = aReadStart + aLength - 1;
		int writeEnd = aWriteStart + aLength - 1;
		if (readEnd >= aWriteStart) {
			throw new ArgumentOutOfRangeException(string.Format("{0}", "Overlapping read/write regions"));
		}

		if (writeEnd >= aBufferLength) {
			throw new ArgumentOutOfRangeException(string.Format("Buffer '{0}' too small to apply memory transform",
				nameof(aBufferLength)));
		}

		return writeEnd;
	}

	private void MemTransform1(ref byte[] aBuffer, int aReadStart, int aWriteStart, int aLength) {
		int writeEnd = ValidateInputData(aBuffer.Length, aReadStart, aWriteStart, aLength);

		// Seed XorShift32 with non-zero seed (checksum of input or 1)
		uint state = Checksum(aBuffer, aReadStart, aLength);

		if (state == 0) {
			state = 1;
		}

		// Select random bytes from input using XorShift32 RNG
		for (int i = aWriteStart; i <= writeEnd; i++) {
			aBuffer[i] = aBuffer[aReadStart + (XorShift32.Next(ref state) % aLength)];
		}
	}

	private void MemTransform2(ref byte[] aBuffer, int aReadStart, int aWriteStart, int aLength) {
		ValidateInputData(aBuffer.Length, aReadStart, aWriteStart, aLength);

		int pivot = aLength >> 1;
		int odd = aLength % 2;
		Array.Copy(aBuffer, aReadStart + pivot + odd, aBuffer, aWriteStart, pivot);
		Array.Copy(aBuffer, aReadStart, aBuffer, aWriteStart + pivot + odd, pivot);
		// Set middle-byte for odd-length arrays
		if (odd == 1) {
			aBuffer[aWriteStart + pivot] = aBuffer[aReadStart + pivot];
		}
	}

	private unsafe void MemTransform3(ref byte[] aBuffer, int aReadStart, int aWriteStart, int aLength) {
		int writeEnd = ValidateInputData(aBuffer.Length, aReadStart, aWriteStart, aLength);

		fixed (byte* aBufferPtr = aBuffer) {
			byte* readPtr = aBufferPtr + aReadStart;
			byte* writePtr = aBufferPtr + writeEnd;

			for (int i = 0; i < aLength; i++) {
				*writePtr = *readPtr;
				readPtr++;
				writePtr--;
			}
		}
	}

	private void MemTransform4(ref byte[] aBuffer, int aReadStart, int aWriteStart, int aLength) {
		int writeEnd = ValidateInputData(aBuffer.Length, aReadStart, aWriteStart, aLength);

		int pivot = aLength >> 1;
		int odd = aLength % 2;
		for (int i = 0; i < pivot; i++) {
			aBuffer[aWriteStart + (i * 2)] = aBuffer[aReadStart + i];
			aBuffer[aWriteStart + (i * 2) + 1] = aBuffer[aReadStart + i + pivot + odd];
		}

		// Set final byte for odd-lengths
		if (odd == 1) {
			aBuffer[writeEnd] = aBuffer[aReadStart + pivot];
		}
	}

	private void MemTransform5(ref byte[] aBuffer, int aReadStart, int aWriteStart, int aLength) {
		int writeEnd = ValidateInputData(aBuffer.Length, aReadStart, aWriteStart, aLength);

		int pivot = aLength >> 1;
		int odd = aLength % 2;
		for (int i = 0; i < pivot; i++) {
			aBuffer[aWriteStart + (i * 2)] = aBuffer[aReadStart + i + pivot + odd];
			aBuffer[aWriteStart + (i * 2) + 1] = aBuffer[aReadStart + i];
		}

		// Set final byte for odd-lengths
		if (odd == 1) {
			aBuffer[writeEnd] = aBuffer[aReadStart + pivot];
		}
	}

	private void MemTransform6(ref byte[] aBuffer, int aReadStart, int aWriteStart, int aLength) {
		ValidateInputData(aBuffer.Length, aReadStart, aWriteStart, aLength);

		int pivot = aLength >> 1;
		int odd = aLength % 2;
		for (int i = 0; i < pivot; i++) {
			aBuffer[aWriteStart + i] = (byte)(aBuffer[aReadStart + (i * 2)] ^ aBuffer[aReadStart + (i * 2) + 1]);
			aBuffer[aWriteStart + i + pivot + odd] =
				(byte)(aBuffer[aReadStart + i] ^ aBuffer[aReadStart + aLength - i - 1]);
		}

		// Set middle-byte for odd-lengths
		if (odd == 1) {
			aBuffer[aWriteStart + pivot] = aBuffer[aReadStart + aLength - 1];
		}
	}


	private void MemTransform7(ref byte[] aBuffer, int aReadStart, int aWriteStart, int aLength) {
		ValidateInputData(aBuffer.Length, aReadStart, aWriteStart, aLength);

		for (int i = 0; i < aLength; i++) {
			aBuffer[aWriteStart + i] = Bits.RotateLeft8(aBuffer[aReadStart + i], aLength - i);
		}
	}

	private void MemTransform8(ref byte[] aBuffer, int aReadStart, int aWriteStart, int aLength) {
		ValidateInputData(aBuffer.Length, aReadStart, aWriteStart, aLength);

		for (int i = 0; i < aLength; i++) {
			aBuffer[aWriteStart + i] = Bits.RotateRight8(aBuffer[aReadStart + i], aLength - i);
		}
	}


	private byte[] Expand(byte[] aInput, int aExpansionFactor) {
		int inputSize = aInput.Length;
		Mersenne32 gen = new Mersenne32(Checksum(aInput));
		byte[] output = new byte[inputSize + (aExpansionFactor * M)];

		// Copy the genesis blob
		Array.Copy(aInput, 0, output, 0, inputSize);
		int readEnd = inputSize - 1;
		int copyLen = inputSize;

		while (readEnd < (output.Length - 1)) {
			if ((readEnd + 1 + copyLen) > output.Length) {
				copyLen = (output.Length) - (readEnd + 1);
			}

			switch (gen.NextUInt32() % 8) {
				case 0:
					MemTransform1(ref output, 0, readEnd + 1, copyLen);
					break;
				case 1:
					MemTransform2(ref output, 0, readEnd + 1, copyLen);
					break;
				case 2:
					MemTransform3(ref output, 0, readEnd + 1, copyLen);
					break;
				case 3:
					MemTransform4(ref output, 0, readEnd + 1, copyLen);
					break;
				case 4:
					MemTransform5(ref output, 0, readEnd + 1, copyLen);
					break;
				case 5:
					MemTransform6(ref output, 0, readEnd + 1, copyLen);
					break;
				case 6:
					MemTransform7(ref output, 0, readEnd + 1, copyLen);
					break;
				case 7:
					MemTransform8(ref output, 0, readEnd + 1, copyLen);
					break;

				default:
					throw new ArgumentException("invalid argument");
			}

			readEnd += copyLen;
			copyLen += copyLen;
		}

		return output;
	}

	private byte[] Compress(ChecksummedByteCollection aInputs) {
		byte[] result = new byte[100];
		uint seed = aInputs.Checksum;
		Mersenne32 gen = new Mersenne32(seed);
		int inputCount = aInputs.Count;
		for (int i = 0; i < 100; i++) {
			byte[] source = aInputs.Get((int)(gen.NextUInt32() % inputCount));
			result[i] = source[gen.NextUInt32() % source.Length];
		}

		return result;
	}

	private uint GetNonce(byte[] aBlockHeader) {
		int len = aBlockHeader.Length;
		if (len < 4) {
			throw new ArgumentOutOfRangeException(string.Format("Buffer '{0}' too small to contain nonce",
				nameof(aBlockHeader)));
		}

		// Last 4 bytes are nonce (LE)
		return (uint)(aBlockHeader[len - 4] |
					   (aBlockHeader[len - 3] << 8) |
					   (aBlockHeader[len - 2] << 16) |
					   (aBlockHeader[len - 1] << 24));
	}

	private byte[] ChangeNonce(byte[] aBlockHeader, uint aNonce) {
		// NOTE: NONCE is last 4 bytes of header!

		// Clone the original header
		byte[] result = (byte[])aBlockHeader.Clone();

		// If digest not big enough to contain a nonce, just return the clone
		int headerLength = aBlockHeader.Length;
		if (headerLength < 4) {
			return result;
		}

		// Overwrite the nonce in little-endian
		result[headerLength - 4] = (byte)aNonce;
		result[headerLength - 3] = (byte)((aNonce >> 8) & 255);
		result[headerLength - 2] = (byte)((aNonce >> 16) & 255);
		result[headerLength - 1] = (byte)((aNonce >> 24) & 255);
		return result;
	}

	private uint Checksum(byte[] aInput) {
		return Checksum(aInput, 0, aInput.Length);
	}

	private uint Checksum(byte[] aInput, int aOffset, int aLength) {
		_murmurHash3.Reset();
		_murmurHash3.BlockUpdate(aInput, aOffset, aLength);
		return _murmurHash3.DoFinal();
	}

	private byte[] Hash(byte[] aBlockHeader) {
		ChecksummedByteCollection allOutputs = Hash(aBlockHeader, N);
		return _hashAlg[0](Compress(allOutputs));
	}

	private ChecksummedByteCollection Hash(byte[] aBlockHeader, int aRound) {
		if ((aRound < 1) || (aRound > N)) {
			throw new ArgumentOutOfRangeException(string.Format("Round '{0}' must be between 0 and N inclusive",
				nameof(aRound)));
		}

		// NOTE: instance is destroyed by caller!
		ChecksummedByteCollection roundOutputs = new ChecksummedByteCollection();

		var gen = new Mersenne32Algorithm(0);
		byte[] roundInput;
		uint seed;
		if (aRound == 1) {
			seed = Checksum(aBlockHeader);
			gen.Initialize(seed);
			roundInput = aBlockHeader;
		} else {
			ChecksummedByteCollection parentOutputs;
			if ((aRound == N) && (aBlockHeader.Length >= 4) && (NextNonce == GetNonce(aBlockHeader)) &&
			    _cachedHeader is not null && aBlockHeader.SequenceEqual(_cachedHeader)) {
				// Parent (round N - 1) has already been calculated so re-use values. This saves 50% of calculations!
				parentOutputs = _cachedOutput;
			} else {
				// Need to calculate parent output
				parentOutputs = Hash(aBlockHeader, aRound - 1);
			}

			seed = parentOutputs.Checksum;

			gen.Initialize(seed);
			roundOutputs.AddRange(parentOutputs);

			// Determine the neighbouring nonce
			uint neighbourNonce = gen.NextUInt32();
			byte[] neighbourNonceHeader = ChangeNonce(aBlockHeader, neighbourNonce);
			ChecksummedByteCollection neighbourOutputs = Hash(neighbourNonceHeader, aRound - 1);

			// Cache neighbour nonce n-1 calculation if on final round (neighbour will be next nonce)
			if (aRound == N) {
				_cachedNonce = neighbourNonce;
				_cachedHeader = neighbourNonceHeader;
				_cachedOutput = neighbourOutputs.Clone();
			}

			roundOutputs.AddRange(neighbourOutputs);
			roundInput = Compress(roundOutputs);
		}

		Func<byte[], byte[]> hashFunc = _hashAlg[gen.NextUInt32() % 18];
		byte[] output = hashFunc(roundInput);
		output = Expand(output, N - aRound);
		roundOutputs.Add(output);

		return roundOutputs;
	}

	public RandomHashFast() {
		_murmurHash3 = new Murmur3_x86_32Digest();
		_hashAlg[0] = x => Hashers.Hash(CHF.SHA2_256, x);
		_hashAlg[1] = x => Hashers.Hash(CHF.SHA2_384, x);
		_hashAlg[2] = x => Hashers.Hash(CHF.SHA2_512, x);
		_hashAlg[3] = x => Hashers.Hash(CHF.SHA3_256, x);
		_hashAlg[4] = x => Hashers.Hash(CHF.SHA3_384, x);
		_hashAlg[5] = x => Hashers.Hash(CHF.SHA3_512, x);
		_hashAlg[6] = x => Hashers.Hash(CHF.RIPEMD_160, x);
		_hashAlg[7] = x => Hashers.Hash(CHF.RIPEMD_256, x);
		_hashAlg[8] = x => Hashers.Hash(CHF.RIPEMD_320, x);
		_hashAlg[9] = x => Hashers.Hash(CHF.Blake2b_512, x);
		_hashAlg[10] = x => Hashers.Hash(CHF.Blake2s_256, x);
		_hashAlg[11] = x => Hashers.Hash(CHF.Tiger2_5_192, x);
		_hashAlg[12] = x => Hashers.Hash(CHF.Snefru_8_256, x);
		_hashAlg[13] = x => Hashers.Hash(CHF.Grindahl512, x);
		_hashAlg[14] = x => Hashers.Hash(CHF.Haval_5_256, x);
		_hashAlg[15] = x => Hashers.Hash(CHF.MD5, x);
		_hashAlg[16] = x => Hashers.Hash(CHF.RadioGatun32, x);
		_hashAlg[17] = x => Hashers.Hash(CHF.WhirlPool, x);
	}

	public static byte[] Compute(byte[] aBlockHeader) {
		RandomHashFast hasher = new RandomHashFast();
		return hasher.Hash(aBlockHeader);
	}

	public sealed class ChecksummedByteCollection {
		private List<byte[]> _bytes;
		private int _computedIndex;
		private uint _checksum;
		private Murmur3_x86_32Digest _murMur3;


		public int Count => _bytes.Count;

		public uint Checksum {
			get {
				if (_computedIndex == _bytes.Count - 1) {
					return _checksum; // already computed
				}

				for (int i = _computedIndex + 1; i < _bytes.Count; i++) {
					_murMur3.BlockUpdate(_bytes[i], 0, _bytes[i].Length);
					_computedIndex++;
				}

				IMemoable clonedMurMur3 = _murMur3.Copy();
				_checksum = _murMur3.DoFinal();
				// note: original instance should collect with implicit dereference
				_murMur3 = (Murmur3_x86_32Digest)clonedMurMur3;
				return _checksum;
			}
		}

		public ChecksummedByteCollection() : this(new byte[0][]) {
		}

		private ChecksummedByteCollection(byte[][] aManyBytes) {
			_bytes = new List<byte[]>();
			_computedIndex = -1;
			_checksum = 0;
			_murMur3 = new Murmur3_x86_32Digest();
			AddRange(aManyBytes);
		}

		public ChecksummedByteCollection Clone() {
			ChecksummedByteCollection result = new ChecksummedByteCollection { _bytes = new List<byte[]>() };
			result._bytes.AddRange(_bytes);
			result._computedIndex = _computedIndex;
			result._checksum = Checksum;
			result._murMur3 = (Murmur3_x86_32Digest)_murMur3.Copy();
			return result;
		}

		public byte[] Get(int aIndex) {
			return _bytes[aIndex];
		}

		public void Add(byte[] aBytes) {
			_bytes.Add(aBytes);
		}

		private void AddRange(byte[][] aManyBytes) {
			_bytes.AddRange(aManyBytes);
		}

		public void AddRange(ChecksummedByteCollection aCollection) {
			if (_bytes.Count == 0) {
				// Is empty so just copy checksum from argument
				_computedIndex = aCollection._computedIndex;
				_checksum = aCollection.Checksum;
				_murMur3 = (Murmur3_x86_32Digest)aCollection._murMur3.Copy();
			}

			_bytes.AddRange(aCollection._bytes);
		}

		public void Clear() {
			_bytes.Clear();
			_computedIndex = -1;
			_checksum = 0;
			// note: original instance should collect with implicit dereference 
			_murMur3 = new Murmur3_x86_32Digest();
		}

		public byte[] ToByteArray() {
			List<byte> list = new List<byte>();
			foreach (byte[] bytes in _bytes) {
				list.AddRange(bytes);
			}

			return list.ToArray();
		}
	}
}
