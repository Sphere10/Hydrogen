// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe, Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Math;

namespace Hydrogen.CryptoEx.EC.Schnorr;

public class MuSigBuilder {
	private readonly byte[] _sessionId;
	private readonly Schnorr.PrivateKey _privateKey;
	private readonly byte[] _messageDigest;
	private readonly MuSig _muSig;
	private readonly HashSet<byte[]> _publicKeys;
	private readonly Dictionary<byte[], byte[]> _publicNonces;
	private readonly Dictionary<byte[], byte[]> _partialSignatures;
	private readonly Dictionary<byte[], BigInteger> _keyAggregationCoefficients;
	private AggregatedPublicKeyData _aggregatedPublicKey;
	private SignerMuSigSession _signerMuSigSession;
	private MuSigSessionCache _muSigSessionCache;
	private byte[] _publicKey;
	private byte[] _publicNonce;

	private byte[] _partialSignature;

	// used to check if a MuSigBuilder instance has signed a message before to avoid reuse.
	private bool _hasSignedBefore;

	public byte[] PublicKey => _publicKey ?? DerivePublicKey();
	public byte[] PublicNonce => _publicNonce ?? ComputePublicNonce();
	public byte[] PartialSignature => _partialSignature ?? ComputePartialSignature();

	/// <summary>
	/// this method sorts the public Keys in lexicographic order as defined by the specification.
	/// https://github.com/ElementsProject/secp256k1-zkp/blob/master/doc/musig-spec.mediawiki#Key_Sorting
	/// </summary>
	/// <param name="publicKeys"></param>
	public static void SortPublicKeysInLexicographicOrder(byte[][] publicKeys) {
		Array.Sort(publicKeys, ByteArrayComparer.Instance);
	}

	public MuSigBuilder(Schnorr.PrivateKey privateKey, byte[] messageDigest, byte[] sessionId = null) {
		_privateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
		_messageDigest = messageDigest;
		Schnorr.ValidateBuffer(nameof(messageDigest), messageDigest, 32);
		_sessionId = sessionId;

		if (_sessionId is not { Length: 32 }) {
			_sessionId = Tools.Crypto.GenerateCryptographicallyRandomBytes(32);
		}

		_muSig = new MuSig(new Schnorr(ECDSAKeyType.SECP256K1));
		// duplicate public keys not allowed
		_publicKeys = new HashSet<byte[]>(ByteArrayEqualityComparer.Instance);
		_publicNonces = new Dictionary<byte[], byte[]>(ByteArrayEqualityComparer.Instance);
		_partialSignatures = new Dictionary<byte[], byte[]>(ByteArrayEqualityComparer.Instance);
		_keyAggregationCoefficients = new Dictionary<byte[], BigInteger>(ByteArrayEqualityComparer.Instance);
		_hasSignedBefore = false;
	}

	public void AddPublicKey(byte[] publicKey) {
		if (publicKey == null) {
			throw new ArgumentNullException(nameof(publicKey));
		}
		if (!_publicKeys.Add(publicKey)) {
			throw new ArgumentException($"public key {publicKey.ToHexString()} already added");
		}
	}

	public void AddPublicNonce(byte[] publicKey, byte[] publicNonce) {
		if (publicKey == null) {
			throw new ArgumentNullException(nameof(publicKey));
		}
		if (publicNonce == null) {
			throw new ArgumentNullException(nameof(publicNonce));
		}
		if (!_publicNonces.TryAdd(publicKey, publicNonce)) {
			throw new ArgumentException($"public key {publicKey.ToHexString()} of pair already added in {nameof(_publicNonces)}");
		}
	}

	public void AddPartialSignature(byte[] publicKey, byte[] partialSignature) {
		if (publicKey == null) {
			throw new ArgumentNullException(nameof(publicKey));
		}
		if (partialSignature == null) {
			throw new ArgumentNullException(nameof(partialSignature));
		}
		if (!_partialSignatures.TryAdd(publicKey, partialSignature)) {
			throw new ArgumentException($"public key {publicKey.ToHexString()} of pair already added in {nameof(_partialSignatures)}");
		}
	}

	public void VerifyPartialSignatures() {
		if (_partialSignatures.Count != _publicKeys.Count) {
			throw new InvalidOperationException("partial signature count must be equal to participant count");
		}
		foreach (var (publicKey, partialSignature) in _partialSignatures) {
			var keyCoefficient = GetKeyAggregationCoefficient(publicKey);
			var publicNonce = GetPublicNonce(publicKey);
			if (!VerifyPartialSignature(_muSigSessionCache, keyCoefficient, publicKey, publicNonce, Schnorr.BytesToBigIntPositive(partialSignature))) {
				throw new InvalidOperationException($"partial signature verification of participant (publicKey: {publicKey.ToHexString()}) failed");
			}
		}
	}

	public MuSigData BuildAggregatedSignature() {
		if (_partialSignatures.Count != _publicKeys.Count) {
			throw new InvalidOperationException("partial signature count must be equal to participant count");
		}
		var aggregatedSigs = _muSig.AggregatePartialSignatures(_muSigSessionCache.FinalNonce,
			_partialSignatures.Select(x => Schnorr.BytesToBigIntPositive(x.Value))
				.ToArray());
		return new MuSigData {
			AggregatedSignature = aggregatedSigs,
			AggregatedPublicKey = _muSig.Schnorr.BytesOfXCoord(_aggregatedPublicKey.CombinedPoint)
		};
	}

