using System;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Math;

namespace Sphere10.Framework.CryptoEx.EC.MuSigBuilder; 

public class MuSigBuilder {
	private readonly byte[] _sessionId;
	private readonly Schnorr.PrivateKey _privateKey;
	private readonly byte[] _messageDigest;
	private readonly MuSig _muSig;
	private readonly List<byte[]> _publicKeys;
	private readonly List<byte[]> _publicNonces;
	private readonly List<byte[]> _partialSignatures;
	private readonly List<Tuple<byte[], BigInteger>> _keyAggregationCoefficients;
	private AggregatedPublicKeyData _aggregatedPublicKey;
	private SignerMuSigSession _signerMuSigSession;
	private MuSigSessionCache _muSigSessionCache;
	private byte[] _publicNonce;
	private byte[] _partialSignature;

	public byte[] PublicNonce => _publicNonce ?? ComputePublicNonce();
	public byte[] PartialSignature => _partialSignature ?? ComputePartialSignature();

	public MuSigBuilder(Schnorr.PrivateKey privateKey, byte[] messageDigest, byte[] sessionId = null) {
		_privateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
		_messageDigest = messageDigest;
		Schnorr.ValidateBuffer(nameof(messageDigest), messageDigest, 32);
		_sessionId = sessionId;
		
		if (_sessionId is not { Length: 32 }) {
			_sessionId = Tools.Crypto.GenerateCryptographicallyRandomBytes(32);
		}
		
		_muSig = new MuSig(new Schnorr(ECDSAKeyType.SECP256K1));
		_publicKeys = new List<byte[]>();
		_publicNonces = new List<byte[]>();
		_partialSignatures = new List<byte[]>();
		_keyAggregationCoefficients = new List<Tuple<byte[], BigInteger>>();
	}

	public void AddPublicKey(byte[] publicKey) {
		if (publicKey == null) {
			throw new ArgumentNullException(nameof(publicKey));
		}
		_publicKeys.Add(publicKey);
	}
	
	public void AddPublicNonce(byte[] publicNonce) {
		if (publicNonce == null) {
			throw new ArgumentNullException(nameof(publicNonce));
		}
		_publicNonces.Add(publicNonce);
	}
	
	public void AddPartialSignature(byte[] partialSignature) {
		if (partialSignature == null) {
			throw new ArgumentNullException(nameof(partialSignature));
		}
		_partialSignatures.Add(partialSignature);
	}

	public MuSigData BuildAggregatedSignature() {
		if (_partialSignatures.Count != _publicKeys.Count) {
			throw new InvalidOperationException("partial signature count must be equal to participant count");
		}
		var aggregatedSigs = _muSig.CombinePartialSigs(_muSigSessionCache.FinalNonce,
			_partialSignatures.Select(x => Schnorr.BytesToBigInt(x))
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
			_keyAggregationCoefficients.Add(new Tuple<byte[], BigInteger>(publicKey, keyAggregationCoefficient));
		}
	}
	
	private void ComputeAggregatedPublicKeys() {
		if (_aggregatedPublicKey != null) {
			// if already computed, return.
			return;
		}
		
		// combine public keys.
		_aggregatedPublicKey = _muSig.CombinePublicKeys(_keyAggregationCoefficients.Select(x => x.Item2)
		                                                                           .ToArray(),
			_keyAggregationCoefficients.Select(x => x.Item1)
			                           .ToArray());
	}
	
	private void InitializeSignerSession() {
		if (_signerMuSigSession != null) {
			// if already computed, return.
			return;
		}
		
		var privateKeyAsBytes = _privateKey.RawBytes;
		var publicKeyAsBytes = _muSig.Schnorr.DerivePublicKey(_privateKey).RawBytes;

		var secretKey = Schnorr.BytesToBigInt(privateKeyAsBytes);
		// 1. generate nonce data
		var nonceData = _muSig.GenerateNonce(_sessionId, Schnorr.BytesOfBigInt(secretKey, _muSig.KeySize), _messageDigest, null);
		
		var keyCoefficient = _keyAggregationCoefficients.FirstOrDefault(x => x.Item1.SequenceEqual(publicKeyAsBytes))?.Item2;
		if (keyCoefficient is null) {
			throw new InvalidOperationException("own public key not found");	
		}
		// 2. create private signing session
		_signerMuSigSession = new SignerMuSigSession {
			SecretKey = secretKey,
			KeyCoefficient = keyCoefficient,
			PublicNonce = nonceData.PublicNonce,
			PrivateNonce = nonceData.PrivateNonce,
			InternalKeyParity = false,
		};
	}
	
	private byte[] ComputePublicNonce() {
		ComputeKeyCoefficients();
		ComputeAggregatedPublicKeys();
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
		var aggregatedSessionNonce = _muSig.CombineSessionNonce(_publicNonces.ToArray(), aggregatedPublicKey, _messageDigest);
		var challenge = _muSig.ComputeChallenge(aggregatedSessionNonce.FinalNonce, aggregatedPublicKey, _messageDigest); 
		_muSigSessionCache = new MuSigSessionCache
		{
			FinalNonceParity = aggregatedSessionNonce.FinalNonceParity,
			FinalNonce = aggregatedSessionNonce.FinalNonce,
			Challenge = challenge,
			NonceCoefficient = aggregatedSessionNonce.NonceCoefficient,
			PublicKeyParity = _aggregatedPublicKey.PublicKeyParity
		};
	}
	
	private byte[] ComputePartialSignature() {
		InitializeMuSigSessionCache();
		_partialSignature = Schnorr.BytesOfBigInt(_muSig.PartialSign(_signerMuSigSession, _muSigSessionCache), _muSig.KeySize);
		return _partialSignature;
	}
}
