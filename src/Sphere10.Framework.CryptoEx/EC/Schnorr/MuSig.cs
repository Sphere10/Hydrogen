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

	private BigInteger ComputeKeyAggregationCoefficient(byte[] ell, int idx, byte[] currentPublicKey) {
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

	public PublicKeyAggregationData CombinePublicKey(byte[][] publicKeys, byte[] publicKeyHash = null) {
		var ell = publicKeyHash ?? ComputeEll(publicKeys);
		ECPoint x = null;
		for (var i = 0; i < publicKeys.Length; i++) {
			var xi = Schnorr.LiftX(publicKeys[i]);
			var coefficient = ComputeKeyAggregationCoefficient(ell, i, publicKeys[i]);
			var summand = xi.Multiply(coefficient);
			x = x == null ? summand : x.Add(summand);
		}
		x = x?.Normalize();
		var isPointEven = Schnorr.IsEven(x);
		var combinedPoint = isPointEven ? x : x?.Negate();
		var publicKeyParity = !isPointEven;
		Schnorr.ThrowIfPointIsAtInfinity(x);
		return new PublicKeyAggregationData() {
			CombinedPoint = combinedPoint,
			PublicKeyParity = publicKeyParity
		};
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
		session.SecretKey = privateKey;

		session.KeyCoefficient = ComputeKeyAggregationCoefficient(ell, idx, publicKey);

		var nonceData = GenerateNonce(sessionId, BigIntegerUtils.BigIntegerToBytes(privateKey, 32), messageDigest, null);

		session.PublicNonce = nonceData.PublicNonce;
		session.PrivateNonce = nonceData.PrivateNonce;
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

	public MusigSessionNonce CombineSessionNonce(byte[][] nonces, byte[] aggregatedPublicKey, byte[] message) {
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

		var aggregatedNonce = Arrays.Concatenate(CBytes(rA), CBytes(rB));
		var nonceCoefficient = GetB(aggregatedNonce, aggregatedPublicKey, message); // also known as b
		var finalNoncePoint = rB?.Multiply(nonceCoefficient);
		finalNoncePoint = finalNoncePoint?.Add(rA).Normalize();
		var finalNonce = Schnorr.BytesOfXCoord(finalNoncePoint);
		var finalNonceParity = !Schnorr.IsEven(finalNoncePoint);
		
		return new MusigSessionNonce {
			AggregatedNonce = aggregatedNonce,
			FinalNonce = finalNonce,
			NonceCoefficient = nonceCoefficient,
			FinalNonceParity = finalNonceParity
		};
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

	public BigInteger PartialSign(MuSigSession session) {
		var n = Schnorr.N;
		var g = Schnorr.G;

		var k1 = BigIntegerUtils.BytesToBigInteger(session.PrivateNonce.AsSpan().Slice(0, 32).ToArray());
		var k2 = BigIntegerUtils.BytesToBigInteger(session.PrivateNonce.AsSpan().Slice(32, 32).ToArray());
		Schnorr.ValidateRange(nameof(k1), k1);
		Schnorr.ValidateRange(nameof(k2), k2);
		
		var mu = session.KeyCoefficient;
		var sk = session.SecretKey;
		var pk = g.Multiply(sk).Normalize();
;
		var l = 0;
		if (!Schnorr.IsEven(pk))
			l++;
		if (session.PublicKeyParity)
			l++;
		if (session.InternalKeyParity)
			l++;
		if (l % 2 == 1)
			sk = n.Subtract(sk); //sk = sk.Negate().Mod(n); also works

		sk = sk.Multiply(mu);
		
		if (session.FinalNonceParity) {
			k1 = n.Subtract(k1);
			k2 = n.Subtract(k2);
		}
		;
		var signature = sk.Multiply(session.Challenge);
		k2 = k2.Multiply(session.NonceCoefficient);
		k1 = k1.Add(k2);
		signature = signature.Add(k1).Mod(n);
		return signature;
	}
	
	public BigInteger ComputeChallenge(byte[] finalNonce, byte[] combinedPublicKey, byte[] messageDigest) {
		return Schnorr.GetE(finalNonce, combinedPublicKey, messageDigest);
	}

	public void PartialSigVerify(MuSigSession session, BigInteger partialSignature, byte[] combinedNonce, int idx, byte[] publicKey,
	                             byte[] nonce) {
		var e = Schnorr.GetE(combinedNonce, session.CombinedPublicKey, session.MessageDigest);
		var coefficient = ComputeKeyAggregationCoefficient(session.Ell, idx, publicKey);
		var pj = Schnorr.LiftX(publicKey);
		var ri = Schnorr.LiftX(nonce);
		var n = Schnorr.N;

		if (!session.PublicKeyParity) {
			e = n.Subtract(e);
		}

		var rp = Schnorr.GetR(partialSignature, e.Multiply(coefficient).Mod(n), pj);
		if (session.FinalNonceParity) {
			rp = rp.Negate();
		}
		var sum = rp.Add(ri);
		if (!Schnorr.IsPointInfinity(sum)) {
			throw new Exception("partial signature verification failed");
		}
	}
	
	public byte[] CombinePartialSigs(byte[] finalNonce, BigInteger[] partialSignatures) {
		if (!(partialSignatures?.Any() ?? false)) {
			throw new Exception($"{nameof(partialSignatures)} cannot be null or empty");
		}
		BigInteger s = null;
		var n = Schnorr.N;
		for (var i = 0; i < partialSignatures.Length; i++) {
			var summand = partialSignatures[i];
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
		var publicKeyAggregationData = CombinePublicKey(publicKeys, publicKeyHash);
		var combinedPublicKey = Schnorr.BytesOfXCoord(publicKeyAggregationData.CombinedPoint);
		var publicKeyParity = publicKeyAggregationData.PublicKeyParity;
		
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
				combinedPublicKey,
				publicKeyParity,
				publicKeyHash,
				i);
		}

		// 4. combine nonces
		var publicNonces = sessions.Select(x => x.PublicNonce).ToArray();
		var combinedNonce = CombineSessionNonce(publicNonces, combinedPublicKey, messageDigest);
		// 5. compute challenge
		var challenge = ComputeChallenge(combinedNonce.FinalNonce, combinedPublicKey, messageDigest);
		// 6. update newly computed details to session
		for (var i = 0; i < sessions.Length; i++) {
			sessions[i].FinalNonce = combinedNonce.FinalNonce;
			sessions[i].FinalNonceParity = combinedNonce.FinalNonceParity;
			sessions[i].AggregatedNonce = combinedNonce.AggregatedNonce;
			sessions[i].NonceCoefficient = combinedNonce.NonceCoefficient;
			sessions[i].Challenge = challenge;
		}

		//var b = Arrays.ConcatenateAll(BigIntegerUtils.BigIntegerToBytes(combinedNonce.NonceCoefficient, 32), challenge, combinedNonce.FinalNonceParity ? new byte[]{ 1 } : new byte[]{ 0 });

		// 5. generate partial signatures
		for (var i = 0; i < sessions.Length; i++) {
			sessions[i].PartialSignature = PartialSign(sessions[i]);
		}
		//
		// // 6. generate final nonce
		// //var finalNonce = GenerateFinalNonce(messageDigest, combinedNonce, combinedPublicKey);
		//
		// var partialSignatures = sessions.Select(x => x.PartialSignature).ToArray();
		//
		// // 7. verify individual partial signatures
		// for (var i = 0; i < publicKeys.Length; i++) {
		// 	PartialSigVerify(signerSession, sessions[i].PartialSignature, combinedNonce, i, publicKeys[i], publicNonces[i]);
		// }

		// return new MuSigData {
		// 	// 8. combine partial signatures
		// 	Signature = CombinePartialSigs(finalNonce, partialSignatures),
		// 	MessageDigest = messageDigest,
		// 	CombinedPublicKey = combinedPublicKey
		// };
		return null;
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