	private void ComputeKeyCoefficients() {
		if (!_publicKeys.Any()) {
			throw new Exception("you need to add public keys before calculating coefficients");
		}

		if (_keyAggregationCoefficients.Count == _publicKeys.Count) {
			// if already computed, return.
			return;
		}
		var publicKeys = _publicKeys.ToArray();

		// 1. compute the public keys hash.
		var publicKeysHash = _muSig.ComputeEll(publicKeys);
		// 2. get second public key
		var secondPublicKey = _muSig.GetSecondPublicKey(publicKeys);
		// 3. compute key coefficients
		foreach (var publicKey in publicKeys) {
			var keyAggregationCoefficient = _muSig.ComputeKeyAggregationCoefficient(publicKeysHash,
				publicKey,
				secondPublicKey);
			if (!_keyAggregationCoefficients.TryAdd(publicKey, keyAggregationCoefficient)) {
				throw new ArgumentException($"public key {publicKey.ToHexString()} of pair already added in {nameof(_keyAggregationCoefficients)}");
			}
		}
	}

	private void AggregatePublicKeys() {
		if (_aggregatedPublicKey != null) {
			// if already computed, return.
			return;
		}

		var keyCoefficients = _keyAggregationCoefficients.Select(item =>
			new Tuple<byte[], BigInteger>(
				item.Key,
				item.Value
			)).ToArray();
		// aggregate public keys.
		_aggregatedPublicKey = _muSig.AggregatePublicKeys(
			keyCoefficients.Select(x => x.Item2).ToArray(),
			keyCoefficients.Select(x => x.Item1).ToArray()
		);
	}

	private void InitializeSignerSession() {
		if (_signerMuSigSession != null) {
			// if already computed, return.
			return;
		}

		var privateKeyAsBytes = _privateKey.RawBytes;

		var secretKey = Schnorr.BytesToBigIntPositive(privateKeyAsBytes);
		// 1. generate nonce data
		var nonceData = _muSig.GenerateNonce(_sessionId, Schnorr.BytesOfBigInt(secretKey, _muSig.KeySize), _messageDigest, null);

		var keyCoefficient = GetKeyAggregationCoefficient(PublicKey);
		// 2. create private signing session
		_signerMuSigSession = new SignerMuSigSession {
			SecretKey = secretKey,
			KeyCoefficient = keyCoefficient,
			PublicNonce = nonceData.PublicNonce,
			PrivateNonce = nonceData.PrivateNonce,
			InternalKeyParity = false,
		};
	}

	private BigInteger GetKeyAggregationCoefficient(byte[] publicKey) {
		return !_keyAggregationCoefficients.TryGetValue(publicKey, out var keyCoefficient)
			? throw new InvalidOperationException("own public key not found")
			: keyCoefficient;
	}

	private byte[] GetPublicNonce(byte[] publicKey) {
		return !_publicNonces.TryGetValue(publicKey, out var publicNonce)
			? throw new InvalidOperationException("own public key not found")
			: publicNonce;
	}

	private byte[] ComputePublicNonce() {
		ComputeKeyCoefficients();
		AggregatePublicKeys();
		InitializeSignerSession();
		_publicNonce = _signerMuSigSession.PublicNonce;
		return _publicNonce;
	}

	private void InitializeMuSigSessionCache() {
		if (_publicNonces.Count != _publicKeys.Count) {
			throw new InvalidOperationException("public nonce count must be equal to participant count");
		}

		if (_muSigSessionCache != null) {
			// if already computed, return.
			return;
		}
		var aggregatedPublicKey = _muSig.Schnorr.BytesOfXCoord(_aggregatedPublicKey.CombinedPoint);
		var aggregatedPublicNonce = _muSig.AggregatePublicNonces(_publicNonces.Values.ToArray(), aggregatedPublicKey, _messageDigest);
		var challenge = _muSig.ComputeChallenge(aggregatedPublicNonce.FinalNonce, aggregatedPublicKey, _messageDigest);
		_muSigSessionCache = _muSig.InitializeSessionCache(aggregatedPublicNonce, challenge, _aggregatedPublicKey.PublicKeyParity);
	}

	private bool VerifyPartialSignature(MuSigSessionCache sessionCache, BigInteger keyCoefficient, byte[] publicKey, byte[] publicNonce, BigInteger partialSignature) {
		return _muSig.PartialSigVerify(sessionCache, keyCoefficient, publicKey, publicNonce, partialSignature);
	}

	private byte[] ComputePartialSignature() {
		if (_hasSignedBefore) {
			throw new InvalidOperationException($"you cannot reuse a {GetType().Name} instance. please instantiate a new instance");
		}
		InitializeMuSigSessionCache();
		_partialSignature = Schnorr.BytesOfBigInt(_muSig.PartialSign(_signerMuSigSession, _muSigSessionCache), _muSig.KeySize);
		_hasSignedBefore = true;
		return _partialSignature;
	}

	private byte[] DerivePublicKey() {
		_publicKey = _muSig.Schnorr.DerivePublicKey(_privateKey).RawBytes;
		return _publicKey;
	}
}
