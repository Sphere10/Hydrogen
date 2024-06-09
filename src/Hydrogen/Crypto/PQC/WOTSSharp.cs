// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Hydrogen;

/// <summary>
/// W-OTS# scheme, a salt-hardened W-OTS variant that facilitates shorter/faster hash functions.
/// </summary>
public class WOTSSharp : WOTS {

	public WOTSSharp()
		: this(WOTSSharp.Configuration.Default) {
	}

	public WOTSSharp(int w, bool usePublicKeyHashOptimization = false)
		: this(w, Configuration.Default.HashFunction, usePublicKeyHashOptimization) {
	}

	public WOTSSharp(int w, CHF hashFunction, bool usePublicKeyHashOptimization = false)
		: this(new Configuration(w, hashFunction, usePublicKeyHashOptimization)) {
	}

	public WOTSSharp(Configuration config)
		: base(config) {
	}

	public override byte[,] SignDigest(byte[,] privateKey, ReadOnlySpan<byte> digest)
		=> SignDigest(privateKey, digest, Tools.Crypto.GenerateCryptographicallyRandomBytes(digest.Length));

	public byte[,] SignDigest(byte[,] privateKey, ReadOnlySpan<byte> digest, ReadOnlySpan<byte> seed) {
		Guard.Argument(seed.Length == digest.Length, nameof(seed), "Must be same size as digest");
		var wotsSig = base.SignDigest(privateKey, HMAC(digest, seed));
		Debug.Assert(wotsSig.Length == Config.SignatureSize.Length * Config.SignatureSize.Width);
		seed.CopyTo(wotsSig.GetRow(Config.SignatureSize.Length - 1)); // concat seed to sig
		return wotsSig;
	}

	public override bool VerifyDigest(byte[,] signature, byte[,] publicKey, ReadOnlySpan<byte> digest) {
		Debug.Assert(signature.Length == Config.SignatureSize.Length * Config.SignatureSize.Width);
		var seed = signature.GetRow(Config.SignatureSize.Length - 1);
		return base.VerifyDigest(signature, publicKey, HMAC(digest, seed));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private byte[] SMAC(ReadOnlySpan<byte> message, ReadOnlySpan<byte> seed)
		=> HMAC(ComputeMessageDigest(message), seed);

	private byte[] HMAC(ReadOnlySpan<byte> digest, ReadOnlySpan<byte> seed) {
		using (Hashers.BorrowHasher(Config.HashFunction, out var hasher)) {
			hasher.Transform(seed);
			hasher.Transform(digest);
			var innerHash = hasher.GetResult();
			hasher.Transform(seed);
			hasher.Transform(innerHash);
			return hasher.GetResult();
		}
	}


	public new class Configuration : WOTS.Configuration {
		public new static readonly Configuration Default;

		static Configuration() {
			Default = new Configuration(4, CHF.Blake2b_128, true);
		}

		public Configuration()
			: this(Default.W, Default.HashFunction, Default.UsePublicKeyHashOptimization) {
		}

		public Configuration(int w, CHF hasher, bool usePubKeyHashOptimization)
			: base(
				w,
				hasher,
				usePubKeyHashOptimization,
				AMSOTS.WOTS_Sharp,
				Hashers.GetDigestSizeBytes(hasher),
				new OTSKeySize(
					Hashers.GetDigestSizeBytes(hasher),
					(int)Math.Ceiling(256.0 / w) + (int)Math.Floor(Math.Log(((1 << w) - 1) * (256 / w), 1 << w)) + 1
				),
				new OTSKeySize(
					Hashers.GetDigestSizeBytes(hasher),
					usePubKeyHashOptimization ? 1 : (int)Math.Ceiling(256.0 / w) + (int)Math.Floor(Math.Log(((1 << w) - 1) * (256 / w), 1 << w)) + 1
				),
				new OTSKeySize(
					Hashers.GetDigestSizeBytes(hasher),
					(int)Math.Ceiling(256.0 / w) + (int)Math.Floor(Math.Log(((1 << w) - 1) * (256 / w), 1 << w)) + 1 + 1 // Adds extra row for seed here
				)
			) {
		}
	}
}
