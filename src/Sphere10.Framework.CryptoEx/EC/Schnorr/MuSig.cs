using System;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities;

namespace Sphere10.Framework.CryptoEx.EC;

//https://github.com/ElementsProject/secp256k1-zkp/blob/master/doc/musig-spec.mediawiki
public class MuSig {
	private static readonly byte[] MusigTag = Schnorr.ComputeSha256Hash(Encoding.UTF8.GetBytes("KeyAgg coefficient"));
	private static readonly byte[] KeyAggListTag = Schnorr.ComputeSha256Hash(Encoding.UTF8.GetBytes("KeyAgg list"));
	private static readonly byte[] MusigNonceTag = Schnorr.ComputeSha256Hash(Encoding.UTF8.GetBytes("MuSig/nonce"));
	public Schnorr Schnorr { get; }

	public MuSig(Schnorr schnorr) {
		Schnorr = schnorr;
	}
	
	// Computes ell = SHA256(publicKeys[0], ..., publicKeys[publicKeys.Length-1]) with
	// publicKeys serialized in compressed form.
	public static byte[] ComputeEll(byte[][] publicKeys) {
		Schnorr.ValidatePublicKeyArrays(publicKeys);
		var hasher = Schnorr.BorrowSHA256Hasher();
		hasher.Transform(KeyAggListTag);
		hasher.Transform(KeyAggListTag);
		hasher.Transform(Arrays.ConcatenateAll(publicKeys));
		return hasher.GetResult();
	}
	
	public static bool GetYCoordParity(ECPoint point) {
		return !Schnorr.IsEven(point);
	}

