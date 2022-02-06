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
using Sphere10.Framework.CryptoEx.EC.IES;

namespace Sphere10.Framework.CryptoEx.EC; 

//https://github.com/bitcoin/bips/blob/master/bip-0340.mediawiki
public class Schnorr: StatelessDigitalSignatureScheme<Schnorr.PrivateKey, Schnorr.PublicKey> {
	
	private readonly ECDSAKeyType _keyType;
	private readonly X9ECParameters _curveParams;
	private readonly ECDomainParameters _domainParams;
	private readonly SecureRandom _secureRandom;

	public Schnorr(ECDSAKeyType keyType) : this(keyType, CHF.SHA2_256) {
	}

	private Schnorr(ECDSAKeyType keyType, CHF digestCHF) : base(digestCHF) {
		
		if (keyType != ECDSAKeyType.SECP256K1) {
			throw new Exception($"Only {nameof(ECDSAKeyType.SECP256K1)} is supported in Schnorr");
		}
		_keyType = keyType;
		_curveParams = CustomNamedCurves.GetByName(keyType.ToString());
		_domainParams = new ECDomainParameters(_curveParams.Curve, _curveParams.G, _curveParams.N, _curveParams.H, _curveParams.GetSeed());
		_secureRandom = new SecureRandom();
		Traits = Traits & DigitalSignatureSchemeTraits.Schnorr & DigitalSignatureSchemeTraits.SupportsIES;
	}
	public override IIESAlgorithm IES => new ECIES();  // defaults to a Pascalcoin style ECIES

	internal ECPoint G => _curveParams.G;
	internal ECCurve Curve => _curveParams.Curve; 
	private BigInteger P => _curveParams.Curve.Field.Characteristic;
	internal BigInteger N => _curveParams.N;
	internal int KeySize => (Curve.FieldSize + 7) >> 3;

	public override bool TryParsePublicKey(ReadOnlySpan<byte> bytes, out PublicKey publicKey) {
		if (bytes.Length <= 0 || bytes.Length > 32) {
			publicKey = null;
			return false;
		}
		publicKey = new PublicKey(bytes.ToArray(), _keyType, _curveParams, _domainParams);
		return true;
	}

	public override bool TryParsePrivateKey(ReadOnlySpan<byte> bytes, out PrivateKey privateKey) {
		if (bytes.Length <= 0 || bytes.Length > 32) {
			privateKey = null;
			return false;
		}
		var order = _keyType.GetAttribute<KeyTypeOrderAttribute>().Value;
		var d = BigIntegerUtils.BytesToBigInteger(bytes.ToArray());
		if (d.CompareTo(BigInteger.One) < 0 || d.CompareTo(order) >= 0) {
			privateKey = null;
			return false;
		}
		privateKey = new PrivateKey(bytes.ToArray(), _keyType, _curveParams, _domainParams);
		return true;
	}

	public override PrivateKey GeneratePrivateKey(ReadOnlySpan<byte> seed) {
		var keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
		keyPairGenerator.Init(new ECKeyGenerationParameters(_domainParams, _secureRandom));
		var keyPair = keyPairGenerator.GenerateKeyPair();
		var privateKeyBytes = BigIntegerUtils.BigIntegerToBytes((keyPair.Private as ECPrivateKeyParameters)?.D, 32);
		return (PrivateKey)this.ParsePrivateKey(privateKeyBytes);
	}

	public override PublicKey DerivePublicKey(PrivateKey privateKey) {
		var privateKeyParameters = new ECPrivateKeyParameters("ECDSA", privateKey.AsInteger.Value, _domainParams);
		var domainParameters = privateKeyParameters.Parameters;
		var ecPoint = (new FixedPointCombMultiplier() as ECMultiplier).Multiply(domainParameters.G, privateKeyParameters.D);
		var pubKeyParams = new ECPublicKeyParameters(privateKeyParameters.AlgorithmName, ecPoint, domainParameters);
		return new PublicKey(BigIntegerUtils.BigIntegerToBytes(pubKeyParams.Q.AffineXCoord.ToBigInteger(), 32), _keyType, _curveParams, _domainParams);
	}
	
