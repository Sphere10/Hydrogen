// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.CompilerServices;

namespace Hydrogen.CryptoEx;

/* This class is property of the BouncyCastle Cryptographic Library Project and was borrowed from 
<a href="https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/util/Pack.cs">https://github.com/bcgit/bc-csharp/blob/master/crypto/src/crypto/util/Pack.cs</a>
 for internal use.
*/
internal sealed class Pack {
	private Pack() {
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt16_To_BE(ushort n, Span<byte> bs) {
		bs[0] = (byte)(n >> 8);
		bs[1] = (byte)(n);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt16_To_BE(ushort n, Span<byte> bs, int off) {
		bs[off] = (byte)(n >> 8);
		bs[off + 1] = (byte)(n);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ushort BE_To_UInt16(ReadOnlySpan<byte> bs) {
		uint n = (uint)bs[0] << 8
		         | (uint)bs[1];
		return (ushort)n;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ushort BE_To_UInt16(ReadOnlySpan<byte> bs, int off) {
		uint n = (uint)bs[off] << 8
		         | (uint)bs[off + 1];
		return (ushort)n;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static byte[] UInt32_To_BE(uint n) {
		var bs = new byte[4];
		UInt32_To_BE(n, bs, 0);
		return bs;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt32_To_BE(uint n, Span<byte> bs) {
		bs[0] = (byte)(n >> 24);
		bs[1] = (byte)(n >> 16);
		bs[2] = (byte)(n >> 8);
		bs[3] = (byte)(n);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt32_To_BE(uint n, Span<byte> bs, int off) {
		bs[off] = (byte)(n >> 24);
		bs[off + 1] = (byte)(n >> 16);
		bs[off + 2] = (byte)(n >> 8);
		bs[off + 3] = (byte)(n);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static byte[] UInt32_To_BE(uint[] ns) {
		var bs = new byte[4 * ns.Length];
		UInt32_To_BE(ns, bs, 0);
		return bs;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt32_To_BE(uint[] ns, Span<byte> bs, int off) {
		for (int i = 0; i < ns.Length; ++i) {
			UInt32_To_BE(ns[i], bs, off);
			off += 4;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static uint BE_To_UInt32(ReadOnlySpan<byte> bs) {
		return (uint)bs[0] << 24
		       | (uint)bs[1] << 16
		       | (uint)bs[2] << 8
		       | (uint)bs[3];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static uint BE_To_UInt32(ReadOnlySpan<byte> bs, int off) {
		return (uint)bs[off] << 24
		       | (uint)bs[off + 1] << 16
		       | (uint)bs[off + 2] << 8
		       | (uint)bs[off + 3];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void BE_To_UInt32(ReadOnlySpan<byte> bs, int off, Span<uint> ns) {
		for (int i = 0; i < ns.Length; ++i) {
			ns[i] = BE_To_UInt32(bs, off);
			off += 4;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static byte[] UInt64_To_BE(ulong n) {
		byte[] bs = new byte[8];
		UInt64_To_BE(n, bs, 0);
		return bs;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt64_To_BE(ulong n, Span<byte> bs) {
		UInt32_To_BE((uint)(n >> 32), bs);
		UInt32_To_BE((uint)(n), bs, 4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt64_To_BE(ulong n, Span<byte> bs, int off) {
		UInt32_To_BE((uint)(n >> 32), bs, off);
		UInt32_To_BE((uint)(n), bs, off + 4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static byte[] UInt64_To_BE(ReadOnlySpan<ulong> ns) {
		byte[] bs = new byte[8 * ns.Length];
		UInt64_To_BE(ns, bs, 0);
		return bs;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt64_To_BE(ReadOnlySpan<ulong> ns, Span<byte> bs, int off) {
		for (int i = 0; i < ns.Length; ++i) {
			UInt64_To_BE(ns[i], bs, off);
			off += 8;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong BE_To_UInt64(ReadOnlySpan<byte> bs) {
		uint hi = BE_To_UInt32(bs);
		uint lo = BE_To_UInt32(bs, 4);
		return ((ulong)hi << 32) | (ulong)lo;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong BE_To_UInt64(ReadOnlySpan<byte> bs, int off) {
		uint hi = BE_To_UInt32(bs, off);
		uint lo = BE_To_UInt32(bs, off + 4);
		return ((ulong)hi << 32) | (ulong)lo;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void BE_To_UInt64(ReadOnlySpan<byte> bs, int off, Span<ulong> ns) {
		for (int i = 0; i < ns.Length; ++i) {
			ns[i] = BE_To_UInt64(bs, off);
			off += 8;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt16_To_LE(ushort n, Span<byte> bs) {
		bs[0] = (byte)(n);
		bs[1] = (byte)(n >> 8);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt16_To_LE(ushort n, Span<byte> bs, int off) {
		bs[off] = (byte)(n);
		bs[off + 1] = (byte)(n >> 8);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ushort LE_To_UInt16(ReadOnlySpan<byte> bs) {
		uint n = (uint)bs[0]
		         | (uint)bs[1] << 8;
		return (ushort)n;
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ushort LE_To_UInt16(ReadOnlySpan<byte> bs, int off) {
		uint n = (uint)bs[off]
		         | (uint)bs[off + 1] << 8;
		return (ushort)n;
	}

	internal static byte[] UInt32_To_LE(uint n) {
		byte[] bs = new byte[4];
		UInt32_To_LE(n, bs, 0);
		return bs;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt32_To_LE(uint n, Span<byte> bs) {
		bs[0] = (byte)(n);
		bs[1] = (byte)(n >> 8);
		bs[2] = (byte)(n >> 16);
		bs[3] = (byte)(n >> 24);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt32_To_LE(uint n, Span<byte> bs, int off) {
		bs[off] = (byte)(n);
		bs[off + 1] = (byte)(n >> 8);
		bs[off + 2] = (byte)(n >> 16);
		bs[off + 3] = (byte)(n >> 24);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static byte[] UInt32_To_LE(ReadOnlySpan<uint> ns) {
		byte[] bs = new byte[4 * ns.Length];
		UInt32_To_LE(ns, bs, 0);
		return bs;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt32_To_LE(ReadOnlySpan<uint> ns, Span<byte> bs, int off) {
		for (int i = 0; i < ns.Length; ++i) {
			UInt32_To_LE(ns[i], bs, off);
			off += 4;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static uint LE_To_UInt32(ReadOnlySpan<byte> bs) {
		return (uint)bs[0]
		       | (uint)bs[1] << 8
		       | (uint)bs[2] << 16
		       | (uint)bs[3] << 24;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static uint LE_To_UInt32(ReadOnlySpan<byte> bs, int off) {
		return (uint)bs[off]
		       | (uint)bs[off + 1] << 8
		       | (uint)bs[off + 2] << 16
		       | (uint)bs[off + 3] << 24;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void LE_To_UInt32(ReadOnlySpan<byte> bs, int off, uint[] ns) {
		for (int i = 0; i < ns.Length; ++i) {
			ns[i] = LE_To_UInt32(bs, off);
			off += 4;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void LE_To_UInt32(ReadOnlySpan<byte> bs, int bOff, Span<uint> ns, int nOff, int count) {
		for (int i = 0; i < count; ++i) {
			ns[nOff + i] = LE_To_UInt32(bs, bOff);
			bOff += 4;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static uint[] LE_To_UInt32(ReadOnlySpan<byte> bs, int off, int count) {
		uint[] ns = new uint[count];
		for (int i = 0; i < ns.Length; ++i) {
			ns[i] = LE_To_UInt32(bs, off);
			off += 4;
		}

		return ns;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static byte[] UInt64_To_LE(ulong n) {
		byte[] bs = new byte[8];
		UInt64_To_LE(n, bs, 0);
		return bs;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt64_To_LE(ulong n, Span<byte> bs) {
		UInt32_To_LE((uint)(n), bs);
		UInt32_To_LE((uint)(n >> 32), bs, 4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt64_To_LE(ulong n, Span<byte> bs, int off) {
		UInt32_To_LE((uint)(n), bs, off);
		UInt32_To_LE((uint)(n >> 32), bs, off + 4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static byte[] UInt64_To_LE(ulong[] ns) {
		byte[] bs = new byte[8 * ns.Length];
		UInt64_To_LE(ns, bs, 0);
		return bs;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt64_To_LE(ReadOnlySpan<ulong> ns, Span<byte> bs, int off) {
		for (int i = 0; i < ns.Length; ++i) {
			UInt64_To_LE(ns[i], bs, off);
			off += 8;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void UInt64_To_LE(ReadOnlySpan<ulong> ns, int nsOff, int nsLen, Span<byte> bs, int bsOff) {
		for (int i = 0; i < nsLen; ++i) {
			UInt64_To_LE(ns[nsOff + i], bs, bsOff);
			bsOff += 8;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong LE_To_UInt64(ReadOnlySpan<byte> bs) {
		uint lo = LE_To_UInt32(bs);
		uint hi = LE_To_UInt32(bs, 4);
		return ((ulong)hi << 32) | (ulong)lo;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong LE_To_UInt64(ReadOnlySpan<byte> bs, int off) {
		uint lo = LE_To_UInt32(bs, off);
		uint hi = LE_To_UInt32(bs, off + 4);
		return ((ulong)hi << 32) | (ulong)lo;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void LE_To_UInt64(ReadOnlySpan<byte> bs, int off, Span<ulong> ns) {
		for (int i = 0; i < ns.Length; ++i) {
			ns[i] = LE_To_UInt64(bs, off);
			off += 8;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void LE_To_UInt64(ReadOnlySpan<byte> bs, int bsOff, Span<ulong> ns, int nsOff, int nsLen) {
		for (int i = 0; i < nsLen; ++i) {
			ns[nsOff + i] = LE_To_UInt64(bs, bsOff);
			bsOff += 8;
		}
	}
}
