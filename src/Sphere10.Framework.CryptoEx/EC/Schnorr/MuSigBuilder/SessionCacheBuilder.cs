using System;

namespace Sphere10.Framework.CryptoEx.EC.MuSigBuilder;

public class SessionCacheBuilder {
	private readonly MuSig _muSig;
	private readonly MuSigSessionNonce _muSigSessionNonce;
	private readonly byte[] _aggregatedPublicKey;
	private readonly byte[] _messageDigest;
	private readonly bool _publicKeyParity;
	public SessionCacheBuilder(MuSig muSig, MuSigSessionNonce muSigSessionNonce, byte[] aggregatedPublicKey, byte[] messageDigest, bool publicKeyParity) {
		_muSig = muSig ?? throw new ArgumentNullException(nameof(muSig));
		_muSigSessionNonce = muSigSessionNonce ?? throw new ArgumentNullException(nameof(muSigSessionNonce));
		Schnorr.ValidateBuffer(nameof(aggregatedPublicKey), aggregatedPublicKey, 32);
		Schnorr.ValidateBuffer(nameof(messageDigest), messageDigest, 32);
		_aggregatedPublicKey = aggregatedPublicKey;
		_messageDigest = messageDigest;
		_publicKeyParity = publicKeyParity;

	}

	public MuSigSessionCache Build() {
		var challenge = _muSig.ComputeChallenge(_muSigSessionNonce.FinalNonce, _aggregatedPublicKey, _messageDigest);
		return _muSig.InitializeSessionCache(_muSigSessionNonce, challenge, _publicKeyParity);
	}
}