	public override bool IsPublicKey(PrivateKey privateKey, ReadOnlySpan<byte> publicKeyBytes) {
		return DerivePublicKey(privateKey).RawBytes.AsSpan().SequenceEqual(publicKeyBytes);
	}

	public override byte[] SignDigest(PrivateKey privateKey, ReadOnlySpan<byte> messageDigest) {
		return SignDigestWithAuxRandomData(privateKey, messageDigest, ReadOnlySpan<byte>.Empty);
	}
	
	public byte[] SignDigestWithAuxRandomData(PrivateKey privateKey, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> auxRandomData) {
		// https://github.com/bitcoin/bips/blob/master/bip-0340.mediawiki#signing
		ValidateSignatureParams(BigIntegerUtils.BigIntegerToBytes(privateKey.AsInteger.Value, 32), messageDigest);
		var message = messageDigest.ToArray();
		var pk = privateKey.AsInteger.Value; 
		ValidateRange(nameof(pk), pk);
		var p = G.Multiply(pk).Normalize();
		var px = BigIntegerUtils.BigIntegerToBytes(p.AffineXCoord.ToBigInteger(), 32);

		var d = GetEvenKey(p, pk);
		BigInteger kPrime;
		if (!auxRandomData.IsEmpty) {
			ValidateBuffer(nameof(auxRandomData), auxRandomData, 32);

			var t = BigIntegerUtils.BigIntegerToBytes(d.Xor(BigIntegerUtils.BytesToBigInteger(TaggedHash("BIP0340/aux", auxRandomData.ToArray()))), 32);
			var rand = TaggedHash("BIP0340/nonce", Arrays.ConcatenateAll(t, px, message));
			kPrime = BigIntegerUtils.BytesToBigInteger(rand).Mod(N);
		} else {
			kPrime = GetDeterministicKPrime(d, px, message);
		}
		
		if (kPrime.SignValue == 0) {
			throw new Exception("kPrime is zero");
		}

		var r = G.Multiply(kPrime).Normalize();
		var k = GetEvenKey(r, kPrime);
		var rx = BigIntegerUtils.BigIntegerToBytes(r.AffineXCoord.ToBigInteger(), 32);
		var e = GetE(rx, px, message);
		var sig = Arrays.ConcatenateAll(rx, BigIntegerUtils.BigIntegerToBytes(k.Add(e.Multiply(d)).Mod(N), 32));
		if (!VerifyDigest(sig, messageDigest, BigIntegerUtils.BigIntegerToBytes(p.AffineXCoord.ToBigInteger(), 32))) {
			throw new Exception("The created signature did not pass verification.");
		}
		return sig;
	}
	
	public override bool VerifyDigest(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> publicKey) {
		// https://github.com/bitcoin/bips/blob/master/bip-0340.mediawiki#verification
		ValidateVerificationParams(signature, messageDigest, publicKey);
		var p = LiftX(publicKey.ToArray());
		var px = BigIntegerUtils.BigIntegerToBytes(p.AffineXCoord.ToBigInteger(), 32);
		var rSig = BigIntegerUtils.BytesToBigInteger(signature.Slice(0, 32).ToArray());
		var sSig = BigIntegerUtils.BytesToBigInteger(signature.Slice(32, 32).ToArray());
		ValidateSignature(rSig, sSig);
		var e = GetE(BigIntegerUtils.BigIntegerToBytes(rSig, 32), px, messageDigest.ToArray());
		var r = GetR(sSig, e, p);
		return !IsPointInfinity(r) && IsEven(r) && r.AffineXCoord.ToBigInteger().Equals(rSig);
	}

