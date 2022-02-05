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

	public Schnorr(ECDSAKeyType keyType, CHF digestCHF) : base(digestCHF) {
		
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
	
	private ECPoint G => _curveParams.G; 
	private ECCurve Curve => _curveParams.Curve; 
	private BigInteger P => _curveParams.Curve.Field.Characteristic; 
	private BigInteger N => _curveParams.N;

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
		Validations.ValidateSignatureParams(BigIntegerUtils.BigIntegerToBytes(privateKey.AsInteger.Value, 32), messageDigest);
		var message = messageDigest.ToArray();
		var pk = privateKey.AsInteger.Value; 
		ValidateRange(nameof(pk), pk);
		var p = G.Multiply(pk).Normalize();
		var px = BigIntegerUtils.BigIntegerToBytes(p.AffineXCoord.ToBigInteger(), 32);

		var d = GetEvenKey(p, pk);
		BigInteger kPrime;
		if (!auxRandomData.IsEmpty) {
			Validations.ValidateBuffer(nameof(auxRandomData), auxRandomData, 32);

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
		Validations.ValidateVerificationParams(signature, messageDigest, publicKey);
		var p = LiftX(publicKey.ToArray());
		var px = BigIntegerUtils.BigIntegerToBytes(p.AffineXCoord.ToBigInteger(), 32);
		var rSig = BigIntegerUtils.BytesToBigInteger(signature.Slice(0, 32).ToArray());
		var sSig = BigIntegerUtils.BytesToBigInteger(signature.Slice(32, 32).ToArray());
		ValidateSignature(rSig, sSig);
		var e = GetE(BigIntegerUtils.BigIntegerToBytes(rSig, 32), px, messageDigest.ToArray());
		var r = GetR(sSig, e, p);
		return !r.IsInfinity && Math.IsEven(r) && r.AffineXCoord.ToBigInteger().Equals(rSig);
	}

	public bool BatchVerifyDigest(byte[][] signatures, byte[][] messageDigests, byte[][] publicKeys) {
		// https://github.com/bitcoin/bips/blob/master/bip-0340.mediawiki#Batch_Verification
		Validations.ValidateBatchVerificationParams(signatures, messageDigests, publicKeys);
		var leftSide = BigInteger.Zero;
		var rightSide = Curve.Infinity;
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
				var a = RandomBigInteger(_secureRandom);
				leftSide = leftSide.Add(a.Multiply(sSig));
				rightSide = rightSide.Add(r.Multiply(a));
				rightSide = rightSide.Add(p.Multiply(a.Multiply(e)));
			}
		}
		return G.Multiply(leftSide).Equals(rightSide);
	}

	private BigInteger GetEvenKey(ECPoint publicKey, BigInteger privateKey) {
		return Math.IsEven(publicKey) ? privateKey : N.Subtract(privateKey);
	}
	
	private byte[] TaggedHash(string tag, byte[] msg) {
		var tagHash = CalculateMessageDigest(Encoding.UTF8.GetBytes(tag));
		return CalculateMessageDigest(Arrays.ConcatenateAll(tagHash, tagHash, msg));
	}
	
	private BigInteger GetDeterministicKPrime(BigInteger privateKey, byte[] publicKey, byte[] message) {
		Validations.ValidateSignatureParams(BigIntegerUtils.BigIntegerToBytes(privateKey, 32), message);

		var h = TaggedHash("BIP0340/nonce", Arrays.ConcatenateAll(BigIntegerUtils.BigIntegerToBytes(privateKey, 32), publicKey, message));
		var i = BigIntegerUtils.BytesToBigInteger(h);
		return i.Mod(N);
	}
	
	private BigInteger GetE(byte[] r, byte[] p, byte[] m) {
		var hash = TaggedHash("BIP0340/challenge", Arrays.ConcatenateAll(r, p, m));
		return BigIntegerUtils.BytesToBigInteger(hash).Mod(N);
	}

	private ECPoint GetR(BigInteger s, BigInteger e, ECPoint p) {
		var sG = G.Multiply(s);
		var eP = p.Multiply(e);
		return sG.Add(eP.Negate()).Normalize();
	}

	private ECPoint LiftX(byte[] publicKey) {
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
		if (!Math.IsEven(point)) {
			point = Curve.CreatePoint(x, P.Subtract(y));
		}
		Validations.ValidatePoint(point);
		return point.Normalize();
	}
	
	private BigInteger RandomBigInteger(SecureRandom random, int sizeInBits = 256) {
		for (;;) {
			var tmp = BigIntegers.CreateRandomBigInteger(sizeInBits, random);
			if (ValidateRangeNoThrow(tmp)) {
				return tmp;
			}
		}
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
	private void ValidateRange(string name, BigInteger scalar) {
		if (!ValidateRangeNoThrow(scalar)) {
			throw new Exception($"{name} must be an integer in the range 1..n-1");
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
