// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe, Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Hydrogen.CryptoEx.EC.IES;

namespace Hydrogen.CryptoEx.EC.Schnorr;

//https://github.com/bitcoin/bips/blob/master/bip-0340.mediawiki
public class Schnorr : StatelessDigitalSignatureScheme<Schnorr.PrivateKey, Schnorr.PublicKey> {

	private readonly ECDSAKeyType _keyType;
	private readonly X9ECParameters _curveParams;
	private readonly ECDomainParameters _domainParams;
	private readonly SecureRandom _secureRandom;

	private static readonly ECDSAKeyType[] SupportedKeyTypes = {
		ECDSAKeyType.SECP256K1
	};

	public Schnorr(ECDSAKeyType keyType) : this(keyType, CHF.SHA2_256) {
	}

	private Schnorr(ECDSAKeyType keyType, CHF digestCHF) : base(digestCHF) {

		if (!SupportedKeyTypes.Contains(keyType)) {
			throw new InvalidOperationException($"{keyType} is not supported in Schnorr");
		}
		_keyType = keyType;
		_curveParams = CustomNamedCurves.GetByName(keyType.ToString());
		_domainParams =
			new ECDomainParameters(_curveParams.Curve, _curveParams.G, _curveParams.N, _curveParams.H, _curveParams.GetSeed());
		_secureRandom = new SecureRandom();
		Traits = Traits & DigitalSignatureSchemeTraits.Schnorr & DigitalSignatureSchemeTraits.SupportsIES;
	}
	public override IIESAlgorithm IES => new ECIES(); // defaults to a Pascalcoin style ECIES

	internal ECPoint G => _curveParams.G;
	private ECCurve Curve => _curveParams.Curve;
	private BigInteger P => _curveParams.Curve.Field.Characteristic;
	internal BigInteger N => _curveParams.N;
	public int KeySize => (Curve.FieldSize + 7) >> 3;

	public override bool TryParsePublicKey(ReadOnlySpan<byte> bytes, out PublicKey publicKey) {
		publicKey = null;
		if (bytes.Length != KeySize) {
			return false;
		}
		var pubKey = bytes.ToArray();
		if (!ValidatePublicKeyRangeNoThrow(BytesToBigIntPositive(pubKey))) {
			return false;
		}
		try {
			publicKey = new PublicKey(LiftX(pubKey), _keyType, _curveParams, _domainParams);
			return true;
		} catch (Exception) {
			return false;
		}
	}

	public override bool TryParsePrivateKey(ReadOnlySpan<byte> bytes, out PrivateKey privateKey) {
		privateKey = null;
		if (bytes.Length != KeySize) {
			return false;
		}
		var secretKey = bytes.ToArray();
		var d = BytesToBigIntPositive(secretKey);
		if (!ValidatePrivateKeyRangeNoThrow(d)) {
			return false;
		}
		privateKey = new PrivateKey(secretKey, _keyType, _curveParams, _domainParams);
		return true;
	}

	public override PrivateKey GeneratePrivateKey(ReadOnlySpan<byte> seed) {
		var keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
		// add seed to RNG
		_secureRandom.SetSeed(seed.ToArray());
		keyPairGenerator.Init(new ECKeyGenerationParameters(_domainParams, _secureRandom));
		var keyPair = keyPairGenerator.GenerateKeyPair();
		var privateKeyBytes = BytesOfBigInt((keyPair.Private as ECPrivateKeyParameters)?.D, KeySize);
		return (PrivateKey)this.ParsePrivateKey(privateKeyBytes);
	}

	public override PublicKey DerivePublicKey(PrivateKey privateKey) {
		var privateKeyParameters = new ECPrivateKeyParameters("ECDSA", privateKey.AsInteger.Value, _domainParams);
		var domainParameters = privateKeyParameters.Parameters;
		var ecPoint = (new FixedPointCombMultiplier() as ECMultiplier).Multiply(domainParameters.G, privateKeyParameters.D);
		var pubKeyParams = new ECPublicKeyParameters(privateKeyParameters.AlgorithmName, ecPoint, domainParameters);
		return new PublicKey(pubKeyParams.Q, _keyType, _curveParams, _domainParams);
	}

	public override bool IsPublicKey(PrivateKey privateKey, ReadOnlySpan<byte> publicKeyBytes) {
		return DerivePublicKey(privateKey).RawBytes.AsSpan().SequenceEqual(publicKeyBytes);
	}

	public override byte[] SignDigest(PrivateKey privateKey, ReadOnlySpan<byte> messageDigest) {
		return SignDigestWithAuxRandomData(privateKey, messageDigest, ReadOnlySpan<byte>.Empty);
	}

