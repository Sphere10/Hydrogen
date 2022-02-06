using System;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities;

namespace Sphere10.Framework.CryptoEx.EC; 

public class MuSig {
	private static readonly byte[] MusigTag = Schnorr.ComputeSha256Hash(Encoding.UTF8.GetBytes("KeyAgg coefficient"));
	private static readonly byte[] KeyAggListTag = Schnorr.ComputeSha256Hash(Encoding.UTF8.GetBytes("KeyAgg list"));
	public Schnorr Schnorr { get; }
	
	public MuSig(Schnorr schnorr) {
		Schnorr = schnorr;
	}

	private BigInteger ComputeCoefficient(byte[] ell, int idx, byte[] currentPublicKey) {
		return idx == 1
			? BigInteger.One
			: BigIntegerUtils
			  .BytesToBigInteger(
				  Schnorr.ComputeSha256Hash(Arrays.ConcatenateAll(Arrays.ConcatenateAll(MusigTag, MusigTag), ell, currentPublicKey)))
			  .Mod(Schnorr.N);
	}

	// Computes ell = SHA256(publicKeys[0], ..., publicKeys[publicKeys.Length-1]) with
	// publicKeys serialized in compressed form.
	public static byte[] ComputeEll(byte[][] publicKeys) {
		Schnorr.ValidatePublicKeyArrays(publicKeys);
		return Schnorr.ComputeSha256Hash(Arrays.ConcatenateAll(KeyAggListTag, KeyAggListTag, Arrays.ConcatenateAll(publicKeys)));
	}
	
	public ECPoint CombinePublicKey(byte[][] publicKeys, byte[] publicKeyHash = null) {
		var ell = publicKeyHash ?? ComputeEll(publicKeys);
		ECPoint x = null;
		for (var i = 0; i < publicKeys.Length; i++) {
			var xi = Schnorr.LiftX(publicKeys[i]);
			var coefficient = ComputeCoefficient(ell, i, publicKeys[i]);
			var jj = BigIntegerUtils.BigIntegerToBytes(coefficient, 32);
			var summand = xi.Multiply(coefficient);
			x = x == null ? summand : x.Add(summand);
		}
		x = x?.Normalize();
		return Schnorr.IsPointInfinity(x) ? throw new Exception("public key combination failed") : x;
	}
	
	public MuSigSession InitializeSession(byte[] sessionId, BigInteger privateKey, byte[] publicKey, byte[] messageDigest, byte[] combinedPublicKey, bool publicKeyParity, byte[] ell, int idx) {
		ValidateSessionParams(sessionId, privateKey, messageDigest, combinedPublicKey, ell);
		var session = new MuSigSession {
			SessionId = sessionId,
			MessageDigest = messageDigest,
			CombinedPublicKey = combinedPublicKey,
			PublicKeyParity = publicKeyParity,
			Ell = ell,
			Idx = idx
		};

		var n = Schnorr.N;
		var coefficient = ComputeCoefficient(ell, idx, publicKey);
		session.SecretKey = privateKey.Multiply(coefficient).Mod(n);
		session.OwnKeyParity = Schnorr.IsEven(Schnorr.G.Multiply(privateKey).Normalize());
		if (session.PublicKeyParity != session.OwnKeyParity) {
			session.SecretKey = n.Subtract(session.SecretKey);
		}

		var nonceData = Arrays.ConcatenateAll(sessionId, messageDigest, combinedPublicKey, BigIntegerUtils.BigIntegerToBytes(privateKey, 32));
		session.SecretNonce = BigIntegerUtils.BytesToBigInteger(Schnorr.ComputeSha256Hash(nonceData));
		Schnorr.ValidateRange(nameof(session.SecretNonce), session.SecretNonce);
		var r = Schnorr.G.Multiply(session.SecretNonce).Normalize();
		session.Nonce = BigIntegerUtils.BigIntegerToBytes(r.AffineXCoord.ToBigInteger(), 32);
		session.NonceParity = Schnorr.IsEven(r);
		session.Commitment = Schnorr.ComputeSha256Hash(session.Nonce);
		session.PartialSignature = null;
		return session;
	}
	
	public byte[] CombineSessionNonce(MuSigSession session, byte[][] nonces) {
		Schnorr.ValidateNonceArrays(nonces);
		var r = Schnorr.LiftX(nonces[0]);
		for (var i = 1; i < nonces.Length; i++) {
			r = r.Add(Schnorr.LiftX(nonces[i]));
		}
		r = r.Normalize();
		session.CombinedNonceParity = Schnorr.IsEven(r);
		return BigIntegerUtils.BigIntegerToBytes(r.AffineXCoord.ToBigInteger(), 32);
	}
	
	public BigInteger PartialSign(MuSigSession session, byte[] message, byte[] combinedNonce, byte[] combinedPublicKey) {
		var e = Schnorr.GetE(combinedNonce, combinedPublicKey, message);
		var sk = session.SecretKey;
		var k = session.SecretNonce;
		var n = Schnorr.N;
		if (session.NonceParity != session.CombinedNonceParity) {
			k = n.Subtract(k);
		}
		return sk.Multiply(e).Add(k).Mod(n);
	}
	
