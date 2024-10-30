// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.CompilerServices;

namespace Hydrogen;

/// <summary>
/// Winternitz One-Time Signature scheme (W-OTS).
/// </summary>
public class WOTS : IOTSAlgorithm {

	public WOTS()
		: this(Configuration.Default) {
	}

	public WOTS(int w, bool usePublicKeyHashOptimization = false)
		: this(w, Configuration.Default.HashFunction, usePublicKeyHashOptimization) {
	}

	public WOTS(int w, CHF hashFunction, bool usePublicKeyHashOptimization = false)
		: this(new Configuration(w, hashFunction, usePublicKeyHashOptimization)) {
	}

	public WOTS(Configuration config) {
		Config = (Configuration)config.Clone();
	}

	public Configuration Config { get; }

	OTSConfig IOTSAlgorithm.Config => Config;

	public void SerializeParameters(Span<byte> buffer) {
		buffer[0] = (byte)Config.W;
	}

	public byte[,] GeneratePrivateKey()
		=> GenerateKeys().PrivateKey;

	public byte[,] DerivePublicKey(byte[,] privateKey) {
		var publicKey = new byte[Config.KeySize.Length, Config.KeySize.Width];
		for (var i = 0; i < Config.KeySize.Length; i++) {
			publicKey.SetRow(i, Hashers.Iterate(Config.HashFunction, privateKey.GetRow(i), Config.ChainLength));
		}
		return Config.UsePublicKeyHashOptimization ? ToOptimizedPublicKey(publicKey) : publicKey;
	}

	public OTSKeyPair GenerateKeys()
		=> GenerateKeys(Tools.Crypto.GenerateCryptographicallyRandomBytes(Config.DigestSize - 1));

	public OTSKeyPair GenerateKeys(ReadOnlySpan<byte> seed) {
		var enumeratedSeed = new byte[seed.Length + 1];
		seed.CopyTo(enumeratedSeed.AsSpan(1));
		return GenerateKeys(i => {
			enumeratedSeed[0] = (byte)i;
			return Hashers.Iterate(Config.HashFunction, enumeratedSeed, 2);
		});
	}

