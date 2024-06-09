// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

// Testing shows fastest when not unrolling or inlining :-/
//#define UNROLL_LOOPS
//#define AGGRESSIVE_INLINE

using System;
using System.Diagnostics;

namespace Hydrogen;

public class Blake2b : HashFunctionBase {
	public static readonly Config DefaultConfig = new Config();
	public static readonly Config _512Config = new Config { OutputSizeInBytes = 512 / 8 };
	public static readonly Config _384Config = new Config { OutputSizeInBytes = 384 / 8 };
	public static readonly Config _256Config = new Config { OutputSizeInBytes = 256 / 8 };
	public static readonly Config _224Config = new Config { OutputSizeInBytes = 224 / 8 };
	public static readonly Config _160Config = new Config { OutputSizeInBytes = 160 / 8 };
	public static readonly Config _128Config = new Config { OutputSizeInBytes = 128 / 8 };

	private const int NumberOfRounds = 12;
	private const int BlockSizeInBytes = 128;

	private const ulong IV0 = 0x6A09E667F3BCC908UL;
	private const ulong IV1 = 0xBB67AE8584CAA73BUL;
	private const ulong IV2 = 0x3C6EF372FE94F82BUL;
	private const ulong IV3 = 0xA54FF53A5F1D36F1UL;
	private const ulong IV4 = 0x510E527FADE682D1UL;
	private const ulong IV5 = 0x9B05688C2B3E6C1FUL;
	private const ulong IV6 = 0x1F83D9ABFB41BD6BUL;
	private const ulong IV7 = 0x5BE0CD19137E2179UL;

	private static readonly int[] Sigma = new int[NumberOfRounds * 16] {
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
		14, 10, 4, 8, 9, 15, 13, 6, 1, 12, 0, 2, 11, 7, 5, 3,
		11, 8, 12, 0, 5, 2, 15, 13, 10, 14, 3, 6, 7, 1, 9, 4,
		7, 9, 3, 1, 13, 12, 11, 14, 2, 6, 5, 10, 4, 0, 15, 8,
		9, 0, 5, 7, 2, 4, 10, 15, 14, 1, 11, 12, 6, 8, 3, 13,
		2, 12, 6, 10, 0, 11, 8, 3, 4, 13, 7, 5, 15, 14, 1, 9,
		12, 5, 1, 15, 14, 13, 4, 10, 0, 7, 6, 3, 9, 2, 8, 11,
		13, 11, 7, 14, 12, 1, 3, 9, 5, 0, 15, 4, 8, 6, 2, 10,
		6, 15, 14, 9, 11, 3, 0, 8, 12, 2, 13, 7, 1, 4, 10, 5,
		10, 2, 8, 4, 7, 6, 1, 5, 15, 11, 9, 14, 3, 12, 13, 0,
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
		14, 10, 4, 8, 9, 15, 13, 6, 1, 12, 0, 2, 11, 7, 5, 3
	};

	private readonly ulong[] _rawConfig;
	private readonly byte[] _key;
	private readonly byte[] _buf = new byte[128];
	private readonly ulong[] _m = new ulong[16];
	private readonly ulong[] _h = new ulong[8];
	private readonly ulong[] _v = new ulong[16];
	private readonly byte[] _fullDigest = new byte[64];
	private static readonly EndianBitConverter _littleEndian = EndianBitConverter.Little;

	private int _bufferFilled;
	private ulong _counter0;
	private ulong _counter1;
	private ulong _finalizationFlag0;
	private ulong _finalizationFlag1;

	public Blake2b()
		: this(DefaultConfig) {
	}

	public Blake2b(Config config) {
		Guard.ArgumentNotNull(config, nameof(config));
		_rawConfig = IVBuilder.ConfigB(config, null);
		Debug.Assert(_rawConfig.Length == 8);
		if (config.Key != null && config.Key.Length != 0) {
			_key = new byte[128];
			Array.Copy(config.Key, _key, config.Key.Length);
		}
		DigestSize = config.OutputSizeInBytes;
		Initialize();
	}

	public override int DigestSize { get; }

