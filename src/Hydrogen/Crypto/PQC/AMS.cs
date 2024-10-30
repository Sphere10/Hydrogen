// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen.Collections.Spans;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hydrogen;

public class AMS : DigitalSignatureSchemeBase<AMS.PrivateKey, AMS.PublicKey> {
	public const int MaxHeight = 20;
	public const byte Version = 1;
	private readonly IOTSAlgorithm _ots;

	public AMS(AMSOTS ots)
		: this(InstantiateOTSAlgorithm(ots)) {
	}

	public AMS(AMSOTS ots, int h)
		: this(InstantiateOTSAlgorithm(ots), h) {
	}

	public AMS(IOTSAlgorithm algorithm)
		: this(algorithm, Configuration.DefaultHeight) {
	}

	public AMS(IOTSAlgorithm algorithm, int h)
		: this(algorithm, new Configuration(algorithm.Config, h)) {
	}

	public AMS(IOTSAlgorithm algorithm, Configuration config)
		: base(algorithm.Config.HashFunction) {
		Config = config;
		_ots = algorithm;
		Traits = Traits & DigitalSignatureSchemeTraits.PQC;
	}

	public Configuration Config { get; }

	public override IIESAlgorithm IES => throw new NotSupportedException("PQC algorithms have no known IES algorithms");

	public override bool TryParsePublicKey(ReadOnlySpan<byte> bytes, out PublicKey publicKey)
		=> PublicKey.TryParse(bytes, _ots.Config.HashFunction, out publicKey);

	public override bool TryParsePrivateKey(ReadOnlySpan<byte> bytes, out PrivateKey privateKey)
		=> PrivateKey.TryParse(bytes, _ots.Config.HashFunction, out privateKey);

	public override PrivateKey GeneratePrivateKey(ReadOnlySpan<byte> secret) {
		if (secret.Length != 32)
			throw new ArgumentException("Must be 256-bit value", nameof(secret));
		var rawBytes = new byte[64];
		Array.Fill(rawBytes, (byte)0);
		rawBytes[0] = Version;
		EndianBitConverter.Little.WriteTo((byte)_ots.Config.AMSID, rawBytes, 1);
		rawBytes[3] = (byte)Config.H;
		_ots.SerializeParameters(rawBytes.AsSpan(4, 28));
		secret.CopyTo(rawBytes.AsSpan(^32));
		return new PrivateKey(rawBytes, _ots.Config.HashFunction);
	}

	public override PublicKey DerivePublicKey(PrivateKey privateKey, ulong signerNonce) {
		var batchLength = 1U << privateKey.Height;
		var batchNo = signerNonce / batchLength;
		return DerivePublicKeyForBatch(privateKey, batchNo, true);
	}

	public PublicKey DerivePublicKeyForBatch(PrivateKey privateKey, ulong batchNo, bool rememberBatch = false) {
		var batch = CalculateBatch(privateKey, batchNo, out var spamCode);
		var rawPubKey = Tools.Array.Concat<byte>(
			EndianBitConverter.Little.GetBytes((uint)privateKey.KeyCode),
			EndianBitConverter.Little.GetBytes((ulong)batchNo),
			EndianBitConverter.Little.GetBytes((uint)spamCode),
			batch.Root
		);
		if (rememberBatch) {
			var publicKeyWithBatch = new PublicKeyWithBatch(rawPubKey, batch);
			privateKey.RememberDerivedKey(publicKeyWithBatch);
		}
		return new PublicKey(rawPubKey);
	}

