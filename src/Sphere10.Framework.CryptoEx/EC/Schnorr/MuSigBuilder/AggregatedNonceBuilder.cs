using System;

namespace Sphere10.Framework.CryptoEx.EC.MuSigBuilder;

public class AggregatedNonceBuilder {
	private readonly byte[][] _publicNonces;
	private readonly byte[] _aggregatedPublicKey;
	private readonly byte[] _messageDigest;
	private readonly MuSig _muSig;

	public AggregatedNonceBuilder(MuSig muSig, byte[][] publicNonces, byte[] aggregatedPublicKey, byte[] messageDigest) {
		Schnorr.ValidateJaggedArray(nameof(publicNonces), publicNonces);
		Schnorr.ValidateBuffer(nameof(aggregatedPublicKey), aggregatedPublicKey, 32);
		Schnorr.ValidateBuffer(nameof(messageDigest), messageDigest, 32);
		_muSig = muSig ?? throw new ArgumentNullException(nameof(muSig));
		_publicNonces = publicNonces;
		_aggregatedPublicKey = aggregatedPublicKey;
		_messageDigest = messageDigest;
	}

	public MuSigSessionNonce Build() {
		return _muSig.CombineSessionNonce(_publicNonces, _aggregatedPublicKey, _messageDigest);
	}
}