	public bool BatchVerifyDigest(byte[][] signatures, byte[][] messageDigests, byte[][] publicKeys) {
		// https://github.com/bitcoin/bips/blob/master/bip-0340.mediawiki#Batch_Verification
		ValidateBatchVerificationParams(signatures, messageDigests, publicKeys);
		var leftSide = BigInteger.Zero;
		ECPoint rightSide = null;
		for (var i = 0; i < publicKeys.Length; i++) {
			var p = LiftX(publicKeys[i]);
			var px = BigIntegerUtils.BigIntegerToBytes(p.AffineXCoord.ToBigInteger(), 32);
			var rSig = BigIntegerUtils.BytesToBigInteger(signatures[i].AsSpan().Slice(0, 32).ToArray());
			var sSig = BigIntegerUtils.BytesToBigInteger(signatures[i].AsSpan().Slice(32, 32).ToArray());
			ValidateSignature(rSig, sSig);
			var e = GetE(BigIntegerUtils.BigIntegerToBytes(rSig, 32), px, messageDigests[i]);
			var r = LiftX(signatures[i].AsSpan().Slice(0, 32).ToArray());

			if (i == 0) {
				leftSide = leftSide.Add(sSig);
				rightSide = r;
				rightSide = rightSide.Add(p.Multiply(e));
			} else {
				var a = RandomBigInteger();
				leftSide = leftSide.Add(a.Multiply(sSig));
				rightSide = rightSide?.Add(r.Multiply(a));
				rightSide = rightSide?.Add(p.Multiply(a.Multiply(e)));
			}
		}
		return G.Multiply(leftSide).Equals(rightSide);
	}
	
