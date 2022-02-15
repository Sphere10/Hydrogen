using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework.CryptoEx.EC.MuSigBuilder;

public class Workflow {
	private byte[][] PublishMyNonceAndCollectFromOthers(byte[] nonce) {
		// using a HashSet as duplicate nonce should not happen
		var nonces = new HashSet<byte[]>(ByteArrayEqualityComparer.Instance);
		nonces.Add(nonce);
		// Add nonce we retrieve from other participants
		return nonces.ToArray();
	}

	private byte[][] PublishMyPartialSigAndCollectFromOthers(byte[] partialSig) {
		// using a HashSet as duplicate sig should not happen
		var partialSigs = new HashSet<byte[]>(ByteArrayEqualityComparer.Instance);
		partialSigs.Add(partialSig);
		// Add signature we retrieve from other participants
		return partialSigs.ToArray();
	}

	private Schnorr.PrivateKey GetMyPrivateKeyFromWallet() {
		// get current participant private key
		return null;
	}

	private byte[][] GetAllParticipantsPublicKey() {
		// get all participants public key
		return null;
	}

	private byte[] GetMessageDigest() {
		return Tools.Crypto.GenerateCryptographicallyRandomBytes(32);
	}

	public void WorkflowDemo() {
		var muSig = new MuSig(new Schnorr(ECDSAKeyType.SECP256K1));
		var privateKey = GetMyPrivateKeyFromWallet();
		var publicKeys = GetAllParticipantsPublicKey();
		var sessionId = Tools.Crypto.GenerateCryptographicallyRandomBytes(32);
		var messageDigest = GetMessageDigest();

		var keyCoefficientBuilder = new KeyCoefficientBuilder(muSig);
		// add all public keys
		for (var i = 0; i < publicKeys.Length; i++) {
			keyCoefficientBuilder.AddPublicKey(publicKeys[i]);
		}
		var keyCoefficients = keyCoefficientBuilder.Build();

		var aggregatedKeyBuilder = new AggregatedKeyBuilder(muSig, keyCoefficients);
		var publicKeyAggregatedData = aggregatedKeyBuilder.Build();
		var aggregatedPubKey = muSig.Schnorr.BytesOfXCoord(publicKeyAggregatedData.CombinedPoint);
		var publicKeyParity = publicKeyAggregatedData.PublicKeyParity;

		var signerSessionBuilder = new SignerSessionBuilder(muSig, sessionId, privateKey, keyCoefficients, messageDigest);
		var signerSession = signerSessionBuilder.Build();

		// Communication Round 1: publish my public nonce and get other participants own.
		var allPublicNonces = PublishMyNonceAndCollectFromOthers(signerSession.PublicNonce);

		var aggregateNonceBuilder = new AggregatedNonceBuilder(muSig, allPublicNonces, aggregatedPubKey, messageDigest);
		var aggregatedSessionNonce = aggregateNonceBuilder.Build();

		var sessionCacheBuilder = new SessionCacheBuilder(muSig,
			aggregatedSessionNonce,
			aggregatedPubKey,
			messageDigest,
			publicKeyParity);
		var sessionCache = sessionCacheBuilder.Build();

		var partialSig = Schnorr.BytesOfBigInt(muSig.PartialSign(signerSession, sessionCache), muSig.KeySize);

		// Communication Round 2: publish my partial sig and get other participants own.
		var allPartialSigs = PublishMyPartialSigAndCollectFromOthers(partialSig);

		var aggregatedSigBuilder = new AggregatedSigBuilder(muSig, sessionCache, allPartialSigs);
		var aggregatedSigs = aggregatedSigBuilder.Build();
		var isVerified = muSig.Schnorr.VerifyDigest(aggregatedSigs, messageDigest, aggregatedPubKey);
	}
}