	public byte[] SignDigestWithAuxRandomData(PrivateKey privateKey, ReadOnlySpan<byte> messageDigest,
	                                          ReadOnlySpan<byte> auxRandomData) {
		// https://github.com/bitcoin/bips/blob/master/bip-0340.mediawiki#signing
		var sk = privateKey.AsInteger.Value;
		var message = messageDigest.ToArray();
		ValidatePrivateKeyRange(nameof(sk), sk);
		ValidateArray(nameof(message), message);

		var p = G.Multiply(sk).Normalize();
		var px = BytesOfXCoord(p);
		var d = GetEvenKey(p, sk);

		if (!auxRandomData.IsEmpty) {
			ValidateBuffer(nameof(auxRandomData), auxRandomData, 32);
		} else {
			auxRandomData = RandomBytes(32);
		}

		var t = BytesOfBigInt(d.Xor(BytesToBigIntPositive(TaggedHash("BIP0340/aux", auxRandomData.ToArray()))), KeySize);
		var rand = TaggedHash("BIP0340/nonce", Arrays.ConcatenateAll(t, px, message));
		var kPrime = BytesToBigIntPositive(rand).Mod(N);

		if (kPrime.SignValue == 0) {
			throw new InvalidOperationException("kPrime is zero");
		}

		var r = G.Multiply(kPrime).Normalize();
		var k = GetEvenKey(r, kPrime);
		var rx = BytesOfXCoord(r);
		var e = GetE(rx, px, message);
		var sig = Arrays.ConcatenateAll(rx, BytesOfBigInt(k.Add(e.Multiply(d)).Mod(N), KeySize));
		if (!VerifyDigest(sig, messageDigest, BytesOfXCoord(p))) {
			throw new InvalidOperationException("The created signature did not pass verification.");
		}
		return sig;
	}

	public override bool VerifyDigest(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> publicKey) {
		// https://github.com/bitcoin/bips/blob/master/bip-0340.mediawiki#verification
		var message = messageDigest.ToArray();
		var sig = signature.ToArray();
		var pubKey = publicKey.ToArray();
		ValidateArray(nameof(sig), sig);
		ValidateArray(nameof(message), message);
		ValidatePublicKeyRange(nameof(pubKey), BytesToBigIntPositive(pubKey));

		var p = LiftX(pubKey);
		var px = BytesOfXCoord(p);
		var rSig = BytesToBigIntPositive(sig.AsSpan().Slice(0, KeySize).ToArray());
		var sSig = BytesToBigIntPositive(sig.AsSpan().Slice(KeySize, KeySize).ToArray());
		ValidateSignature(rSig, sSig);
		var e = GetE(BytesOfBigInt(rSig, KeySize), px, message.ToArray());
		var r = GetR(sSig, e, p);
		return !IsPointInfinity(r) && IsEven(r) && r.AffineXCoord.ToBigInteger().Equals(rSig);
	}

	public bool BatchVerifyDigest(byte[][] signatures, byte[][] messageDigests, byte[][] publicKeys) {
		// https://github.com/bitcoin/bips/blob/master/bip-0340.mediawiki#Batch_Verification
		ValidateJaggedArray(nameof(signatures), signatures);
		ValidateJaggedArray(nameof(messageDigests), messageDigests);
		ValidateJaggedArray(nameof(publicKeys), publicKeys);
		var leftSide = BigInteger.Zero;
		ECPoint rightSide = null;
		for (var i = 0; i < publicKeys.Length; i++) {
			var p = LiftX(publicKeys[i]);
			var px = BytesOfXCoord(p);
			var rSig = BytesToBigIntPositive(signatures[i].AsSpan().Slice(0, KeySize).ToArray());
			var sSig = BytesToBigIntPositive(signatures[i].AsSpan().Slice(KeySize, KeySize).ToArray());
			ValidateSignature(rSig, sSig);
			var e = GetE(BytesOfBigInt(rSig, KeySize), px, messageDigests[i]);
			var r = LiftX(signatures[i].AsSpan().Slice(0, KeySize).ToArray());

			if (i == 0) {
				leftSide = leftSide.Add(sSig);
				rightSide = r;
				rightSide = rightSide.Add(p.Multiply(e));
			} else {
				var a = RandomBigInteger(KeySize);
				leftSide = leftSide.Add(a.Multiply(sSig));
				rightSide = rightSide?.Add(r.Multiply(a));
				rightSide = rightSide?.Add(p.Multiply(a.Multiply(e)));
			}
		}
		return G.Multiply(leftSide).Equals(rightSide);
	}

	// Hash Methods
	internal static byte[] TaggedHash(string tag, byte[] msg) {
		var tagHash = ComputeSha256Hash(Encoding.UTF8.GetBytes(tag));
		return ComputeSha256Hash(Arrays.ConcatenateAll(tagHash, tagHash, msg));
	}

