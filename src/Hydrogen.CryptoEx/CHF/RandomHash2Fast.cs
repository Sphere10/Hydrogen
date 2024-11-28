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

public sealed class RandomHash2Fast : RandomHash2 {
	private const string OverlappingArgs = "Overlapping read/write regions";
	private const string BufferTooSmall = "Buffer too small to apply memory transform";


	public Cache CacheInstance { get; private set; }
	public bool EnableCaching { get; set; }

	public bool CaptureMemStats { get; set; }

	public Statistics MemStats { get; }

	public RandomHash2Fast() {
		EnableCaching = false;
		CacheInstance = new Cache();
		MemStats = new Statistics();
		MemStats.Reset();
	}

	~RandomHash2Fast() {
		CacheInstance = null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public new bool TryHash(byte[] blockHeader, uint maxRound, out byte[] hash) {
		CachedHash? cachedHash = null;
		if (!CalculateRoundOutputs(blockHeader, (int)maxRound, ref cachedHash, out var outputs)) {
			hash = null;
			return false;
		}

		hash = ComputeVeneerRound(outputs);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public new byte[] Hash(byte[] blockHeader) {
		if (!TryHash(blockHeader, MAX_N, out var result))
			throw new RandomHash2Exception(
				"Internal Error: 974F52882131417E8D63A43BD686E5B2); // Should have found final round!"); // Should have found final round!
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public new static byte[] Compute(byte[] blockHeader) => new RandomHash2Fast().Hash(blockHeader);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void MemTransform1(ref byte[] chunk, int readStart, int writeStart, int length) {
		var readEnd = readStart + length - 1;
		var writeEnd = writeStart + length - 1;
		if (readEnd >= writeStart)
			throw new ArgumentOutOfRangeException(OverlappingArgs);
		// Seed XorShift with last byte
		var state = RandomHashUtils.GetDWordLE(chunk, readEnd - 3);
		if (state == 0)
			state = 1;

		// Select random bytes from input using XorShift RNG
		for (var idx = writeStart; idx <= writeEnd; idx++)
			chunk[idx] = chunk[readStart + XorShift32.Next(ref state) % length];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void MemTransform2(ref byte[] chunk, int readStart, int writeStart, int length) {
		var readEnd = readStart + length - 1;
		var writeEnd = writeStart + length - 1;
		if (readEnd >= writeStart)
			throw new ArgumentOutOfRangeException(OverlappingArgs);
		if (writeEnd >= chunk.Length)
			throw new ArgumentOutOfRangeException(BufferTooSmall);

		var pivot = length >> 1;
		var odd = length % 2;
		Buffer.BlockCopy(chunk, readStart + pivot + odd, chunk, writeStart, pivot);
		Buffer.BlockCopy(chunk, readStart, chunk, writeStart + pivot + odd, pivot);
		// Set middle-byte for odd-length arrays
		if (odd == 1)
			chunk[writeStart + pivot] = chunk[readStart + pivot];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void MemTransform3(ref byte[] chunk, int readStart, int writeStart, int length) {
		var readEnd = readStart + length - 1;
		var writeEnd = writeStart + length - 1;
		if (readEnd >= writeStart)
			throw new ArgumentOutOfRangeException(OverlappingArgs);
		if (writeEnd >= chunk.Length)
			throw new ArgumentOutOfRangeException(BufferTooSmall);

		for (var idx = 0; idx < length; idx++)
			chunk[writeEnd--] = chunk[readStart++];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void MemTransform4(ref byte[] chunk, int readStart, int writeStart, int length) {
		var readEnd = readStart + length - 1;
		var writeEnd = writeStart + length - 1;
		if (readEnd >= writeStart)
			throw new ArgumentOutOfRangeException(OverlappingArgs);
		if (writeEnd >= chunk.Length)
			throw new ArgumentOutOfRangeException(BufferTooSmall);

		var pivot = length >> 1;
		var odd = length % 2;
		for (var idx = 0; idx < pivot; idx++) {
			chunk[writeStart + idx * 2] = chunk[readStart + idx];
			chunk[writeStart + idx * 2 + 1] = chunk[readStart + idx + pivot + odd];
		}

		// Set final byte for odd-lengths
		if (odd == 1)
			chunk[writeEnd] = chunk[readStart + pivot];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void MemTransform5(ref byte[] chunk, int readStart, int writeStart, int length) {
		var readEnd = readStart + length - 1;
		var writeEnd = writeStart + length - 1;
		if (readEnd >= writeStart)
			throw new ArgumentOutOfRangeException(OverlappingArgs);
		if (writeEnd >= chunk.Length)
			throw new ArgumentOutOfRangeException(BufferTooSmall);

		var pivot = length >> 1;
		var odd = length % 2;
		for (var idx = 0; idx < pivot; idx++) {
			chunk[writeStart + idx * 2] = chunk[readStart + idx + pivot + odd];
			chunk[writeStart + idx * 2 + 1] = chunk[readStart + idx];
		}

		// Set final byte for odd-lengths
		if (odd == 1)
			chunk[writeEnd] = chunk[readStart + pivot];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void MemTransform6(ref byte[] chunk, int readStart, int writeStart, int length) {
		var readEnd = readStart + length - 1;
		var writeEnd = writeStart + length - 1;
		if (readEnd >= writeStart)
			throw new ArgumentOutOfRangeException(OverlappingArgs);
		if (writeEnd >= chunk.Length)
			throw new ArgumentOutOfRangeException(BufferTooSmall);

		var pivot = length >> 1;
		var odd = length % 2;
		for (var idx = 0; idx < pivot; idx++) {
			chunk[writeStart + idx] = (byte)(chunk[readStart + idx * 2] ^ chunk[readStart + idx * 2 + 1]);
			chunk[writeStart + idx + pivot + odd] =
				(byte)(chunk[readStart + idx] ^ chunk[readStart + length - idx - 1]);
		}

		// Set middle-byte for odd-lengths
		if (odd == 1)
			chunk[writeStart + pivot] = chunk[readStart + length - 1];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void MemTransform7(ref byte[] chunk, int readStart, int writeStart, int length) {
		var readEnd = readStart + length - 1;
		var writeEnd = writeStart + length - 1;
		if (readEnd >= writeStart)
			throw new ArgumentOutOfRangeException(OverlappingArgs);
		if (writeEnd >= chunk.Length)
			throw new ArgumentOutOfRangeException(BufferTooSmall);

		for (var idx = 0; idx < length; idx++)
			chunk[writeStart + idx] = RandomHashUtils.RotateLeft8(chunk[readStart + idx], length - idx);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void MemTransform8(ref byte[] chunk, int readStart, int writeStart, int length) {
		var readEnd = readStart + length - 1;
		var writeEnd = writeStart + length - 1;
		if (readEnd >= writeStart)
			throw new ArgumentOutOfRangeException(OverlappingArgs);
		if (writeEnd >= chunk.Length)
			throw new ArgumentOutOfRangeException(BufferTooSmall);

		for (var idx = 0; idx < length; idx++)
			chunk[writeStart + idx] = RandomHashUtils.RotateRight8(chunk[readStart + idx], length - idx);
	}

	private static byte[] Expand(byte[] input, int expansionFactor, uint seed) {
		var generator = new Mersenne32Algorithm(seed);
		var inputLength = input.Length;
		var result = new byte[inputLength + expansionFactor * M];
		// Copy the genesis blob
		Buffer.BlockCopy(input, 0, result, 0, inputLength);
		var readEnd = inputLength - 1;
		var copyLen = inputLength;

		while (readEnd < result.Length - 1) {
			if (readEnd + 1 + copyLen > result.Length)
				copyLen = result.Length - (readEnd + 1);

			var random = generator.NextUInt32();
			switch (random % 8) {
				case 0:
					MemTransform1(ref result, 0, readEnd + 1, copyLen);
					break;
				case 1:
					MemTransform2(ref result, 0, readEnd + 1, copyLen);
					break;
				case 2:
					MemTransform3(ref result, 0, readEnd + 1, copyLen);
					break;
				case 3:
					MemTransform4(ref result, 0, readEnd + 1, copyLen);
					break;
				case 4:
					MemTransform5(ref result, 0, readEnd + 1, copyLen);
					break;
				case 5:
					MemTransform6(ref result, 0, readEnd + 1, copyLen);
					break;
				case 6:
					MemTransform7(ref result, 0, readEnd + 1, copyLen);
					break;
				case 7:
					MemTransform8(ref result, 0, readEnd + 1, copyLen);
					break;
			}

			readEnd += copyLen;
			copyLen += copyLen;
		}

		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private byte[] ComputeVeneerRound(byte[][] roundOutputs) {
		var seed = RandomHashUtils.GetLastDWordLE(roundOutputs[roundOutputs.Length - 1]);
		// Final "veneer" round of RandomHash is a SHA2-256 of compression of prior round outputs
		var result = Hashers.Hash(HashAlgorithms[SHA2_256_IX], Compress(roundOutputs, seed));

		if (CaptureMemStats) {
			var size = 0;
			for (var idx = 0; idx <= roundOutputs.Length - 1; idx++) {
				size += roundOutputs[idx].Length;
				if (EnableCaching)
					size += CacheInstance.ComputeMemorySize();
				MemStats.AddDatum(size);
			}
		}

		return result;
	}

	private bool CalculateRoundOutputs(byte[] blockHeader, int round, ref CachedHash? cachedHash, out byte[][] roundOutputs) {
		if (round < 1 || round > MAX_N)
			throw new ArgumentOutOfRangeException(InvalidRound);

		if (cachedHash.HasValue) {
			if (cachedHash.Value.Level == round &&
			    RandomHashUtils.BytesEqual(cachedHash.Value.Header, blockHeader)) {
				roundOutputs = cachedHash.Value.RoundOutputs;
				return false; // assume partially evaluated 
			}
		}

		var roundOutputsList = new List<byte[]>();
		var generator = new Mersenne32Algorithm(0);
		byte[] roundInput;
		uint seed;
		if (round == 1) {
			roundInput = Hashers.Hash(HashAlgorithms[SHA2_256_IX], blockHeader);
			seed = RandomHashUtils.GetLastDWordLE(roundInput);
			generator.Initialize(seed);
		} else {
			if (CalculateRoundOutputs(blockHeader, round - 1, ref cachedHash, out var parentOutputs)) {
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
				CachedHash? internalCachedHash = null;
				var neighbourWasLastRound = CalculateRoundOutputs(neighbourNonceHeader,
					round - 1,
					ref internalCachedHash,
					out var neighborOutputs);
				roundOutputsList.AddRange(neighborOutputs);

				// If neighbour was a fully evaluated nonce, cache it for re-use
				if (!EnableCaching) continue;
				if (neighbourWasLastRound)
					CacheInstance.AddFullyComputed(neighbourNonceHeader,
						round - 1,
						ComputeVeneerRound(neighborOutputs));
				else
					CacheInstance.AddPartiallyComputed(neighbourNonceHeader, round - 1, neighborOutputs);
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


	public class Cache {
		private byte[] _headerTemplate;
		private List<CachedHash> _computed, _partiallyComputed;

		public Cache() {
			EnablePartiallyComputed = false;
			_computed = new List<CachedHash> { Capacity = 100 };
			Array.Resize(ref _headerTemplate, 0);
			_partiallyComputed = new List<CachedHash> { Capacity = 1000 };
		}

		~Cache() {
			_computed.Clear();
			_computed = null;
			_partiallyComputed.Clear();
			_partiallyComputed = null;
		}

		public bool EnablePartiallyComputed { get; set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void PreProcessNewHash(byte[] header) {
			// if header is a new template, flush cache
			if (RandomHashUtils.BytesEqual(_headerTemplate, header, 0, 32 - 4))
				return;
			Clear();
			_headerTemplate = RandomHashUtils.SetLastDWordLE(header, 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddPartiallyComputed(byte[] header, int level, byte[][] outputs) {
			PreProcessNewHash(header);
			if (!EnablePartiallyComputed)
				return;

			// Only keep 10 level 3's partially calculated
			if (level < 3 || _partiallyComputed.Count > 10)
				return;

			var cachedHash = new CachedHash {
				Nonce = RandomHashUtils.GetLastDWordLE(header),
				Header = header,
				Level = level,
				RoundOutputs = outputs
			};
			_partiallyComputed.Add(cachedHash);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddFullyComputed(byte[] header, int level, byte[] hash) {
			PreProcessNewHash(header);
			var cachedHash = new CachedHash {
				Nonce = RandomHashUtils.GetLastDWordLE(header),
				Header = header,
				Level = level,
				RoundOutputs = new[] { hash }
			};
			_computed.Add(cachedHash);
		}

		public void Clear() {
			_computed.Clear();
			_computed.Capacity = 100;
			_partiallyComputed.Clear();
			_partiallyComputed.Capacity = 1000;
			Array.Resize(ref _headerTemplate, 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasComputedHash() => _computed.Count > 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CachedHash PopComputedHash() {
			var result = _computed[_computed.Count - 1];
			_computed.RemoveAt(_computed.Count - 1);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasNextPartiallyComputedHash() => _partiallyComputed.Count > 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CachedHash PeekNextPartiallyComputedHash() {
			if (_partiallyComputed.Count > 0)
				return _partiallyComputed[0];
			throw new Exception("Cache is empty");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CachedHash PopNextPartiallyComputedHash() {
			if (_partiallyComputed.Count <= 0)
				throw new Exception("Cache is empty");
			var result = _partiallyComputed[0];
			_partiallyComputed.RemoveAt(0);
			return result;
		}

		public int ComputeMemorySize() {
			var result = (_headerTemplate.Length);
			result += _computed.Count * (4 + 4 + 32);
			for (var j = 0; j <= _partiallyComputed.Count - 1; j++) {
				var cachedHash = _partiallyComputed[j];
				result += 4;
				result += 4;
				result += 32;
				for (var k = 0; k <= cachedHash.RoundOutputs.Length - 1; k++)
					result += cachedHash.RoundOutputs[k].Length;
			}

			return result;
		}

	}


	public struct CachedHash {
		public uint Nonce;
		public int Level;
		public byte[] Header;
		public byte[][] RoundOutputs;
	}

}