	public override void Initialize() {
		base.Initialize();
		_h[0] = IV0;
		_h[1] = IV1;
		_h[2] = IV2;
		_h[3] = IV3;
		_h[4] = IV4;
		_h[5] = IV5;
		_h[6] = IV6;
		_h[7] = IV7;

		_counter0 = 0;
		_counter1 = 0;
		_finalizationFlag0 = 0;
		_finalizationFlag1 = 0;

		_bufferFilled = 0;

		Array.Clear(_fullDigest, 0, _fullDigest.Length);
		Array.Clear(_buf, 0, _buf.Length);

#if !UNROLL_LOOPS
		for (int i = 0; i < 8; i++)
			_h[i] ^= _rawConfig[i];
#else
			_h[0] ^= _rawConfig[0];
			_h[1] ^= _rawConfig[1];
			_h[2] ^= _rawConfig[2];
			_h[3] ^= _rawConfig[3];
			_h[4] ^= _rawConfig[4];
			_h[5] ^= _rawConfig[5];
			_h[6] ^= _rawConfig[6];
			_h[7] ^= _rawConfig[7];
#endif
		// Transform key (if provided)
		if (_key != null) {
			Transform(_key);
		}
	}

	public override void Transform(ReadOnlySpan<byte> array) {
		base.Transform(array);
		var start = 0;
		var count = array.Length;
		int offset = start;
		int bufferRemaining = BlockSizeInBytes - _bufferFilled;

		if (_bufferFilled > 0 && count > bufferRemaining) {
			array.Slice(offset, bufferRemaining).CopyTo(_buf.AsSpan(_bufferFilled));
			//Array.Copy(array, offset, _buf, _bufferFilled, bufferRemaining);
			_counter0 += BlockSizeInBytes;
			if (_counter0 == 0)
				_counter1++;
			Compress(_buf, 0);
			offset += bufferRemaining;
			count -= bufferRemaining;
			_bufferFilled = 0;
		}

		while (count > BlockSizeInBytes) {
			_counter0 += BlockSizeInBytes;
			if (_counter0 == 0)
				_counter1++;
			Compress(array, offset);
			offset += BlockSizeInBytes;
			count -= BlockSizeInBytes;
		}

		if (count > 0) {
			array.Slice(offset, count).CopyTo(_buf.AsSpan(_bufferFilled));
			//Array.Copy(array, offset, _buf, _bufferFilled, count);
			_bufferFilled += count;
		}
	}

	public override object Clone() {
		throw new NotImplementedException();
	}

	protected override void Finalize(Span<byte> digest) {

		var isEndOfLayer = false;
		_counter0 += (uint)_bufferFilled;
		_finalizationFlag0 = ulong.MaxValue;
		if (isEndOfLayer)
			_finalizationFlag1 = ulong.MaxValue;
		for (int i = _bufferFilled; i < _buf.Length; i++)
			_buf[i] = 0;
		Compress(_buf, 0);

		//Output
		for (int i = 0; i < 8; ++i)
			_littleEndian.WriteTo(_h[i], _fullDigest, i << 3);

		_fullDigest.AsSpan(0, DigestSize).CopyTo(digest);
	}