	private BigInteger ComputeCoefficient(byte[] ell, int idx, byte[] currentPublicKey) {
		if (idx == 1) {
			return BigInteger.One;
		}
		var hasher = Schnorr.BorrowSHA256Hasher();
		hasher.Transform(MusigTag);
		hasher.Transform(MusigTag);
		hasher.Transform(ell);
		hasher.Transform(currentPublicKey);
		return BigIntegerUtils.BytesToBigInteger(hasher.GetResult()).Mod(Schnorr.N);
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

	public MusigNonceData GenerateNonce(byte[] sessionId, byte[] privateKey, byte[] messageDigest, byte[] aggregatePubKey,
	                                    byte[] extraInput = null) {

		Schnorr.ValidateBuffer(nameof(sessionId), sessionId, 32);
		Schnorr.ValidateBuffer(nameof(privateKey), privateKey, 32);
		Schnorr.ValidateBuffer(nameof(messageDigest), messageDigest, 32);
		if (aggregatePubKey != null) {
			Schnorr.ValidateBuffer(nameof(aggregatePubKey), aggregatePubKey, 32);
		}
		if (extraInput != null) {
			Schnorr.ValidateBuffer(nameof(extraInput), extraInput, 32);
		}
		var privateNonce = GeneratePrivateNonce(sessionId, messageDigest, privateKey, aggregatePubKey, extraInput);
		var publicNonce = GeneratePublicNonce(privateNonce);
		return new MusigNonceData {
			PrivateNonce = privateNonce.GetFullNonce(),
			PublicNonce = publicNonce.GetFullNonce()
		};
	}

	private static byte[] CBytes(ECPoint point) {
		var xCoord = Schnorr.BytesOfXCoord(point);
		return Schnorr.IsEven(point)
			? Arrays.Concatenate(new byte[] { 2 }, xCoord)
			: Arrays.Concatenate(new byte[] { 3 }, xCoord);
	}

	private MusigPublicNonce GeneratePublicNonce(MusigPrivateNonce privateNonce) {
		if (!Schnorr.TryParsePrivateKey(privateNonce.K1, out var k1)) {
			throw new Exception($"unable to parse {nameof(privateNonce.K1)} privatekey");
		}
		if (!Schnorr.TryParsePrivateKey(privateNonce.K2, out var k2)) {
			throw new Exception($"unable to parse {nameof(privateNonce.K2)} privatekey");
		}

		var pointR1 = Schnorr.DerivePublicKey(k1).AsPoint.Value;
		var pointR2 = Schnorr.DerivePublicKey(k2).AsPoint.Value;
		
		return new MusigPublicNonce() {
			R1 = CBytes(pointR1),
			R2 = CBytes(pointR2)
		};
	}

	private static MusigPrivateNonce GeneratePrivateNonce(byte[] sessionId, byte[] messageDigest, byte[] privateKey, byte[] aggregatedPublicKey, byte[] extraInput) {
		var hasher = Schnorr.BorrowSHA256Hasher();
		hasher.Transform(MusigNonceTag);
		hasher.Transform(MusigNonceTag);
		hasher.Transform(sessionId);
		var marker = new byte[1];

		if (messageDigest != null) {
			marker[0] = 32;
			hasher.Transform(marker);
			hasher.Transform(messageDigest);
		} else {
			marker[0] = 0;
			hasher.Transform(marker);
		}

		if (privateKey != null) {
			marker[0] = 32;
			hasher.Transform(marker);
			hasher.Transform(privateKey);
		} else {
			marker[0] = 0;
			hasher.Transform(marker);
		}

		if (aggregatedPublicKey != null) {
			marker[0] = 32;
			hasher.Transform(marker);
			hasher.Transform(aggregatedPublicKey);
		} else {
			marker[0] = 0;
			hasher.Transform(marker);
		}

		if (extraInput != null) {
			marker[0] = 32;
			hasher.Transform(marker);
			hasher.Transform(extraInput);
		} else {
			marker[0] = 0;
			hasher.Transform(marker);
		}

		var seed = hasher.GetResult();
		var result = new MusigPrivateNonce();
		hasher.Transform(seed);
		hasher.Transform(new byte[] {
			0
		});
		result.K1 = hasher.GetResult();

		hasher.Transform(seed);
		hasher.Transform(new byte[] {
			1
		});
		result.K2 = hasher.GetResult();
		return result;
	}

	public MuSigSession InitializeSession(byte[] sessionId, BigInteger privateKey, byte[] publicKey, byte[] messageDigest,
	                                      byte[] combinedPublicKey, bool publicKeyParity, byte[] ell, int idx) {
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

		var nonceData = Arrays.ConcatenateAll(sessionId,
			messageDigest,
			combinedPublicKey,
			BigIntegerUtils.BigIntegerToBytes(privateKey, 32));
		session.PrivateNonce = BigIntegerUtils.BytesToBigInteger(Schnorr.ComputeSha256Hash(nonceData));
		Schnorr.ValidateRange(nameof(session.PrivateNonce), session.PrivateNonce);
		var r = Schnorr.G.Multiply(session.PrivateNonce).Normalize();
		session.PublicNonce = Schnorr.BytesOfXCoord(r);
		session.NonceParity = Schnorr.IsEven(r);
		session.Commitment = Schnorr.ComputeSha256Hash(session.PublicNonce);
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
		return Schnorr.BytesOfXCoord(r);
	}

	public BigInteger PartialSign(MuSigSession session, byte[] message, byte[] combinedNonce, byte[] combinedPublicKey) {
		var e = Schnorr.GetE(combinedNonce, combinedPublicKey, message);
		var sk = session.SecretKey;
		var k = session.PrivateNonce;
		var n = Schnorr.N;
		if (session.NonceParity != session.CombinedNonceParity) {
			k = n.Subtract(k);
		}
		return sk.Multiply(e).Add(k).Mod(n);
	}

	public void PartialSigVerify(MuSigSession session, BigInteger partialSignature, byte[] combinedNonce, int idx, byte[] publicKey,
	                             byte[] nonce) {
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
		var rx = Schnorr.BytesOfXCoord(r);
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
		var combinedPublicKey = Schnorr.BytesOfXCoord(combinedPublicKeyPoint);
		var uy = BigIntegerUtils.BigIntegerToBytes(combinedPublicKeyPoint.AffineYCoord.ToBigInteger(), 32);
		var publicKeyParity = GetYCoordParity(combinedPublicKeyPoint);

		// 3. create private signing sessions
		var sessions = new MuSigSession[privateKeys.Length];
		for (var i = 0; i < sessions.Length; i++) {
			sessions[i] = InitializeSession(Schnorr.RandomBytes(),
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
		var publicNonces = sessions.Select(x => x.PublicNonce).ToArray();
		var combinedNonce = CombineSessionNonce(signerSession, publicNonces);
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
			PartialSigVerify(signerSession, sessions[i].PartialSignature, combinedNonce, i, publicKeys[i], publicNonces[i]);
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
	private static void ValidateSessionParams(byte[] sessionId, BigInteger privateKey, byte[] messageDigest, byte[] combinedPublicKey,
	                                          byte[] ell) {
		Schnorr.ValidateSignatureParams(BigIntegerUtils.BigIntegerToBytes(privateKey, 32), messageDigest);
		Schnorr.ValidateBuffer(nameof(sessionId), sessionId, 32);
		Schnorr.ValidateBuffer(nameof(combinedPublicKey), combinedPublicKey, 32);
		Schnorr.ValidateBuffer(nameof(ell), ell, 32);
	}
}
