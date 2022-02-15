using System;
using System.Collections.Generic;
using Org.BouncyCastle.Math;

namespace Sphere10.Framework.CryptoEx.EC.MuSigBuilder;

public class SignerSessionBuilder {
	private readonly MuSig _muSig;
	private readonly byte[] _sessionId;
	private readonly Schnorr.PrivateKey _privateKey;
	private readonly Dictionary<byte[], BigInteger> _keyCoefficients;
	private readonly byte[] _messageDigest;

	public SignerSessionBuilder(MuSig muSig, byte[] sessionId, Schnorr.PrivateKey privateKey, Dictionary<byte[], BigInteger> keyCoefficients, byte[] messageDigest) {
		_privateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
		_keyCoefficients = keyCoefficients ?? throw new ArgumentNullException(nameof(keyCoefficients));
		_muSig = muSig ?? throw new ArgumentNullException(nameof(muSig));
		Schnorr.ValidateBuffer(nameof(sessionId), sessionId, _muSig.KeySize);
		Schnorr.ValidateBuffer(nameof(messageDigest), messageDigest, _muSig.KeySize);
		_sessionId = sessionId;
		_privateKey = privateKey;
		_messageDigest = messageDigest;
	}

	public SignerMuSigSession Build() {
		var privateKeyAsBytes = _privateKey.RawBytes;
		var publicKeyAsBytes = _muSig.Schnorr.DerivePublicKey(_privateKey).RawBytes;

		// 1. compute secret key
		var secretKey = Schnorr.BytesToBigInt(privateKeyAsBytes);
		// 2. generate nonce data
		var nonceData = _muSig.GenerateNonce(_sessionId, Schnorr.BytesOfBigInt(secretKey, _muSig.KeySize), _messageDigest, null);

		if (!_keyCoefficients.TryGetValue(publicKeyAsBytes, out var keyCoefficient)) {
			throw new InvalidOperationException("public key not found");
		}
		// 3. create private signing session
		return new SignerMuSigSession {
			SecretKey = secretKey,
			KeyCoefficient = keyCoefficient,
			PublicNonce = nonceData.PublicNonce,
			PrivateNonce = nonceData.PrivateNonce,
			InternalKeyParity = false,
		};
	}
}