	private void Compress(ReadOnlySpan<byte> block, int offset) {
		var v = _v;
		var h = _h;
		var m = _m;

#if !UNROLL_LOOPS
		for (var i = 0; i < 16; ++i)
			m[i] = _littleEndian.ToUInt64(block, offset + (i << 3));
#else
			m[0] = _littleEndian.ToUInt64(block, offset + (0 << 3));
			m[1] = _littleEndian.ToUInt64(block, offset + (1 << 3));
			m[2] = _littleEndian.ToUInt64(block, offset + (2 << 3));
			m[3] = _littleEndian.ToUInt64(block, offset + (3 << 3));
			m[4] = _littleEndian.ToUInt64(block, offset + (4 << 3));
			m[5] = _littleEndian.ToUInt64(block, offset + (5 << 3));
			m[6] = _littleEndian.ToUInt64(block, offset + (6 << 3));
			m[7] = _littleEndian.ToUInt64(block, offset + (7 << 3));
			m[8] = _littleEndian.ToUInt64(block, offset + (8 << 3));
			m[9] = _littleEndian.ToUInt64(block, offset + (9 << 3));
			m[10] = _littleEndian.ToUInt64(block, offset + (10 << 3));
			m[11] = _littleEndian.ToUInt64(block, offset + (11 << 3));
			m[12] = _littleEndian.ToUInt64(block, offset + (12 << 3));
			m[13] = _littleEndian.ToUInt64(block, offset + (13 << 3));
			m[14] = _littleEndian.ToUInt64(block, offset + (14 << 3));
			m[15] = _littleEndian.ToUInt64(block, offset + (15 << 3));
#endif

#if !UNROLL_LOOPS
		for (var i = 0; i < 8; i++)
			v[i] = h[i];
#else
			v[0] = h[0];
			v[1] = h[1];
			v[2] = h[2];
			v[3] = h[3];
			v[4] = h[4];
			v[5] = h[5];
			v[6] = h[6];
			v[7] = h[7];
#endif
		v[8] = IV0;
		v[9] = IV1;
		v[10] = IV2;
		v[11] = IV3;
		v[12] = IV4 ^ _counter0;
		v[13] = IV5 ^ _counter1;
		v[14] = IV6 ^ _finalizationFlag0;
		v[15] = IV7 ^ _finalizationFlag1;

#if !UNROLL_LOOPS
		for (int r = 0; r < NumberOfRounds; ++r) {
			G(0, 4, 8, 12, r, 0);
			G(1, 5, 9, 13, r, 2);
			G(2, 6, 10, 14, r, 4);
			G(3, 7, 11, 15, r, 6);
			G(3, 4, 9, 14, r, 14);
			G(2, 7, 8, 13, r, 12);
			G(0, 5, 10, 15, r, 8);
			G(1, 6, 11, 12, r, 10);
		}
#else
			G(0, 4, 8, 12, 0, 0);
			G(1, 5, 9, 13, 0, 2);
			G(2, 6, 10, 14, 0, 4);
			G(3, 7, 11, 15, 0, 6);
			G(3, 4, 9, 14, 0, 14);
			G(2, 7, 8, 13, 0, 12);
			G(0, 5, 10, 15, 0, 8);
			G(1, 6, 11, 12, 0, 10);

			G(0, 4, 8, 12, 1, 0);
			G(1, 5, 9, 13, 1, 2);
			G(2, 6, 10, 14, 1, 4);
			G(3, 7, 11, 15, 1, 6);
			G(3, 4, 9, 14, 1, 14);
			G(2, 7, 8, 13, 1, 12);
			G(0, 5, 10, 15, 1, 8);
			G(1, 6, 11, 12, 1, 10);

			G(0, 4, 8, 12, 2, 0);
			G(1, 5, 9, 13, 2, 2);
			G(2, 6, 10, 14, 2, 4);
			G(3, 7, 11, 15, 2, 6);
			G(3, 4, 9, 14, 2, 14);
			G(2, 7, 8, 13, 2, 12);
			G(0, 5, 10, 15, 2, 8);
			G(1, 6, 11, 12, 2, 10);

			G(0, 4, 8, 12, 3, 0);
			G(1, 5, 9, 13, 3, 2);
			G(2, 6, 10, 14, 3, 4);
			G(3, 7, 11, 15, 3, 6);
			G(3, 4, 9, 14, 3, 14);
			G(2, 7, 8, 13, 3, 12);
			G(0, 5, 10, 15, 3, 8);
			G(1, 6, 11, 12, 3, 10);

			G(0, 4, 8, 12, 4, 0);
			G(1, 5, 9, 13, 4, 2);
			G(2, 6, 10, 14, 4, 4);
			G(3, 7, 11, 15, 4, 6);
			G(3, 4, 9, 14, 4, 14);
			G(2, 7, 8, 13, 4, 12);
			G(0, 5, 10, 15, 4, 8);
			G(1, 6, 11, 12, 4, 10);

			G(0, 4, 8, 12, 5, 0);
			G(1, 5, 9, 13, 5, 2);
			G(2, 6, 10, 14, 5, 4);
			G(3, 7, 11, 15, 5, 6);
			G(3, 4, 9, 14, 5, 14);
			G(2, 7, 8, 13, 5, 12);
			G(0, 5, 10, 15, 5, 8);
			G(1, 6, 11, 12, 5, 10);

			G(0, 4, 8, 12, 6, 0);
			G(1, 5, 9, 13, 6, 2);
			G(2, 6, 10, 14, 6, 4);
			G(3, 7, 11, 15, 6, 6);
			G(3, 4, 9, 14, 6, 14);
			G(2, 7, 8, 13, 6, 12);
			G(0, 5, 10, 15, 6, 8);
			G(1, 6, 11, 12, 6, 10);

			G(0, 4, 8, 12, 7, 0);
			G(1, 5, 9, 13, 7, 2);
			G(2, 6, 10, 14, 7, 4);
			G(3, 7, 11, 15, 7, 6);
			G(3, 4, 9, 14, 7, 14);
			G(2, 7, 8, 13, 7, 12);
			G(0, 5, 10, 15, 7, 8);
			G(1, 6, 11, 12, 7, 10);

			G(0, 4, 8, 12, 8, 0);
			G(1, 5, 9, 13, 8, 2);
			G(2, 6, 10, 14, 8, 4);
			G(3, 7, 11, 15, 8, 6);
			G(3, 4, 9, 14, 8, 14);
			G(2, 7, 8, 13, 8, 12);
			G(0, 5, 10, 15, 8, 8);
			G(1, 6, 11, 12, 8, 10);

			G(0, 4, 8, 12, 9, 0);
			G(1, 5, 9, 13, 9, 2);
			G(2, 6, 10, 14, 9, 4);
			G(3, 7, 11, 15, 9, 6);
			G(3, 4, 9, 14, 9, 14);
			G(2, 7, 8, 13, 9, 12);
			G(0, 5, 10, 15, 9, 8);
			G(1, 6, 11, 12, 9, 10);

			G(0, 4, 8, 12, 10, 0);
			G(1, 5, 9, 13, 10, 2);
			G(2, 6, 10, 14, 10, 4);
			G(3, 7, 11, 15, 10, 6);
			G(3, 4, 9, 14, 10, 14);
			G(2, 7, 8, 13, 10, 12);
			G(0, 5, 10, 15, 10, 8);
			G(1, 6, 11, 12, 10, 10);

			G(0, 4, 8, 12, 11, 0);
			G(1, 5, 9, 13, 11, 2);
			G(2, 6, 10, 14, 11, 4);
			G(3, 7, 11, 15, 11, 6);
			G(3, 4, 9, 14, 11, 14);
			G(2, 7, 8, 13, 11, 12);
			G(0, 5, 10, 15, 11, 8);
			G(1, 6, 11, 12, 11, 10);
#endif

#if !UNROLL_LOOPS
		for (int i = 0; i < 8; ++i)
			h[i] ^= v[i] ^ v[i + 8];
#else
			h[0] ^= v[0] ^ v[0 + 8];
			h[1] ^= v[1] ^ v[1 + 8];
			h[2] ^= v[2] ^ v[2 + 8];
			h[3] ^= v[3] ^ v[3 + 8];
			h[4] ^= v[4] ^ v[4 + 8];
			h[5] ^= v[5] ^ v[5 + 8];
			h[6] ^= v[6] ^ v[6 + 8];
			h[7] ^= v[7] ^ v[7 + 8];
#endif
	}

#if AGGRESSIVE_INLINE
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	private void G(int a, int b, int c, int d, int r, int i) {
		int p = (r << 4) + i;
		int p0 = Sigma[p];
		int p1 = Sigma[p + 1];
		var v = _v;
		var m = _m;

		v[a] += v[b] + m[p0];
		v[d] = Bits.RotateRight(v[d] ^ v[a], 32);
		v[c] += v[d];
		v[b] = Bits.RotateRight(v[b] ^ v[c], 24);
		v[a] += v[b] + m[p1];
		v[d] = Bits.RotateRight(v[d] ^ v[a], 16);
		v[c] += v[d];
		v[b] = Bits.RotateRight(v[b] ^ v[c], 63);
	}


