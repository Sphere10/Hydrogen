// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe, Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities;

namespace Hydrogen.CryptoEx.EC.Schnorr;

//https://github.com/ElementsProject/secp256k1-zkp/blob/master/doc/musig-spec.mediawiki
public class MuSig {
	private static readonly byte[] MuSigNonceTag = Schnorr.ComputeSha256Hash(Encoding.UTF8.GetBytes("MuSig/nonce"));
	public Schnorr Schnorr { get; }
	private ECPoint G => Schnorr.G;
	private BigInteger N => Schnorr.N;
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

	/* Compute KeyAgg coefficient which is constant 1 for the second pubkey and
	* tagged_hash(pk_hash, x) where pk_hash is the hash of public keys otherwise.
	* second_pk_x can be 0 in case there is no second_pk. Assumes both field
	* elements x and second_pk_x are normalized. */
	internal BigInteger ComputeKeyAggregationCoefficient(byte[] ell, byte[] currentPublicKey, byte[] secondPublicKey) {
		if (Arrays.AreEqual(currentPublicKey, secondPublicKey)) {
			return BigInteger.One;
		}
		var hash = Schnorr.TaggedHash("KeyAgg coefficient", Arrays.ConcatenateAll(ell, currentPublicKey));
		return Schnorr.BytesToBigIntPositive(hash).Mod(Schnorr.N);
	}

