using Sphere10.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sphere10.Hydrogen.Core.Maths {
	public class Mersenne32 {
		// Define MT19937 constants (32-bit RNG)
		private const int N = 624;
		private const int M = 397;
		private const int R = 31;
		private const int F = 1812433253;
		private const int U = 11;
		private const int S = 7;
		private const int T = 15;
		private const int L = 18;
		private const uint A = 0x9908B0DF;
		private const uint B = 0x9D2C5680;
		private const uint C = 0xEFC60000;
		private const uint MaskLower = (uint)(((ulong)1 << R) - 1);
		private const uint MaskUpper = (uint)((ulong)1 << R);

		private ushort _index;
		private readonly uint[] _mt = new uint[N];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Twist() {
			int idx;
			for (idx = 0; idx <= N - M - 1; idx++) {
				_mt[idx] = _mt[idx + M] ^ (((_mt[idx] & MaskUpper) | (_mt[idx + 1] & MaskLower)) >> 1) ^
						   ((uint)-(_mt[idx + 1] & 1) & A);
			}

			for (idx = N - M; idx <= N - 2; idx++) {
				_mt[idx] = _mt[idx + (M - N)] ^ (((_mt[idx] & MaskUpper) | (_mt[idx + 1] & MaskLower)) >> 1) ^
						   ((uint)-(_mt[idx + 1] & 1) & A);
				_mt[N - 1] = _mt[M - 1] ^ (((_mt[N - 1] & MaskUpper) | (_mt[0] & MaskLower)) >> 1) ^
							 ((uint)-(_mt[0] & 1) & A);
			}

			_index = 0;
		}

		private Mersenne32(uint seed) => Initialize(seed);

		public void Initialize(uint seed) {
			_mt[0] = seed;
			for (var idx = 1; idx < N; idx++) {
				_mt[idx] = (uint)(F * (_mt[idx - 1] ^ (_mt[idx - 1] >> 30)) + idx);
				_index = N;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int NextInt32() => (int)NextUInt32();

		public uint NextUInt32() {
			int index = _index;
			if (_index >= N) {
				Twist();
				index = _index;
			}

			var result = _mt[index];
			_index = (ushort)(index + 1);
			result ^= _mt[index] >> U;
			result ^= (result << S) & B;
			result ^= (result << T) & C;
			result ^= result >> L;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float NextSingle() => (float)(NextUInt32() * 4.6566128730773926E-010);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float NextUSingle() => (float)(NextUInt32() * 2.32830643653869628906E-10);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Mersenne32 CreateInstance(uint seed) => new Mersenne32(seed);
	}

	internal static class XorShift32 {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Next(ref uint state) {
			state ^= state << 13;
			state ^= state >> 17;
			state ^= state << 5;
			return state;
		}
	}

	public static class RandomHashUtils {
		public struct Statistics {
			private const double EPSILON = 0.00001;
			public uint SampleCount { get; private set; }
			public double Sum { get; private set; }
			public double SquaredSum { get; private set; }
			public double ReciprocalSum { get; private set; }
			public double Minimum { get; private set; }
			public double Maximum { get; private set; }

			public void Reset() {
				SampleCount = 0;
				Sum = 0.0;
				SquaredSum = 0.0;
				ReciprocalSum = 0.0;
				Minimum = 0.0;
				Maximum = 0.0;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public double Mean() => SampleCount > 0 ? Sum / SampleCount : double.NaN;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public double PopulationStandardDeviation() => Math.Sqrt(PopulationVariance());

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public double PopulationVariance() {
				var sum = Sum;
				return SampleCount > 2
					? (SampleCount * SquaredSum - sum * sum) / (SampleCount * SampleCount)
					: double.NaN;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public double HarmonicMean() =>
				SampleCount > 0 ? SampleCount / ReciprocalSum : double.NaN;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public double MinimumError() =>
				Mean() * Mean() > EPSILON * EPSILON ? 100.0 * (Minimum - Mean()) / Mean() : double.NaN;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public double MaximumError() =>
				Mean() * Mean() > EPSILON * EPSILON ? 100.0 * (Maximum - Mean()) / Mean() : double.NaN;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public double PopulationVariationCoefficient() =>
				SampleCount > 0 ? PopulationVariance() / Mean() * 100.0 : double.NaN;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public double SampleStandardDeviation() =>
				SampleCount >= 2 ? Math.Sqrt(SampleVariance()) : double.NaN;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private double SampleVariance() {
				var sum = Sum;
				return SampleCount > 2
					? (SampleCount * SquaredSum - sum * sum) / ((SampleCount - 1) * (SampleCount - 1))
					: double.NaN;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public double SampleVariationCoefficient() =>
				SampleCount >= 2 ? 100 * (SampleStandardDeviation() / Mean()) : double.NaN;

			public void AddDatum(double datum) {
				if (SampleCount == 0)
					Reset();
				SampleCount++;
				Sum += datum;
				SquaredSum += datum * datum;
				if (double.IsNaN(ReciprocalSum) || datum * datum < EPSILON * EPSILON)
					ReciprocalSum = double.NaN;
				else
					ReciprocalSum += 1.0 / datum;
				if (SampleCount == 1) {
					// first data so set _min/_max
					Minimum = datum;
					Maximum = datum;
				} else {
					// adjust _min/_max boundaries if necessary
					if (datum < Minimum)
						Minimum = datum;
					if (datum > Maximum)
						Maximum = datum;
				}
			}

			public void AddDatum(double datum, uint numTimes) {
				if (SampleCount == 0)
					Reset();
				SampleCount += numTimes;
				Sum += datum * numTimes;
				SquaredSum += datum * datum * numTimes;
				if (double.IsNaN(ReciprocalSum) || datum * datum < EPSILON * EPSILON)
					ReciprocalSum = double.NaN;
				else
					ReciprocalSum += 1.0 / datum * numTimes;
				if (SampleCount == 1) {
					// first data so set _min/_max
					Minimum = datum;
					Maximum = datum;
				} else {
					// adjust _min/_max boundaries if necessary
					if (datum < Minimum)
						Minimum = datum;
					if (datum > Maximum)
						Maximum = datum;
				}
			}

			public void RemoveDatum(double datum) {
				if (SampleCount == 0)
					return;
				SampleCount--;
				Sum -= datum;
				SquaredSum -= datum * datum;
				ReciprocalSum -= 1.0 / datum;
			}

			public static Statistics DefaultInstance() => new Statistics();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint GetLastDWordLE(byte[] chunk) {
			var chunkLength = chunk.Length;

			if (chunkLength < 4)
				throw new ArgumentException($"{nameof(chunk)} needs to be at least 4 bytes");

			// Last 4 bytes are nonce (LE)
			return (uint)(chunk[chunkLength - 4] |
						   (chunk[chunkLength - 3] << 8) |
						   (chunk[chunkLength - 2] << 16) |
						   (chunk[chunkLength - 1] << 24));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint GetDWordLE(byte[] chunk, int offset) {
			var chunkLength = chunk.Length;
			if (chunkLength < offset + 3)
				throw new ArgumentException($"{nameof(chunk)}[{nameof(offset)}] needs at least 4 more bytes");

			// Last 4 bytes are nonce (LE)
			return (uint)(chunk[offset + 0] |
						   (chunk[offset + 1] << 8) |
						   (chunk[offset + 2] << 16) |
						   (chunk[offset + 3] << 24));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] SetLastDWordLE(byte[] chunk, uint value) {
			// Clone the original header
			var result = Clone(chunk);

			// If digest not big enough to contain a nonce, just return the clone
			var chunkLength = chunk.Length;
			if (chunkLength < 4)
				return result;

			// Overwrite the nonce in little-endian
			result[chunkLength - 4] = (byte)value;
			result[chunkLength - 3] = (byte)((value >> 8) & 255);
			result[chunkLength - 2] = (byte)((value >> 16) & 255);
			result[chunkLength - 1] = (byte)((value >> 24) & 255);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte RotateRight8(byte value, int distance) {
			Debug.Assert(distance >= 0);
			distance &= 7;
			return (byte)((value >> distance) | (value << (8 - distance)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte RotateLeft8(byte value, int distance) {
			Debug.Assert(distance >= 0);
			distance &= 7;
			return (byte)((value << distance) | (value >> (8 - distance)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] Clone<T>(T[] buffer) => (T[])buffer?.Clone();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] Concatenate<T>(T[] left, T[] right) {
			if (left == null)
				return Clone(right);
			if (right == null)
				return Clone(left);

			var result = new T[left.Length + right.Length];
			Array.Copy(left, 0, result, 0, left.Length);
			Array.Copy(right, 0, result, left.Length, right.Length);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool BytesEqual(byte[] left, byte[] right) => BytesEqual(left, right, 0, (uint)left.Length);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool BytesEqual(byte[] left, byte[] right, uint from, uint length) {
			if (length == 0)
				return false;
			var leftLength = left.Length;
			var rightLength = right.Length;
			if (leftLength - from < length || rightLength - from < length)
				return false;
			for (var idx = (int)from; idx <= length; idx++) {
				if (left[idx] != right[idx])
					return false;
			}

			return true;
		}
	}

	public class RandomHash2Exception : Exception {
		public RandomHash2Exception(string message) : base(message) {
		}
	}

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
			var generator = Mersenne32.CreateInstance(seed);
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
			var generator = Mersenne32.CreateInstance(0);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected static byte[] Compress(byte[][] inputs, uint seed) {
			var result = new byte[100];
			var generator = Mersenne32.CreateInstance(seed);
			for (var idx = 0; idx <= 99; idx++) {
				var source = inputs[generator.NextUInt32() % inputs.Length];
				result[idx] = source[generator.NextUInt32() % source.Length];
			}

			return result;
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
	}

	public sealed class RandomHash2Fast : RandomHash2 {
		private const string OverlappingArgs = "Overlapping read/write regions";
		private const string BufferTooSmall = "Buffer too small to apply memory transform";

		public class Cache {
			private byte[] _headerTemplate;
			private List<CachedHash> _computed, _partiallyComputed;

			public bool EnablePartiallyComputed { get; set; }

			private Cache() {
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

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void PreProcessNewHash(byte[] header) {
				// if header is a new template, flush cache
				if (RandomHashUtils.BytesEqual(_headerTemplate, header, 0, 32 - 4)) return;
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
				if (_partiallyComputed.Count <= 0) throw new Exception("Cache is empty");
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

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static Cache CacheInstance() => new Cache();
		}

		public struct CachedHash {
			public uint Nonce;
			public int Level;
			public byte[] Header;
			public byte[][] RoundOutputs;
		}

		public Cache CacheInstance { get; private set; }
		public bool EnableCaching { get; set; }

		public bool CaptureMemStats { get; set; }

		public RandomHashUtils.Statistics MemStats { get; }

		private RandomHash2Fast() {
			EnableCaching = false;
			CacheInstance = Cache.CacheInstance();
			MemStats = RandomHashUtils.Statistics.DefaultInstance();
			MemStats.Reset();
		}

		~RandomHash2Fast() {
			CacheInstance = null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void MemTransform1(ref byte[] chunk, int readStart, int writeStart, int length) {
			var readEnd = readStart + length - 1;
			var writeEnd = writeStart + length - 1;
			if (readEnd >= writeStart)
				throw new ArgumentOutOfRangeException(OverlappingArgs);
			// Seed XorShift32 with last byte
			var state = RandomHashUtils.GetDWordLE(chunk, readEnd - 3);
			if (state == 0)
				state = 1;

			// Select random bytes from input using XorShift32 RNG
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
			var generator = Mersenne32.CreateInstance(seed);
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

		private bool CalculateRoundOutputs(byte[] blockHeader, int round, ref CachedHash? cachedHash,
			out byte[][] roundOutputs) {
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
			var generator = Mersenne32.CreateInstance(0);
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
					var neighbourWasLastRound = CalculateRoundOutputs(neighbourNonceHeader, round - 1,
						ref internalCachedHash,
						out var neighborOutputs);
					roundOutputsList.AddRange(neighborOutputs);

					// If neighbour was a fully evaluated nonce, cache it for re-use
					if (!EnableCaching) continue;
					if (neighbourWasLastRound)
						CacheInstance.AddFullyComputed(neighbourNonceHeader, round - 1,
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
		public new static byte[] Compute(byte[] blockHeader) => RandomHash2FastInstance().Hash(blockHeader);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RandomHash2Fast RandomHash2FastInstance() => new RandomHash2Fast();
	}
}