	public void PartialSigVerify(MuSigSession session, BigInteger partialSignature, byte[] combinedNonce, int idx, byte[] publicKey, byte[] nonce) {
		var e = Schnorr.GetE(combinedNonce, session.CombinedPublicKey, session.MessageDigest);
		var coefficient = ComputeCoefficient(session.Ell, idx, publicKey);
		var pj = Schnorr.LiftX(publicKey);
		var ri = Schnorr.LiftX(nonce);
		var n = Schnorr.N;
		
		if (!session.PublicKeyParity) {
			e = n.Subtract(e);
		}

		var rp = Schnorr.GetR(partialSignature, e.Multiply(coefficient).Mod(n), pj);
		if (session.CombinedNonceParity) {
			rp = rp.Negate();
		}
		var sum = rp.Add(ri);
		if (!Schnorr.IsPointInfinity(sum)) {
			throw new Exception("partial signature verification failed");
		}
	}
	
	public byte[] CombinePartialSigs(byte[] combinedNonce, BigInteger[] partialSignatures) {
		var r = Schnorr.LiftX(combinedNonce);
		if (!(partialSignatures?.Any() ?? false)) {
			throw new Exception($"{nameof(partialSignatures)} cannot be null or empty");
		}
		var rx = BigIntegerUtils.BigIntegerToBytes(r.AffineXCoord.ToBigInteger(), 32);
		var s = partialSignatures[0];
		var n = Schnorr.N;
		for (var i = 1; i < partialSignatures.Length; i++) {
			var tmp = partialSignatures[i];
			if (tmp.CompareTo(n) >= 0) {
				throw new Exception($"{tmp} must be an integer less than n");	
			}
			s = s.Add(partialSignatures[i]).Mod(n);
		}
		return Arrays.ConcatenateAll(rx, BigIntegerUtils.BigIntegerToBytes(s, 32));
	}

	public MuSigData MuSigNonInteractive(Schnorr.PrivateKey[] privateKeys, byte[] messageDigest) {
		if (!(privateKeys?.Any() ?? false)) {
			throw new Exception($"{nameof(privateKeys)} cannot be null or empty");
		}
		
		if (messageDigest == null) {
			throw new Exception($"{nameof(messageDigest)} cannot be null");
		}

		// 1. derive the public keys.
		var publicKeys = privateKeys.Select(x => Schnorr.DerivePublicKey(x).RawBytes).ToArray();
		
		// 2. combine the public keys.
		var publicKeyHash = ComputeEll(publicKeys);
		var combinedPublicKeyPoint = CombinePublicKey(publicKeys, publicKeyHash);
		var combinedPublicKey = BigIntegerUtils.BigIntegerToBytes(combinedPublicKeyPoint.AffineXCoord.ToBigInteger(), 32);
		var publicKeyParity = Schnorr.IsEven(combinedPublicKeyPoint);
		
		// 3. create private signing sessions
		var sessions = new MuSigSession[privateKeys.Length];
		for (var i = 0; i < sessions.Length; i++) {
			sessions[i] = InitializeSession(
				Schnorr.RandomBytes(),
				BigIntegerUtils.BytesToBigInteger(privateKeys[i].RawBytes),
				publicKeys[i],
				messageDigest,
				combinedPublicKey,
				publicKeyParity,
				publicKeyHash,
				i);
		}
		var signerSession = sessions[0];
		
		// 4. combine nonces and keep track of whether the nonce was negated or not
		var nonces = sessions.Select(x => x.Nonce).ToArray();
		var combinedNonce = CombineSessionNonce(signerSession, nonces);
		for (var i = 0; i < sessions.Length; i++) {
			sessions[i].CombinedNonceParity = signerSession.CombinedNonceParity;
		}
		
		// 5. generate partial signatures
		for (var i = 0; i < sessions.Length; i++) {
			sessions[i].PartialSignature = PartialSign(sessions[i], messageDigest, combinedNonce, combinedPublicKey);
		}
		var partialSignatures = sessions.Select(x => x.PartialSignature).ToArray();
		
		// 6. verify individual partial signatures
		for (var i = 0; i < publicKeys.Length; i++) {
			PartialSigVerify(signerSession, sessions[i].PartialSignature, combinedNonce, i, publicKeys[i], nonces[i]);
		}
		
		return new MuSigData {
			// 7. combine partial signatures
			Signature = CombinePartialSigs(combinedNonce, partialSignatures),
			MessageDigest = messageDigest,
			CombinedPublicKey = combinedPublicKey
		};
	}

	// Validation Methods
	/// <summary>
	/// Validate Session Params
	/// </summary>
	/// <param name="sessionId"></param>
	/// <param name="privateKey"></param>
	/// <param name="messageDigest"></param>
	/// <param name="combinedPublicKey"></param>
	/// <param name="ell"></param>
	private static void ValidateSessionParams(byte[] sessionId, BigInteger privateKey, byte[] messageDigest, byte[] combinedPublicKey, byte[] ell) {
		Schnorr.ValidateSignatureParams(BigIntegerUtils.BigIntegerToBytes(privateKey, 32), messageDigest);
		Schnorr.ValidateBuffer(nameof(sessionId), sessionId, 32);
		Schnorr.ValidateBuffer(nameof(combinedPublicKey), combinedPublicKey, 32);
		Schnorr.ValidateBuffer(nameof(ell), ell, 32);
	}
}