	public AggregatedPublicKeyData AggregatePublicKeys(BigInteger[] keyCoefficients, byte[][] publicKeys) {
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
		return new AggregatedPublicKeyData() {
			CombinedPoint = combinedPoint,
			PublicKeyParity = publicKeyParity
		};
	}
	internal MuSigNonceData GenerateNonce(byte[] sessionId, byte[] privateKey, byte[] messageDigest, byte[] aggregatePubKey,
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
			hasher.Transform(MuSigNonceTag);
			hasher.Transform(MuSigNonceTag);
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

	public MuSigSessionCache InitializeSessionCache(AggregatedPublicNonce combinedNonce, BigInteger challenge, bool publicKeyParity) {

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

	public AggregatedPublicNonce AggregatePublicNonces(byte[][] publicNonces, byte[] aggregatedPublicKey, byte[] message) {
		Schnorr.ValidateJaggedArray(nameof(publicNonces), publicNonces);
		Schnorr.ValidateArray(nameof(aggregatedPublicKey), aggregatedPublicKey);
		Schnorr.ValidateArray(nameof(message), message);
		ECPoint rA = null;
		ECPoint rB = null;
		for (var i = 0; i < publicNonces.Length; i++) {
			var sliceA = publicNonces[i].AsSpan().Slice(0, KeySize + 1).ToArray();
			var sliceB = publicNonces[i].AsSpan().Slice(KeySize + 1, KeySize + 1).ToArray();
			var summandA = PointC(sliceA);
			var summandB = PointC(sliceB);
			rA = rA == null ? summandA : rA.Add(summandA);
			rB = rB == null ? summandB : rB.Add(summandB);
		}
		rA = rA?.Normalize();
		rB = rB?.Normalize();

		//In "AggregatePublicNonces", if an output R'i would be infinity, instead output the generator (an arbitrary choice).
		rA = Schnorr.IsPointInfinity(rA) ? G : rA;
		rB = Schnorr.IsPointInfinity(rB) ? G : rB;

		var aggregatedPublicNonce = Arrays.Concatenate(CBytes(rA), CBytes(rB));
		var nonceCoefficient = GetB(aggregatedPublicNonce, aggregatedPublicKey, message);
		var finalNoncePoint = rB?.Multiply(nonceCoefficient).Add(rA).Normalize();
		var finalNonce = Schnorr.BytesOfXCoord(finalNoncePoint);
		var finalNonceParity = !Schnorr.IsEven(finalNoncePoint);

		return new AggregatedPublicNonce {
			AggregatedNonce = aggregatedPublicNonce,
			FinalNonce = finalNonce,
			NonceCoefficient = nonceCoefficient,
			FinalNonceParity = finalNonceParity
		};
	}

	private BigInteger GetB(byte[] aggPublicNonce, byte[] q, byte[] m) {
		var hash = Schnorr.TaggedHash("MuSig/noncecoef", Arrays.ConcatenateAll(aggPublicNonce, q, m));
		return Schnorr.BytesToBigIntPositive(hash).Mod(Schnorr.N);
	}

	private void ValidatePartialSignature(BigInteger partialSig) {
		if (partialSig.CompareTo(N) >= 0) {
			throw new ArgumentException($"{nameof(partialSig)} is larger than or equal to curve order");
		}
	}

	public BigInteger PartialSign(SignerMuSigSession signerSession, MuSigSessionCache sessionCache) {
		if (signerSession == null)
			throw new InvalidOperationException("You need to run MuSig.InitializeSignerSession first");
		if (sessionCache == null)
			throw new InvalidOperationException("You need to run MuSig.InitializeSessionCache first");

		Schnorr.ThrowIfPointIsAtInfinity(Schnorr.LiftX(sessionCache.FinalNonce));

		var k1 = Schnorr.BytesToBigIntPositive(signerSession.PrivateNonce.AsSpan().Slice(0, KeySize).ToArray());
		var k2 = Schnorr.BytesToBigIntPositive(signerSession.PrivateNonce.AsSpan().Slice(KeySize, KeySize).ToArray());
		Schnorr.ValidatePrivateKeyRange(nameof(k1), k1);
		Schnorr.ValidatePrivateKeyRange(nameof(k2), k2);

		var mu = signerSession.KeyCoefficient;
		var sk = signerSession.SecretKey;
		var pk = G.Multiply(sk).Normalize();

		// if (!Schnorr.IsEven(pk) != sessionCache.PublicKeyParity != signerSession.InternalKeyParity) {
		// 	sk = sk.Negate().Mod(N);
		// }

		if (Schnorr.IsEven(pk) ^ !sessionCache.PublicKeyParity) {
			sk = sk.Negate().Mod(N);
		}

		sk = sk.Multiply(mu);

		if (sessionCache.FinalNonceParity) {
			k1 = N.Subtract(k1);
			k2 = N.Subtract(k2);
		}
		var signature = sk.Multiply(sessionCache.Challenge);
		k2 = k2.Multiply(sessionCache.NonceCoefficient);
		k1 = k1.Add(k2);
		signature = signature.Add(k1).Mod(N);

		if (!PartialSigVerify(signerSession, sessionCache, Schnorr.BytesOfXCoord(pk), signature)) {
			throw new InvalidOperationException("The created partial signature did not pass verification.");
		}
		return signature;
	}

	internal bool PartialSigVerify(MuSigSessionCache sessionCache, BigInteger keyCoefficient, byte[] publicKey, byte[] publicNonce, BigInteger partialSignature) {
		if (sessionCache == null) {
			throw new InvalidOperationException("You need to run MuSig.InitializeSessionCache first");
		}
		if (keyCoefficient == null) {
			throw new ArgumentNullException(nameof(keyCoefficient));
		}
		Schnorr.ValidateBuffer(nameof(publicKey), publicKey, KeySize);
		Schnorr.ValidateArray(nameof(publicNonce), publicNonce);
		if (partialSignature == null)
			throw new ArgumentNullException(nameof(partialSignature));

		ValidatePartialSignature(partialSignature);

		var b = sessionCache.NonceCoefficient;

		var r1 = PointC(publicNonce.AsSpan().Slice(0, KeySize + 1).ToArray());
		var r2 = PointC(publicNonce.AsSpan().Slice(KeySize + 1, KeySize + 1).ToArray());

		var rj = r2;
		rj = rj.Multiply(b).Add(r1);

		var pkp = Schnorr.LiftX(publicKey);
		/* Multiplying the message hash by the musig coefficient is equivalent
		 * to multiplying the signer's public key by the coefficient, except
		 * much easier to do. */
		var mu = keyCoefficient;
		var e = sessionCache.Challenge.Multiply(mu).Mod(N);

		// if (sessionCache.PublicKeyParity != signerSession.InternalKeyParity) {
		// 	e = e.Negate().Mod(N);
		// }

		if (sessionCache.PublicKeyParity) {
			e = e.Negate().Mod(N);
		}

		var s = partialSignature;
		/* Compute -s*G + e*pkj + rj */
		s = s.Negate().Mod(N);

		var tmp = pkp.Multiply(e).Add(G.Multiply(s));

		if (sessionCache.FinalNonceParity) {
			rj = rj.Negate();
		}

		tmp = tmp.Add(rj);
		return Schnorr.IsPointInfinity(tmp);
	}

	public bool PartialSigVerify(SignerMuSigSession signerSession, MuSigSessionCache sessionCache, byte[] publicKey, BigInteger partialSignature) {
		if (signerSession == null) {
			throw new InvalidOperationException("You need to run MuSig.InitializeSignerSession first");
		}
		return PartialSigVerify(sessionCache, signerSession.KeyCoefficient, publicKey, signerSession.PublicNonce, partialSignature);
	}

	public BigInteger ComputeChallenge(byte[] finalNonce, byte[] combinedPublicKey, byte[] messageDigest) {
		return Schnorr.GetE(finalNonce, combinedPublicKey, messageDigest);
	}

	public byte[] AggregatePartialSignatures(byte[] finalNonce, BigInteger[] partialSignatures) {
		Schnorr.ValidateArray(nameof(finalNonce), finalNonce);
		Schnorr.ValidateArray(nameof(partialSignatures), partialSignatures);
		BigInteger s = null;
		for (var i = 0; i < partialSignatures.Length; i++) {
			var summand = partialSignatures[i];
			if (summand.CompareTo(N) >= 0) {
				throw new ArgumentException($"{summand} must be an integer less than n");
			}
			s = s == null ? summand.Mod(N) : s.Add(summand).Mod(N);
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
				Schnorr.BytesToBigIntPositive(privateKeys[i].RawBytes),
				publicKeys[i],
				messageDigest,
				publicKeyHash,
				secondPublicKey);
		}

		var keyCoefficients = signerSessions.Select(k => k.KeyCoefficient).ToArray();
		// 5. aggregate the public keys.
		var publicKeyAggregationData = AggregatePublicKeys(keyCoefficients, publicKeys);
		var combinedPublicKey = Schnorr.BytesOfXCoord(publicKeyAggregationData.CombinedPoint);
		var publicKeyParity = publicKeyAggregationData.PublicKeyParity;


		/* Communication round 1: A production system would exchange public nonces
		* here before moving on. */

		// 6. combine nonce
		var publicNonces = signerSessions.Select(x => x.PublicNonce).ToArray();
		/* Create aggregate nonce */
		var combinedNonce = AggregatePublicNonces(publicNonces, combinedPublicKey, messageDigest);
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

		// 11. aggregate partial signatures
		var combinedSignature = AggregatePartialSignatures(sessionCache.FinalNonce, partialSignatures);
		return new MuSigData {
			AggregatedSignature = combinedSignature,
			AggregatedPublicKey = combinedPublicKey
		};
	}
}