	internal static byte[] ComputeSha256Hash(ReadOnlySpan<byte> message) {
		return Hashers.Hash(CHF.SHA2_256, message);
	}

	// Math Methods
	private BigInteger GetEvenKey(ECPoint publicKey, BigInteger privateKey) {
		return IsEven(publicKey) ? privateKey : N.Subtract(privateKey);
	}

	internal BigInteger GetE(byte[] r, byte[] p, byte[] m) {
		var hash = TaggedHash("BIP0340/challenge", Arrays.ConcatenateAll(r, p, m));
		return BytesToBigIntPositive(hash).Mod(N);
	}

	private ECPoint GetR(BigInteger s, BigInteger e, ECPoint p) {
		var sG = G.Multiply(s);
		var eP = p.Multiply(e);
		return sG.Add(eP.Negate()).Normalize();
	}

	internal ECPoint LiftX(byte[] publicKey) {
		var xPubKey = BytesToBigIntPositive(publicKey);
		ValidatePublicKeyRange(nameof(xPubKey), xPubKey);
		var c = xPubKey.Pow(3).Add(BigInteger.ValueOf(7)).Mod(P);
		var y = c.ModPow(P.Add(BigInteger.One).Divide(BigInteger.Four), P);
		if (c.CompareTo(y.ModPow(BigInteger.Two, P)) != 0) {
			throw new ArgumentException($"{nameof(c)} is not equal to y^2");
		}
		var point = Curve.CreatePoint(xPubKey, y);
		if (!IsEven(point)) {
			point = Curve.CreatePoint(xPubKey, P.Subtract(y));
		}
		ValidatePoint(point);
		return point.Normalize();
	}

	internal static bool IsPointInfinity(ECPoint publicKey) {
		return publicKey.IsInfinity;
	}

	private static bool IsEven(BigInteger publicKey) {
		//return BigInteger.Jacobi(publicKey, P) == 1);
		return publicKey.Mod(BigInteger.Two).Equals(BigInteger.Zero);
	}

	internal static bool IsEven(ECPoint publicKey) {
		ThrowIfPointIsAtInfinity(publicKey);
		return IsEven(publicKey.AffineYCoord.ToBigInteger());
	}

	public byte[] BytesOfXCoord(ECPoint point) {
		return BytesOfBigInt(point.AffineXCoord.ToBigInteger(), KeySize);
	}

	public static byte[] BytesOfBigInt(BigInteger bi, int numBytes) {
		return BigIntegerUtils.BigIntegerToBytes(bi, numBytes);
	}

	public static BigInteger BytesToBigIntPositive(byte[] bytes) {
		return BigIntegerUtils.BytesToBigIntegerPositive(bytes);
	}

	// Random Generation Methods
	private BigInteger RandomBigInteger(int sizeInBytes) {
		for (;;) {
			var tmp = BigIntegers.CreateRandomBigInteger(sizeInBytes * 8, _secureRandom);
			if (tmp.CompareTo(BigInteger.One) >= 0 && tmp.CompareTo(N.Subtract(BigInteger.One)) <= 0) {
				return tmp;
			}
		}
	}

	internal static byte[] RandomBytes(int sizeInBytes) {
		return Tools.Crypto.GenerateCryptographicallyRandomBytes(sizeInBytes);
	}

	// Validation Methods

	/// <summary>
	/// Validate Signatures
	/// </summary>
	/// <param name="r"></param>
	/// <param name="s"></param>
	/// <exception cref="ArgumentException"></exception>
	private void ValidateSignature(BigInteger r, BigInteger s) {
		if (r.CompareTo(P) >= 0) {
			throw new ArgumentException($"{nameof(r)} is larger than or equal to field size");
		}
		if (s.CompareTo(N) >= 0) {
			throw new ArgumentException($"{nameof(s)} is larger than or equal to curve order");
		}
	}

	/// <summary>
	/// Validate Private Key Range No Throw
	/// </summary>
	/// <param name="scalar"></param>
	/// <returns></returns>
	private bool ValidatePrivateKeyRangeNoThrow(BigInteger scalar) {
		// 1 to n - 1
		return scalar.CompareTo(BigInteger.One) >= 0 && scalar.CompareTo(N.Subtract(BigInteger.One)) <= 0;
	}

	/// <summary>
	/// Validate Private Key Range Throw
	/// </summary>
	/// <param name="name"></param>
	/// <param name="scalar"></param>
	/// <exception cref="ArgumentException"></exception>
	internal void ValidatePrivateKeyRange(string name, BigInteger scalar) {
		if (!ValidatePrivateKeyRangeNoThrow(scalar)) {
			throw new ArgumentException($"{name} must be an integer in the range 1..n-1");
		}
	}