	public OTSKeyPair GenerateKeys(Func<int, byte[]> gen) {
		var priv = new byte[Config.KeySize.Length, Config.KeySize.Width];
		var pub = new byte[Config.KeySize.Length, Config.KeySize.Width]; // actual W-OTS pubkey is same size as priv key, we may optimize below
		for (var i = 0; i < Config.KeySize.Length; i++) {
			var randomBytes = gen(i);
			priv.SetRow(i, randomBytes);
			pub.SetRow(i, Hashers.Iterate(Config.HashFunction, randomBytes, Config.ChainLength));
		}

		IFuture<byte[]> pubKeyHash;
		if (Config.UsePublicKeyHashOptimization) {
			pub = ToOptimizedPublicKey(pub);
			pubKeyHash = ExplicitFuture<byte[]>.For(pub.ToFlatArray());
		} else {
			pubKeyHash = LazyLoad<byte[]>.From(() => ToOptimizedPublicKey(pub).ToFlatArray());
		}

		return new OTSKeyPair(priv, pub, pubKeyHash);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual byte[,] Sign(byte[,] privateKey, ReadOnlySpan<byte> message)
		=> SignDigest(privateKey, ComputeMessageDigest(message));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual byte[,] SignDigest(byte[,] privateKey, ReadOnlySpan<byte> digest) {
		var signature = new byte[Config.SignatureSize.Length, Config.SignatureSize.Width];

		// Sign the digest (and build the checksum in process)
		uint checksum = 0U;
		for (var i = 0; i < Config.SignatureDigits; i++) {
			var signValue = (int)Bits.ReadBinaryNumber(digest, Config.W * i, Config.W, IterateDirection.LeftToRight);
			var c = Config.ChainLength - signValue;
			checksum += (uint)c;
			signature.SetRow(i, Hashers.Iterate(Config.HashFunction, privateKey.GetRow(i), c));
		}

		// Sign the checksum
		Span<byte> checksumBytes = stackalloc byte[4];
		Bits.WriteBinaryNumber(checksum, checksumBytes, 0, 32, IterateDirection.LeftToRight);
		for (var i = 0; i < Config.ChecksumDigits; i++) {
			var signValue = (int)Bits.ReadBinaryNumber(checksumBytes, Config.W * i, Config.W, IterateDirection.LeftToRight);
			var c = Config.ChainLength - signValue;
			var row = Config.SignatureDigits + i;
			signature.SetRow(row, Hashers.Iterate(Config.HashFunction, privateKey.GetRow(row), c));
		}

		return signature;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Verify(byte[,] signature, byte[,] publicKey, ReadOnlySpan<byte> message)
		=> VerifyDigest(signature, publicKey, ComputeMessageDigest(message));

	public virtual bool VerifyDigest(byte[,] signature, byte[,] publicKey, ReadOnlySpan<byte> digest) {
		var verify = new byte[Config.KeySize.Length, Config.KeySize.Width];

		// Verify Digest
		uint checksum = 0U;
		for (var i = 0; i < Config.SignatureDigits; i++) {
			var signValue = (int)Bits.ReadBinaryNumber(digest, Config.W * i, Config.W, IterateDirection.LeftToRight);
			var c = Config.ChainLength - signValue;
			checksum += (uint)c;
			verify.SetRow(i, Hashers.Iterate(Config.HashFunction, signature.GetRow(i), signValue));
		}

		// Verify checksum
		Span<byte> checksumBytes = stackalloc byte[4];
		Bits.WriteBinaryNumber(checksum, checksumBytes, 0, 32, IterateDirection.LeftToRight);
		for (var i = 0; i < Config.ChecksumDigits; i++) {
			var signValue = (int)Bits.ReadBinaryNumber(checksumBytes, Config.W * i, Config.W, IterateDirection.LeftToRight);
			var row = Config.SignatureDigits + i;
			verify.SetRow(row, Hashers.Iterate(Config.HashFunction, signature.GetRow(row), signValue));
		}

		return (Config.UsePublicKeyHashOptimization ? this.ComputeKeyHash(verify) : verify.AsFlatSpan()).SequenceEqual(publicKey.AsFlatSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ComputeKeyHash(byte[,] key, Span<byte> result) {
		ComputeKeyHash(key.AsFlatSpan(), result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ComputeKeyHash(ReadOnlySpan<byte> key, Span<byte> result) {
		Hashers.Hash(Config.HashFunction, key, result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected byte[,] ToOptimizedPublicKey(byte[,] publicKey) {
		var publicKeyHash = new byte[1, Config.DigestSize];
		ComputeKeyHash(publicKey, publicKeyHash.AsFlatSpan());
		return publicKeyHash;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte[] ComputeMessageDigest(ReadOnlySpan<byte> message)
		=> Hashers.Hash(Config.HashFunction, message);


	public class Configuration : OTSConfig {
		public static readonly Configuration Default;

		static Configuration() {
			Default = new Configuration(8, CHF.SHA2_256, false);
		}

		public Configuration() : this(Default.W, Default.HashFunction, Default.UsePublicKeyHashOptimization) {
		}

		public Configuration(int w, CHF hasher, bool usePubKeyHashOptimization)
			: this(
				w,
				hasher,
				usePubKeyHashOptimization,
				AMSOTS.WOTS,
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
					(int)Math.Ceiling(256.0 / w) + (int)Math.Floor(Math.Log(((1 << w) - 1) * (256 / w), 1 << w)) + 1
				)
			) {
		}

		protected Configuration(int w, CHF hasher, bool usePubKeyHashOptimization, AMSOTS id, int digestSize, OTSKeySize keySize, OTSKeySize publicKeySize, OTSKeySize signatureSize)
			: base(id, hasher, digestSize, usePubKeyHashOptimization, keySize, publicKeySize, signatureSize) {
			Guard.ArgumentInRange(w, 1, 16, nameof(w));
			W = (byte)w;
			ChainLength = (1 << w) - 1; // 2^w - 1 (length of Winternitz chain)
			SignatureDigits = (int)Math.Ceiling(256.0 / w); // how many chains required; 
			ChecksumDigits = (int)Math.Floor(Math.Log(((1 << w) - 1) * (256 / w), 1 << w)) + 1; // floor ( log_b (2^w - 1) * (256/w) ) where b = 2^w
		}

		public int W { get; }

		public int ChainLength { get; }

		public int SignatureDigits { get; }

		public int ChecksumDigits { get; }

		public override object Clone() => new Configuration(W, HashFunction, UsePublicKeyHashOptimization, AMSID, DigestSize, KeySize, PublicKeySize, SignatureSize);

	}

}
