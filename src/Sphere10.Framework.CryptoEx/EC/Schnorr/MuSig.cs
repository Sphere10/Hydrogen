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
	public int KeySize => Schnorr.KeySize;

	public MuSig(Schnorr schnorr) {
		Schnorr = schnorr;
	}

	// Computes ell = SHA256(publicKeys[0], ..., publicKeys[publicKeys.Length-1]) with
	// publicKeys serialized in compressed form.
	public byte[] ComputeEll(byte[][] publicKeys) {
		Schnorr.ValidateJaggedArray(nameof(publicKeys), publicKeys);
		return Schnorr.TaggedHash("KeyAgg list", Arrays.ConcatenateAll(publicKeys));
	}

	private BigInteger ComputeKeyAggregationCoefficient(byte[] ell, byte[] currentPublicKey, byte[] secondPublicKey) {
		if (Arrays.AreEqual(currentPublicKey, secondPublicKey)) {
			return BigInteger.One;
		}
		var hash = Schnorr.TaggedHash("KeyAgg coefficient", Arrays.ConcatenateAll(ell, currentPublicKey));
		return Schnorr.BytesToBigInt(hash).Mod(Schnorr.N);
	}

	public PublicKeyAggregationData CombinePublicKey(BigInteger[] keyCoefficients, byte[][] publicKeys) {
		Schnorr.ValidateArray(nameof(keyCoefficients), keyCoefficients);
		Schnorr.ValidateJaggedArray(nameof(publicKeys), publicKeys);

		ECPoint x = null;
		for (var i = 0; i < publicKeys.Length; i++) {
			var xi = Schnorr.LiftX(publicKeys[i]);
			var summand = xi.Multiply(keyCoefficients[i]);
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
	private MuSigNonceData GenerateNonce(byte[] sessionId, byte[] privateKey, byte[] messageDigest, byte[] aggregatePubKey,
										 byte[] extraInput = null) {

		Schnorr.ValidateArray(nameof(sessionId), sessionId);
		Schnorr.ValidateArray(nameof(privateKey), privateKey);
		Schnorr.ValidateArray(nameof(messageDigest), messageDigest);

		if (aggregatePubKey != null) {
			Schnorr.ValidateBuffer(nameof(aggregatePubKey), aggregatePubKey, KeySize);
		}
		if (extraInput != null) {
			Schnorr.ValidateBuffer(nameof(extraInput), extraInput, 32);
		}
		var privateNonce = GeneratePrivateNonce(sessionId, messageDigest, privateKey, aggregatePubKey, extraInput);
		var publicNonce = GeneratePublicNonce(privateNonce);
		return new MuSigNonceData {
			PrivateNonce = privateNonce.GetFullNonce(),
			PublicNonce = publicNonce.GetFullNonce()
		};
	}

	private byte[] CBytes(ECPoint point) {
		var xCoord = Schnorr.BytesOfXCoord(point);
		return Schnorr.IsEven(point)
			? Arrays.Concatenate(new byte[] { 2 }, xCoord)
			: Arrays.Concatenate(new byte[] { 3 }, xCoord);
	}

	private MuSigPublicNonce GeneratePublicNonce(MuSigPrivateNonce privateNonce) {
		if (!Schnorr.TryParsePrivateKey(privateNonce.K1, out var k1)) {
			throw new ArgumentException($"unable to parse {nameof(privateNonce.K1)} private key");
		}
		if (!Schnorr.TryParsePrivateKey(privateNonce.K2, out var k2)) {
			throw new ArgumentException($"unable to parse {nameof(privateNonce.K2)} private key");
		}

		var pointR1 = Schnorr.DerivePublicKey(k1).AsPoint.Value;
		var pointR2 = Schnorr.DerivePublicKey(k2).AsPoint.Value;

		return new MuSigPublicNonce {
			R1 = CBytes(pointR1),
			R2 = CBytes(pointR2)
		};
	}

	private static MuSigPrivateNonce GeneratePrivateNonce(byte[] sessionId, byte[] messageDigest, byte[] privateKey, byte[] aggregatedPublicKey, byte[] extraInput) {
		using (Hashers.BorrowHasher(CHF.SHA2_256, out var hasher)) {
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
			return new MuSigPrivateNonce() {
				K1 = k1,
				K2 = k2
			};
		}
	}

	public byte[] GetSecondPublicKey(byte[][] publicKeys) {
		/* No point on the curve has an X coordinate equal to 0 */
		var secondPublicKey = Schnorr.BytesOfBigInt(BigInteger.Zero, KeySize);
		for (var i = 1; i < publicKeys.Length; i++) {
			if (!Arrays.AreEqual(publicKeys[0], publicKeys[i])) {
				secondPublicKey = publicKeys[i];
				break;
			}
		}
		return secondPublicKey;
	}

	public SignerMuSigSession InitializeSignerSession(byte[] sessionId, BigInteger privateKey, byte[] publicKey, byte[] messageDigest,
											byte[] ell, byte[] secondPublicKey) {
		Schnorr.ValidatePrivateKeyRange(nameof(privateKey), privateKey);
		Schnorr.ValidateArray(nameof(sessionId), sessionId);
		Schnorr.ValidateArray(nameof(publicKey), publicKey);
		Schnorr.ValidateArray(nameof(messageDigest), messageDigest);
		Schnorr.ValidateArray(nameof(ell), ell);
		var nonceData = GenerateNonce(sessionId, Schnorr.BytesOfBigInt(privateKey, KeySize), messageDigest, null);
		var session = new SignerMuSigSession {
			SecretKey = privateKey,
			KeyCoefficient = ComputeKeyAggregationCoefficient(ell, publicKey, secondPublicKey),
			PublicNonce = nonceData.PublicNonce,
			PrivateNonce = nonceData.PrivateNonce,
			InternalKeyParity = false
		};
		return session;
	}

	public MuSigSessionCache InitializeSessionCache(MuSigSessionNonce combinedNonce, BigInteger challenge, bool publicKeyParity) {

		var sessionCache = new MuSigSessionCache {
			FinalNonceParity = combinedNonce.FinalNonceParity,
			FinalNonce = combinedNonce.FinalNonce,
			Challenge = challenge,
			NonceCoefficient = combinedNonce.NonceCoefficient,
			PublicKeyParity = publicKeyParity
		};
		return sessionCache;
	}

	private ECPoint PointC(byte[] compressedPublicKey) {
		var point = Schnorr.LiftX(compressedPublicKey.AsSpan().Slice(1, KeySize).ToArray());
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

	public MuSigSessionNonce CombineSessionNonce(byte[][] nonces, byte[] aggregatedPublicKey, byte[] message) {
		Schnorr.ValidateJaggedArray(nameof(nonces), nonces);
		Schnorr.ValidateArray(nameof(aggregatedPublicKey), aggregatedPublicKey);
		Schnorr.ValidateArray(nameof(message), message);
		ECPoint rA = null;
		ECPoint rB = null;
		for (var i = 0; i < nonces.Length; i++) {
			var sliceA = nonces[i].AsSpan().Slice(0, KeySize + 1).ToArray();
			var sliceB = nonces[i].AsSpan().Slice(KeySize + 1, KeySize + 1).ToArray();
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
		var finalNoncePoint = rB?.Multiply(nonceCoefficient).Add(rA).Normalize();
		var finalNonce = Schnorr.BytesOfXCoord(finalNoncePoint);
		var finalNonceParity = !Schnorr.IsEven(finalNoncePoint);

		return new MuSigSessionNonce {
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

	public BigInteger PartialSign(SignerMuSigSession signerSession, MuSigSessionCache sessionCache) {
		if (signerSession == null)
			throw new InvalidOperationException("You need to run Musig.InitializeSignerSession first");
		if (sessionCache == null)
			throw new InvalidOperationException("You need to run Musig.InitializeSessionCache first");
		var n = Schnorr.N;
		var g = Schnorr.G;

		var k1 = Schnorr.BytesToBigInt(signerSession.PrivateNonce.AsSpan().Slice(0, KeySize).ToArray());
		var k2 = Schnorr.BytesToBigInt(signerSession.PrivateNonce.AsSpan().Slice(KeySize, KeySize).ToArray());
		Schnorr.ValidatePrivateKeyRange(nameof(k1), k1);
		Schnorr.ValidatePrivateKeyRange(nameof(k2), k2);

		var mu = signerSession.KeyCoefficient;
		var sk = signerSession.SecretKey;
		var pk = g.Multiply(sk).Normalize();

		if ((!Schnorr.IsEven(pk) != sessionCache.PublicKeyParity) != signerSession.InternalKeyParity) {
			sk = n.Subtract(sk); //sk = sk.Negate().Mod(n); also works
		}

		sk = sk.Multiply(mu);

		if (sessionCache.FinalNonceParity) {
			k1 = n.Subtract(k1);
			k2 = n.Subtract(k2);
		}
		var signature = sk.Multiply(sessionCache.Challenge);
		k2 = k2.Multiply(sessionCache.NonceCoefficient);
		k1 = k1.Add(k2);
		signature = signature.Add(k1).Mod(n);

		if (!PartialSigVerify(signerSession, sessionCache, Schnorr.BytesOfXCoord(pk), signature)) {
			throw new InvalidOperationException("The created partial signature did not pass verification.");
		}
		return signature;
	}

	public bool PartialSigVerify(SignerMuSigSession signerSession, MuSigSessionCache sessionCache, byte[] publicKey, BigInteger partialSignature) {
		if (signerSession == null)
			throw new InvalidOperationException("You need to run Musig.InitializeSignerSession first");
		if (sessionCache == null)
			throw new InvalidOperationException("You need to run Musig.InitializeSessionCache first");
		Schnorr.ValidateArray(nameof(publicKey), publicKey);
		if (partialSignature == null)
			throw new ArgumentNullException(nameof(partialSignature));

		var n = Schnorr.N;
		var g = Schnorr.G;
		var b = sessionCache.NonceCoefficient;

		var r1 = PointC(signerSession.PublicNonce.AsSpan().Slice(0, KeySize + 1).ToArray());
		var r2 = PointC(signerSession.PublicNonce.AsSpan().Slice(KeySize + 1, KeySize + 1).ToArray());

		var rj = r2;
		rj = rj.Multiply(b).Add(r1);

		var pkp = Schnorr.LiftX(publicKey);
		/* Multiplying the messagehash by the musig coefficient is equivalent
		 * to multiplying the signer's public key by the coefficient, except
		 * much easier to do. */
		var mu = signerSession.KeyCoefficient;
		var e = sessionCache.Challenge.Multiply(mu).Mod(n);

		if (sessionCache.PublicKeyParity != signerSession.InternalKeyParity) {
			e = n.Subtract(e); //e = e.Negate().Mod(n); also works
		}

		var s = partialSignature;
		/* Compute -s*G + e*pkj + rj */
		s = n.Subtract(s); // s = s.Negate().Mod(n); also works

		var tmp = pkp.Multiply(e).Add(g.Multiply(s));

		if (sessionCache.FinalNonceParity) {
			rj = rj.Negate();
		}
		tmp = tmp.Add(rj);
		return Schnorr.IsPointInfinity(tmp);
	}

	public BigInteger ComputeChallenge(byte[] finalNonce, byte[] combinedPublicKey, byte[] messageDigest) {
		return Schnorr.GetE(finalNonce, combinedPublicKey, messageDigest);
	}

	public byte[] CombinePartialSigs(byte[] finalNonce, BigInteger[] partialSignatures) {
		Schnorr.ValidateArray(nameof(finalNonce), finalNonce);
		Schnorr.ValidateArray(nameof(partialSignatures), partialSignatures);
		BigInteger s = null;
		var n = Schnorr.N;
		for (var i = 0; i < partialSignatures.Length; i++) {
			var summand = partialSignatures[i];
			if (summand.CompareTo(n) >= 0) {
				throw new ArgumentException($"{summand} must be an integer less than n");
			}
			s = s == null ? summand.Mod(n) : s.Add(summand).Mod(n);
		}
		return Arrays.Concatenate(finalNonce, Schnorr.BytesOfBigInt(s, KeySize));
	}

	public MuSigData MuSigNonInteractive(Schnorr.PrivateKey[] privateKeys, byte[] messageDigest) {
		Schnorr.ValidateArray(nameof(privateKeys), privateKeys);
		Schnorr.ValidateBuffer(nameof(messageDigest), messageDigest, 32);

		var numberOfSigners = privateKeys.Length;

		// 1. derive the public keys.
		var publicKeys = privateKeys.Select(x => Schnorr.DerivePublicKey(x).RawBytes).ToArray();

		// 2. compute the public keys hash.
		var publicKeyHash = ComputeEll(publicKeys);

		// 3. get second public key
		var secondPublicKey = GetSecondPublicKey(publicKeys);

		// 4. create private signing sessions
		var signerSessions = new SignerMuSigSession[numberOfSigners];
		for (var i = 0; i < numberOfSigners; i++) {
			signerSessions[i] = InitializeSignerSession(Schnorr.RandomBytes(32),
				Schnorr.BytesToBigInt(privateKeys[i].RawBytes),
				publicKeys[i],
				messageDigest,
				publicKeyHash,
				secondPublicKey);
		}

		var keyCoefficients = signerSessions.Select(k => k.KeyCoefficient).ToArray();
		// 5. combine the public keys.
		var publicKeyAggregationData = CombinePublicKey(keyCoefficients, publicKeys);
		var combinedPublicKey = Schnorr.BytesOfXCoord(publicKeyAggregationData.CombinedPoint);
		var publicKeyParity = publicKeyAggregationData.PublicKeyParity;


		/* Communication round 1: A production system would exchange public nonces
		* here before moving on. */

		// 6. combine nonce
		var publicNonces = signerSessions.Select(x => x.PublicNonce).ToArray();
		/* Create aggregate nonce */
		var combinedNonce = CombineSessionNonce(publicNonces, combinedPublicKey, messageDigest);
		// 7. compute challenge
		var challenge = ComputeChallenge(combinedNonce.FinalNonce, combinedPublicKey, messageDigest);

		// 8. initialize musig session cache. same for all signers
		var sessionCache = InitializeSessionCache(combinedNonce, challenge, publicKeyParity);

		// 9. generate partial signatures
		var partialSignatures = new BigInteger[numberOfSigners];
		for (var i = 0; i < signerSessions.Length; i++) {
			partialSignatures[i] = PartialSign(signerSessions[i], sessionCache);
		}

		/* Communication round 2: A production system would exchange
		* partial signatures here before moving on. */

		// 10. verify individual partial signatures
		for (var i = 0; i < numberOfSigners; i++) {
			if (!PartialSigVerify(signerSessions[i], sessionCache, publicKeys[i], partialSignatures[i])) {
				throw new Exception($"verification of partial signature at index {i} failed");
			}
		}

		// 11. combine partial signatures
		var combinedSignature = CombinePartialSigs(sessionCache.FinalNonce, partialSignatures);
		return new MuSigData {
			CombinedSignature = combinedSignature,
			CombinedPublicKey = combinedPublicKey
		};
	}
}