	/// <summary>
	/// Validate Public Key Range No Throw
	/// </summary>
	/// <param name="publicKey"></param>
	/// <returns></returns>
	private bool ValidatePublicKeyRangeNoThrow(BigInteger publicKey) {
		return publicKey.CompareTo(BigInteger.Zero) >= 0 && publicKey.CompareTo(P.Subtract(BigInteger.One)) <= 0;
	}

	/// <summary>
	/// Validate Public Key Range Throw
	/// </summary>
	/// <param name="name"></param>
	/// <param name="publicKey"></param>
	/// <exception cref="ArgumentException"></exception>
	private void ValidatePublicKeyRange(string name, BigInteger publicKey) {
		if (!ValidatePublicKeyRangeNoThrow(publicKey)) {
			throw new ArgumentException($"{name} is not in the range 0..p-1");
		}
	}

	internal static void ValidateArray<T>(string name, T[] buffer) {
		if (!(buffer?.Any() ?? false)) {
			throw new ArgumentNullException(nameof(buffer), name);
		}
	}

	internal static void ValidateJaggedArray<T>(string name, T[][] buffer) {
		ValidateArray(name, buffer);
		for (var i = 0; i < buffer.Length; i++) {
			var tmp = buffer[i];
			ValidateArray($"{name}[{i}]", tmp);
		}
	}

	/// <summary>
	/// Throw If Point Is At Infinity
	/// </summary>
	/// <param name="point"></param>
	/// <exception cref="ArgumentException"></exception>
	internal static void ThrowIfPointIsAtInfinity(ECPoint point) {
		if (IsPointInfinity(point)) {
			throw new ArgumentException($"{nameof(point)} is at infinity");
		}
	}

	/// <summary>
	/// Validate Point
	/// </summary>
	/// <param name="point"></param>
	/// <exception cref="InvalidOperationException"></exception>
	private static void ValidatePoint(ECPoint point) {
		ThrowIfPointIsAtInfinity(point);
		if (!IsEven(point)) {
			throw new InvalidOperationException($"{nameof(point)} does not exist");
		}
	}

	/// <summary>
	/// Validate Buffers
	/// </summary>
	/// <param name="name"></param>
	/// <param name="buf"></param>
	/// <param name="len"></param>
	/// <param name="idx"></param>
	/// <exception cref="ArgumentException"></exception>
	internal static void ValidateBuffer(string name, ReadOnlySpan<byte> buf, int len, int? idx = null) {
		var idxStr = (idx.HasValue ? "[" + idx + "]" : "");
		if (buf.Length != len) {
			throw new ArgumentException($"{name + idxStr} must be {len} bytes long");
		}
	}


	public abstract class Key : IKey {
		protected Key(byte[] immutableRawBytes, ECDSAKeyType keyType, X9ECParameters curveParams, ECDomainParameters domainParams) {
			RawBytes = immutableRawBytes;
			KeyType = keyType;
			CurveParams = curveParams;
			DomainParams = domainParams;
			AsInteger = Tools.Values.Future.LazyLoad(() => BytesToBigIntPositive(RawBytes));
		}

		public ECDSAKeyType KeyType { get; }

		public byte[] RawBytes { get; }

		internal X9ECParameters CurveParams { get; }

		internal ECDomainParameters DomainParams { get; }

		internal IFuture<BigInteger> AsInteger { get; }

		public override bool Equals(object obj) {
			if (obj is Key key) {
				return Equals(key);
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool Equals(Key other) => RawBytes.SequenceEqual(other.RawBytes);

		public override int GetHashCode() => (RawBytes != null ? RawBytes.GetHashCode() : 0);

	}


	public class PrivateKey : Key, IPrivateKey {
		public PrivateKey(byte[] rawKeyBytes, ECDSAKeyType keyType, X9ECParameters curveParams, ECDomainParameters domainParams) : base(
			rawKeyBytes,
			keyType,
			curveParams,
			domainParams) {
			Parameters = Tools.Values.Future.LazyLoad(() => new ECPrivateKeyParameters("ECDSA", AsInteger.Value, DomainParams));
		}

		public IFuture<ECPrivateKeyParameters> Parameters { get; }
	}


	public class PublicKey : Key, IPublicKey {
		public PublicKey(ECPoint point, ECDSAKeyType keyType, X9ECParameters curveParams, ECDomainParameters domainParams) :
			base(BytesOfBigInt(point.AffineXCoord.ToBigInteger(), (curveParams.Curve.FieldSize + 7) >> 3),
				keyType,
				curveParams,
				domainParams) {
			AsPoint = Tools.Values.Future.LazyLoad(() => point);
			Parameters = Tools.Values.Future.LazyLoad(() => new ECPublicKeyParameters("ECDSA", AsPoint.Value, DomainParams));
		}

		public IFuture<ECPublicKeyParameters> Parameters { get; }

		internal IFuture<ECPoint> AsPoint { get; }

	}


}
