using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Sphere10.Framework;


namespace Sphere10.Framework {
	
	/// <summary>
	/// Fast, thread-safe static hash methods.
	/// </summary>
	public static class MURMUR3_32 {

		/// http://en.wikipedia.org/wiki/MurmurHash
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="seed"></param>
		/// <returns></returns>
		public static int Execute(Span<byte> buffer, int seed = 37) {
			const uint c1 = 0xcc9e2d51;
			const uint c2 = 0x1b873593;
			const int r1 = 15;
			const int r2 = 13;
			const uint m = 5;
			const uint n = 0xe6546b64;

			unchecked {
				uint hash = (uint)seed;

				Span<uint> array = MemoryMarshal.Cast<byte, uint>(buffer);

				int length = buffer.Length;
				int remainder = length & 3;
				int len = length >> 2;

				int i = 0;

				while (i < len) {
					uint k = array[i];

					k *= c1;
					k = (k << r1) | (k >> (32 - r1)); //k = rotl32(k, r1);
					k *= c2;

					hash ^= k;
					hash = (hash << r2) | (hash >> (32 - r2)); //hash = rotl32(hash, r2);
					hash = hash * m + n;

					i++;
				}

				if (remainder > 0) {
					int shift = sizeof(uint) - remainder;
					uint k = (array[i] << shift) >> shift;

					k *= c1;
					k = (k << r1) | (k >> (32 - r1)); //k = rotl32(k, r1);
					k *= c2;

					hash ^= k;
				}

				hash ^= (uint)length;

				//hash = fmix(hash);
				hash ^= hash >> 16;
				hash *= 0x85ebca6b;
				hash ^= hash >> 13;
				hash *= 0xc2b2ae35;
				hash ^= hash >> 16;

				return (int)hash;
			}
		}

	}
}
