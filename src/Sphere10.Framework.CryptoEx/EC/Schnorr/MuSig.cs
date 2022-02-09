using System;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities;

namespace Sphere10.Framework.CryptoEx.EC;

//https://github.com/ElementsProject/secp256k1-zkp/blob/master/doc/musig-spec.mediawiki
public class MuSig {
	private static readonly byte[] MusigNonceTag = Schnorr.ComputeSha256Hash(Encoding.UTF8.GetBytes("MuSig/nonce"));
	public Schnorr Schnorr { get; }
	internal ECPoint G => Schnorr.G;
	internal ECCurve Curve => Schnorr.Curve; 
	private BigInteger P => Curve.Field.Characteristic;
	internal BigInteger N => Schnorr.N;
	internal int KeySize => Schnorr.KeySize;

	public MuSig(Schnorr schnorr) {
		Schnorr = schnorr;
	}

	private static IHashFunction BorrowSha256Hasher() {
		Hashers.BorrowHasher(CHF.SHA2_256, out var hasher);
		return hasher;
	}

	// Computes ell = SHA256(publicKeys[0], ..., publicKeys[publicKeys.Length-1]) with
	// publicKeys serialized in compressed form.
	public static byte[] ComputeEll(byte[][] publicKeys) {
		Schnorr.ValidatePublicKeyArrays(publicKeys);
		return Schnorr.TaggedHash("KeyAgg list", Arrays.ConcatenateAll(publicKeys));
	}

	private BigInteger ComputeKeyAggregationCoefficient(byte[] ell, int idx, byte[] currentPublicKey) {
		if (idx == 1) {
			return BigInteger.One;
		}
		var hash = Schnorr.TaggedHash("KeyAgg coefficient", Arrays.ConcatenateAll(ell, currentPublicKey));
		return Schnorr.BytesToBigInt(hash).Mod(Schnorr.N);
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

	private MusigNonceData GenerateNonce(byte[] sessionId, byte[] privateKey, byte[] messageDigest, byte[] aggregatePubKey,
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
			throw new ArgumentException($"unable to parse {nameof(privateNonce.K1)} private key");
		}
		if (!Schnorr.TryParsePrivateKey(privateNonce.K2, out var k2)) {
			throw new ArgumentException($"unable to parse {nameof(privateNonce.K2)} private key");
		}

		var pointR1 = Schnorr.DerivePublicKey(k1).AsPoint.Value;
		var pointR2 = Schnorr.DerivePublicKey(k2).AsPoint.Value;
		
		return new MusigPublicNonce {
			R1 = CBytes(pointR1),
			R2 = CBytes(pointR2)
		};
	}

	private static MusigPrivateNonce GeneratePrivateNonce(byte[] sessionId, byte[] messageDigest, byte[] privateKey, byte[] aggregatedPublicKey, byte[] extraInput) {
		var hasher = BorrowSha256Hasher();
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
		
		hasher.Transform(seed);
		hasher.Transform(new byte[] {
			0
		});
		var k1 = hasher.GetResult();

		hasher.Transform(seed);
		hasher.Transform(new byte[] {
			1
		});
		var k2 = hasher.GetResult();
		return new MusigPrivateNonce() {
			K1 = k1,
			K2 = k2
		};
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
			Idx = idx,
			SecretKey = privateKey,
			KeyCoefficient = ComputeKeyAggregationCoefficient(ell, idx, publicKey)
		};

		var nonceData = GenerateNonce(sessionId, Schnorr.BytesOfBigInt(privateKey, 32), messageDigest, null);
		session.PublicNonce = nonceData.PublicNonce;
		session.PrivateNonce = nonceData.PrivateNonce;
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
				throw new InvalidOperationException("unknown compression marker encountered");
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
		var nonceCoefficient = GetB(aggregatedNonce, aggregatedPublicKey, message);
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

	private BigInteger GetB(byte[] aggNonce, byte[] q, byte[] m) {
		var hash = Schnorr.TaggedHash("MuSig/noncecoef", Arrays.ConcatenateAll(aggNonce, q, m));
		return Schnorr.BytesToBigInt(hash).Mod(Schnorr.N);
	}

	public BigInteger PartialSign(MuSigSession session) {
		var n = Schnorr.N;
		var g = Schnorr.G;

		var k1 = Schnorr.BytesToBigInt(session.PrivateNonce.AsSpan().Slice(0, 32).ToArray());
		var k2 = Schnorr.BytesToBigInt(session.PrivateNonce.AsSpan().Slice(32, 32).ToArray());
		Schnorr.ValidateRange(nameof(k1), k1);
		Schnorr.ValidateRange(nameof(k2), k2);
		
		var mu = session.KeyCoefficient;
		var sk = session.SecretKey;
		var pk = g.Multiply(sk).Normalize();
		var l = 0;
		if (!Schnorr.IsEven(pk)) {
			l++;
		}
		if (session.PublicKeyParity) {
			l++;
		}
		if (session.InternalKeyParity) {
			l++;
		}
		if (l % 2 == 1) {
			sk = n.Subtract(sk); //sk = sk.Negate().Mod(n); also works
		}

		sk = sk.Multiply(mu);
		
		if (session.FinalNonceParity) {
			k1 = n.Subtract(k1);
			k2 = n.Subtract(k2);
		}
		var signature = sk.Multiply(session.Challenge);
		k2 = k2.Multiply(session.NonceCoefficient);
		k1 = k1.Add(k2);
		signature = signature.Add(k1).Mod(n);
		
		if (!PartialSigVerify(session, Schnorr.BytesOfXCoord(pk), signature)) {
			throw new InvalidOperationException("The created partial signature did not pass verification.");
		}
		return signature;
	}
	
