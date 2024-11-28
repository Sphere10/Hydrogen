// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Hydrogen.Maths;

namespace Hydrogen.CryptoEx;

public class RandomHash2 {
	protected const string InvalidRound = "Round must be between 0 and N inclusive";
	protected const int MIN_N = 2; // Min-number of hashing rounds required to compute a nonce
	protected const int MAX_N = 4; // Max-number of hashing rounds required to compute a nonce
	protected const int MIN_J = 1; // Min-number of dependent neighbouring nonces required to evaluate a nonce round
	protected const int MAX_J = 8; // Max-number of dependent neighbouring nonces required to evaluate a nonce round
	protected const int M = 64; // The memory expansion unit (in bytes)
	protected const int NUM_HASH_ALGO = 77;
	protected const int SHA2_256_IX = 47;
	protected readonly CHF[] HashAlgorithms = new CHF[NUM_HASH_ALGO];

	protected RandomHash2() {
		HashAlgorithms[0] = CHF.Blake2b_160;
		HashAlgorithms[1] = CHF.Blake2b_256;
		HashAlgorithms[2] = CHF.Blake2b_512;
		HashAlgorithms[3] = CHF.Blake2b_384;
		HashAlgorithms[4] = CHF.Blake2s_128;
		HashAlgorithms[5] = CHF.Blake2s_160;
		HashAlgorithms[6] = CHF.Blake2s_224;
		HashAlgorithms[7] = CHF.Blake2s_256;
		HashAlgorithms[8] = CHF.Gost;
		HashAlgorithms[9] = CHF.Gost3411_2012_256;
		HashAlgorithms[10] = CHF.Gost3411_2012_512;
		HashAlgorithms[11] = CHF.Grindahl256;
		HashAlgorithms[12] = CHF.Grindahl512;
		HashAlgorithms[13] = CHF.Has160;
		HashAlgorithms[14] = CHF.Haval_3_128;
		HashAlgorithms[15] = CHF.Haval_3_160;
		HashAlgorithms[16] = CHF.Haval_3_192;
		HashAlgorithms[17] = CHF.Haval_3_224;
		HashAlgorithms[18] = CHF.Haval_3_256;
		HashAlgorithms[19] = CHF.Haval_4_128;
		HashAlgorithms[20] = CHF.Haval_4_160;
		HashAlgorithms[21] = CHF.Haval_4_192;
		HashAlgorithms[22] = CHF.Haval_4_224;
		HashAlgorithms[23] = CHF.Haval_4_256;
		HashAlgorithms[24] = CHF.Haval_5_128;
		HashAlgorithms[25] = CHF.Haval_5_160;
		HashAlgorithms[26] = CHF.Haval_5_192;
		HashAlgorithms[27] = CHF.Haval_5_224;
		HashAlgorithms[28] = CHF.Haval_5_256;
		HashAlgorithms[29] = CHF.Keccak_224;
		HashAlgorithms[30] = CHF.Keccak_256;
		HashAlgorithms[31] = CHF.Keccak_288;
		HashAlgorithms[32] = CHF.Keccak_384;
		HashAlgorithms[33] = CHF.Keccak_512;
		HashAlgorithms[34] = CHF.MD2;
		HashAlgorithms[35] = CHF.MD5;
		HashAlgorithms[36] = CHF.MD4;
		HashAlgorithms[37] = CHF.Panama;
		HashAlgorithms[38] = CHF.RadioGatun32;
		HashAlgorithms[39] = CHF.RIPEMD;
		HashAlgorithms[40] = CHF.RIPEMD_128;
		HashAlgorithms[41] = CHF.RIPEMD_160;
		HashAlgorithms[42] = CHF.RIPEMD_256;
		HashAlgorithms[43] = CHF.RIPEMD_320;
		HashAlgorithms[44] = CHF.SHA0;
		HashAlgorithms[45] = CHF.SHA1_160;
		HashAlgorithms[46] = CHF.SHA2_224;
		HashAlgorithms[47] = CHF.SHA2_256;
		HashAlgorithms[48] = CHF.SHA2_384;
		HashAlgorithms[49] = CHF.SHA2_512;
		HashAlgorithms[50] = CHF.SHA2_512_224;
		HashAlgorithms[51] = CHF.SHA2_512_256;
		HashAlgorithms[52] = CHF.SHA3_224;
		HashAlgorithms[53] = CHF.SHA3_256;
		HashAlgorithms[54] = CHF.SHA3_384;
		HashAlgorithms[55] = CHF.SHA3_512;
		HashAlgorithms[56] = CHF.Snefru_8_128;
		HashAlgorithms[57] = CHF.Snefru_8_256;
		HashAlgorithms[58] = CHF.Tiger_3_128;
		HashAlgorithms[59] = CHF.Tiger_3_160;
		HashAlgorithms[60] = CHF.Tiger_3_192;
		HashAlgorithms[61] = CHF.Tiger_4_128;
		HashAlgorithms[62] = CHF.Tiger_4_160;
		HashAlgorithms[63] = CHF.Tiger_4_192;
		HashAlgorithms[64] = CHF.Tiger_5_128;
		HashAlgorithms[65] = CHF.Tiger_5_160;
		HashAlgorithms[66] = CHF.Tiger_5_192;
		HashAlgorithms[67] = CHF.Tiger2_3_128;
		HashAlgorithms[68] = CHF.Tiger2_3_160;
		HashAlgorithms[69] = CHF.Tiger2_3_192;
		HashAlgorithms[70] = CHF.Tiger2_4_128;
		HashAlgorithms[71] = CHF.Tiger2_4_160;
		HashAlgorithms[72] = CHF.Tiger2_4_192;
		HashAlgorithms[73] = CHF.Tiger2_5_128;
		HashAlgorithms[74] = CHF.Tiger2_5_160;
		HashAlgorithms[75] = CHF.Tiger2_5_192;
		HashAlgorithms[76] = CHF.WhirlPool;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryHash(byte[] blockHeader, uint maxRound, out byte[] hash) {
		if (!CalculateRoundOutputs(blockHeader, (int)maxRound, out var outputs)) {
			hash = null;
			return false;
		}

		hash = ComputeVeneerRound(outputs);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte[] Hash(byte[] blockHeader) {
		if (!TryHash(blockHeader, MAX_N, out var result))
			throw new RandomHash2Exception(
				"Internal Error: 984F52997131417E8D63C43BD686F5B2); // Should have found final round!"); // Should have found final round!
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] Compute(byte[] blockHeader) => RandomHash2Instance().Hash(blockHeader);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RandomHash2 RandomHash2Instance() => new RandomHash2();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static byte[] Compress(byte[][] inputs, uint seed) {
		var result = new byte[100];
		var generator = new Mersenne32Algorithm(seed);
		for (var idx = 0; idx <= 99; idx++) {
			var source = inputs[generator.NextUInt32() % inputs.Length];
			result[idx] = source[generator.NextUInt32() % source.Length];
		}

		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte[] MemTransform1(byte[] chunk) {
		// Seed XorShift32 with last byte
		var state = RandomHashUtils.GetLastDWordLE(chunk);
		if (state == 0)
			state = 1;

		// Select random bytes from input using XorShift32 RNG
		var chunkLength = chunk.Length;
		var result = new byte[chunkLength];
		for (var idx = 0; idx < chunkLength; idx++)
			result[idx] = chunk[XorShift32.Next(ref state) % chunkLength];

		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte[] MemTransform2(byte[] chunk) {
		var chunkLength = chunk.Length;
		var pivot = chunkLength >> 1;
		var odd = chunkLength % 2;
		var result = new byte[chunkLength];
		Buffer.BlockCopy(chunk, pivot + odd, result, 0, pivot);
		Buffer.BlockCopy(chunk, 0, result, pivot + odd, pivot);
		// Set middle-byte for odd-length arrays
		if (odd == 1)
			result[pivot] = chunk[pivot];
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte[] MemTransform3(byte[] chunk) {
		var chunkLength = chunk.Length;
		var result = new byte[chunkLength];
		for (var idx = 0; idx < chunkLength; idx++)
			result[idx] = chunk[chunkLength - idx - 1];

		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte[] MemTransform4(byte[] chunk) {
		var chunkLength = chunk.Length;
		var pivot = chunkLength >> 1;
		var odd = chunkLength % 2;
		var result = new byte[chunkLength];
		for (var idx = 0; idx < pivot; idx++) {
			result[idx * 2] = chunk[idx];
			result[idx * 2 + 1] = chunk[idx + pivot + odd];
		}

		// Set final byte for odd-lengths
		if (odd == 1)
			result[chunkLength - 1] = chunk[pivot];
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte[] MemTransform5(byte[] chunk) {
		var chunkLength = chunk.Length;
		var pivot = chunkLength >> 1;
		var odd = chunkLength % 2;
		var result = new byte[chunkLength];
		for (var idx = 0; idx < pivot; idx++) {
			result[idx * 2] = chunk[idx + pivot + odd];
			result[idx * 2 + 1] = chunk[idx];
		}

		// Set final byte for odd-lengths
		if (odd == 1)
			result[chunkLength - 1] = chunk[pivot];
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte[] MemTransform6(byte[] chunk) {
		var chunkLength = chunk.Length;
		var pivot = chunkLength >> 1;
		var odd = chunkLength % 2;
		var result = new byte[chunkLength];
		for (var idx = 0; idx < pivot; idx++) {
			result[idx] = (byte)(chunk[idx * 2] ^ chunk[idx * 2 + 1]);
			result[idx + pivot + odd] = (byte)(chunk[idx] ^ chunk[chunkLength - idx - 1]);
		}

		// Set middle-byte for odd-lengths
		if (odd == 1)
			result[pivot] = chunk[^1];
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte[] MemTransform7(byte[] chunk) {
		var chunkLength = chunk.Length;
		var result = new byte[chunkLength];
		for (var idx = 0; idx < chunkLength; idx++)
			result[idx] = RandomHashUtils.RotateLeft8(chunk[idx], chunkLength - idx);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte[] MemTransform8(byte[] chunk) {
		var chunkLength = chunk.Length;
		var result = new byte[chunkLength];
		for (var idx = 0; idx < chunkLength; idx++)
			result[idx] = RandomHashUtils.RotateRight8(chunk[idx], chunkLength - idx);
		return result;
	}

	private static byte[] Expand(byte[] input, int expansionFactor, uint seed) {
		var generator = new Mersenne32Algorithm(seed);
		var size = input.Length + expansionFactor * M;
		var result = RandomHashUtils.Clone(input);
		var bytesToAdd = size - input.Length;

		while (bytesToAdd > 0) {
			var nextChunk = RandomHashUtils.Clone(result);
			if (nextChunk.Length > bytesToAdd) {
				Array.Resize(ref nextChunk, bytesToAdd);
			}

			var random = generator.NextUInt32();
			switch (random % 8) {
				case 0:
					result = RandomHashUtils.Concatenate(result, MemTransform1(nextChunk));
					break;
				case 1:
					result = RandomHashUtils.Concatenate(result, MemTransform2(nextChunk));
					break;
				case 2:
					result = RandomHashUtils.Concatenate(result, MemTransform3(nextChunk));
					break;
				case 3:
					result = RandomHashUtils.Concatenate(result, MemTransform4(nextChunk));
					break;
				case 4:
					result = RandomHashUtils.Concatenate(result, MemTransform5(nextChunk));
					break;
				case 5:
					result = RandomHashUtils.Concatenate(result, MemTransform6(nextChunk));
					break;
				case 6:
					result = RandomHashUtils.Concatenate(result, MemTransform7(nextChunk));
					break;
				case 7:
					result = RandomHashUtils.Concatenate(result, MemTransform8(nextChunk));
					break;
			}

			bytesToAdd -= nextChunk.Length;
		}

		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private byte[] ComputeVeneerRound(byte[][] roundOutputs) {
		var seed = RandomHashUtils.GetLastDWordLE(roundOutputs[roundOutputs.Length - 1]);
		// Final "veneer" round of RandomHash is a SHA2-256 of compression of prior round outputs
		return Hashers.Hash(HashAlgorithms[SHA2_256_IX], Compress(roundOutputs, seed));
	}

	private bool CalculateRoundOutputs(byte[] blockHeader, int round, out byte[][] roundOutputs) {
		if (round < 1 || round > MAX_N)
			throw new ArgumentOutOfRangeException(InvalidRound);

		var roundOutputsList = new List<byte[]>();
		var generator = new Mersenne32Algorithm(0);
		byte[] roundInput;
		uint seed;
		if (round == 1) {
			roundInput = Hashers.Hash(HashAlgorithms[SHA2_256_IX], blockHeader);
			seed = RandomHashUtils.GetLastDWordLE(roundInput);
			generator.Initialize(seed);
		} else {
			if (CalculateRoundOutputs(blockHeader, round - 1, out var parentOutputs)) {
				// Previous round was the final round, so just return it's value
				roundOutputs = parentOutputs;
				return true;
			}

			// Add parent round outputs to this round outputs
			seed = RandomHashUtils.GetLastDWordLE(parentOutputs[parentOutputs.Length - 1]);
			generator.Initialize(seed);
			roundOutputsList.AddRange(parentOutputs);

			// Add neighbouring nonce outputs to this round outputs
			var numNeighbours = generator.NextUInt32() % (MAX_J - MIN_J) + MIN_J;
			for (var i = 1; i <= numNeighbours; i++) {
				var neighbourNonceHeader =
					RandomHashUtils.SetLastDWordLE(blockHeader, generator.NextUInt32()); // change nonce
				CalculateRoundOutputs(neighbourNonceHeader, round - 1, out var neighborOutputs);
				roundOutputsList.AddRange(neighborOutputs);
			}

			// Compress the parent/neighbouring outputs to form this rounds input
			roundInput = Compress(roundOutputsList.ToArray(), generator.NextUInt32());
		}

		// Select a random hash function and hash the input to find the output
		var output = Hashers.Hash(HashAlgorithms[generator.NextUInt32() % NUM_HASH_ALGO], roundInput);

		// Memory-expand the output, add to output list and return output list
		output = Expand(output, MAX_N - round, generator.NextUInt32());
		roundOutputsList.Add(output);
		roundOutputs = roundOutputsList.ToArray();

		// Determine if final round
		return round == MAX_N || round >= MIN_N && RandomHashUtils.GetLastDWordLE(output) % MAX_N == 0;
	}

}
