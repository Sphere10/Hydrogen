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
			var summand = xi.Multiply(coefficient);
			x = x == null ? summand : x.Add(summand);
		}
		x = x?.Normalize();
		x = Schnorr.IsEven(x) ? x : x?.Negate();
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
		var x = BigIntegerUtils.BigIntegerToBytes(point.AffineYCoord.ToBigInteger(), 32);
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
		//ValidateSessionParams(sessionId, privateKey, messageDigest, combinedPublicKey, ell);
		var session = new MuSigSession {
			SessionId = sessionId,
			MessageDigest = messageDigest,
			CombinedPublicKey = combinedPublicKey,
			PublicKeyParity = publicKeyParity,
			Ell = ell,
			Idx = idx
		};
		
		var n = Schnorr.N;
		session.SecretKey = privateKey;

		session.Coefficient = ComputeCoefficient(ell, idx, publicKey);
		//session.SecretKey = privateKey.Multiply(coefficient).Mod(n);
		
		var point = Schnorr.G.Multiply(session.SecretKey).Normalize();
		session.OwnKeyParity = Schnorr.IsEven(point);
		
		if (session.OwnKeyParity ^ session.PublicKeyParity) {
			session.SecretKey = n.Subtract(session.SecretKey);
		}
		
		// if (Schnorr.IsEven(point) ^ Schnorr.IsEven(Schnorr.LiftX(combinedPublicKey))) {
		// 	session.SecretKey = n.Subtract(session.SecretKey);
		// }
		// if (session.PublicKeyParity != session.OwnKeyParity) {
		// 	session.SecretKey = n.Subtract(session.SecretKey);
		// }


		// var nonceData = Arrays.ConcatenateAll(sessionId,
		// 	messageDigest,
		// 	combinedPublicKey,
		// 	BigIntegerUtils.BigIntegerToBytes(privateKey, 32));
		var nonceData = GenerateNonce(sessionId, BigIntegerUtils.BigIntegerToBytes(privateKey, 32), messageDigest, combinedPublicKey);
		//session.PrivateNonce = BigIntegerUtils.BytesToBigInteger(Schnorr.ComputeSha256Hash(nonceData));
		//Schnorr.ValidateRange(nameof(session.PrivateNonce), session.PrivateNonce);
		// var r = Schnorr.G.Multiply(session.PrivateNonce).Normalize();
		// session.PublicNonce = Schnorr.BytesOfXCoord(r);
		//session.NonceParity = Schnorr.IsEven(r);
		session.PublicNonce = nonceData.PublicNonce;
		session.PrivateNonce = nonceData.PrivateNonce;
		//session.Commitment = Schnorr.ComputeSha256Hash(session.PublicNonce);
		session.PartialSignature = null;
		return session;
	}

	private ECPoint PointC(byte[] compressedPublicKey) {
		var point = Schnorr.LiftX(compressedPublicKey.AsSpan().Slice(1, 32).ToArray());
		var compressionMarker = compressedPublicKey[0];
		switch (compressionMarker) {
			case 2:
				return point;
			case 3:
				return point.Negate();
			default:
				throw new Exception("unknown compression marker encountered");
		}
	}

	public byte[] CombineSessionNonce(MuSigSession session, byte[][] nonces) {
		Schnorr.ValidateNonceArrays(nonces);
		ECPoint rA = null;
		ECPoint rB = null;
		for (var i = 0; i < nonces.Length; i++) {
			var sliceA = nonces[i].AsSpan().Slice(0, 33).ToArray();
			var sliceB = nonces[i].AsSpan().Slice(33, 33).ToArray();
			var summandA = PointC(sliceA);
			var summandB = PointC(sliceB);
			rA = rA == null ? summandA : rA.Add(summandA);
			rB = rB == null ? summandB : rB.Add(summandB);
		}
		rA = rA?.Normalize();
		rB = rB?.Normalize();
		rA = Schnorr.IsPointInfinity(rA) ? throw new Exception($"nonce combination failed, {nameof(rA)} is at infinity") : rA;
		rB = Schnorr.IsPointInfinity(rB) ? throw new Exception($"nonce combination failed, {nameof(rB)} is at infinity") : rB;
		return Arrays.Concatenate(CBytes(rA), CBytes(rB));
	}
	
	internal BigInteger GetB(byte[] aggNonce, byte[] q, byte[] m) {
		var hash = Schnorr.TaggedHash("MuSig/noncecoef", Arrays.ConcatenateAll(aggNonce, q, m));
		return BigIntegerUtils.BytesToBigInteger(hash).Mod(Schnorr.N);
	}

	public byte[] GenerateFinalNonce(byte[] message, byte[] combinedNonce, byte[] combinedPublicKey) {
		var r1 = PointC(combinedNonce.AsSpan().Slice(0, 33).ToArray());
		var r2 = PointC(combinedNonce.AsSpan().Slice(33, 33).ToArray());
		var b = GetB(combinedNonce, combinedPublicKey, message);
		var r = r2.Multiply(b).Add(r1).Normalize();
		Schnorr.ThrowIfPointIsAtInfinity(r);
		return Schnorr.BytesOfXCoord(r);
	}

	public PartialMuSig PartialSign(MuSigSession session, byte[] message, byte[] combinedNonce, byte[] combinedPublicKey) {
		var n = Schnorr.N;
		var g = Schnorr.G;
		var b = GetB(combinedNonce, combinedPublicKey, message);
		var e = Schnorr.GetE(combinedNonce, combinedPublicKey, message);
		var k1 = BigIntegerUtils.BytesToBigInteger(session.PrivateNonce.AsSpan().Slice(0, 32).ToArray());
		var k2 = BigIntegerUtils.BytesToBigInteger(session.PrivateNonce.AsSpan().Slice(32, 32).ToArray());
		Schnorr.ValidateRange(nameof(k1), k1);
		Schnorr.ValidateRange(nameof(k2), k2);
		k1 = Schnorr.IsEven(k1) ? k1 : n.Subtract(k1);
		k2 = Schnorr.IsEven(k2) ? k2 : n.Subtract(k2);
		var mu = session.Coefficient;
		var sk = session.SecretKey;
		var aa = b.Multiply(k2);
		var bb = e.Multiply(mu).Multiply(sk);
		var cc = k1.Add(aa).Add(bb);
		var sig = cc.Mod(n);

		//session.PubNonce = Arrays.Concatenate(CBytes(g.Multiply(k1)), CBytes(g.Multiply(k2)));

		return new PartialMuSig {
			Signature = sig
		};
		// if (session.NonceParity != session.CombinedNonceParity) {
		// 	k = n.Subtract(k);
		// }
		//return sk.Multiply(e).Add(k).Mod(n);
	}

	public void PartialSigVerify(MuSigSession session, PartialMuSig partialSignature, byte[] combinedNonce, int idx, byte[] publicKey,
	                             byte[] nonce) {
		var e = Schnorr.GetE(combinedNonce, session.CombinedPublicKey, session.MessageDigest);
		var coefficient = ComputeCoefficient(session.Ell, idx, publicKey);
		var pj = Schnorr.LiftX(publicKey);
		var ri = Schnorr.LiftX(nonce);
		var n = Schnorr.N;

		if (!session.PublicKeyParity) {
			e = n.Subtract(e);
		}

		var rp = Schnorr.GetR(partialSignature.Signature, e.Multiply(coefficient).Mod(n), pj);
		if (session.CombinedNonceParity) {
			rp = rp.Negate();
		}
		var sum = rp.Add(ri);
		if (!Schnorr.IsPointInfinity(sum)) {
			throw new Exception("partial signature verification failed");
		}
	}
	
	public byte[] CombinePartialSigs(byte[] finalNonce, PartialMuSig[] partialSignatures) {
		if (!(partialSignatures?.Any() ?? false)) {
			throw new Exception($"{nameof(partialSignatures)} cannot be null or empty");
		}

		BigInteger s = null;
		var n = Schnorr.N;
		for (var i = 0; i < partialSignatures.Length; i++) {
			var summand = partialSignatures[i].Signature;
			if (summand.CompareTo(n) >= 0) {
				throw new Exception($"{summand} must be an integer less than n");
			}
			s = s == null ? summand.Mod(n) : s.Add(summand).Mod(n);
		}
		return Arrays.Concatenate(finalNonce, BigIntegerUtils.BigIntegerToBytes(s, 32));
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
		//var publicKeyParity = GetYCoordParity(combinedPublicKeyPoint);
		var publicKeyParity = Schnorr.IsEven(combinedPublicKeyPoint);
		
		var session_id = new[]
		{
			"7CB6E93BCF96AEE2BB31AB80AC880E108438FCECCD2E6132B49A2CF103991ED0",
			"236851BDBB4E62E06D08DC228D4E83A0A0971816EED785F994D19B165952F38D",
			"BC1DCCF8BB20655E39ABB6279152D46AB999F4C36982DB3296986DC0368319EB",
		};

		// 3. create private signing sessions
		var sessions = new MuSigSession[privateKeys.Length];
		for (var i = 0; i < sessions.Length; i++) {
			// sessions[i] = InitializeSession(Schnorr.RandomBytes(),
			// 	BigIntegerUtils.BytesToBigInteger(privateKeys[i].RawBytes),
			// 	publicKeys[i],
			// 	messageDigest,
			// 	combinedPublicKey,
			// 	publicKeyParity,
			// 	publicKeyHash,
			// 	i);
			
			sessions[i] = InitializeSession(session_id[i].ToHexByteArray(),
				BigIntegerUtils.BytesToBigInteger(privateKeys[i].RawBytes),
				publicKeys[i],
				messageDigest,
				null,
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
		
		
		var expectedSigs = new[]
		{
			"34ce323cd18ce1193b45f8e1e8ff4d23241053211a3bd26d240f1fbd036f5d07",
			"f985a08c5da23b409a7a8ad2501598c20e71f9566df0168082136b0c9afc0f45",
			"71ae6305bc7cf6c784245daab7f2f5c9aff70abbfc7475708069fa7bf1b7c24d"
		};

		// var sigasbig = expectedSigs.Select(x => BigIntegerUtils.BytesToBigInteger(x.ToHexByteArray())).Select(k => new PartialMuSig(){Signature = k}).ToArray();
		//
		// var finalNonce = "ed7d22176b48817351b197be4ff6df813c938dfc3cd5c9823640c2303e22e80f".ToHexByteArray();
		// var hyu = CombinePartialSigs(finalNonce, sigasbig);

		// 5. generate partial signatures
		for (var i = 0; i < sessions.Length; i++) {
			sessions[i].PartialSignature = PartialSign(sessions[i], messageDigest, combinedNonce, combinedPublicKey);
		}
		
		// 6. generate final nonce
		var finalNonce = GenerateFinalNonce(messageDigest, combinedNonce, combinedPublicKey);

		var partialSignatures = sessions.Select(x => x.PartialSignature).ToArray();

		// 7. verify individual partial signatures
		for (var i = 0; i < publicKeys.Length; i++) {
			PartialSigVerify(signerSession, sessions[i].PartialSignature, combinedNonce, i, publicKeys[i], publicNonces[i]);
		}

		return new MuSigData {
			// 8. combine partial signatures
			Signature = CombinePartialSigs(finalNonce, partialSignatures),
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
	private void ValidateSessionParams(byte[] sessionId, BigInteger privateKey, byte[] messageDigest, byte[] combinedPublicKey,
	                                          byte[] ell) {
		Schnorr.ValidateRange(nameof(privateKey), privateKey);
		Schnorr.ValidateSignatureParams(BigIntegerUtils.BigIntegerToBytes(privateKey, 32), messageDigest);
		Schnorr.ValidateBuffer(nameof(sessionId), sessionId, 32);
		Schnorr.ValidateBuffer(nameof(combinedPublicKey), combinedPublicKey, 32);
		Schnorr.ValidateBuffer(nameof(ell), ell, 32);
	}
}