	#region Inner classes

	public sealed class Config : ICloneable {
		public byte[] Key { get; set; }
		public byte[] Personalization { get; set; }
		public byte[] Salt { get; set; }
		public int OutputSizeInBytes { get; set; }
		public int OutputSizeInBits => OutputSizeInBytes * 8;

		public Config()
			: this(64) {
		}

		public Config(int digestSize) {
			OutputSizeInBytes = digestSize;
		}

		public Config Clone() {
			return new Config {
				Key = this.Key?.Clone() as byte[],
				Personalization = this.Personalization?.Clone() as byte[],
				Salt = this.Salt?.Clone() as byte[],
				OutputSizeInBytes = this.OutputSizeInBytes
			};
		}

		object ICloneable.Clone() {
			return Clone();
		}
	}


	internal static class IVBuilder {
		private static readonly TreeConfig SequentialTreeConfig = new TreeConfig() { IntermediateHashSize = 0, LeafSize = 0, FanOut = 1, MaxHeight = 1 };

		public static ulong[] ConfigB(Config config, TreeConfig treeConfig) {
			bool isSequential = treeConfig == null;
			if (isSequential)
				treeConfig = SequentialTreeConfig;
			var rawConfig = new ulong[8];
			var result = new ulong[8];

			//digest length
			if (config.OutputSizeInBytes <= 0 | config.OutputSizeInBytes > 64)
				throw new ArgumentOutOfRangeException("config.OutputSize");
			rawConfig[0] |= (ulong)(uint)config.OutputSizeInBytes;

			//Key length
			if (config.Key != null) {
				if (config.Key.Length > 64)
					throw new ArgumentException("config.Key", "Key too long");
				rawConfig[0] |= (ulong)((uint)config.Key.Length << 8);
			}
			// FanOut
			rawConfig[0] |= (uint)treeConfig.FanOut << 16;
			// Depth
			rawConfig[0] |= (uint)treeConfig.MaxHeight << 24;
			// Leaf length
			rawConfig[0] |= ((ulong)(uint)treeConfig.LeafSize) << 32;
			// Inner length
			if (!isSequential && (treeConfig.IntermediateHashSize <= 0 || treeConfig.IntermediateHashSize > 64))
				throw new ArgumentOutOfRangeException("treeConfig.TreeIntermediateHashSize");
			rawConfig[2] |= (uint)treeConfig.IntermediateHashSize << 8;
			// Salt
			if (config.Salt != null) {
				if (config.Salt.Length != 16)
					throw new ArgumentException("config.Salt has invalid length");
				rawConfig[4] = _littleEndian.ToUInt64(config.Salt, 0);
				rawConfig[5] = _littleEndian.ToUInt64(config.Salt, 8);
			}
			// Personalization
			if (config.Personalization != null) {
				if (config.Personalization.Length != 16)
					throw new ArgumentException("config.Personalization has invalid length");
				rawConfig[6] = _littleEndian.ToUInt64(config.Personalization, 0);
				rawConfig[7] = _littleEndian.ToUInt64(config.Personalization, 8);
			}

			return rawConfig;
		}

		public static void ConfigBSetNode(ulong[] rawConfig, byte depth, ulong nodeOffset) {
			rawConfig[1] = nodeOffset;
			rawConfig[2] = (rawConfig[2] & ~0xFFul) | depth;
		}
	}


	public sealed class TreeConfig : ICloneable {
		public int IntermediateHashSize { get; set; }
		public int MaxHeight { get; set; }
		public long LeafSize { get; set; }
		public int FanOut { get; set; }

		public TreeConfig() {
			IntermediateHashSize = 64;
		}

		public TreeConfig Clone() {
			var result = new TreeConfig();
			result.IntermediateHashSize = IntermediateHashSize;
			result.MaxHeight = MaxHeight;
			result.LeafSize = LeafSize;
			result.FanOut = FanOut;
			return result;
		}

		public static TreeConfig CreateInterleaved(int parallelism) {
			var result = new TreeConfig();
			result.FanOut = parallelism;
			result.MaxHeight = 2;
			result.IntermediateHashSize = 64;
			return result;
		}

		object ICloneable.Clone() {
			return Clone();
		}
	}

	#endregion

}