	// Hash Methods
	private static byte[] TaggedHash(string tag, byte[] msg) {
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

	private BigInteger GetDeterministicKPrime(BigInteger privateKey, byte[] publicKey, byte[] message) {
		ValidateSignatureParams(BigIntegerUtils.BigIntegerToBytes(privateKey, 32), message);

		var h = TaggedHash("BIP0340/nonce", Arrays.ConcatenateAll(BigIntegerUtils.BigIntegerToBytes(privateKey, 32), publicKey, message));
		var i = BigIntegerUtils.BytesToBigInteger(h);
		return i.Mod(N);
	}

	internal BigInteger GetE(byte[] r, byte[] p, byte[] m) {
		var hash = TaggedHash("BIP0340/challenge", Arrays.ConcatenateAll(r, p, m));
		return BigIntegerUtils.BytesToBigInteger(hash).Mod(N);
	}

	internal ECPoint GetR(BigInteger s, BigInteger e, ECPoint p) {
		var sG = G.Multiply(s);
		var eP = p.Multiply(e);
		return sG.Add(eP.Negate()).Normalize();
	}

	internal ECPoint LiftX(byte[] publicKey) {
		var x = BigIntegerUtils.BytesToBigInteger(publicKey);
		if (x.CompareTo(P) >= 0) {
			throw new Exception($"{nameof(x)} is not in the range 0..p-1");
		}
		var c = x.Pow(3).Add(BigInteger.ValueOf(7)).Mod(P);
		var y = c.ModPow(P.Add(BigInteger.One).Divide(BigInteger.Four), P);
		if (c.CompareTo(y.ModPow(BigInteger.Two, P)) != 0) {
			throw new Exception($"{nameof(c)} is not equal to y^2");
		}
		var point = Curve.CreatePoint(x, y);
		if (!IsEven(point)) {
			point = Curve.CreatePoint(x, P.Subtract(y));
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

	// Random Generation Methods
	private BigInteger RandomBigInteger(int sizeInBytes = 32) {
		for (;;) {
			var tmp = BigIntegers.CreateRandomBigInteger(sizeInBytes * 8, _secureRandom);
			if (ValidateRangeNoThrow(tmp)) {
				return tmp;
			}
		}
	}

	internal byte[] RandomBytes(int sizeInBytes = 32) {
		var bytes = new byte[sizeInBytes];
		_secureRandom.NextBytes(bytes);
		return bytes;
	}

	// Validation Methods

	/// <summary>
	/// Validate Signatures
	/// </summary>
	/// <param name="r"></param>
	/// <param name="s"></param>
	/// <exception cref="Exception"></exception>
	private void ValidateSignature(BigInteger r, BigInteger s) {
		if (r.CompareTo(P) >= 0) {
			throw new Exception($"{nameof(r)} is larger than or equal to field size");
		}
		if (s.CompareTo(N) >= 0) {
			throw new Exception($"{nameof(s)} is larger than or equal to curve order");
		}
	}
	
	/// <summary>
	/// Validate Range No Throw
	/// </summary>
	/// <param name="scalar"></param>
	/// <exception cref="Exception"></exception>
	private bool ValidateRangeNoThrow(BigInteger scalar) {
		return scalar.CompareTo(BigInteger.One) >= 0 && scalar.CompareTo(N.Subtract(BigInteger.One)) <= 0;
	}
	
	/// <summary>
	/// Validate Range
	/// </summary>
	/// <param name="name"></param>
	/// <param name="scalar"></param>
	/// <exception cref="Exception"></exception>
	internal void ValidateRange(string name, BigInteger scalar) {
		if (!ValidateRangeNoThrow(scalar)) {
			throw new Exception($"{name} must be an integer in the range 1..n-1");
		}
	}

	/// <summary>
	/// Validate Private Key
	/// </summary>
	/// <param name="name"></param>
	/// <param name="buf"></param>
	/// <param name="len"></param>
	/// <param name="idx"></param>
	/// <exception cref="Exception"></exception>
	private static void ValidatePrivateKey(string name, ReadOnlySpan<byte> buf, int len, int? idx = null) {
		var idxStr = (idx.HasValue ? "[" + idx + "]" : "");
		// if (buf.IsEmpty) {
		// 	throw new Exception($"{name + idxStr} cannot be empty");
		// }
		if (buf.Length != len) {
			throw new Exception($"{name + idxStr} must be {len} bytes long");
		}
	}

	private static void ValidateSignatureArrays(byte[][] signatures) {
			for (var i = 0; i < signatures.Length; i++) {
				ValidateBuffer(nameof(signatures), signatures[i], 64, i);
			}
	}
	private static void ValidateMessageDigestArrays(byte[][] messageDigests) {
		for (var i = 0; i < messageDigests.Length; i++) {
			ValidateBuffer(nameof(messageDigests), messageDigests[i], 32, i);
		}
	}
	internal static void ValidatePublicKeyArrays(byte[][] publicKeys) {
		for (var i = 0; i < publicKeys.Length; i++) {
			ValidateBuffer(nameof(publicKeys), publicKeys[i], 32, i);
		}
	}
	internal static void ValidateNonceArrays(byte[][] nonces) {
		for (var i = 0; i < nonces.Length; i++) {
			ValidateBuffer(nameof(nonces), nonces[i], 32, i);
		}
	}

	/// <summary>
	/// Throw If Point Is At Infinity
	/// </summary>
	/// <param name="point"></param>
	/// <exception cref="Exception"></exception>
	private static void ThrowIfPointIsAtInfinity(ECPoint point) {
		if (IsPointInfinity(point)) {
			throw new Exception($"{nameof(point)} is at infinity");
		}
	}

	/// <summary>
	/// Validate Point
	/// </summary>
	/// <param name="point"></param>
	/// <exception cref="Exception"></exception>
	private static void ValidatePoint(ECPoint point) {
		ThrowIfPointIsAtInfinity(point);
		if (!IsEven(point))
		{
			throw new Exception($"{nameof(point)} does not exist");
		}
	}

	/// <summary>
	/// Validate Buffers
	/// </summary>
	/// <param name="name"></param>
	/// <param name="buf"></param>
	/// <param name="len"></param>
	/// <param name="idx"></param>
	/// <exception cref="Exception"></exception>
	internal static void ValidateBuffer(string name, ReadOnlySpan<byte> buf, int len, int? idx = null) {
		var idxStr = (idx.HasValue ? "[" + idx + "]" : "");
		// if (buf.IsEmpty) {
		// 	throw new Exception($"{name + idxStr} cannot be empty");
		// }
		if (buf.Length != len) {
			throw new Exception($"{name + idxStr} must be {len} bytes long");
		}
	}

	/// <summary>
	/// Validate Signature Parameters
	/// </summary>
	/// <param name="privateKey"></param>
	/// <param name="messageDigest"></param>
	internal static void ValidateSignatureParams(ReadOnlySpan<byte> privateKey, ReadOnlySpan<byte> messageDigest) {
		ValidateBuffer(nameof(messageDigest), messageDigest, 32);
		ValidatePrivateKey(nameof(privateKey), privateKey, 32);
	}
	/// <summary>
	/// Validate Verification Parameters
	/// </summary>
	/// <param name="signature"></param>
	/// <param name="messageDigest"></param>
	/// <param name="publicKey"></param>
	private static void ValidateVerificationParams(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> publicKey) {
		ValidateBuffer(nameof(signature), signature, 64);
		ValidateBuffer(nameof(publicKey), publicKey, 32);
		ValidateBuffer(nameof(messageDigest), messageDigest, 32);
	}
	/// <summary>
	/// Validate Batch Verification Parameters
	/// </summary>
	/// <param name="signatures"></param>
	/// <param name="messageDigests"></param>
	/// <param name="publicKeys"></param>
	/// <exception cref="Exception"></exception>
	private static void ValidateBatchVerificationParams(byte[][] signatures, byte[][] messageDigests, byte[][] publicKeys) {

		ValidateSignatureArrays(signatures);
		ValidatePublicKeyArrays(publicKeys);
		ValidateMessageDigestArrays(messageDigests);

		if (signatures.Length != messageDigests.Length || messageDigests.Length != publicKeys.Length) {
			throw new Exception("all parameters must be an array with the same length");
		}
	}


	public abstract class Key : IKey {
		protected Key(byte[] immutableRawBytes, ECDSAKeyType keyType, X9ECParameters curveParams, ECDomainParameters domainParams) {
			RawBytes = immutableRawBytes;
			KeyType = keyType;
			CurveParams = curveParams;
			DomainParams = domainParams;
			AsInteger = Tools.Values.LazyLoad(() => BigIntegerUtils.BytesToBigInteger(RawBytes));
			AsPoint = Tools.Values.LazyLoad(() => curveParams.Curve.DecodePoint(Arrays.ConcatenateAll(new byte[] { 02 }, RawBytes)));
		}

		public ECDSAKeyType KeyType { get; }

		public byte[] RawBytes { get; }

		internal X9ECParameters CurveParams { get; }

		internal ECDomainParameters DomainParams { get; }

		internal IFuture<BigInteger> AsInteger { get; }

		internal IFuture<ECPoint> AsPoint { get; }

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
		public PrivateKey(byte[] rawKeyBytes, ECDSAKeyType keyType, X9ECParameters curveParams, ECDomainParameters domainParams) : base(rawKeyBytes, keyType, curveParams, domainParams) {
			Parameters = Tools.Values.LazyLoad( () => new ECPrivateKeyParameters("ECDSA", AsInteger.Value, DomainParams));
		}

		public IFuture<ECPrivateKeyParameters> Parameters { get; }
	}

	public class PublicKey : Key, IPublicKey {
		public PublicKey(byte[] rawKeyBytes, ECDSAKeyType keyType, X9ECParameters curveParams, ECDomainParameters domainParams) :
			base(rawKeyBytes, keyType, curveParams, domainParams) {
			Parameters = Tools.Values.LazyLoad(() => new ECPublicKeyParameters("ECDSA", AsPoint.Value, DomainParams));
		}

		public IFuture<ECPublicKeyParameters> Parameters { get; }

	}


}