	public bool PartialSigVerify(MuSigSession session, byte[] publicKey, BigInteger partialSignature) {
		if (session == null)
			throw new InvalidOperationException("You need to run Musig.InitializeSession first");
		if (partialSignature == null)
			throw new ArgumentNullException(nameof(partialSignature));
		if (publicKey == null)
			throw new ArgumentNullException(nameof(publicKey));

		var n = Schnorr.N;
		var g = Schnorr.G;
		var b = session.NonceCoefficient;
		
		var r1 = PointC(session.PublicNonce.AsSpan().Slice(0, 33).ToArray());
		var r2 = PointC(session.PublicNonce.AsSpan().Slice(33, 33).ToArray());
		
		var rj = r2;
		rj = rj.Multiply(b).Add(r1);

		var pkp = Schnorr.LiftX(publicKey);
		/* Multiplying the messagehash by the musig coefficient is equivalent
		 * to multiplying the signer's public key by the coefficient, except
		 * much easier to do. */
		var mu = session.KeyCoefficient;
		var e = session.Challenge.Multiply(mu).Mod(n);
		
		if (session.PublicKeyParity != session.InternalKeyParity) {
			e = n.Subtract(e); //e = e.Negate().Mod(n); also works
		}

		var s = partialSignature;
		/* Compute -s*G + e*pkj + rj */
		s = n.Subtract(s); // s = s.Negate().Mod(n); also works

		var tmp = pkp.Multiply(e).Add(g.Multiply(s));

		if (session.FinalNonceParity)
		{
			rj = rj.Negate();
		}
		tmp = tmp.Add(rj);
		return Schnorr.IsPointInfinity(tmp);
	}
	
	public BigInteger ComputeChallenge(byte[] finalNonce, byte[] combinedPublicKey, byte[] messageDigest) {
		return Schnorr.GetE(finalNonce, combinedPublicKey, messageDigest);
	}

	public byte[] CombinePartialSigs(byte[] finalNonce, BigInteger[] partialSignatures) {
		if (!(partialSignatures?.Any() ?? false)) {
			throw new ArgumentNullException(nameof(partialSignatures));
		}
		BigInteger s = null;
		var n = Schnorr.N;
		for (var i = 0; i < partialSignatures.Length; i++) {
			var summand = partialSignatures[i];
			if (summand.CompareTo(n) >= 0) {
				throw new ArgumentException($"{summand} must be an integer less than n");
			}
			s = s == null ? summand.Mod(n) : s.Add(summand).Mod(n);
		}
		return Arrays.Concatenate(finalNonce, Schnorr.BytesOfBigInt(s, 32));
	}

	public MuSigData MuSigNonInteractive(Schnorr.PrivateKey[] privateKeys, byte[] messageDigest) {
		if (!(privateKeys?.Any() ?? false)) {
			throw new ArgumentNullException(nameof(privateKeys));
		}

		if (messageDigest == null) {
			throw new ArgumentNullException(nameof(messageDigest));
		}

		// 1. derive the public keys.
		var publicKeys = privateKeys.Select(x => Schnorr.DerivePublicKey(x).RawBytes).ToArray();

		// 2. combine the public keys.
		var publicKeyHash = ComputeEll(publicKeys);
		var publicKeyAggregationData = CombinePublicKey(publicKeys, publicKeyHash);
		var combinedPublicKey = Schnorr.BytesOfXCoord(publicKeyAggregationData.CombinedPoint);
		var publicKeyParity = publicKeyAggregationData.PublicKeyParity;

		// 3. create private signing sessions
		var sessions = new MuSigSession[privateKeys.Length];
		for (var i = 0; i < sessions.Length; i++) {
			sessions[i] = InitializeSession(Schnorr.RandomBytes(),
				Schnorr.BytesToBigInt(privateKeys[i].RawBytes),
				publicKeys[i],
				messageDigest,
				combinedPublicKey,
				publicKeyParity,
				publicKeyHash,
				i);
		}

		// 4. combine nonce
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

		// 5. generate partial signatures
		for (var i = 0; i < sessions.Length; i++) {
			sessions[i].PartialSignature = PartialSign(sessions[i]);
		}
		
		// 6. verify individual partial signatures
		for (var i = 0; i < publicKeys.Length; i++) {
			if (!PartialSigVerify(sessions[i], publicKeys[i], sessions[i].PartialSignature)) {
				throw new Exception($"verification of partial signature at index {i} failed");
			}
		}

		// 7. combine partial signatures
		var partialSignatures = sessions.Select(x => x.PartialSignature).ToArray();
		var combinedSignature = CombinePartialSigs(combinedNonce.FinalNonce, partialSignatures);
		 return new MuSigData {
			 CombinedSignature = combinedSignature,
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
		Schnorr.ValidateSignatureParams(Schnorr.BytesOfBigInt(privateKey, 32), messageDigest);
		Schnorr.ValidateBuffer(nameof(sessionId), sessionId, 32);
		Schnorr.ValidateBuffer(nameof(combinedPublicKey), combinedPublicKey, 32);
		Schnorr.ValidateBuffer(nameof(ell), ell, 32);
	}
}
