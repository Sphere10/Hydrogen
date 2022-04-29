using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;


namespace Hydrogen {

	/// <summary>
	/// Fast, thread-safe static hash methods.
	/// </summary>
	public static class Hashers {
		private static readonly Func<IHashFunction>[] Constructors;
		private static readonly ConcurrentStack<IHashFunction>[] HasherStack;
		private static readonly int[] DigestByteSizes;

		static Hashers() {
			Constructors = new Func<IHashFunction>[byte.MaxValue];
			HasherStack = new ConcurrentStack<IHashFunction>[byte.MaxValue];
			DigestByteSizes = new int[byte.MaxValue];
			RegisterDefaultAlgorithms();
		}

		// NOTE: attack vector if malicious plug-in library plugs in a man-in-the-middle sniffer to learn private details
		// MUST ACTION!
		public static void Register(CHF algorithm, Func<IHashFunction> constructor) {
			var ix = (int)algorithm;
			Constructors[ix] = constructor;
			if (HasherStack[ix] != null)
				HasherStack[ix].ForEach(x => x.Dispose());
			HasherStack[ix] = new ConcurrentStack<IHashFunction>();
			DigestByteSizes[ix] = constructor().DigestSize;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetDigestSizeBytes(CHF algorithm)
			=> DigestByteSizes[(int)algorithm];


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] Hash<TItem>(CHF algorithm, TItem item, IItemSerializer<TItem> serializer, Endianness endianness = Endianness.LittleEndian) 
			=> Hash(algorithm, serializer.Serialize(item, endianness));


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] Hash(CHF algorithm, ReadOnlySpan<byte> bytes) {
			var digestSizeBytes = DigestByteSizes[(int)algorithm];
			var result = new byte[digestSizeBytes > 0 ? digestSizeBytes : bytes.Length];
			Hash(algorithm, bytes, result);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Hash(CHF algorithm, ReadOnlySpan<byte> bytes, Span<byte> result) {
			var hashers = HasherStack[(int)algorithm];
			if (!hashers.TryPop(out var hasher)) {
				hasher = Constructors[(int)algorithm].Invoke();
			}
			try {
				hasher.Compute(bytes, result);
			} finally {
				hashers.Push(hasher);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] JoinHash(CHF algorithm, ReadOnlySpan<byte> left, ReadOnlySpan<byte> right) {
			var digestSize = DigestByteSizes[(int)algorithm];
			var result = new byte[digestSize > 0 ? digestSize : left.Length + right.Length];
			JoinHash(algorithm, left, right, result);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void JoinHash(CHF algorithm, ReadOnlySpan<byte> left, ReadOnlySpan<byte> right, Span<byte> result) {
			var hashers = HasherStack[(int)algorithm];
			if (!hashers.TryPop(out var hasher)) {
				hasher = Constructors[(int)algorithm].Invoke();
			}
			try {
				hasher.Transform(left);
				hasher.Transform(right);
				hasher.GetResult(result);
			} finally {
				hashers.Push(hasher);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] Iterate(CHF algorithm, ReadOnlySpan<byte> bytes, int n) {
			var result = new byte[DigestByteSizes[(int)algorithm]];
			Iterate(algorithm, bytes, n, result);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Iterate(CHF algorithm, ReadOnlySpan<byte> bytes, int n, Span<byte> result) {
			var expectedSize = DigestByteSizes[(int)algorithm];
			Guard.Argument(expectedSize > 0, nameof(algorithm), "Dynamically sized digest hash functions cannot be iterated");
			Guard.Argument(result.Length >= expectedSize, nameof(result), $"Expected {expectedSize} bytes");
			if (n == 0) {
				if (bytes.Length != expectedSize)
					throw new ArgumentOutOfRangeException(nameof(bytes), $"Must be {expectedSize} bytes when n = 0");
				bytes.CopyTo(result);
				return;
			}
			var hashers = HasherStack[(int)algorithm];
			if (!hashers.TryPop(out var hasher)) {
				hasher = Constructors[(int)algorithm].Invoke();
			}

			var tempArr = new byte[expectedSize];
			try {
				while (n-- > 0) {
					hasher.Compute(bytes, result);
					result.CopyTo(tempArr);
					bytes = tempArr;
				}
			} finally {
				hashers.Push(hasher);
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Aggregate(CHF algorithm, IEnumerable<byte[]> digests, Span<byte> result, bool flipJoinHash = false) {
			Guard.Argument(algorithm != CHF.ConcatBytes, nameof(algorithm), "Algorithm not supported");
			Guard.ArgumentNotNullOrEmpty(digests, nameof(digests));
			var expectedSize = DigestByteSizes[(int)algorithm];
			Guard.Argument(result.Length >= expectedSize, nameof(result), $"Expected {expectedSize} bytes");
			
			var hashers = HasherStack[(int)algorithm];
			if (!hashers.TryPop(out var hasher)) {
				hasher = Constructors[(int)algorithm].Invoke();
			}
			try {
				var aggregatedValue = new byte[expectedSize];
				Tools.Array.Fill<byte>(aggregatedValue, 0);
				//var tail = digests.Head(out var head);
				//head.CopyTo(result);
				foreach (var (item, index) in digests.WithIndex()) {
					if (index == 0) {
						item.CopyTo(result);
						continue;
					}
					if (flipJoinHash) {
						hasher.Transform(item);
						hasher.Transform(result);
					} else {
						hasher.Transform(result);
						hasher.Transform(item);
					}
					hasher.GetResult(result);
				}
			} finally {
				hashers.Push(hasher);
			}
		}

		public static byte[] Aggregate(CHF algorithm, IEnumerable<byte[]> digests, bool flipJoinHash = false) {
			var result = new byte[DigestByteSizes[(int)algorithm]];
			Aggregate(algorithm, digests, result, flipJoinHash);
			return result;
		}

		public static IDisposable BorrowHasher(CHF algorithm, out IHashFunction hasher) {
			var hashers = HasherStack[(int)algorithm];
			if (!hashers.TryPop(out hasher)) {
				hasher = Constructors[(int)algorithm].Invoke();
			}
			var hasherObj = hasher;
			return new ActionScope(() => hashers.Push(hasherObj));
		}		

		public static void RegisterDefaultAlgorithms() {
			Register(CHF.ConcatBytes, () => new ConcatBytes());
			Register(CHF.SHA2_256, () => new HashAlgorithmAdapter(new SHA256Managed()));
			Register(CHF.SHA2_384, () => new HashAlgorithmAdapter(new SHA384Managed()));
			Register(CHF.SHA2_512, () => new HashAlgorithmAdapter(new SHA512Managed()));
			Register(CHF.SHA1_160, () => new HashAlgorithmAdapter(new SHA1Managed()));
			Register(CHF.Blake2b_256, () => new Blake2b(Blake2b._256Config));
			Register(CHF.Blake2b_160, () => new Blake2b(Blake2b._160Config));
			Register(CHF.Blake2b_128, () => new Blake2b(Blake2b._128Config));
			
		}
	}
}