	public override bool IsPublicKey(PrivateKey privateKey, ReadOnlySpan<byte> publicKeyBytes) {
		var batchNo = PublicKey.ExtractBatchNo(publicKeyBytes);
		return
			privateKey.KeyCode == PublicKey.ExtractKeyCode(publicKeyBytes) &&
			CalculateSpamCode(privateKey, batchNo) == PublicKey.ExtractSpamCode(publicKeyBytes) &&
			DerivePublicKeyForBatch(privateKey, batchNo).RawBytes.AsSpan().SequenceEqual(publicKeyBytes);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte[] Sign(PrivateKey privateKey, ReadOnlySpan<byte> message, ulong batchNo, int otsIndex) {
		var messageDigest = CalculateMessageDigest(message);
		return SignDigest(privateKey, messageDigest, batchNo, otsIndex);
	}

	public override byte[] SignDigest(PrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ulong signerNonce) {
		var (batchNo, otsIndex) = GetOTSIndex(privateKey.Height, signerNonce);
		return SignDigest(privateKey, messageDigest, batchNo, otsIndex);
	}

	public byte[] SignDigest(PrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ulong batchNo, int otsIndex) {
		var builder = new ByteArrayBuilder();
		// Append header
		builder.Append(privateKey.Height);
		builder.Append(EndianBitConverter.Little.GetBytes((ushort)otsIndex));

		// Get/Calc the OTS batch
		if (!privateKey.DerivedKeys.TryGetValue(batchNo, out var publicKeyWithBatch)) {
			DerivePublicKeyForBatch(privateKey, batchNo, true);
			publicKeyWithBatch = privateKey.DerivedKeys[batchNo];
		}
		var otsPubKey =
			Config.OTS.UsePublicKeyHashOptimization ? publicKeyWithBatch.Batch.GetValue(MerkleCoordinate.LeafAt(otsIndex)) : this.GetOTSKeys(privateKey, batchNo, otsIndex).PublicKey.AsFlatSpan();

		Debug.Assert(otsPubKey.Length == Config.OTS.PublicKeySize.Length * Config.OTS.PublicKeySize.Width);
		builder.Append(otsPubKey);

		// Derive the individual private key again
		// NOTE: possibility to optimize here if we want to cache ephemeral OTS private key, but large in memory
		var otsKey = GetOTSKeys(privateKey, batchNo, otsIndex);

		// Perform the OTS sig
		var otsSig = _ots.SignDigest(otsKey.PrivateKey, messageDigest).ToFlatArray();
		Debug.Assert(otsSig.Length == _ots.Config.SignatureSize.Length * _ots.Config.SignatureSize.Width);
		builder.Append(otsSig);

		// Append merkle-existence proof of pubKey in Batch (will always be 2^h hashes)
		var authPath = publicKeyWithBatch.Batch.GenerateExistenceProof(otsIndex).ToArray();
		foreach (var bytes in authPath) {
			builder.Append(bytes);
		}

		var sig = builder.ToArray();
		return sig;
	}

	public override bool VerifyDigest(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> digest, ReadOnlySpan<byte> publicKey) {
		Guard.Argument(digest.Length == _ots.Config.DigestSize, nameof(digest), $"Message digest must be {_ots.Config.DigestSize} bytes");
		Guard.Ensure(IsWellFormedSignature(signature), "Not a valid AMS signature");
		var reader = new ByteSpanReader(EndianBitConverter.Little);
		var height = reader.ReadByte(signature);
		var otsIndex = reader.ReadUInt16(signature);
		var otsPubKey = reader.ReadBytes2D(signature, _ots.Config.PublicKeySize.Length, _ots.Config.PublicKeySize.Width);
		var otsSig = reader.ReadBytes2D(signature, _ots.Config.SignatureSize.Length, _ots.Config.SignatureSize.Width);
		var proof = new byte[height][];
		for (var i = 0; i < proof.Length; i++)
			proof[i] = reader.ReadBytes(signature, _ots.Config.DigestSize);

		// OTS Key must exist in batch
		var otsPubKeyHash = Config.OTS.UsePublicKeyHashOptimization ? otsPubKey.AsFlatSpan() : _ots.ComputeKeyHash(otsPubKey);
		if (!MerkleMath.VerifyExistenceProof(_ots.Config.HashFunction, PublicKey.ExtractBatchRoot(publicKey).ToArray(), MerkleSize.FromLeafCount(1 << height), MerkleCoordinate.LeafAt(otsIndex), otsPubKeyHash, proof))
			return false;

		// OTS sig must be valid
		return _ots.VerifyDigest(otsSig, otsPubKey, digest);
	}

	public bool IsWellFormedSignature(ReadOnlySpan<byte> signature) {
		if (signature == null || signature.Length == 0)
			return false;
		var h = signature[0];
		return signature.Length == (
			3
			+ (_ots.Config.PublicKeySize.Length * _ots.Config.PublicKeySize.Width)
			+ (_ots.Config.SignatureSize.Length * _ots.Config.SignatureSize.Width)
			+ h * _ots.Config.DigestSize);
	}

	private IMerkleTree CalculateBatch(PrivateKey privateKey, ulong batchNo, out uint spamCode) {
		var batchSize = 1 << privateKey.Height;
		var batchLeafs = new byte[batchSize][];
		Parallel.For(0, batchSize, i => { batchLeafs[i] = GetOTSKeys(privateKey, batchNo, i).PublicKeyHash.Value; });
		spamCode = CalculateSpamCode(batchLeafs[0]);
		var merkleTree = new SimpleMerkleTree(_ots.Config.HashFunction);
		merkleTree.Leafs.AddRange(batchLeafs);
		return merkleTree;
	}

	private uint CalculateSpamCode(PrivateKey privateKey, ulong batchNo)
		=> CalculateSpamCode(GetOTSKeys(privateKey, batchNo, 0).PublicKeyHash.Value);

	private uint CalculateSpamCode(ReadOnlySpan<byte> wotsKey0)
		=> EndianBitConverter.Little.ToUInt32(wotsKey0.Slice(^4));

	private OTSKeyPair GetOTSKeys(PrivateKey privateKey, ulong batchNo, int index) =>
		_ots.GenerateKeys(Tools.Array.Concat<byte>(EndianBitConverter.Little.GetBytes((uint)index), EndianBitConverter.Little.GetBytes((ulong)batchNo), privateKey.RawBytes));

	private (ulong batchNo, int otsIndex) GetOTSIndex(int height, ulong signerNonce) {
		var batchLength = 1U << height;
		return (signerNonce / batchLength, (int)(signerNonce % batchLength));
	}

	private static IOTSAlgorithm InstantiateOTSAlgorithm(AMSOTS ots) {
		switch (ots) {
			case AMSOTS.WOTS:
				return new WOTS(WOTS.Configuration.Default.W, true);
			case AMSOTS.WOTS_Sharp:
				return new WOTSSharp(WOTSSharp.Configuration.Default.W, true);
			default:
				throw new NotSupportedException(ots.ToString());
		}
	}


	public abstract class Key : IKey {

		protected Key(byte[] immutableRawBytes) {
			RawBytes = immutableRawBytes;
		}

		public readonly byte[] RawBytes;

		public override bool Equals(object obj) {
			if (obj is Key key) {
				return Equals(key);
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected bool Equals(Key other) {
			return Equals(RawBytes, other.RawBytes);
		}

		public override int GetHashCode() {
			return (RawBytes != null ? RawBytes.GetHashCode() : 0);
		}

		#region IKey

		byte[] IKey.RawBytes => RawBytes;

		#endregion

	}


	public class PrivateKey : Key, IPrivateKey {

		public readonly byte Version;
		public readonly byte Height;
		public readonly uint KeyCode;
		private readonly Dictionary<ulong, PublicKeyWithBatch> _derivedKeys;

		internal PrivateKey(byte[] immutableRawBytes, CHF chf)
			: base(immutableRawBytes) {
			Version = immutableRawBytes[0];
			Guard.Argument(Version == AMS.Version, nameof(immutableRawBytes), "Unrecognized version");

			Height = immutableRawBytes[1];
			Guard.Argument(0 <= Height && Height <= MaxHeight, nameof(immutableRawBytes), "Unsupported key height");
			KeyCode = CalculateKeyCode(immutableRawBytes, chf);
			_derivedKeys = new Dictionary<ulong, PublicKeyWithBatch>();
		}

		internal void RememberDerivedKey(PublicKeyWithBatch publicKey) => _derivedKeys[publicKey.BatchNo] = publicKey;

		public IReadOnlyDictionary<ulong, PublicKeyWithBatch> DerivedKeys => _derivedKeys;

		public static bool TryParse(ReadOnlySpan<byte> rawBytes, CHF chf, out PrivateKey privateKey) {
			var version = rawBytes[0];
			if (version != AMS.Version) {
				privateKey = null;
				return false;
			}
			var height = rawBytes[1];
			if (height > MaxHeight) {
				privateKey = null;
				return false;
			}
			privateKey = new PrivateKey(rawBytes.ToArray(), chf);
			return true;
		}

		private static uint CalculateKeyCode(ReadOnlySpan<byte> privateKeyRawBytes, CHF chf)
			=> EndianBitConverter.Little.ToUInt32(Hashers.Iterate(chf, privateKeyRawBytes, 2).AsSpan(^4));

	}


	public class PublicKey : Key, IPublicKey {
		public readonly ulong BatchNo;
		public readonly uint KeyCode;
		public readonly uint SpamCode;
		public readonly byte[] BatchRoot;

		internal PublicKey(byte[] immutableRawBytes)
			: base(immutableRawBytes) {
			KeyCode = ExtractKeyCode(immutableRawBytes);
			BatchNo = ExtractBatchNo(immutableRawBytes);
			SpamCode = ExtractSpamCode(immutableRawBytes);
			BatchRoot = ExtractBatchRoot(immutableRawBytes).ToArray();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ExtractKeyCode(ReadOnlySpan<byte> publicKeyRawBytes)
			=> EndianBitConverter.Little.ToUInt32(publicKeyRawBytes, 0);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ExtractBatchNo(ReadOnlySpan<byte> publicKeyRawBytes)
			=> EndianBitConverter.Little.ToUInt64(publicKeyRawBytes, 4);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ExtractSpamCode(ReadOnlySpan<byte> publicKeyRawBytes)
			=> EndianBitConverter.Little.ToUInt32(publicKeyRawBytes, 12);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<byte> ExtractBatchRoot(ReadOnlySpan<byte> publicKeyRawBytes)
			=> publicKeyRawBytes.Slice(16);

		public static bool TryParse(ReadOnlySpan<byte> rawBytes, CHF chf, out PublicKey publicKey) {
			if (rawBytes.Length != Hashers.GetDigestSizeBytes(chf) + 16) {
				publicKey = null;
				return false;
			}
			publicKey = new PublicKey(rawBytes.ToArray());
			return true;
		}
	}


	public class PublicKeyWithBatch : PublicKey {
		public readonly IMerkleTree Batch;

		internal PublicKeyWithBatch(byte[] immutableRawBytes, IMerkleTree batch)
			: base(immutableRawBytes) {
			Batch = batch;
		}

	}


	public sealed class Configuration : ICloneable {
		public const int DefaultHeight = 8;
		public readonly int H;
		public readonly OTSConfig OTS;

		public Configuration(OTSConfig otsConfig) : this(otsConfig, 8) {
		}

		public Configuration(OTSConfig otsConfig, int h) {
			Guard.ArgumentInRange(h, 0, AMS.MaxHeight, nameof(h));
			OTS = (OTSConfig)otsConfig.Clone();
			H = h;
		}

		public Configuration Clone() => new Configuration(OTS, H);

		object ICloneable.Clone() => Clone();

	}

}
